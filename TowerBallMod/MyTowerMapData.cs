using FortRise;
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
        var levelData = self.DynGetData<LevelData>("levelData");
        if (levelData is VersusTowerData && MainMenu.VersusMatchSettings.Mode == ModRegisters.GameModeType<TowerBall>())
        {
            return new TowerBallVersusLevelSystem(DynamicData.For(self).Get<LevelData>("levelData") as VersusTowerData);
        }
        return orig(self);
    }
}