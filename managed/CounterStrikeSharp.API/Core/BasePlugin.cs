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
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using CounterStrikeSharp.API.Core.Attributes;
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

        /*

        public void AddCommand(string name, string description, CommandInfo.CommandCallback handler)
        {
            var wrappedHandler = new Action<int, IntPtr>((i, ptr) =>
            {
                var command = new CommandInfo(ptr);
                handler?.Invoke(i, command);
            });

            var subscriber = new CallbackSubscriber(name, handler, wrappedHandler);
            NativeAPI.AddCommand(name, description, false, 0, subscriber.GetInputArgument());
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

        private void AddListener<T>(string name, Listeners.SourceEventHandler<T> handler,
            Action<T, ScriptContext> input = null, Action<T, ScriptContext> output = null) where T : EventArgs, new()
        {
            var wrappedHandler = new Action<ScriptContext>(context =>
            {
                var eventArgs = new T();

                // Before crossing the border, gets all the correct data from the context
                input?.Invoke(eventArgs, context);

                // Invoke the actual event.
                handler?.Invoke(eventArgs);

                // After crossing the border, puts all the correct "return" data back onto the context
                output?.Invoke(eventArgs, context);
            });

            var subscriber = new CallbackSubscriber(handler, wrappedHandler, () => { RemoveListener(name, handler); });

            NativeAPI.AddListener(name, subscriber.GetInputArgument());
            Listeners[handler] = subscriber;
        }

        public void RemoveListener<T>(string name, Listeners.SourceEventHandler<T> handler)
            where T : EventArgs, new()
        {
            if (!Listeners.TryGetValue(handler, out var subscriber)) return;

            NativeAPI.RemoveListener(name, subscriber.GetInputArgument());
            FunctionReference.Remove(subscriber.GetReferenceIdentifier());
            Listeners.Remove(handler);
        }

        public void RemoveListener(string name, Delegate handler)
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

        public event Listeners.SourceEventHandler<Listeners.PlayerConnectArgs> OnClientConnect
        {
            add => AddListener("OnClientConnect", value,
                (args, context) =>
                {
                    args.PlayerIndex = context.GetArgument<int>(0);
                    args.Name = context.GetArgument<string>(1);
                    args.Address = context.GetArgument<string>(2);
                }
            );
            remove => RemoveListener("OnClientConnect", value);
        }

        public event Listeners.SourceEventHandler<Listeners.PlayerArgs> OnClientConnected
        {
            add => AddListener("OnClientConnected", value,
                (args, context) => args.PlayerSlot = context.GetArgument<int>(0));
            remove => RemoveListener("OnClientConnected", value);
        }

        public event Listeners.SourceEventHandler<Listeners.PlayerArgs> OnClientDisconnect
        {
            add => AddListener("OnClientDisconnect", value,
                (args, context) => args.PlayerSlot = context.GetArgument<int>(0));
            remove => RemoveListener("OnClientDisconnect", value);
        }

        public event Listeners.SourceEventHandler<Listeners.MapStartArgs> OnMapStart
        {
            add => AddListener("OnMapStart", value,
                (args, context) => args.MapName = context.GetArgument<string>(0));
            remove => RemoveListener("OnMapStart", value);
        }

        public event Listeners.SourceEventHandler<EventArgs> OnTick
        {
            add => AddListener("OnTick", value);
            remove => RemoveListener("OnTick", value);
        }

        public event Listeners.SourceEventHandler<EventArgs> OnMapEnd
        {
            add => AddListener("OnMapEnd", value);
            remove => RemoveListener("OnMapEnd", value);
        }

        public event Listeners.SourceEventHandler<Listeners.PlayerArgs> OnClientDisconnectPost
        {
            add => AddListener("OnClientDisconnectPost", value,
                (args, context) => args.PlayerSlot = context.GetArgument<int>(0));
            remove => RemoveListener("OnClientDisconnectPost", value);
        }

        public event Listeners.SourceEventHandler<Listeners.PlayerArgs> OnClientPutInServer
        {
            add => AddListener("OnClientPutInServer", value,
                (args, context) => args.PlayerSlot = context.GetArgument<int>(0));
            remove => RemoveListener("OnClientPutInServer", value);
        }

        public event Listeners.SourceEventHandler<Listeners.EntityArgs> OnEntityCreated
        {
            add => AddListener("OnEntityCreated", value,
                (args, context) =>
                {
                    args.EntityIndex = context.GetArgument<int>(0);
                    args.Classname = context.GetArgument<string>(1);
                });
            remove => RemoveListener("OnEntityCreated", value);
        }

        public event Listeners.SourceEventHandler<Listeners.EntityArgs> OnEntitySpawned
        {
            add => AddListener("OnEntitySpawned", value,
                (args, context) =>
                {
                    args.EntityIndex = context.GetArgument<int>(0);
                    args.Classname = context.GetArgument<string>(1);
                });
            remove => RemoveListener("OnEntitySpawned", value);
        }

        public event Listeners.SourceEventHandler<Listeners.EntityArgs> OnEntityDeleted
        {
            add => AddListener("OnEntityDeleted", value,
                (args, context) => { args.EntityIndex = context.GetArgument<int>(0); });
            remove => RemoveListener("OnEntityDeleted", value);
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

            foreach (var kv in CommandHandlers)
            {
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