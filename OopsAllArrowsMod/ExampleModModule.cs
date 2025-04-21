using FortRise;
using Monocle;
using MonoMod.ModInterop;
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
        ArrowAtlas = Content.LoadAtlas("Atlas/ArrowAtlas.xml", "Atlas/ArrowAtlas.png");
        VariantAtlas = Content.LoadAtlas("Atlas/VariantAtlas.xml", "Atlas/VariantAtlas.png");
        SpriteData = Content.LoadSpriteData("Atlas/SpriteData.xml", ArrowAtlas);
    }

    public override void Initialize()
    {
    }

    public override void Load()
    {
        MechArrow.Load();
        MyPlayer.Load();
        typeof(ExplosiveImports).ModInterop();
    }
    


    public override void OnVariantsRegister(VariantManager manager, bool noPerPlayer = false)
    {
        base.OnVariantsRegister(manager, noPerPlayer);

        manager.AddArrowVariant(ModRegisters.ArrowData<IceArrow>(), VariantAtlas["variants/startWithIceArrows"], VariantAtlas["variants/excludeIceArrows"]);
        manager.AddArrowVariant(ModRegisters.ArrowData<SlimeArrow>(), VariantAtlas["variants/startWithSlimeArrows"], VariantAtlas["variants/excludeSlimeArrows"]);
        manager.AddArrowVariant(ModRegisters.ArrowData<BaitArrow>(), VariantAtlas["variants/startWithBaitArrows"], VariantAtlas["variants/excludeBaitArrows"]);
        manager.AddArrowVariant(ModRegisters.ArrowData<PrismTrapArrow>(), VariantAtlas["variants/startWithPrismTrapArrows"], VariantAtlas["variants/excludePrismTrapArrows"]);
        manager.AddArrowVariant(ModRegisters.ArrowData<LandMineArrow>(), VariantAtlas["variants/startWithLandMineArrows"], VariantAtlas["variants/excludeLandMineArrows"]);
        manager.AddArrowVariant(ModRegisters.ArrowData<MissleArrow>(), VariantAtlas["variants/startWithMissleArrows"], VariantAtlas["variants/excludeMissleArrows"]);
        manager.AddArrowVariant(ModRegisters.ArrowData<FreakyArrow>(), VariantAtlas["variants/startWithFreakyArrows"], VariantAtlas["variants/excludeFreakyArrows"]);
        manager.AddArrowVariant(ModRegisters.ArrowData<TornadoArrow>(), VariantAtlas["variants/startWithTornadoArrows"], VariantAtlas["variants/excludeTornadoArrows"]);
        manager.AddArrowVariant(ModRegisters.ArrowData<MechArrow>(), VariantAtlas["variants/startWithMechArrows"], VariantAtlas["variants/excludeMechArrows"]);
        manager.AddArrowVariant(ModRegisters.ArrowData<BoomerangArrow>(), VariantAtlas["variants/startWithBoomerangArrows"], VariantAtlas["variants/excludeBoomerangArrows"]);
        manager.AddArrowVariant(ModRegisters.ArrowData<NyanArrow>(), VariantAtlas["variants/startWithNyanArrows"], VariantAtlas["variants/excludeNyanArrows"]);
        //manager.AddArrowVariant(RiseCore.ArrowsRegistry["Shock"], VariantAtlas["variants/startWithShockArrows"], VariantAtlas["variants/excludeShockArrows"]);
        manager.AddVariant(new CustomVariantInfo("DoubleSpread", VariantAtlas["variants/doubleSpread"], "MECH ARROWS SPLIT INTO 6 INSTEAD OF 3", CustomVariantFlags.CanRandom), true);
        manager.AddVariant(new CustomVariantInfo("SonicBoom", VariantAtlas["variants/sonicBoom"], "WHY DID I STICK A LANDMINE ON A BOOMERANG?", CustomVariantFlags.CanRandom), true);
        manager.AddVariant(new CustomVariantInfo("ChaoticBaits", VariantAtlas["variants/chaoticBaits"], "SUMMONS THE RECKONING", CustomVariantFlags.CanRandom), true);
        manager.AddVariant(new CustomVariantInfo("InfiniteWarping", VariantAtlas["variants/infiniteWarping"], "FREAKY ARROWS ARE NOT DESTROYED ON WARP", CustomVariantFlags.CanRandom), true);
    }

   
       // var VarietyPack = variants.AddVariant("VarietyPack", info with { Description = "START WITH MULTIPLE KINDS OF ARROWS" }, VariantFlags.PerPlayer | VariantFlags.CanRandom, true); -- A reminder of the past

    public override void Unload()
    {
        MechArrow.Unload();
        MyPlayer.Unload();
    }
}

[ModImportName("ExplosionLibraryExport")]
public static class ExplosiveImports
{
    public static Func<Level, Vector2, int, bool, bool, bool, bool> SpawnSmall;
    public static Func<Level, Vector2, int, bool, bool> SpawnSmallSuper;
}