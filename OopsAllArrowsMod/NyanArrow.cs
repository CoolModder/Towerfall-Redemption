using System;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using FortRise;
namespace OopsAllArrowsMod;

[CustomArrowPickup("OopsAllArrowsMod/NyanArrow", typeof(NyanArrow))]
public class NyanArrowPickup : ArrowTypePickup
{
    public NyanArrowPickup(Vector2 position, Vector2 targetPosition, ArrowTypes type) : base(position, targetPosition, type)
    {
        Name = "Nyan Arrows";

        var graphic = new Sprite<int>(OopsArrowsModModule.ArrowAtlas["NyanArrowPickup"], 12, 12, 0);
        graphic.Add(0, 0.3f, new int[2] { 0, 0 });
        graphic.Play(0, false);
        graphic.CenterOrigin();
        AddGraphic(graphic);
    }
}

[CustomArrows("OopsAllArrowsMod/Nyan", nameof(CreateHud))]
public class NyanArrow : Arrow
{
    // This is automatically been set by the mod loader
    public override ArrowTypes ArrowType { get; set; }
    private bool used, canDie;
    private Image normalImage;
    private Image buriedImage;
    private const float SEEK_TURN = (float)Math.PI / 180f;

    private const float SPEED = 5f;
    protected override float StartSpeed => 5f;
    private Alarm explodeAlarm;

    protected override float SeekTurnRate => (float)Math.PI / 180f;
    public static Subtexture CreateHud() 
    {
        return OopsArrowsModModule.ArrowAtlas["NyanArrowHud"];
    }

    public NyanArrow() : base()
    {
    }
    public void Explode()
    {
        Explosion.Spawn(base.Level, Position, PlayerIndex, true, false, false);
        canDie = true;
    }
    protected override void Init(LevelEntity owner, Vector2 position, float direction)
    {
        base.Init(owner, position, direction);
        used = (canDie = false);
        explodeAlarm = Alarm.Create(Alarm.AlarmMode.Persist, Explode, 60);
        explodeAlarm.Start();
        StopFlashing();
        LightRadius = 30f;
    }
    protected override void CreateGraphics()
    {
        normalImage = new Image(OopsArrowsModModule.ArrowAtlas["NyanArrow"]);
        normalImage.Origin = new Vector2(5f, 3f);
        buriedImage = new Image(OopsArrowsModModule.ArrowAtlas["NyanArrowBuried"]);
        buriedImage.Origin = new Vector2(5f, 3f);
        Graphics = new Image[2] { normalImage, buriedImage };
        Add(Graphics);
    }

    public override void ShootUpdate()
    {
        UpdateSeeking();
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
    public override void Update()
    {
       
        base.Update();
        if (base.Level.OnInterval(1))
        {
            Vector2 vector = Calc.AngleToVector(base.Direction + (float)Math.PI, 4f);
        }
        if (explodeAlarm.Active)
        {
            explodeAlarm.Update();
            Add(new Coroutine(NyanTrail.CreateNyanTrail(Level, Position, PlayerIndex)));
        }

        if (canDie)
        { 
            RemoveSelf();
        }
        if ((bool)BuriedIn)
        {
            Explosion.Spawn(base.Level, Position, PlayerIndex, true, false, false);
            canDie = true;
            used = true;
        }
        
    }
    protected override void OnCollideH(Platform platform)
    {
        if (base.State != 0)
        {
            base.OnCollideH(platform);
            return;
        }
        
        Speed.X *= -1f;
        base.Direction = Calc.Angle(Speed);
    }

    protected override void OnCollideV(Platform platform)
    {
        if (base.State != 0)
        {
            base.OnCollideV(platform);
            return;
        }

        Speed.Y *= -1f;
        base.Direction = Calc.Angle(Speed);
   
    }
    public override void HitLava()
    {
        Explosion.Spawn(base.Level, Position, PlayerIndex, true, false, false);
        canDie = true;
        used = true;
    }

}