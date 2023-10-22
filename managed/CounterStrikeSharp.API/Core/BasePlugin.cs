/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Listeners;
using CounterStrikeSharp.API.Modules.Timers;

namespace CounterStrikeSharp.API.Core
{
    public abstract class BasePlugin : IPlugin, IDisposable
    {
        private bool _disposed;

        public BasePlugin()
        {
        }

        public abstract string ModuleName { get; }
        public abstract string ModuleVersion { get; }

        public string ModulePath { get; internal set; }

        public string ModuleDirectory => Path.GetDirectoryName(ModulePath);

        public virtual void Load(bool hotReload)
        {
        }

        public virtual void Unload(bool hotReload)
        {
        }

        public class CallbackSubscriber : IDisposable
        {
            private Delegate _underlyingMethod;
            private readonly int _functionReferenceIdentifier;

            private readonly InputArgument _inputArgument;
            private readonly Action _dispose;

            public CallbackSubscriber(Delegate underlyingMethod, Delegate wrapperMethod, Action dispose)
            {
                _dispose = dispose;
                _underlyingMethod = underlyingMethod;

                var functionReference = FunctionReference.Create(wrapperMethod);
                _inputArgument = (InputArgument)functionReference;

                _functionReferenceIdentifier = functionReference.Identifier;
            }

            public InputArgument GetInputArgument()
            {
                return _inputArgument;
            }

            public int GetReferenceIdentifier()
            {
                return _functionReferenceIdentifier;
            }

            public void Dispose()
            {
                _dispose();
            }
        }

        public readonly Dictionary<Delegate, CallbackSubscriber> Handlers =
            new Dictionary<Delegate, CallbackSubscriber>();

        public readonly Dictionary<Delegate, CallbackSubscriber> CommandHandlers =
            new Dictionary<Delegate, CallbackSubscriber>();

        public readonly Dictionary<Delegate, CallbackSubscriber> ConvarChangeHandlers =
            new Dictionary<Delegate, CallbackSubscriber>();

        public readonly Dictionary<Delegate, CallbackSubscriber> Listeners =
            new Dictionary<Delegate, CallbackSubscriber>();

        public readonly List<Timer> Timers = new List<Timer>();

        private void RegisterEventHandlerInternal<T>(string name, Action<T> handler, bool post = false)
            where T : GameEvent, new()
        {
            var wrappedHandler = new Action<IntPtr>(pointer =>
            {
                var @event = new T
                {
                    Handle = pointer
                };
                handler.Invoke(@event);
            });

            var subscriber = new CallbackSubscriber(handler, wrappedHandler,
                () => DeregisterEventHandler(name, handler, post));

            NativeAPI.HookEvent(name, subscriber.GetInputArgument(), post);
            Handlers[handler] = subscriber;
        }

        public void RegisterEventHandler<T>(Action<T> handler, bool post = false) where T : GameEvent, new()
        {
            var name = typeof(T).GetCustomAttribute<EventNameAttribute>()?.Name;
            RegisterEventHandlerInternal(name, handler, post);
        }

        public void DeregisterEventHandler(string name, Delegate handler, bool post)
        {
            if (!Handlers.TryGetValue(handler, out var subscriber)) return;

            NativeAPI.UnhookEvent(name, subscriber.GetInputArgument(), post);
            FunctionReference.Remove(subscriber.GetReferenceIdentifier());
            Handlers.Remove(handler);
        }


        public void AddCommand(string name, string description, CommandInfo.CommandCallback handler)
        {
            var wrappedHandler = new Action<int, IntPtr>((i, ptr) =>
            {
                var command = new CommandInfo(ptr);
                if (i == -1)
                {
                    handler?.Invoke(null, command);
                    return;
                }

                var entity = new CCSPlayerController(NativeAPI.GetEntityFromIndex(i + 1));
                handler?.Invoke(entity.IsValid ? entity : null, command);
            });

            var subscriber = new CallbackSubscriber(handler, wrappedHandler, () => { RemoveCommand(name, handler); });
            NativeAPI.AddCommand(name, description, false, (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND, subscriber.GetInputArgument());
            CommandHandlers[handler] = subscriber;
        }

        public void RemoveCommand(string name, CommandInfo.CommandCallback handler)
        {
            if (CommandHandlers.ContainsKey(handler))
            {
                var subscriber = CommandHandlers[handler];

                NativeAPI.RemoveCommand(name, subscriber.GetInputArgument());

                FunctionReference.Remove(subscriber.GetReferenceIdentifier());
                CommandHandlers.Remove(handler);
            }
        }

        /*

        public void HookConVarChange(ConVar convar, ConVar.ConVarChangedCallback handler)
        {
            var wrappedHandler = new Action<IntPtr, string, string>((ptr, oldVal, newVal) =>
            {
                handler?.Invoke(new ConVar(ptr), oldVal, newVal);
            });

            var subscriber = new CallbackSubscriber(convar, handler, wrappedHandler);
            NativeAPI.HookConvarChange(convar.Handle, subscriber.GetInputArgument());
            ConvarChangeHandlers[handler] = subscriber;
        }

        public void UnhookConVarChange(ConVar convar, ConVar.ConVarChangedCallback handler)
        {
            if (ConvarChangeHandlers.ContainsKey(handler))
            {
                var subscriber = ConvarChangeHandlers[handler];

                NativeAPI.UnhookConvarChange(convar.Handle, subscriber.GetInputArgument());
                FunctionReference.Remove(subscriber.GetReferenceIdentifier());
                CommandHandlers.Remove(handler);
            }
        }*/

        // Adds global listener, e.g. OnTick, OnClientConnect
        protected void RegisterListener<T>(T handler) where T : Delegate
        {
            var listenerName = typeof(T).GetCustomAttribute<ListenerNameAttribute>()?.Name;
            if (string.IsNullOrEmpty(listenerName))
            {
                throw new Exception("Listener of type T is invalid and does not have a name attribute");
            }

            var parameterTypes = typeof(T).GetMethod("Invoke").GetParameters().Select(p => p.ParameterType).ToArray();
            var castedParameterTypes = typeof(T).GetMethod("Invoke").GetParameters()
                .Select(p => p.GetCustomAttribute<CastFromAttribute>()?.Type)
                .ToArray();

            Console.WriteLine($"Registering listener for {listenerName} with {parameterTypes.Length}");

            var wrappedHandler = new Action<ScriptContext>(context =>
            {
                var args = new object[parameterTypes.Length];
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    args[i] = context.GetArgument(castedParameterTypes[i] ?? parameterTypes[i], i);
                    if (castedParameterTypes[i] != null)
                        args[i] = Activator.CreateInstance(parameterTypes[i], new[] { args[i] });
                }

                handler.DynamicInvoke(args);
            });

            var subscriber =
                new CallbackSubscriber(handler, wrappedHandler, () => { RemoveListener(listenerName, handler); });

            NativeAPI.AddListener(listenerName, subscriber.GetInputArgument());
            Listeners[handler] = subscriber;
        }

        protected void RemoveListener(string name, Delegate handler)
        {
            if (!Listeners.TryGetValue(handler, out var subscriber)) return;

            NativeAPI.RemoveListener(name, subscriber.GetInputArgument());
            FunctionReference.Remove(subscriber.GetReferenceIdentifier());
            Listeners.Remove(handler);
        }

        public Timer AddTimer(float interval, Action callback, TimerFlags? flags = null)
        {
            var timer = new Timer(interval, callback, flags ?? 0);
            Timers.Add(timer);
            return timer;
        }


        public void RegisterAllAttributes(object instance)
        {
            this.RegisterAttributeHandlers(instance);
            this.RegisterConsoleCommandAttributeHandlers(instance);
        }

        /**
         * Automatically registers all game event handlers that are decorated with the [GameEventHandler] attribute.
         */
        public void RegisterAttributeHandlers(object instance)
        {
            var eventHandlers = instance.GetType()
                .GetMethods()
                .Where(method => method.GetCustomAttribute<GameEventHandlerAttribute>() != null)
                .Where(method =>
                    method.GetParameters().FirstOrDefault()?.ParameterType.IsSubclassOf(typeof(GameEvent)) == true)
                .ToArray();

            var method = typeof(BasePlugin).GetMethod("RegisterEventHandlerInternal", BindingFlags.NonPublic |
                BindingFlags.Instance)!;

            foreach (var eventHandler in eventHandlers)
            {
                var parameterType = eventHandler.GetParameters().First().ParameterType;
                var eventName = parameterType.GetCustomAttribute<EventNameAttribute>()?.Name;

                var actionType = typeof(Action<>).MakeGenericType(parameterType);
                var action = eventHandler.CreateDelegate(actionType, instance);

                var generic = method.MakeGenericMethod(parameterType);
                generic.Invoke(this, new object[] { eventName, action, false });
            }
        }

        public void RegisterConsoleCommandAttributeHandlers(object instance)
        {
            var eventHandlers = instance.GetType()
                .GetMethods()
                .Where(method => method.GetCustomAttribute<ConsoleCommandAttribute>() != null)
                .ToArray();

            foreach (var eventHandler in eventHandlers)
            {
                var commandInfo = eventHandler.GetCustomAttribute<ConsoleCommandAttribute>();
                AddCommand(commandInfo.Command, commandInfo.Description,
                    eventHandler.CreateDelegate<CommandInfo.CommandCallback>(instance));
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (!disposing) return;

            foreach (var subscriber in Handlers.Values)
            {
                subscriber.Dispose();
            }

            foreach (var subscriber in CommandHandlers.Values)
            {
                subscriber.Dispose();
                // _plugin.RemoveCommand((string)kv.Value.GetValue(), (CommandInfo.CommandCallback)kv.Key);
            }

            foreach (var kv in ConvarChangeHandlers)
            {
                // var convar = (ConVar)kv.Value.GetValue();
                // _plugin.UnhookConVarChange((ConVar)kv.Value.GetValue(), (ConVar.ConVarChangedCallback)kv.Key);
                // convar.Unregister();
            }

            foreach (var subscriber in Listeners.Values)
            {
                subscriber.Dispose();
            }

            foreach (var timer in Timers)
            {
                timer.Kill();
            }

            _disposed = true;
        }
    }
}