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
using OnTower = IL.FortRise.OnTower;
using TFGame = On.TowerFall.TFGame;

namespace MenuVariantsMod;


[Fort("com.CoolModder.MenuVariantsMod", "MenuVariantsMod")]
public class MenuVariantModModule : FortModule
{
    public static Atlas ExampleAtlas;
    public static SpriteData Data;

    public static MenuVariantModModule Instance;
    public static List<bool> Vanilla;
    public static FortContent s_Content;

    public static List<string> MenuVariantNames = new List<string>();
    public MenuVariantModModule() 
    {
        Instance = this;
    }
    public override Type SettingsType => typeof(ModSettings);
    public static ModSettings Settings => (ModSettings)Instance.InternalSettings;

    public override void Load()
    {
        On.TowerFall.Logo.ctor += MyLogo.ctor;
        On.TowerFall.TFGame.LoadContent += TFGameOnLoadContent;
        typeof(ModExports).ModInterop();
    }
    
    public override void LoadContent()
    {
        s_Content = Content;
        
        string _separator = Path.DirectorySeparatorChar.ToString();
        string _customLogos = "Mods" + _separator + "MenuVariantsMod" + _separator + "Content" + _separator + "CustomLogos" + _separator ; 
        Directory.CreateDirectory(_customLogos);
        
        string _customBezels = "Mods" + _separator + "MenuVariantsMod" + _separator + "Content" + _separator + "CustomBezels" + _separator; 
        Directory.CreateDirectory(_customBezels);
        
        MenuVariantNames.Add("ASCENSION");
        MenuVariantNames.Add("DARK WORLD");
        
        LogoLoad.Load(Content);
        BezelLoad.Load(Content);
        
        MyBezel.MyLoad();
    }

    public override void Unload()
    {
        On.TowerFall.Logo.ctor -= MyLogo.ctor;
        On.TowerFall.TFGame.LoadContent -= TFGameOnLoadContent;
    }

    private void TFGameOnLoadContent(TFGame.orig_LoadContent orig, TowerFall.TFGame self)
    {
        orig(self);
        if (Settings.BezelVariant > 0 && Settings.BezelVariant < BezelLoad.BezelList.Count)
        {
            var LoadedBezel = BezelLoad.BezelList[Settings.BezelVariant - 1];
            var atlas = LoadedBezel.Atlas;
            Screen.LeftImage = atlas[LoadedBezel.Left];
            Screen.RightImage = atlas[LoadedBezel.Right];
        }
    }

    public override void CreateModSettings(TextContainer textContainer)
    {
        var logoSelect = new TextContainer.SelectionOption("Logo", MenuVariantNames.ToArray());
        logoSelect.Value = (MenuVariantNames[Settings.MenuVariant], Settings.MenuVariant);
        logoSelect.OnValueChanged = value =>
        {
            Settings.MenuVariant = value.Item2;
            Refresher.RefreshLogo();
        };
        var bezelSelect = new TextContainer.SelectionOption("Bezel", BezelLoad.BezelNames.ToArray());
        bezelSelect.Value = (BezelLoad.BezelNames[Settings.BezelVariant], Settings.BezelVariant);
        bezelSelect.OnValueChanged = value =>
        {
            Settings.BezelVariant = value.Item2;
            Refresher.RefreshBezel();
        };
        
        textContainer.Add(logoSelect);
        textContainer.Add(bezelSelect);
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

[ModExportName("MenuVariantMod")]
public static class ModExports 
{
    public static int Add(int x, int y) => x + y;
}