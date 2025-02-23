using System;
using System.Diagnostics;
using FortRise;
using Monocle;
using MonoMod.ModInterop;
using TowerFall;

namespace TowerBall;


[Fort("com.CoolModder.TowerBall", "TowerBall")]
public class ExampleModModule : FortModule
{
    public static Atlas Atlas;
    public static Atlas MenuAtlas;

    public string LoadPath;
    public static ExampleModModule Instance;
    public static bool TowerBallMode;

    public ExampleModModule() 
    {
        Instance = this;
    }
    public override Type SettingsType => typeof(ExampleModSettings);
    public static ExampleModSettings Settings => (ExampleModSettings)Instance.InternalSettings;

    public override void LoadContent()
    {
        LoadPath = Audio.LOAD_PREFIX;
        Atlas = Content.LoadAtlas("Atlas/atlas.xml", "Atlas/atlas.png");
        MenuAtlas = Content.LoadAtlas("Atlas/menuAtlas.xml", "Atlas/menuAtlas.png");
    }
    public override void OnVariantsRegister(VariantManager manager, bool noPerPlayer = false)
    {
        base.OnVariantsRegister(manager, noPerPlayer);

    
        manager.AddVariant(new CustomVariantInfo("TimedRounds", MenuAtlas["variants/timedRounds"], "Towerball rounds are now timed", CustomVariantFlags.None), true);
        manager.AddVariant(new CustomVariantInfo("HoopTreasure", MenuAtlas["variants/hoopTreasure"], "Every hoop spawns treasure", CustomVariantFlags.None), true);
        manager.AddVariant(new CustomVariantInfo("NoDunking", MenuAtlas["variants/noDunking"], CustomVariantFlags.None), true);
    }
    public override void Load()
    {
        MyPlayerCorpse.Load();
        MyPlayer.Load();
        MyTowerMapData.Load();
        typeof(EigthPlayerImport).ModInterop();
    }

    public override void Unload()
    {
        MyPlayerCorpse.Unload();
        MyPlayer.Unload();
        MyTowerMapData.Unload();
    }
}