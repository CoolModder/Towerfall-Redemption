using System.Reflection;
using System.Runtime.CompilerServices;
using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using TowerFall;

namespace OopsAllArrowsMod;

[CustomArrows("MechArrow", "CreateGraphicPickup")]
public class MechArrow : Arrow
{
    // This is automatically been set by the mod loader
    public override ArrowTypes ArrowType { get; set; }
    private bool used, canDie;
    private Image normalImage;
    private Image buriedImage;

    private Alarm explodeAlarm;
    public bool CanExplode;
    public static ArrowInfo CreateGraphicPickup() 
    {
        var graphic = new Sprite<int>(ExampleModModule.MechAtlas["MechArrowPickup"], 12, 12, 0);
        graphic.Add(0, 0.3f, new int[2] { 0, 0 });
        graphic.Play(0, false);
        graphic.CenterOrigin();
        var arrowInfo = ArrowInfo.Create(graphic, ExampleModModule.MechAtlas["MechArrowHud"]);
        arrowInfo.Name = "Mech Arrows";

        return arrowInfo;
    }

    public MechArrow() : base()
    {
    }
    protected override void Init(LevelEntity owner, Vector2 position, float direction)
    {
        base.Init(owner, position, direction);
        used = (canDie = false);
        CanExplode = false;
        explodeAlarm = Alarm.Create(Alarm.AlarmMode.Persist, Explode, 30);
        explodeAlarm.Start();
        StopFlashing();
    }
    protected override void CreateGraphics()
    {
        normalImage = new Image(ExampleModModule.MechAtlas["MechArrow"]);
        normalImage.Origin = new Vector2(13f, 3f);
        buriedImage = new Image(ExampleModModule.MechAtlas["MechArrowBuried"]);
        buriedImage.Origin = new Vector2(13f, 3f);
        Graphics = new Image[2] { normalImage, buriedImage };
        Add(Graphics);
    }

    protected override void InitGraphics()
    {
        normalImage.Visible = true;
        buriedImage.Visible = false;
    }
    public void Explode()
    {
        CanExplode = true;
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
    public override void Update()
    {
       
        base.Update();
        if (canDie) 
        {
            RemoveSelf();
        }
        if (explodeAlarm.Active)
        {
            explodeAlarm.Update();
        }
        if (State != ArrowStates.Stuck && State != ArrowStates.Buried && State != ArrowStates.LayingOnGround)
        {
            if (Direction > 0f && !used && CanExplode)
            {
                if (!Level.Session.MatchSettings.Variants.GetCustomVariant("DoubleSpread"))
                {
                    var middle = Arrow.Create(RiseCore.ArrowsID["MiniMechArrow"], Owner, Position, Direction);
                    var top = Arrow.Create(RiseCore.ArrowsID["MiniMechArrow"], Owner, Position + new Vector2(0, 1), Direction + 0.6011317f);
                    var bottom = Arrow.Create(RiseCore.ArrowsID["MiniMechArrow"], Owner, Position - new Vector2(0, 1), Direction - 0.6011317f);
                    Level.Add(middle, top, bottom);
                    canDie = true;
                    used = true;
                }
                else
                {
                    var corebottom = Arrow.Create(RiseCore.ArrowsID["MiniMechArrow"], Owner, Position - new Vector2(0, 0.5f), Direction - 0.151f);
                    var coretop = Arrow.Create(RiseCore.ArrowsID["MiniMechArrow"], Owner, Position + new Vector2(0, 0.5f), Direction + 0.151f);
                    var top = Arrow.Create(RiseCore.ArrowsID["MiniMechArrow"], Owner, Position + new Vector2(0, 1), Direction + 0.4511317f);
                    var bottom = Arrow.Create(RiseCore.ArrowsID["MiniMechArrow"], Owner, Position - new Vector2(0, 1), Direction - 0.4511317f);
                    var finaltop = Arrow.Create(RiseCore.ArrowsID["MiniMechArrow"], Owner, Position + new Vector2(0, 1.5f), Direction + 0.7011317f);
                    var finalbottom = Arrow.Create(RiseCore.ArrowsID["MiniMechArrow"], Owner, Position - new Vector2(0, 1.5f), Direction - 0.7011317f);
                    Level.Add(corebottom, coretop, finaltop, finalbottom, top, bottom);
                    canDie = true;
                    used = true;
                }
            }
        }
    }
}