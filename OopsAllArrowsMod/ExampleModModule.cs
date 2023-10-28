using FortRise;
using Monocle;
using MonoMod.ModInterop;
using OopsAllArrowsMod;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using TowerFall;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace OopsAllArrowsMod;


[Fort("com.CoolModder.OopsArrows", "OopsAllArrows")]
public class OopsArrowsModModule : FortModule
{
    public static Atlas ArrowAtlas;
    public static Atlas VariantAtlas;
    public static SpriteData SpriteData;
    public static OopsArrowsModModule Instance;
    public List<Variant> ArrowVariantList = new List<Variant>();
    public static List<CustomArrowFormat> CustomArrowList = new List<CustomArrowFormat>();
    public static List<CustomPickupFormat> CustomPickupList = new List<CustomPickupFormat>();
    public OopsArrowsModModule() 
    {
        Instance = this;
    }
    public override Type SettingsType => typeof(ExampleModSettings);
    public static ExampleModSettings Settings => (ExampleModSettings)Instance.InternalSettings;

    public override void LoadContent()
    {
        // From Terria: I recommend just having a one atlas, cuz that's point of an atlas and takes up less memory.
        ArrowAtlas = Content.LoadAtlas("Atlas/ArrowAtlas.xml", "Atlas/ArrowAtlas.png");
        VariantAtlas = Content.LoadAtlas("Atlas/VariantAtlas.xml", "Atlas/VariantAtlas.png");
        SpriteData = Content.LoadSpriteData("Atlas/SpriteData.xml", ArrowAtlas);
    }
    public override void Load()
    {
        Debugger.Launch();
        MechArrow.Load();
        MyPlayer.Load();
        typeof(ModExports).ModInterop();
        typeof(ExplosiveImports).ModInterop();
       // AddArrows();
  

    }
    public void AddArrows()
    {
        CustomArrowList.Add(new CustomArrowFormat(GetID("IceArrow"), "StartWithIceArrows", "ExcludeIceArrows", "DARKFANG", OopsArrowsModModule.CustomArrowList.Count + 1));
        CustomArrowList.Add(new CustomArrowFormat(GetID("SlimeArrow"), "StartWithSlimeArrows", "ExcludeSlimeArrows", "THORNWOOD", OopsArrowsModModule.CustomArrowList.Count + 1));
        CustomArrowList.Add(new CustomArrowFormat(GetID("BaitArrow"), "StartWithBaitArrows", "ExcludeBaitArrows", "MOONSTONE", OopsArrowsModModule.CustomArrowList.Count + 1));
        CustomArrowList.Add(new CustomArrowFormat(GetID("PrismTrapArrow"), "StartWithPrismTrapArrows", "ExcludePrismTrapArrows", "ASCENSION", OopsArrowsModModule.CustomArrowList.Count + 1));
        CustomArrowList.Add(new CustomArrowFormat(GetID("LandMineArrow"), "StartWithLandMineArrows", "ExcludeLandMineArrows", "TOWERFORGE", OopsArrowsModModule.CustomArrowList.Count + 1));
        CustomArrowList.Add(new CustomArrowFormat(GetID("MissleArrow"), "StartWithMissleArrows", "ExcludeMissleArrows", "KING'S COURT", OopsArrowsModModule.CustomArrowList.Count + 1));
        CustomArrowList.Add(new CustomArrowFormat(GetID("FreakyArrow"), "StartWithFreakyArrows", "ExcludeFreakyArrows", "CATACLYSM", OopsArrowsModModule.CustomArrowList.Count + 1));
        CustomArrowList.Add(new CustomArrowFormat(GetID("TornadoArrow"), "StartWithTornadoArrows", "ExcludeTornadoArrows", "FLIGHT", OopsArrowsModModule.CustomArrowList.Count + 1));
        CustomArrowList.Add(new CustomArrowFormat(GetID("MechArrow"), "StartWithMechArrows", "ExcludeMechArrows", "BACKFIRE", OopsArrowsModModule.CustomArrowList.Count + 1));
        CustomArrowList.Add(new CustomArrowFormat(GetID("BoomerangArrow"), "StartWithBoomerangArrows", "ExcludeBoomerangArrows", "DREADWOOD", OopsArrowsModModule.CustomArrowList.Count + 1));
        ArrowTypes GetID(string arrowName)
        {
            return RiseCore.ArrowsRegistry[arrowName].Types;
        }
    }
    
    public static void CreatePickups()
    {
        OnTower Patcher;


        Patcher.VERSUS_Darkfang.IncreaseTreasureRates(RiseCore.PickupRegistry["Ice"].ID);
        Patcher.VERSUS_Thornwood.IncreaseTreasureRates(RiseCore.PickupRegistry["Slime"].ID);
        Patcher.VERSUS_Moonstone.IncreaseTreasureRates(RiseCore.PickupRegistry["Bait"].ID);
        Patcher.VERSUS_Ascension.IncreaseTreasureRates(RiseCore.PickupRegistry["PrismTrap"].ID);
        Patcher.VERSUS_Towerforge.IncreaseTreasureRates(RiseCore.PickupRegistry["LandMine"].ID);
        Patcher.VERSUS_KingsCourt.IncreaseTreasureRates(RiseCore.PickupRegistry["Missle"].ID);
        Patcher.VERSUS_Cataclysm.IncreaseTreasureRates(RiseCore.PickupRegistry["Freaky"].ID);
        Patcher.VERSUS_Flight.IncreaseTreasureRates(RiseCore.PickupRegistry["Tornado"].ID);
        Patcher.VERSUS_Backfire.IncreaseTreasureRates(RiseCore.PickupRegistry["Mech"].ID);
        Patcher.VERSUS_Dreadwood.IncreaseTreasureRates(RiseCore.PickupRegistry["Boomerang"].ID);
    }
    public override void OnVariantsRegister(VariantManager manager, bool noPerPlayer = false)
    {
        base.OnVariantsRegister(manager, noPerPlayer);

        manager.AddArrowVariant(RiseCore.ArrowsRegistry["Ice"], VariantAtlas["variants/startWithIceArrows"], VariantAtlas["variants/excludeIceArrows"]);
        manager.AddArrowVariant(RiseCore.ArrowsRegistry["Slime"], VariantAtlas["variants/startWithSlimeArrows"], VariantAtlas["variants/excludeSlimeArrows"]);
        manager.AddArrowVariant(RiseCore.ArrowsRegistry["Bait"], VariantAtlas["variants/startWithBaitArrows"], VariantAtlas["variants/excludeBaitArrows"]);
        manager.AddArrowVariant(RiseCore.ArrowsRegistry["PrismTrap"], VariantAtlas["variants/startWithPrismTrapArrows"], VariantAtlas["variants/excludePrismTrapArrows"]);
        manager.AddArrowVariant(RiseCore.ArrowsRegistry["LandMine"], VariantAtlas["variants/startWithLandMineArrows"], VariantAtlas["variants/excludeLandMineArrows"]);
        manager.AddArrowVariant(RiseCore.ArrowsRegistry["Missle"], VariantAtlas["variants/startWithMissleArrows"], VariantAtlas["variants/excludeMissleArrows"]);
        manager.AddArrowVariant(RiseCore.ArrowsRegistry["Freaky"], VariantAtlas["variants/startWithFreakyArrows"], VariantAtlas["variants/excludeFreakyArrows"]);
        manager.AddArrowVariant(RiseCore.ArrowsRegistry["Tornado"], VariantAtlas["variants/startWithTornadoArrows"], VariantAtlas["variants/excludeTornadoArrows"]);
        manager.AddArrowVariant(RiseCore.ArrowsRegistry["Mech"], VariantAtlas["variants/startWithMechArrows"], VariantAtlas["variants/excludeMechArrows"]);
        manager.AddArrowVariant(RiseCore.ArrowsRegistry["Boomerang"], VariantAtlas["variants/startWithBoomerangArrows"], VariantAtlas["variants/excludeBoomerangArrows"]);
        manager.AddArrowVariant(RiseCore.ArrowsRegistry["Shock"], VariantAtlas["variants/startWithShockArrows"], VariantAtlas["variants/excludeShockArrows"]);
        //manager.AddVariant(new CustomVariantInfo("DoubleSpread", VariantAtlas["variants/doubleSpread"], "MECH ARROWS SPLIT INTO 6 INSTEAD OF 3", CustomVariantFlags.CanRandom), true);
        manager.AddVariant(new CustomVariantInfo("SonicBoom", VariantAtlas["variants/sonicBoom"], "WHY DID I STICK A LANDMINE ON A BOOMERANG?", CustomVariantFlags.CanRandom), true);
        manager.AddVariant(new CustomVariantInfo("ChoaticBaits", VariantAtlas["variants/chaoticBaits"], "SUMMONS THE RECKONING", CustomVariantFlags.CanRandom), true);
        manager.AddVariant(new CustomVariantInfo("InfiniteWarping", VariantAtlas["variants/infiniteWarping"], "FREAKY ARROWS ARE NOT DESTROYED ON WARP", CustomVariantFlags.CanRandom), true);
        CreatePickups();
    }

   
       // var VarietyPack = variants.AddVariant("VarietyPack", info with { Description = "START WITH MULTIPLE KINDS OF ARROWS" }, VariantFlags.PerPlayer | VariantFlags.CanRandom, true); -- A reminder of the past

    public override void Unload()
    {
        MechArrow.Unload();
        MyPlayer.Unload();
    }
}
[ModExportName("com.fortrise.OopsArrowsMod")]
public static class ModExports
{
    public static void AutoLinkArrowStartVariants(MatchVariants variants, Variant VariantToBeLinked)
    {
        variants.CreateCustomLinks(VariantToBeLinked, variants.StartWithBoltArrows);
        variants.CreateCustomLinks(VariantToBeLinked, variants.StartWithBombArrows);
        variants.CreateCustomLinks(VariantToBeLinked, variants.StartWithBrambleArrows);
        variants.CreateCustomLinks(VariantToBeLinked, variants.StartWithDrillArrows);
        variants.CreateCustomLinks(VariantToBeLinked, variants.StartWithFeatherArrows);
        variants.CreateCustomLinks(VariantToBeLinked, variants.StartWithLaserArrows);
        variants.CreateCustomLinks(VariantToBeLinked, variants.StartWithPrismArrows);
        variants.CreateCustomLinks(VariantToBeLinked, variants.StartWithRandomArrows);
        variants.CreateCustomLinks(VariantToBeLinked, variants.StartWithSuperBombArrows);
        variants.CreateCustomLinks(VariantToBeLinked, variants.StartWithTriggerArrows);
        variants.CreateCustomLinks(VariantToBeLinked, variants.StartWithToyArrows);
        foreach (var OtherArrow in OopsArrowsModModule.Instance.ArrowVariantList)
        {
            variants.CreateCustomLinks(VariantToBeLinked, OtherArrow);
        }
        OopsArrowsModModule.Instance.ArrowVariantList.Add(VariantToBeLinked);
    }
    public static void AddCustomArrow(ArrowTypes Arrow, string StartVariant, string ExcludeVariant, string TowerTheme)
    {
        OopsArrowsModModule.CustomArrowList.Add(new CustomArrowFormat(Arrow, StartVariant, ExcludeVariant, TowerTheme, OopsArrowsModModule.CustomArrowList.Count + 1));
    }

    public static void AddCustomArrowSpawn(ArrowTypes Arrow, string StartVariant, string ExcludeVariant, string TowerTheme, string SpawnWith)
    {
        OopsArrowsModModule.CustomArrowList.Add(new CustomArrowFormat(Arrow, StartVariant, ExcludeVariant, TowerTheme, OopsArrowsModModule.CustomArrowList.Count + 1, SpawnWith));
    }
    public static void AddCustomPickup(Pickups Pickup, string ExcludeVariant)
    {
        OopsArrowsModModule.CustomPickupList.Add(new CustomPickupFormat(Pickup, ExcludeVariant));
    }

    public static void AddCustomPickupSpawn(Pickups Pickup, string ExcludeVariant, string SpawnWith)
    {
        OopsArrowsModModule.CustomPickupList.Add(new CustomPickupFormat(Pickup, ExcludeVariant, SpawnWith));
    }
}

[ModImportName("ExplosionLibraryExport")]
public static class ExplosiveImports
{
    public static Func<Level, Vector2, int, bool, bool, bool, bool> SpawnSmall;
    public static Func<Level, Vector2, int, bool, bool> SpawnSmallSuper;
}