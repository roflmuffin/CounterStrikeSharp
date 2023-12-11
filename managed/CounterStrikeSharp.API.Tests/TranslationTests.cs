using System.Globalization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Plugin;
using CounterStrikeSharp.API.Core.Translations;
using Microsoft.Extensions.Localization;
using Moq;

namespace CounterStrikeSharp.API.Tests;

public class TranslationTests
{
    private readonly JsonStringLocalizerFactory _factory;
    private readonly IStringLocalizer _localizer;

    public TranslationTests()
    {
        var pluginContextMock = new Mock<IPluginContext>();
        pluginContextMock.SetupGet(x => x.FilePath).Returns(TestUtils.GetTestPath("test_plugin.dll"));
        _factory = new JsonStringLocalizerFactory(pluginContextMock.Object);
        _localizer = _factory.Create(this.GetType());
        
        // This is generally derived from the core config, but for the sake of these tests we default to `en`.
        var serverCulture = CultureInfo.GetCultureInfo("en");
        CultureInfo.DefaultThreadCurrentUICulture = serverCulture;
        CultureInfo.DefaultThreadCurrentCulture = serverCulture;
        CultureInfo.CurrentUICulture = serverCulture;
        CultureInfo.CurrentCulture = serverCulture;
    }

    [Fact]
    public void TranslatesLanguagesCorrectly()
    {
        using (new WithTemporaryCulture(CultureInfo.GetCultureInfo("en")))
        {
            Assert.Equal("This is the english translation", _localizer["test.translation"]);
        }

        using (new WithTemporaryCulture(CultureInfo.GetCultureInfo("en-US")))
        {
            Assert.Equal("This is the english translation", _localizer["test.translation"]);
        }

        using (new WithTemporaryCulture(CultureInfo.GetCultureInfo("en-GB")))
        {
            Assert.Equal("This is the english (GB) translation", _localizer["test.translation"]);
        }
        
        using (new WithTemporaryCulture(CultureInfo.GetCultureInfo("fr")))
        {
            Assert.Equal("This is the french translation", _localizer["test.translation"]);
        }

        using (new WithTemporaryCulture(CultureInfo.GetCultureInfo("fr-FR")))
        {
            Assert.Equal("This is the french translation", _localizer["test.translation"]);
        }
    }

    [Fact]
    public void ShouldFallbackToServerLanguage()
    {
        using (new WithTemporaryCulture(CultureInfo.GetCultureInfo("de")))
        {
            Assert.Equal("This is the english translation", _localizer["test.translation"]);
        }
    }

    [Fact]
    public void ShouldReturnKeyIfNotFound()
    {
        Assert.Equal("test.notfound", _localizer["test.notfound"]);
    }

    [Fact]
    public void ShouldHandleFormatStrings()
    {
        Assert.Equal("This number has 2 decimal places 0.25", _localizer["test.format", 0.251]);
    }
}