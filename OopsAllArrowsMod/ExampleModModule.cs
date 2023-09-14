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
public class ExampleModModule : FortModule
{
    public static Atlas IceAtlas;
    public static Atlas SlimeAtlas;
    public static Atlas VariantAtlas;
    public static Atlas BaitAtlas;
    public static Atlas PrismTrapAtlas;
    public static Atlas LandMineAtlas;
    public static Atlas ShockAtlas;
    public static Atlas MissleAtlas;
    public static Atlas FreakyAtlas;
    public static Atlas TornadoAtlas;
    public static Atlas CrystalAtlas;
    public static Atlas MechAtlas;
    public static Atlas BoomerangAtlas;
    public static SpriteData ShockSpriteData;
    public static ExampleModModule Instance;
    public List<Variant> ArrowVariantList = new List<Variant>();
    public static List<CustomArrowFormat> CustomArrowList = new List<CustomArrowFormat>();
    public static List<CustomPickupFormat> CustomPickupList = new List<CustomPickupFormat>();
    public ExampleModModule() 
    {
        Instance = this;
    }
    public override Type SettingsType => typeof(ExampleModSettings);
    public static ExampleModSettings Settings => (ExampleModSettings)Instance.InternalSettings;

    public override void LoadContent()
    {
        // From Terria: I recommend just having a one atlas, cuz that's point of an atlas and takes up less memory.
        IceAtlas = Content.LoadAtlas("Atlas/IceArrow.xml", "Atlas/IceArrow.png");
        VariantAtlas = Content.LoadAtlas("Atlas/VariantAtlas.xml", "Atlas/VariantAtlas.png");
        SlimeAtlas = Content.LoadAtlas("Atlas/SlimeArrow.xml", "Atlas/SlimeArrow.png");
        BaitAtlas = Content.LoadAtlas("Atlas/BaitArrow.xml", "Atlas/BaitArrow.png");
        PrismTrapAtlas = Content.LoadAtlas("Atlas/PrismTrapArrow.xml", "Atlas/PrismTrapArrow.png");
        LandMineAtlas = Content.LoadAtlas("Atlas/LandMineArrow.xml", "Atlas/LandMineArrow.png");
        ShockAtlas = Content.LoadAtlas("Atlas/ShockArrow.xml", "Atlas/ShockArrow.png");
        ShockSpriteData = Content.LoadSpriteData("Atlas/spriteShockData.xml", ShockAtlas);
        MissleAtlas = Content.LoadAtlas("Atlas/MissleArrow.xml", "Atlas/MissleArrow.png");
        FreakyAtlas = Content.LoadAtlas("Atlas/FreakyArrow.xml", "Atlas/FreakyArrow.png");
        TornadoAtlas = Content.LoadAtlas("Atlas/TornadoArrow.xml", "Atlas/TornadoArrow.png");
        CrystalAtlas = Content.LoadAtlas("Atlas/CrystalArrow.xml", "Atlas/CrystalArrow.png");
        MechAtlas = Content.LoadAtlas("Atlas/MechArrow.xml", "Atlas/MechArrow.png");
        BoomerangAtlas = Content.LoadAtlas("Atlas/BoomerangArrow.xml", "Atlas/BoomerangArrow.png");
    }
    public override void Load()
    {
        MechArrow.Load();
        MyPlayer.Load();
        StartArrowPatch.Load();
        MyTreasueChest.Load();
        typeof(ModExports).ModInterop();
        typeof(ExplosiveImports).ModInterop();
    }
    public override void OnVariantsRegister(MatchVariants variants, bool noPerPlayer = false)
    {
        var info = new VariantInfo(ExampleModModule.VariantAtlas);
        var IceArrow = variants.AddVariant("StartWithIceArrows", info with { Header = "OOPS, ALL ARROWS" }, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var SlimeArrow = variants.AddVariant("StartWithSlimeArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var BaitArrow = variants.AddVariant("StartWithBaitArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var PrismTrapArrow = variants.AddVariant("StartWithPrismTrapArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var LandMineArrow = variants.AddVariant("StartWithLandMineArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var MissleArrow = variants.AddVariant("StartWithMissleArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var FreakyArrow = variants.AddVariant("StartWithFreakyArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var TornadoArrow = variants.AddVariant("StartWithTornadoArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var MechArrow = variants.AddVariant("StartWithMechArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var BoomerangArrow = variants.AddVariant("StartWithBoomerangArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        //var CrystalArrow = variants.AddVariant("StartWithCrystalArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        //var ShockArrow = variants.AddVariant("StartWithShockArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var IceExclude = variants.AddVariant("ExcludeIceArrows", info,  VariantFlags.CanRandom, noPerPlayer);
        var SlimeExclude = variants.AddVariant("ExcludeSlimeArrows", info,  VariantFlags.CanRandom, noPerPlayer);
        var BaitExclude = variants.AddVariant("ExcludeBaitArrows", info,  VariantFlags.CanRandom, noPerPlayer);
        var PrismTrapExclude = variants.AddVariant("ExcludePrismTrapArrows", info,  VariantFlags.CanRandom, noPerPlayer);
        var LandMineExclude = variants.AddVariant("ExcludeLandMineArrows", info, VariantFlags.CanRandom, noPerPlayer);
        var MissleExclude = variants.AddVariant("ExcludeMissleArrows", info,  VariantFlags.CanRandom, noPerPlayer);
        var FreakyExclude = variants.AddVariant("ExcludeFreakyArrows", info, VariantFlags.CanRandom, noPerPlayer);
        var TornadoExclude = variants.AddVariant("ExcludeTornadoArrows", info, VariantFlags.CanRandom, noPerPlayer);
        var MechExclude = variants.AddVariant("ExcludeMechArrows", info, VariantFlags.CanRandom, noPerPlayer);
        var BoomerangExclude = variants.AddVariant("ExcludeBoomerangArrows", info, VariantFlags.CanRandom, noPerPlayer);
        //var CrystalExclude = variants.AddVariant("ExcludeCrystalArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        //var ShockExclude = variants.AddVariant("ExcludeShockArrows", info, VariantFlags.PerPlayer, noPerPlayer);
        var SpawnInTowers = variants.AddVariant("SpawnInTowers", info, VariantFlags.CanRandom, true);
        var InfiniWarp = variants.AddVariant("InfiniteWarping", info with {Description = "FREAKY ARROWS ARE NOT DESTROYED ON WARP"}, VariantFlags.CanRandom, true);
        var doubleMiniMech = variants.AddVariant("DoubleSpread", info with { Description = "MECH ARROWS SPLIT INTO 6 INSTEAD OF 3" }, VariantFlags.CanRandom, true);
        var Chaos = variants.AddVariant("ChaoticBaits", info with { Description = "SUMMONS THE RECKONING" }, VariantFlags.CanRandom, true);
        var VarietyPack = variants.AddVariant("VarietyPack", info with { Description = "START WITH MULTIPLE KINDS OF ARROWS" }, VariantFlags.PerPlayer | VariantFlags.CanRandom, true);
        var sonicgobrrr = variants.AddVariant("SonicBoom", info with { Description = "WHY DID I STICK A LANDMINE ON A BOOMERANG?" },  VariantFlags.CanRandom, true);
        AutoLinkArrowStartVariants(variants, IceArrow);
        AutoLinkArrowStartVariants(variants, SlimeArrow);
        AutoLinkArrowStartVariants(variants, BaitArrow);
        AutoLinkArrowStartVariants(variants, PrismTrapArrow);
        AutoLinkArrowStartVariants(variants, LandMineArrow);
        AutoLinkArrowStartVariants(variants, MissleArrow);
        AutoLinkArrowStartVariants(variants, FreakyArrow);
        AutoLinkArrowStartVariants(variants, TornadoArrow);
        AutoLinkArrowStartVariants(variants, MechArrow);
        AutoLinkArrowStartVariants(variants, BoomerangArrow);
        //AutoLinkArrowStartVariants(variants, CrystalArrow);
        AutoLinkArrowStartVariants(variants, VarietyPack);
        //AutoLinkArrowStartVariants(variants, ShockArrow);
        CustomArrowList.Add(new CustomArrowFormat(GetID("IceArrow"), "StartWithIceArrows", "ExcludeIceArrows", "DARKFANG", 1));
        CustomArrowList.Add(new CustomArrowFormat(GetID("SlimeArrow"), "StartWithSlimeArrows", "ExcludeSlimeArrows", "THORNWOOD", 2));
        CustomArrowList.Add(new CustomArrowFormat(GetID("BaitArrow"), "StartWithBaitArrows", "ExcludeBaitArrows", "MOONSTONE", 3));
        CustomArrowList.Add(new CustomArrowFormat(GetID("PrismTrapArrow"), "StartWithPrismTrapArrows", "ExcludePrismTrapArrows", "ASCENSION", 4));
        CustomArrowList.Add(new CustomArrowFormat(GetID("LandMineArrow"), "StartWithLandMineArrows", "ExcludeLandMineArrows", "TOWERFORGE", 5));
        CustomArrowList.Add(new CustomArrowFormat(GetID("MissleArrow"), "StartWithMissleArrows", "ExcludeMissleArrows", "KING'S COURT", 6));
        CustomArrowList.Add(new CustomArrowFormat(GetID("FreakyArrow"), "StartWithFreakyArrows", "ExcludeFreakyArrows", "CATACLYSM", 7));
        CustomArrowList.Add(new CustomArrowFormat(GetID("TornadoArrow"), "StartWithTornadoArrows", "ExcludeTornadoArrows", "FLIGHT", 8));
        CustomArrowList.Add(new CustomArrowFormat(GetID("MechArrow"), "StartWithMechArrows", "ExcludeMechArrows", "BACKFIRE", 9));
        CustomArrowList.Add(new CustomArrowFormat(GetID("BoomerangArrow"), "StartWithBoomerangArrows", "ExcludeBoomerangArrows", "DREADWOOD", 10));
        //CustomArrowList.Add(new CustomArrowFormat(RiseCore.ArrowsID["CrystalArrow"], "StartWithCrystalArrows", "ExcludeCrystalArrows", "TWILIGHTSPIRE", 9));
        //CustomArrowList.Add(new CustomArrowFormat(RiseCore.ArrowsID["ShockArrow"], "StartWithShockArrows", "ExcludeShockArrows", "SUNKENCITY", 6));

        ArrowTypes GetID(string arrowName) 
        {
            return RiseCore.ArrowsRegistry[arrowName].Types;
        }
    }

    public void AutoLinkArrowStartVariants(MatchVariants variants, Variant VariantToBeLinked)
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
        foreach(var OtherArrow in ArrowVariantList)
        {
            variants.CreateCustomLinks(VariantToBeLinked, OtherArrow);
        }
        ArrowVariantList.Add(VariantToBeLinked);
    }
    public override void Unload()
    {
        MechArrow.Unload();
        MyPlayer.Unload();
        StartArrowPatch.Unload();
        MyTreasueChest.Unload();
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
        foreach (var OtherArrow in ExampleModModule.Instance.ArrowVariantList)
        {
            variants.CreateCustomLinks(VariantToBeLinked, OtherArrow);
        }
        ExampleModModule.Instance.ArrowVariantList.Add(VariantToBeLinked);
    }
    public static void AddCustomArrow(ArrowTypes Arrow, string StartVariant, string ExcludeVariant, string TowerTheme)
    {
        ExampleModModule.CustomArrowList.Add(new CustomArrowFormat(Arrow, StartVariant, ExcludeVariant, TowerTheme, ExampleModModule.CustomArrowList.Count + 1));
    }

    public static void AddCustomArrowSpawn(ArrowTypes Arrow, string StartVariant, string ExcludeVariant, string TowerTheme, string SpawnWith)
    {
        ExampleModModule.CustomArrowList.Add(new CustomArrowFormat(Arrow, StartVariant, ExcludeVariant, TowerTheme, ExampleModModule.CustomArrowList.Count + 1, SpawnWith));
    }
    public static void AddCustomPickup(Pickups Pickup, string ExcludeVariant)
    {
        ExampleModModule.CustomPickupList.Add(new CustomPickupFormat(Pickup, ExcludeVariant));
    }

    public static void AddCustomPickupSpawn(Pickups Pickup, string ExcludeVariant, string SpawnWith)
    {
        ExampleModModule.CustomPickupList.Add(new CustomPickupFormat(Pickup, ExcludeVariant, SpawnWith));
    }
}

[ModImportName("ExplosionLibraryExport")]
public static class ExplosiveImports
{
    public static Func<Level, Vector2, int, bool, bool, bool, bool> SpawnSmall;
    public static Func<Level, Vector2, int, bool, bool> SpawnSmallSuper;
}