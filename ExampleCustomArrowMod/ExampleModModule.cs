using FortRise;
using HarmonyLib;
using Monocle;
using MonoMod.ModInterop;
using System;
using System.Diagnostics;
using TowerFall;

namespace ExampleArrowMod;


[Fort("com.CoolModder.ExampleArrowMod", "ArrowTest")]
public class ExampleArrowModModule : FortModule
{
    public static ExampleArrowModModule Instance;
    public static Atlas VariantAtlas;
    public ExampleArrowModModule() 
    {
        Instance = this;
    }
    public override Type SettingsType => typeof(ExampleModSettings);
    public static ExampleModSettings Settings => (ExampleModSettings)Instance.InternalSettings;

    public override void LoadContent()
    {
        VariantAtlas = Atlas.Create("Atlas/VariantAtlas.xml", "Atlas/VariantAtlas.png", true, ContentAccess.ModContent);
    }

    public override void Load()
    {
        Debugger.Launch();
        typeof(OopsArrowsModImports).ModInterop();
    }

    public override void OnVariantsRegister(MatchVariants variants, bool noPerPlayer = false)
    {
        var info = new VariantInfo(ExampleArrowModModule.VariantAtlas);
        var NormArrow = variants.AddVariant("StartWithArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer); //Start Variant
        OopsArrowsModImports.AutoLinkArrowStartVariants(variants, NormArrow); //Prevents Multiple Start Variants
        var ExcludeNormArrow = variants.AddVariant("ExcludeArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer); //Exclude Variant
        OopsArrowsModImports.AddCustomArrow(ArrowTypes.Bolt, "StartWithArrows", "ExcludeArrows", "TWILIGHT SPIRE");
        // Replace first arg with your arrows type, the second with your arrows start with variant name, the third is it's exclude variant, and the last thing is the name of the tower (According to its theme title)
    }
    public override void Unload()
    {
        
    }
   
    [ModImportName("com.fortrise.OopsArrowsMod")] 
    public static class OopsArrowsModImports
    {
        // Fields are imported
        public static Action<MatchVariants, Variant> AutoLinkArrowStartVariants; //Links all start with arrow type variants to your variant.
        public static Action<ArrowTypes, string, string, string> AddCustomArrow; //Creates your arrow, will be added to treasure pool with the Spawn In Towers.
        public static Action<ArrowTypes, string, string, string, string> AddCustomArrowSpawn; //Creates your arrow, and will be added to treasure pool with your own variant.
        public static Action<Pickups, string> AddCustomPickup; //Creates your Pickup, spawns with Spawn In Towers.
        public static Action<Pickups, string, string> AddCustomPickupSpawn; //Creates your Pickup, and will be added to treasure pool with your own variant.
    }
}
