using FortRise;
using HarmonyLib;
using Monocle;
using MonoMod.ModInterop;
using OopsAllArrowsMod;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using TowerFall;
using Microsoft.Xna.Framework;
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
    public static ExampleModModule Instance;
    public List<Variant> ArrowVariantList = new List<Variant>();
    public static List<CustomArrowFormat> CustomArrowList;
    public ExampleModModule() 
    {
        Instance = this;
    }
    public override Type SettingsType => typeof(ExampleModSettings);
    public static ExampleModSettings Settings => (ExampleModSettings)Instance.InternalSettings;

    public override void LoadContent()
    {
        IceAtlas = Atlas.Create("Atlas/IceArrow.xml", "Atlas/IceArrow.png", true, ContentAccess.ModContent);
        VariantAtlas = Atlas.Create("Atlas/VariantAtlas.xml", "Atlas/VariantAtlas.png", true, ContentAccess.ModContent);
        SlimeAtlas = Atlas.Create("Atlas/SlimeArrow.xml", "Atlas/SlimeArrow.png", true, ContentAccess.ModContent);
        BaitAtlas = Atlas.Create("Atlas/BaitArrow.xml", "Atlas/BaitArrow.png", true, ContentAccess.ModContent);
        PrismTrapAtlas = Atlas.Create("Atlas/PrismTrapArrow.xml", "Atlas/PrismTrapArrow.png", true, ContentAccess.ModContent);
        LandMineAtlas = Atlas.Create("Atlas/LandMineArrow.xml", "Atlas/LandMineArrow.png", true, ContentAccess.ModContent);
    }
    public override void Load()
    {
        
        MyPlayer.Load();
        StartArrowPatch.Load();
        MyTreasueChest.Load();
        typeof(ModExports).ModInterop();
    }
    public override void OnVariantsRegister(MatchVariants variants, bool noPerPlayer = false)
    {
        CustomArrowList = new List<CustomArrowFormat>();
        var info = new VariantInfo(ExampleModModule.VariantAtlas);
        var IceArrow = variants.AddVariant(
            "StartWithIceArrows", info with { Header = "OOPS, ALL ARROWS" }, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var SlimeArrow = variants.AddVariant(
    "StartWithSlimeArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var BaitArrow = variants.AddVariant(
    "StartWithBaitArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var PrismTrapArrow = variants.AddVariant(
   "StartWithPrismTrapArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var LandMineArrow = variants.AddVariant(
   "StartWithLandMineArrows", info, VariantFlags.PerPlayer | VariantFlags.CanRandom, noPerPlayer);
        var IceExclude = variants.AddVariant(
"ExcludeIceArrows", info, VariantFlags.PerPlayer, noPerPlayer);
        var SlimeExclude = variants.AddVariant(
"ExcludeSlimeArrows", info, VariantFlags.PerPlayer, noPerPlayer);
        var BaitExclude = variants.AddVariant(
"ExcludeBaitArrows", info, VariantFlags.PerPlayer, noPerPlayer);
        var PrismTrapExclude = variants.AddVariant(
"ExcludePrismTrapArrows", info, VariantFlags.PerPlayer, noPerPlayer);
        var LandMineExclude = variants.AddVariant(
"ExcludeLandMineArrows", info, VariantFlags.PerPlayer, noPerPlayer);
        var SpawnInTowers = variants.AddVariant(
"SpawnInTowers", info, VariantFlags.CanRandom, true);
        AutoLinkArrow(variants, IceArrow);
        AutoLinkArrow(variants, SlimeArrow);
        AutoLinkArrow(variants, BaitArrow);
        AutoLinkArrow(variants, PrismTrapArrow);
        AutoLinkArrow(variants, LandMineArrow);
        CustomArrowList.Add(new CustomArrowFormat(RiseCore.ArrowsID["IceArrow"], "StartWithIceArrows", "ExcludeIceArrows", "DARKFANG", 1));
        CustomArrowList.Add(new CustomArrowFormat(RiseCore.ArrowsID["SlimeArrow"], "StartWithSlimeArrows", "ExcludeSlimeArrows", "THORNWOOD", 2));
        CustomArrowList.Add(new CustomArrowFormat(RiseCore.ArrowsID["BaitArrow"], "StartWithBaitArrows", "ExcludeBaitArrows", "MOONSTONE", 3));
        CustomArrowList.Add(new CustomArrowFormat(RiseCore.ArrowsID["PrismTrapArrow"], "StartWithPrismTrapArrows", "ExcludePrismTrapArrows", "ASCENSION", 4));
        CustomArrowList.Add(new CustomArrowFormat(RiseCore.ArrowsID["LandMineArrow"], "StartWithLandMineArrows", "ExcludeLandMineArrows", "TOWERFORGE", 5));
    }

    public void AutoLinkArrow(MatchVariants variants, Variant VariantToBeLinked)
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
        MyPlayer.Unload();
        StartArrowPatch.Unload();
        MyTreasueChest.Unload();
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