using System.Globalization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Logging;

namespace WithTranslations;

[MinimumApiVersion(80)]
public class WithTranslationsPlugin : BasePlugin
{
    public override string ModuleName => "Example: With Translations";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that provides translations";

    public override void Load(bool hotReload)
    {
        // A global `Localizer` is provided on the plugin instance.
        // You can also use dependency injection to inject `IStringLocalizer` in your own services.
        Logger.LogInformation("This message is in the server language: {Message}", Localizer["test.translation"]);
        
        // IStringLocalizer can accept standard string format arguments.
        // "This number has 2 decimal places {0:n2}" -> "This number has 2 decimal places 123.55"
        Logger.LogInformation(Localizer["test.format", 123.551]);
        
        // This message has colour codes
        Server.PrintToChatAll(Localizer["test.colors"]);

        // This message has colour codes and formatted values
        Server.PrintToChatAll(Localizer["test.colors.withformat", 123.551]);
    }
    
    [ConsoleCommand("css_replylanguage", "Test Translations")]
    public void OnCommandReplyLanguage(CCSPlayerController? player, CommandInfo command)
    {
        // Commands are executed in a players provided culture (or fallback to server culture).
        // Players can configure their language using the `!lang` or `css_lang` command.
        Logger.LogInformation("Current Culture is {Culture}", CultureInfo.CurrentCulture);
        command.ReplyToCommand(Localizer["test.translation"]);

        if (player != null)
        {
            // You can also get the players language using the `GetLanguage` extension method.
            // This will always return a culture, defaulting to the server culture if the user has not configured it.
            var language = player.GetLanguage();
        }
    }
}