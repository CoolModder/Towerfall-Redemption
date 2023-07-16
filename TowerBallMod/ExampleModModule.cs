using FortRise;
using HarmonyLib;
using Monocle;
using MonoMod.ModInterop;
using System.Diagnostics;
using TowerFall;
using TowerBall;

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
        Audio.LOAD_PREFIX = Content.GetContentPath("SFX/");
        BasketBallBounce = new SFX("BOUNCYBALL", true);
        Audio.LOAD_PREFIX = LoadPath;
        Atlas = Content.LoadAtlas("Atlas/atlas.xml", "Atlas/atlas.png");
        MenuAtlas = Content.LoadAtlas("Atlas/menuAtlas.xml", "Atlas/menuAtlas.png");
        FakeVersusTowerballData.Load(0, Content.GetContentPath("Levels/Versus/00 - Sacred Ground"));
        FakeVersusTowerballData.Load(1, Content.GetContentPath("Levels/Versus/01 - Twilight Spire"));
        FakeVersusTowerballData.Load(2, Content.GetContentPath("Levels/Versus/02 - Backfire"));
        FakeVersusTowerballData.Load(3, Content.GetContentPath("Levels/Versus/03 - Flight"));
        FakeVersusTowerballData.Load(4, Content.GetContentPath("Levels/Versus/04 - Mirage"));
        FakeVersusTowerballData.Load(5, Content.GetContentPath("Levels/Versus/05 - Thornwood"));
        FakeVersusTowerballData.Load(6, Content.GetContentPath("Levels/Versus/06 - Frostfang Keep"));
        FakeVersusTowerballData.Load(7, Content.GetContentPath("Levels/Versus/07 - Kings Court"));
        FakeVersusTowerballData.Load(8, Content.GetContentPath("Levels/Versus/08 - Sunken City"));
        FakeVersusTowerballData.Load(9, Content.GetContentPath("Levels/Versus/09 - Moonstone"));
        FakeVersusTowerballData.Load(10, Content.GetContentPath("Levels/Versus/10 - TowerForge"));
        FakeVersusTowerballData.Load(11, Content.GetContentPath("Levels/Versus/11 - Ascension"));
        FakeVersusTowerballData.Load(12, Content.GetContentPath("Levels/Versus/12 - The Amaranth"));
        FakeVersusTowerballData.Load(13, Content.GetContentPath("Levels/Versus/13 - Dreadwood"));
        FakeVersusTowerballData.Load(14, Content.GetContentPath("Levels/Versus/14 - Darkfang"));
        FakeVersusTowerballData.Load(15, Content.GetContentPath("Levels/Versus/15 - Cataclysm"));
    }

    public override void Load()
    {
        
        MyPlayerCorpse.Load();
        typeof(ModExports).ModInterop();
        MyVersusLevelSystem.Load();
        MyPlayer.Load();
    }

    public override void Unload()
    {
        MyPlayerCorpse.Unload();
        MyVersusLevelSystem.Unload();
        MyPlayer.Unload();
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