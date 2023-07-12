using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerFall;
using FortRise;
using Monocle;
using MonoMod;
namespace NoSeek;

public class MyQuestOverrides
{
    public static void MyQuestOverride(On.TowerFall.QuestRoundLogic.orig_OnLevelLoadFinish orig, TowerFall.QuestRoundLogic self)
    {
        orig(self);
        if (ExampleModModule.Settings.NoSeekQuest)
        {
            self.Session.MatchSettings.Variants.NoSeekingArrows.Value = true;
        }
        else
        {
            self.Session.MatchSettings.Variants.NoSeekingArrows.Value = false;
        }
    }
    public static void MyQuestOverrideUpdate(On.TowerFall.QuestRoundLogic.orig_OnUpdate orig, TowerFall.QuestRoundLogic self)
    {
        orig(self);
        Console.Write(self.Session.MatchSettings.Variants.NoSeekingArrows.Value);
    }
    public static void MyDarkWorldOverride(On.TowerFall.DarkWorldRoundLogic.orig_OnLevelLoadFinish orig, TowerFall.DarkWorldRoundLogic self)
    {
        orig(self);
        if (ExampleModModule.Settings.NoSeekQuest)
        {
            self.Session.MatchSettings.Variants.NoSeekingArrows.Value = true;
        }
        else
        {
            self.Session.MatchSettings.Variants.NoSeekingArrows.Value = false;
        }
    }
    public static void Load()
    {
        On.TowerFall.QuestRoundLogic.OnLevelLoadFinish += MyQuestOverride;
        On.TowerFall.DarkWorldRoundLogic.OnLevelLoadFinish += MyDarkWorldOverride;
        On.TowerFall.QuestRoundLogic.OnUpdate += MyQuestOverrideUpdate;
    }
    public static void Unload()
    {
        On.TowerFall.QuestRoundLogic.OnLevelLoadFinish -= MyQuestOverride;
        On.TowerFall.DarkWorldRoundLogic.OnLevelLoadFinish -= MyDarkWorldOverride;
    }
}
