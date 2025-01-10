using FortRise;
using HarmonyLib;
using Monocle;
using MonoMod.ModInterop;
using TowerFall;

namespace MenuVariantsMod;


[Fort("com.CoolModder.MenuVariantsMod", "MenuVariantsMod")]
public class MenuVariantModModule : FortModule
{
    public static Atlas ExampleAtlas;
    public static SpriteData Data;

    public static MenuVariantModModule Instance;
    public static List<bool> Vanilla;

    public static List<string> MenuVariantNames = new();
    public MenuVariantModModule() 
    {
        Instance = this;
    }
    public override Type SettingsType => typeof(ModSettings);
    public static ModSettings Settings => (ModSettings)Instance.InternalSettings;

    public override void Load()
    {
        On.TowerFall.Logo.ctor += MyLogo.ctor;
        typeof(ModExports).ModInterop();
    }
    public override void LoadContent()
    {
        string customLogos = Path.Combine("Mods", Content.MetadataPath.Replace("mod:", ""), "Content", "CustomLogos");
        Directory.CreateDirectory(customLogos);
        string customBezels = Path.Combine("Mods", Content.MetadataPath.Replace("mod:", ""), "Content", "CustomBezels");
        Directory.CreateDirectory(customBezels);
        
        MenuVariantNames.Add("ASCENSION");
        MenuVariantNames.Add("DARK WORLD");
        
        LogoLoad.Load(Content);
        BezelLoad.Load(Content);
        
        MyBezel.MyLoad();
    }

    public override void Unload()
    {
        On.TowerFall.Logo.ctor -= MyLogo.ctor;
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
}