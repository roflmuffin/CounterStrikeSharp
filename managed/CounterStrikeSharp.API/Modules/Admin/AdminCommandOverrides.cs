using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Linq;

namespace CounterStrikeSharp.API.Modules.Admin
{

    public partial class CommandData
    {
        [JsonPropertyName("flags")] public required HashSet<string> Flags { get; init; }
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        [JsonPropertyName("check_type")] public required string CheckType { get; init; }
    }

    public static partial class AdminManager
    {
        private static Dictionary<string, CommandData> CommandOverrides = new(StringComparer.InvariantCultureIgnoreCase);
        public static void LoadCommandOverrides(string overridePath)
        {
            try
            {
                if (!File.Exists(overridePath))
                {
                    Console.WriteLine("Admin command overrides file not found. Skipping admin command overrides load.");
                    return;
                }

                var overridesFromFile = JsonSerializer.Deserialize<Dictionary<string, CommandData>>
                    (File.ReadAllText(overridePath), new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip });
                if (overridesFromFile == null) { throw new FileNotFoundException(); }
                foreach (var (command, overrideDef) in overridesFromFile)
                {
                    if (CommandOverrides.ContainsKey(command))
                    {
                        CommandOverrides[command].Flags.UnionWith(overrideDef.Flags);
                    }
                    else
                    {
                        CommandOverrides.Add(command, overrideDef);
                    }
                }

                Console.WriteLine($"Loaded {CommandOverrides.Count} admin command overrides.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load admin command overrides: {ex}");
            }
        }

        /// <summary>
        /// Checks to see if a command has overriden permissions.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns>True if the command has overriden permissions, false if not.</returns>
        public static bool CommandIsOverriden(string commandName)
        {
            CommandOverrides.TryGetValue(commandName, out var overrideDef);
            if (overrideDef == null) return false;
            return overrideDef.Enabled;
        }

        /// <summary>
        /// Grabs the data for a command override that was loaded from "configs/admin_overrides.json".
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns>CommandData class if found, null if not.</returns>
        public static CommandData? GetCommandOverrideData(string commandName)
        {
            return CommandOverrides.GetValueOrDefault(commandName);
        }

        /// <summary>
        /// Grabs the new, overriden flags for a command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns>If the command is valid, a valid array of flags.</returns>
        public static string[] GetPermissionOverrides(string commandName)
        {
            CommandOverrides.TryGetValue(commandName, out var overrideDef);
            return overrideDef?.Flags.ToArray() ?? new string[] { };
        }

        /// <summary>
        /// Adds a new permission to a command override. 
        /// This is not saved to "configs/admin_overrides.json".
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="permissions">Permissions to add to the command override.</param>
        public static void AddPermissionOverride(string commandName, params string[] permissions)
        {
            CommandOverrides.TryGetValue(commandName, out var overrideDef);
            if (overrideDef == null)
            {
                overrideDef = new CommandData()
                {
                    Flags = new(permissions),
                    Enabled = true,
                    CheckType = "all"
                };

                CommandOverrides[commandName] = overrideDef;
                return;
            }

            foreach (var flag in permissions)
            {
                overrideDef.Flags.Add(flag);
            }
            CommandOverrides[commandName] = overrideDef;
        }

        /// <summary>
        /// Removes a permission from a command override.
        /// This is not saved to "configs/admin_overrides.json".
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="permissions">Permissions to remove from the command override.</param>
        public static void RemovePermissionOverride(string commandName, params string[] permissions)
        {
            CommandOverrides.TryGetValue(commandName, out var overrideDef);
            if (overrideDef == null) return;

            overrideDef.Flags.ExceptWith(permissions);
            CommandOverrides[commandName] = overrideDef;
        }

        /// <summary>
        /// Clears all permissions from a command override.
        /// This is not saved to "configs/admin_overrides.json".
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="disable">Whether to disable the command override after clearing.</param>
        public static void ClearPermissionOverride(string commandName, bool disable = true)
        {
            CommandOverrides.TryGetValue(commandName, out var overrideDef);
            if (overrideDef == null) return;

            overrideDef.Flags?.Clear();
            overrideDef.Enabled = !disable;
            CommandOverrides[commandName] = overrideDef;
        }

        /// <summary>
        /// Deletes a command override.
        /// This is not saved to "configs/admin_overrides.json".
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        public static void DeleteCommandOverride(string commandName)
        {
            if (!CommandOverrides.ContainsKey(commandName)) return;
            CommandOverrides.Remove(commandName);
        }

        /// <summary>
        /// Sets a command override to be enabled or disabled.
        /// This is not saved to "configs/admin_overrides.json".
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="state">New state of the command override.</param>
        public static void SetCommandOverideState(string commandName, bool state)
        {
            CommandOverrides.TryGetValue(commandName, out var overrideDef);
            if (overrideDef == null) return;

            overrideDef.Flags?.Clear();
            overrideDef.Enabled = state;
            CommandOverrides[commandName] = overrideDef;
        }
    }
}
