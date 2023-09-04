using System;
using Monocle;
using MonoMod.Utils;
using TowerFall;

namespace TowerBall;

public static class MyTowerMapData
{
    public static void Load() 
    {
        On.TowerFall.TowerMapData.GetLevelSystem += GetLevelSystem_patch;
    }

    public static void Unload() 
    {
        On.TowerFall.TowerMapData.GetLevelSystem -= GetLevelSystem_patch;
    }

    private static TowerFall.LevelSystem GetLevelSystem_patch(On.TowerFall.TowerMapData.orig_GetLevelSystem orig, TowerFall.TowerMapData self)
    {
        if (MainMenu.VersusMatchSettings.CurrentModeName == "TowerBall/TowerBallRoundLogic")
        {
            return new TowerBallVersusLevelSystem(DynamicData.For(self).Get<LevelData>("levelData") as VersusTowerData);
        }
        return orig(self);
    }
}