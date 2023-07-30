// TowerFall.Ice
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;
public class Shock : Actor
{
    private Sprite<int> sprite;
    public int OwnerIndex { get; private set; }
    private Solid riding;

    private Vector2 stickDir;
    public const int SHOCK_LIFE = 600;
    private Vector2 moveDir;

    private float moveMult;
    private Alarm deathAlarm;
    private float targetAngle;
    private Facing Facing;
    private Vector2 Speed;
    public Shock(Vector2 position, float rotation) : base(position)
    {
        Position = position;
        base.Depth = 150;
        Tag(GameTags.PlayerCollider);
        ScreenWrap = true;
        Pushable = false;
        base.Collider = new WrapHitbox(16f, 16f, -4f, -4f);
        sprite = ExampleModModule.ShockSpriteData.GetSpriteInt("Shock");
        sprite.Play(0, false);
        sprite.CenterOrigin();
        
        sprite.Rotation = rotation;
        Add(sprite);
        Add(deathAlarm = Alarm.Create(Alarm.AlarmMode.Persist, RemoveSelf));
    }
    public static IEnumerator CreateShock(Level level, Vector2 at, float rotation, int ownerIndex, Action onComplete)
    {
        Shock MyShock = new Shock(at, rotation);
        MyShock.OwnerIndex = ownerIndex;
        level.Add(MyShock);
        yield return 0.000001f;
        onComplete?.Invoke();
    }
    private void UpdateTargetAngle()
    {
        targetAngle = stickDir.Angle() - (float)Math.PI / 2;
    }
    public override void DoWrapRender()
    {
        sprite.DrawOutline();
        base.DoWrapRender();
    }
    public override bool IsRiding(Solid solid)
    {
        return riding == solid;
    }

    protected void OnCollideH(Platform platform)
    {
         Speed.X = 0f;
         stickDir = stickDir.Rotate(-(float)Math.PI / 2f * (float)Facing);
         moveDir = moveDir.Rotate(-(float)Math.PI / 2f * (float)Facing);
         UpdateTargetAngle();
    }

    protected void OnCollideV(Platform platform)
    {
          Speed.Y = 0f;
          stickDir = stickDir.Rotate(-(float)Math.PI / 2f * (float)Facing);
          moveDir = moveDir.Rotate(-(float)Math.PI / 2f * (float)Facing);
          UpdateTargetAngle();
    }
    private int WalkUpdate()
    {
        if (!CollideCheck(GameTags.Solid, Position + stickDir))
        {
            Vector2 vector = stickDir.Rotate((float)Math.PI / 2f * (float)Facing);
            if (!CollideCheck(GameTags.Solid, Position + stickDir * 2f + vector * 4f))
            {
                return 0;
            }
            Position += stickDir;
            while (!CollideCheck(GameTags.Solid, Position + vector * 4f))
            {
                Position += stickDir;
            }
            while (!CollideCheck(GameTags.Solid, Position + vector))
            {
                Position += vector;
            }
            stickDir = vector;
            moveDir = moveDir.Rotate((float)Math.PI / 2f * (float)Facing);
            targetAngle += (float)Math.PI / 2f * (float)Facing;
        }
        Speed.X = Calc.Approach(Speed.X, moveDir.X * 0.4f * moveMult, 0.1f * Engine.TimeMult);
        Speed.Y = Calc.Approach(Speed.Y, moveDir.Y * 0.4f * moveMult, 0.1f * Engine.TimeMult);
        return 1;
    }
    public override void Update()
    {
        sprite.Scale.X = 0 - Facing;
        sprite.Rotation = Calc.AngleApproach(sprite.Rotation, targetAngle, 0.17453292f * Engine.TimeMult);
        base.Update();
    }
    public override void Render()
    {
        base.Render();
        sprite.DrawOutline();
        sprite.Render();
    }
    public override void Removed()
    {
        riding = null;
    }

    public bool IsRiding(JumpThru jumpThru)
    {
        return false;
    }
    public override void OnPlayerCollide(Player player)
    {
        Collidable = false;
        if (base.Level.Session.CanHurtPlayer(OwnerIndex, player.PlayerIndex))
        {
            player.Hurt(DeathCause.Shock, Position, OwnerIndex, Sounds.sfx_boltArrowExplode);
        }
    }
}
