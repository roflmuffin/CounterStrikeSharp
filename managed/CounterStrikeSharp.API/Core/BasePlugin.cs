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
using System.Reflection;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Config;
using CounterStrikeSharp.API.Modules.Entities;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core
{
    public abstract class BasePlugin : IPlugin
    {
        private bool _disposed;

        public BasePlugin()
        {
            RegisterListener<Listeners.OnMapEnd>(() =>
            {
                foreach (KeyValuePair<Delegate, EntityIO.EntityOutputCallback> callback in EntitySingleOutputHooks)
                {
                    UnhookSingleEntityOutputInternal(callback.Value.Classname, callback.Value.Output, callback.Value.Handler);
                }
            });
        }

        public abstract string ModuleName { get; }
        public abstract string ModuleVersion { get; }
        
        public virtual string ModuleAuthor { get; }
        
        public virtual string ModuleDescription { get; }

        public string ModulePath { get; set; }

        public string ModuleDirectory => Path.GetDirectoryName(ModulePath);
        public ILogger Logger { get; set; }

        public IStringLocalizer Localizer { get; set; }
        
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
        
        public readonly Dictionary<Delegate, CallbackSubscriber> CommandListeners =
            new Dictionary<Delegate, CallbackSubscriber>();

        public readonly Dictionary<Delegate, CallbackSubscriber> ConvarChangeHandlers =
            new Dictionary<Delegate, CallbackSubscriber>();

        public readonly Dictionary<Delegate, CallbackSubscriber> Listeners =
            new Dictionary<Delegate, CallbackSubscriber>();

        public readonly Dictionary<Delegate, CallbackSubscriber> EntityOutputHooks =
            new Dictionary<Delegate, CallbackSubscriber>();

        internal readonly Dictionary<Delegate, EntityIO.EntityOutputCallback> EntitySingleOutputHooks =
            new Dictionary<Delegate, EntityIO.EntityOutputCallback>();

        public readonly List<Timer> Timers = new List<Timer>();
        
        public delegate HookResult GameEventHandler<T>(T @event, GameEventInfo info) where T : GameEvent;

        private void RegisterEventHandlerInternal<T>(string name, GameEventHandler<T> handler, bool post)
            where T : GameEvent
        {
            var subscriber = new CallbackSubscriber(handler, handler,
                () => DeregisterEventHandler(name, handler, post));

            NativeAPI.HookEvent(name, subscriber.GetInputArgument(), post);
            Handlers[handler] = subscriber;
        }

        /// <summary>
        /// Registers a game event handler.
        /// </summary>
        /// <typeparam name="T">The type of the game event.</typeparam>
        /// <param name="handler">The event handler to register.</param>
        /// <param name="hookMode">The mode in which the event handler is hooked. Default is `HookMode.Post`.</param>
        public void RegisterEventHandler<T>(GameEventHandler<T> handler, HookMode hookMode = HookMode.Post) where T : GameEvent
        {
            var name = typeof(T).GetCustomAttribute<EventNameAttribute>()?.Name;
            RegisterEventHandlerInternal(name, handler, hookMode == HookMode.Post);
        }

        public void DeregisterEventHandler(string name, Delegate handler, bool post)
        {
            if (!Handlers.TryGetValue(handler, out var subscriber)) return;

            NativeAPI.UnhookEvent(name, subscriber.GetInputArgument(), post);
            FunctionReference.Remove(subscriber.GetReferenceIdentifier());
            Handlers.Remove(handler);
        }


        /// <summary>
        /// Registers a new server command.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">The description of the command.</param>
        /// <param name="handler">The callback function to be invoked when the command is executed.</param>
        public void AddCommand(string name, string description, CommandInfo.CommandCallback handler)
        {
            var wrappedHandler = new Action<int, IntPtr>((i, ptr) =>
            {
                var caller = (i != -1) ? new CCSPlayerController(NativeAPI.GetEntityFromIndex(i + 1)) : null;
                var command = new CommandInfo(ptr, caller);
                
                using var temporaryCulture = new WithTemporaryCulture(caller.GetLanguage());

                var methodInfo = handler?.GetMethodInfo();

                // We do not need to do permission checks on commands executed from the server console.
                // The server will always be allowed to execute commands (unless marked as client only like above)
                if (caller != null) 
                {
                    // Do not execute command if we do not have the correct permissions.
                    var adminData = AdminManager.GetPlayerAdminData(caller!.AuthorizedSteamID);
                    var permissionsToCheck = new List<BaseRequiresPermissions>();


                    // If our command is overriden, we dynamically create a new permissions attribute
                    // based on the data that is stored in admin_overrides.json.
                    if (AdminManager.CommandIsOverriden(name))
                    {
                        var data = AdminManager.GetCommandOverrideData(name);
                        if (data != null)
                        {
                            var attrType = (data.CheckType == "all") ? typeof(RequiresPermissions) : typeof(RequiresPermissionsOr);
                            var attr = (BaseRequiresPermissions)Activator.CreateInstance(attrType, args: AdminManager.GetPermissionOverrides(name));

                            if (attr != null) permissionsToCheck.Add(attr);
                        }
                    }
                    // The permissions for this command are not being overriden here, so we
                    // grab the permissions to check straight from the attribute.
                    else
                    {
                        var permissions = methodInfo?.GetCustomAttributes<BaseRequiresPermissions>();
                        if (permissions != null) permissionsToCheck.AddRange(permissions);
                    }

                    foreach (var attr in permissionsToCheck)
                    {
                        attr.Command = name;
                        if (!attr.CanExecuteCommand(caller))
                        {
                            var responseStr = (attr.GetType() == typeof(RequiresPermissions)) ?
                            "You are missing the correct permissions" : "You do not have one of the correct permissions";

                            var flags = attr.Permissions.Except(adminData?.GetAllFlags() ?? new HashSet<string>());
                            flags = flags.Except(adminData?.Groups ?? new HashSet<string>());
                            command.ReplyToCommand($"[CSS] {responseStr} ({string.Join(", ", flags)}) to execute this command.");

                            return;
                        }
                    }
                }

                // Do not execute if we shouldn't be calling this command.
                var helperAttribute = methodInfo?.GetCustomAttribute<CommandHelperAttribute>();
                if (helperAttribute != null)
                {
                    switch (helperAttribute.WhoCanExcecute)
                    {
                        case CommandUsage.CLIENT_AND_SERVER: break; // Allow command through.
                        case CommandUsage.CLIENT_ONLY:
                            if (caller == null || !caller.IsValid) { command.ReplyToCommand("[CSS] This command can only be executed by clients."); return; }
                            break;
                        case CommandUsage.SERVER_ONLY:
                            if (caller != null && caller.IsValid) { command.ReplyToCommand("[CSS] This command can only be executed by the server."); return; }
                            break;
                        default: throw new ArgumentException("Unrecognised CommandUsage value passed in CommandHelperAttribute.");
                    }

                    // Technically the command itself counts as the first argument, 
                    // but we'll just ignore that for this check.
                    if (helperAttribute.MinArgs != 0 && command.ArgCount - 1 < helperAttribute.MinArgs)
                    {
                        // Remove the "css_" from the beginning of the command name if it's present.
                        // Most of the time, users will be calling commands from chat.
                        var commandCalled = command.ArgByIndex(0);
                        var properCommandName = (commandCalled.StartsWith("css_")) ? commandCalled.Replace("css_", "") : commandCalled;

                        command.ReplyToCommand($"[CSS] Expected usage: \"!{properCommandName} {helperAttribute.Usage}\".");
                        return;
                    }
                }

                handler?.Invoke(caller, command);
            });

            var methodInfo = handler?.GetMethodInfo();
            var helperAttribute = methodInfo?.GetCustomAttribute<CommandHelperAttribute>();

            var subscriber = new CallbackSubscriber(handler, wrappedHandler, () => { RemoveCommand(name, handler); });
            NativeAPI.AddCommand(name, description, (helperAttribute?.WhoCanExcecute == CommandUsage.SERVER_ONLY),
                (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND, subscriber.GetInputArgument());
            CommandHandlers[handler] = subscriber;
        }

        public void AddCommandListener(string? name, CommandInfo.CommandListenerCallback handler, HookMode mode = HookMode.Pre)
        {
            var wrappedHandler = new Func<int, IntPtr, HookResult>((i, ptr) =>
            {
                var caller = (i != -1) ? new CCSPlayerController(NativeAPI.GetEntityFromIndex(i + 1)) : null;

                var command = new CommandInfo(ptr, caller);
                return handler.Invoke(caller, command);
            });

            var subscriber = new CallbackSubscriber(handler, wrappedHandler, () => { RemoveCommandListener(name, handler, mode); });
            NativeAPI.AddCommandListener(name, subscriber.GetInputArgument(), mode == HookMode.Post);
            CommandListeners[handler] = subscriber;
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

        public void RemoveCommandListener(string name, CommandInfo.CommandListenerCallback handler, HookMode mode)
        {
            if (CommandListeners.ContainsKey(handler))
            {
                var subscriber = CommandListeners[handler];

                NativeAPI.RemoveCommandListener(name, subscriber.GetInputArgument(), mode == HookMode.Post);

                FunctionReference.Remove(subscriber.GetReferenceIdentifier());
                CommandListeners.Remove(handler);
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
        public void RegisterListener<T>(T handler) where T : Delegate
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

            Application.Instance.Logger.LogDebug("Registering listener for {ListenerName} with {ParameterCount} parameters",
                listenerName, parameterTypes.Length);

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


        public void RegisterAllAttributes(object instance)
        {
            this.RegisterAttributeHandlers(instance);
            this.RegisterConsoleCommandAttributeHandlers(instance);
            this.RegisterEntityOutputAttributeHandlers(instance);
        }

        public void InitializeConfig(object instance, Type pluginType)
        {
            Type[] interfaces = pluginType.GetInterfaces();
            Func<Type, bool> predicate = (i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPluginConfig<>));

            // if the plugin has set a configuration type (implements IPluginConfig<>)
            if (interfaces.Any(predicate))
            {
                // IPluginConfig<>
                Type @interface = interfaces.Where(predicate).FirstOrDefault()!;

                // custom config type passed as generic
                Type genericType = @interface!.GetGenericArguments().First();

                var config = typeof(ConfigManager)
                    .GetMethod("Load")!
                    .MakeGenericMethod(genericType)
                    .Invoke(null, new object[] { Path.GetFileName(ModuleDirectory) }) as IBasePluginConfig;

                // we KNOW that we can do this "safely"
                pluginType.GetRuntimeMethod("OnConfigParsed", new Type[] { genericType })
                    .Invoke(instance, new object[] { config });
            }
        }

        /// <summary>
        /// Registers all game event handlers that are decorated with the `[GameEventHandler]` attribute.
        /// </summary>
        /// <param name="instance">The instance of the object where the event handlers are defined.</param>
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
                var attribute = parameterType.GetCustomAttribute<EventNameAttribute>();
                var eventName = attribute?.Name;
                var hookMode = eventHandler.GetCustomAttribute<GameEventHandlerAttribute>()!.Mode;

                var actionType = typeof(GameEventHandler<>).MakeGenericType(parameterType);
                var action = Delegate.CreateDelegate(actionType, instance, eventHandler);

                var generic = method.MakeGenericMethod(parameterType);
                generic.Invoke(this, new object[] { eventName, action, hookMode == HookMode.Post });
            }
        }

        public void RegisterConsoleCommandAttributeHandlers(object instance)
        {
            var eventHandlers = instance.GetType()
                .GetMethods()
                .Where(method => method.GetCustomAttributes<ConsoleCommandAttribute>().Any())
                .ToArray();

            foreach (var eventHandler in eventHandlers)
            {
                var attributes = eventHandler.GetCustomAttributes<ConsoleCommandAttribute>();
                foreach (var commandInfo in attributes)
                {
                    AddCommand(commandInfo.Command, commandInfo.Description,
                        eventHandler.CreateDelegate<CommandInfo.CommandCallback>(instance));
                }
            }
        }

        public void RegisterEntityOutputAttributeHandlers(object instance)
        {
            var handlers = instance.GetType()
                .GetMethods()
                .Where(method => method.GetCustomAttributes<EntityOutputHookAttribute>().Any())
                .ToArray();

            foreach (var handler in handlers)
            {
                var attributes = handler.GetCustomAttributes<EntityOutputHookAttribute>();
                foreach (var outputInfo in attributes)
                {
                    HookEntityOutput(outputInfo.Classname, outputInfo.OutputName, handler.CreateDelegate<EntityIO.EntityOutputHandler>(instance));
                }
            }
        }

        public void HookEntityOutput(string classname, string outputName, EntityIO.EntityOutputHandler handler, HookMode mode = HookMode.Pre)
        {
            var subscriber = new CallbackSubscriber(handler, handler,
                () => UnhookEntityOutput(classname, outputName, handler));

            NativeAPI.HookEntityOutput(classname, outputName, subscriber.GetInputArgument(), mode);
            EntityOutputHooks[handler] = subscriber;
        }

        public void UnhookEntityOutput(string classname, string outputName, EntityIO.EntityOutputHandler handler, HookMode mode = HookMode.Pre)
        {
            if (!EntityOutputHooks.TryGetValue(handler, out var subscriber)) return;

            NativeAPI.UnhookEntityOutput(classname, outputName, subscriber.GetInputArgument(), mode);
            FunctionReference.Remove(subscriber.GetReferenceIdentifier());
            EntityOutputHooks.Remove(handler);
        }

        public void HookSingleEntityOutput(CEntityInstance entityInstance, string outputName, EntityIO.EntityOutputHandler handler)
        {
            // since we wrap around the plugin handler we need to do this to ensure that the plugin callback is only called
            // if the entity instance is the same.
            EntityIO.EntityOutputHandler internalHandler = (output, name, activator, caller, value, delay) =>
            {
                if (caller == entityInstance)
                {
                    return handler(output, name, activator, caller, value, delay);
                }

                return HookResult.Continue;
            };

            HookEntityOutput(entityInstance.DesignerName, outputName, internalHandler);

            // because of ^ we would not be able to unhook since we passed the 'internalHandler' and that's what is being stored, not the original handler
            // but the plugin could only pass the original handler for unhooking.
            // (this dictionary does not needed to be cleared on dispose as it has no unmanaged reference and those are already being disposed, but on map end)
            // (the internal class is needed to be able to remove them on map start)
            EntitySingleOutputHooks[handler] = new EntityIO.EntityOutputCallback(entityInstance.DesignerName, outputName, internalHandler);
        }

        public void UnhookSingleEntityOutput(CEntityInstance entityInstance, string outputName, EntityIO.EntityOutputHandler handler)
        {
            UnhookSingleEntityOutputInternal(entityInstance.DesignerName, outputName, handler);
        }

        private void UnhookSingleEntityOutputInternal(string classname, string outputName, EntityIO.EntityOutputHandler handler)
        {
            if (!EntitySingleOutputHooks.TryGetValue(handler, out var internalHandler)) return;

            UnhookEntityOutput(classname, outputName, internalHandler.Handler);
            EntitySingleOutputHooks.Remove(handler);
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
            }
            
            foreach (var subscriber in CommandListeners.Values)
            {
                subscriber.Dispose();
            }

            foreach (var kv in ConvarChangeHandlers)
            {
            }

            foreach (var subscriber in Listeners.Values)
            {
                subscriber.Dispose();
            }

            foreach (var subscriber in EntityOutputHooks.Values)
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