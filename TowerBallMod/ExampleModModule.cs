using System;
using FortRise;
using Monocle;
using MonoMod.ModInterop;

namespace TowerBall;


[Fort("com.CoolModder.TowerBall", "TowerBall")]
public class ExampleModModule : FortModule
{
    public static Atlas Atlas;
    public static Atlas MenuAtlas;

    public string LoadPath;
    public static ExampleModModule Instance;
    public static SFX BasketBallBounce;
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
        BasketBallBounce = Content.LoadSFX("SFX/BOUNCYBALL.wav");
        Atlas = Content.LoadAtlas("Atlas/atlas.xml", "Atlas/atlas.png");
        MenuAtlas = Content.LoadAtlas("Atlas/menuAtlas.xml", "Atlas/menuAtlas.png");
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