using FortRise;
using HarmonyLib;
using Monocle;
using MonoMod.ModInterop;
using System.Diagnostics;
using TowerFall;

namespace Warlord;


[Fort("com.CoolModder.Warlord", "Warlord")]
public class ExampleModModule : FortModule
{
    public static Atlas Atlas;
    public static SpriteData SpriteData;

    public static ExampleModModule Instance;
 
    public ExampleModModule() 
    {
        Instance = this;
    }
    public override Type SettingsType => typeof(ExampleModSettings);
    public static ExampleModSettings Settings => (ExampleModSettings)Instance.InternalSettings;

    public override void LoadContent()
    {
        Atlas = Content.LoadAtlas("Atlas/atlas.xml", "Atlas/atlas.png");
    }

    public override void Load()
    {
        
        typeof(ModExports).ModInterop();
        MyPlayer.Load();
    }

    public override void Unload()
    {
        MyPlayer.Unload();
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