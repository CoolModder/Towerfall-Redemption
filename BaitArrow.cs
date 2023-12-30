using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using TowerFall;

namespace OopsAllArrowsMod;

[CustomArrows("Bait", "CreateGraphicPickup")]
public class BaitArrow : Arrow
{
    // This is automatically been set by the mod loader
    public override ArrowTypes ArrowType { get; set; }
    private bool used, canDie;
    private Image normalImage;
    private static TowerFall.QuestSpawnPortal MyPortal;
    private static TowerFall.QuestSpawnPortal SnackPortal;
    private Image buriedImage;


    public static ArrowInfo CreateGraphicPickup() 
    {
        var graphic = new Sprite<int>(OopsArrowsModModule.ArrowAtlas["BaitArrowPickup"], 12, 12, 0);
        graphic.Add(0, 0.3f, new int[2] { 0, 0 });
        graphic.Play(0, false);
        graphic.CenterOrigin();
        var arrowInfo = ArrowInfo.Create(graphic, OopsArrowsModModule.ArrowAtlas["BaitArrowHud"]);
        arrowInfo.Name = "Bait Arrows";
        return arrowInfo;
    }

    public BaitArrow() : base()
    {
    }
    protected override void Init(LevelEntity owner, Vector2 position, float direction)
    {
        base.Init(owner, position, direction);
        used = (canDie = false);
        StopFlashing();
  
    }
    protected override void CreateGraphics()
    {
        normalImage = new Image(OopsArrowsModModule.ArrowAtlas["BaitArrow"]);
        normalImage.Origin = new Vector2(13f, 3f);
        buriedImage = new Image(OopsArrowsModModule.ArrowAtlas["BaitArrowBuried"]);
        buriedImage.Origin = new Vector2(10f, 3f);
        Graphics = new Image[2] { normalImage, buriedImage };
        Add(Graphics);
    }

    protected override void InitGraphics()
    {
        normalImage.Visible = true;
        buriedImage.Visible = false;
    }

    protected override void SwapToBuriedGraphics()
    {
        normalImage.Visible = false;
        buriedImage.Visible = true;
    }

    protected override void SwapToUnburiedGraphics()
    {
        normalImage.Visible = true;
        buriedImage.Visible = false;
    }

    public override bool CanCatch(LevelEntity catcher)
    {
        return !used && base.CanCatch(catcher);
    }
    protected override void HitWall(TowerFall.Platform platform)
    {
        if (!used && !Level.Session.CurrentLevel.Ending)
        {
            this.used = true;
            Vector2 PortalPosition = new Vector2(-1000, -1000);
            List<Vector2> xMLPositions = Level.GetXMLPositions("Spawner");
            foreach (Vector2 position in xMLPositions)
            {
                if (Vector2.Distance(Position, position) < Vector2.Distance(Position, PortalPosition))
                {
                    PortalPosition = position;
                }
            }
            MyPortal = new TowerFall.QuestSpawnPortal(PortalPosition, null);
            SnackPortal = new TowerFall.QuestSpawnPortal(Position, null);
            Level.Add(MyPortal, SnackPortal);
            MyPortal.Appear();
            SnackPortal.Appear();
            if (!Level.Session.MatchSettings.Variants.GetCustomVariant("ChaoticBaits"))
            {
                MyPortal.SpawnEnemy(Calc.Random.Choose<string>("Bat", "Slime", "Crow", "Cultist"));
            }
            else
            {
                MyPortal.SpawnEnemy(Calc.Random.Choose<string>("TechnoMage", "ScytheCultist", "BombBat", "Worm"));
            }
            canDie = true;
        }

        base.HitWall(platform);
    }
    public override void Update()
    {
       
        base.Update();
        if (canDie) 
        {
            if (MyPortal != null)
            {
                SnackPortal.ForceDisappear();
                MyPortal.ForceDisappear();
            }
            RemoveSelf();
        }
    }

}