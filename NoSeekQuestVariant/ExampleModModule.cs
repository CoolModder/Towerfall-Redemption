using FortRise;
using HarmonyLib;
using Monocle;
using MonoMod.ModInterop;
using System.Diagnostics;
using TowerFall;

namespace NoSeek;


[Fort("com.CoolModder.NoSeekQuest", "NoSeekQuest")]
public class ExampleModModule : FortModule
{
    public static ExampleModModule Instance;
 
    public ExampleModModule() 
    {
        Instance = this;
    }
    public override Type SettingsType => typeof(ExampleModSettings);
    public static ExampleModSettings Settings => (ExampleModSettings)Instance.InternalSettings;

    public override void LoadContent()
    {
        
    }

    public override void Load()
    {
        MyQuestOverrides.Load();
        typeof(ModExports).ModInterop();
    }

    public override void Unload()
    {
        MyQuestOverrides.Unload();
    }
}

// Harmony can be supported

[HarmonyPatch(typeof(MainMenu), "BoolToString")]
public class MyPatcher 
{
    static void Postfix(ref string __result) 
    {
        if (__result == "ON") 
        {
            __result = "ENABLED";
            return;
        }
        __result = "DISABLED";
    }
}


/* 
Example of interppting with libraries
Learn more: https://github.com/MonoMod/MonoMod/blob/master/README-ModInterop.md
*/

[ModExportName("ExampleModExport")]
public static class ModExports 
{
    public static int Add(int x, int y) => x + y;
}