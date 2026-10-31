using System;
using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Xunit;
using static TestUtils;

namespace NativeTestsPlugin;

public class BigHeadTests
{
    [Fact]
    public async Task BigHead_SettersPreserveIndependentState()
    {
        var (_, pawn) = await CreateTestPlayerAsync();
        var rules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;
        int originalMutators = rules.ActiveMutators;
        float originalScale = pawn.BigHeadScale;

        try
        {
            Assert.True(Schema.IsSchemaFieldNetworked("CCSPlayerPawn", "m_flBigHeadScale"));
            Assert.True(Schema.IsSchemaFieldNetworked("CCSGameRules", "m_nActiveMutators"));

            rules.SetBigHeadEnabled(false);
            Assert.Equal(originalMutators & ~1, rules.ActiveMutators);
            pawn.SetBigHeadScale(2f);
            Assert.Equal(2f, pawn.BigHeadScale);
            Assert.Equal(originalMutators & ~1, rules.ActiveMutators);

            rules.SetBigHeadEnabled(true);
            Assert.Equal(originalMutators | 1, rules.ActiveMutators);
            Assert.Equal(2f, pawn.BigHeadScale);
            rules.SetBigHeadEnabled(true);
            Assert.Equal(originalMutators | 1, rules.ActiveMutators);

            pawn.SetBigHeadScale(0.5f);
            Assert.Equal(0.5f, pawn.BigHeadScale);
            Assert.Equal(originalMutators | 1, rules.ActiveMutators);

            pawn.SetBigHeadScale(1f);
            Assert.Equal(1f, pawn.BigHeadScale);
            rules.SetBigHeadEnabled(false);
            Assert.Equal(originalMutators & ~1, rules.ActiveMutators);
            Assert.Equal(1f, pawn.BigHeadScale);
        }
        finally
        {
            pawn.SetBigHeadScale(originalScale);
            rules.SetBigHeadEnabled((originalMutators & 1) != 0);
        }
    }

    [Fact]
    public async Task BigHead_InvalidScalesDoNotModifyPawn()
    {
        var (_, pawn) = await CreateTestPlayerAsync();
        float originalScale = pawn.BigHeadScale;

        foreach (float scale in new[] { 0f, -1f, float.NaN, float.PositiveInfinity, float.NegativeInfinity })
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => pawn.SetBigHeadScale(scale));
            Assert.Equal("scale", exception.ParamName);
            Assert.Equal(originalScale, pawn.BigHeadScale);
        }
    }
}
