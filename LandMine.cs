// TowerFall.Ice
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;
public class LandMine : Actor
{
    private FlashingImage image;
    public int OwnerIndex { get; private set; }
    private Solid riding;
    private bool used = false;
    public LandMine(Vector2 position, float rotation) : base(position)
    {
        Position = position;
        base.Depth = 150;
        Tag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider, GameTags.LavaCollider, GameTags.Target, GameTags.LightSource);
        ScreenWrap = true;
        Pushable = false;
        base.Collider = new WrapHitbox(8f, 8f, -4f, -4f);
        image = new FlashingImage(OopsArrowsModModule.ArrowAtlas["LandMine"]);
        image.CenterOrigin();
        image.Rotation = rotation;
        Add(image);
        Seek = true;
    }
    public static IEnumerator CreateLandMine(Level level, Vector2 at, float rotation, int ownerIndex, Action onComplete)
    {
        LandMine MyLandMine = new LandMine(at, rotation);
        MyLandMine.OwnerIndex = ownerIndex;
        level.Add(MyLandMine);
        yield return 0.000001f;
        onComplete?.Invoke();
    }

    public override void DoWrapRender()
    {
        image.DrawOutline();
        base.DoWrapRender();
    }
    public override bool IsRiding(Solid solid)
    {
        return riding == solid;
    }

    public override void Removed()
    {
        riding = null;
    }

    public override bool IsRiding(JumpThru jumpThru)
    {
        return false;
    }
    public override void OnPlayerCollide(Player player)
    {
        if (!used)
        {
            used = true;
            Collidable = false;
            Explosion.Spawn(player.Level, Position, OwnerIndex, false, false, false);
            Untag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider, GameTags.LavaCollider, GameTags.Target, GameTags.LightSource);
            RemoveSelf();
        }
    }
    public override void OnExplode(Explosion explosion, Vector2 normal)
    {
        if (!used)
        {
            used = true;
            Collidable = false;
            Explosion.Spawn(explosion.Level, Position, OwnerIndex, false, false, false);
            Untag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider, GameTags.LavaCollider, GameTags.Target, GameTags.LightSource);
            RemoveSelf();
        }
    }
    public override bool OnArrowHit(Arrow arrow)
    {   if (!used)
        {
            used = true;
            Collidable = false;
            Explosion.Spawn(arrow.Level, Position, OwnerIndex, false, false, false);
            Untag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider, GameTags.LavaCollider, GameTags.Target, GameTags.LightSource);
            RemoveSelf();
        }
        return true;
    }
    public override void OnShock(ShockCircle shock)
    {
        if (!used)
        {
            used = true;
            Collidable = false;
            Explosion.Spawn(shock.Level, Position, OwnerIndex, false, false, false);
            Untag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider, GameTags.LavaCollider, GameTags.Target, GameTags.LightSource);
            RemoveSelf();
        }
    }

    public override void OnLavaCollide(Lava lava)
    {
        if (!used)
        {
            used = true;
            Collidable = false;
            Explosion.Spawn(lava.Level, Position, OwnerIndex, false, false, false);
            Untag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider, GameTags.LavaCollider, GameTags.Target, GameTags.LightSource);
            RemoveSelf();
        }
    }

}
