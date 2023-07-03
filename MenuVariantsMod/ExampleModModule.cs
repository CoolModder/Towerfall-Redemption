using FortRise;
using HarmonyLib;
using Monocle;
using MonoMod.ModInterop;
using TowerFall;
using System.Reflection;
using MonoMod.RuntimeDetour;
using System.Diagnostics;
using System.Drawing;
using MonoMod.Utils;

namespace MenuVariantsMod;


[Fort("com.CoolModder.MenuVariantsMod", "MenuVariantsMod")]
public class MenuVariantModModule : FortModule
{
    public static Atlas ExampleAtlas;
    public static SpriteData Data;

    public static MenuVariantModModule Instance;
    public static List<bool> Vanilla;
    public static int SettingLogoCount;

    public static List<string> MenuVariantNames = new List<string>();
    public MenuVariantModModule() 
    {
        Instance = this;
    }
    public override Type SettingsType => typeof(ModSettings);
    public static ModSettings Settings => (ModSettings)Instance.InternalSettings;

    public override void LoadContent()
    {
        
    }

    public override void Load()
    {
        var _separator = Path.DirectorySeparatorChar.ToString();
        var _customLogos = "Content" + _separator + "Mod" + _separator + "CustomLogos" + _separator;
        Directory.CreateDirectory(_customLogos);
        MenuVariantModModule.MenuVariantNames.Add("ASCENSION");
        MenuVariantModModule.MenuVariantNames.Add("DARK WORLD");
        LogoLoad.Load();
        BezelLoad.Load();
        Console.WriteLine("Custom Logos are here");
        On.TowerFall.Logo.ctor += MyLogo.ctor;
        On.TowerFall.TFGame.LoadContent += MyBezel.MyLoad;
        typeof(ModExports).ModInterop();
    }

    public override void Unload()
    {
        On.TowerFall.Logo.ctor -= MyLogo.ctor;
        On.TowerFall.TFGame.LoadContent -= MyBezel.MyLoad;
    }
    public override void CreateModSettings(List<OptionsButton> optionList)
    {
        var optionButton = new OptionsButton("TITLE MODE");
        optionButton.SetCallbacks(
            ()=> {optionButton.State = SettingName();}, 
            ()=> {Settings.MenuVariant--; SettingName(); }, 
            ()=>{Settings.MenuVariant++; SettingName(); },
            null
        );
        optionList.Add(optionButton);
        var optionBezelButton = new OptionsButton("BEZEL MODE");
        optionBezelButton.SetCallbacks(
            () => { optionBezelButton.State = SideSettingName(); },
            () => { Settings.BezelVariant--; SideSettingName(); },
            () => { Settings.BezelVariant++; SideSettingName(); },
            null
        );
        optionList.Add(optionButton);
        optionList.Add(optionBezelButton);
        string SideSettingName()
        {
            optionBezelButton.CanLeft = Settings.BezelVariant > 0;
            optionBezelButton.CanRight = Settings.BezelVariant < BezelLoad.BezelNames.Count - 1;
            Refresher.RefreshBezel();
            return BezelLoad.BezelNames[Settings.BezelVariant];
        }
        string SettingName()
        {
            Refresher.RefreshLogo();
            optionButton.CanLeft = Settings.MenuVariant > 0;
            optionButton.CanRight = Settings.MenuVariant < MenuVariantNames.Count - 1;
            return MenuVariantNames[Settings.MenuVariant];
        }

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