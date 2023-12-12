# With Translations
This example shows how to use an `IStringLocalizer` and language `json` files to provide localization for your plugins.

## How to use
1. Add the `IStringLocalizer` to your services with dependency injection, or use the `Localizer` provided on the plugin instance.
2. Add a `lang` folder to your plugin, and add a `json` file for each language you want to support. The name of the file should be a locale code, like `en.json` or `fr.json` etc.
3. Ensure that the `lang` folder is shipped with your plugin, see the example `.csproj` file for an example to auto-copy to the output folder.
4. Use the `IStringLocalizer` to localize your strings.
