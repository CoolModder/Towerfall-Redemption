// TowerFall.Ice
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;
public class PrismTrap : Actor
{
    private FlashingImage image;
    public int OwnerIndex { get; private set; }
    private Solid riding;
    public PrismTrap(Vector2 position, float rotation) : base(position)
    {
        Position = position;
        base.Depth = 150;
        Tag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider);
        ScreenWrap = true;
        Pushable = false;
        base.Collider = new WrapHitbox(8f, 8f, -4f, -4f);
        image = new FlashingImage(OopsArrowsModModule.ArrowAtlas["PrismTrap"]);
        image.CenterOrigin();
        image.Rotation = rotation;
        Add(image);
    }
    public static IEnumerator CreatePrismTrap(Level level, Vector2 at, float rotation, int ownerIndex, Action onComplete)
    {
        PrismTrap MyPrismTrap = new PrismTrap(at, rotation);
        MyPrismTrap.OwnerIndex = ownerIndex;
        level.Add(MyPrismTrap);
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
        Collidable = false;
        player.StartPrism(OwnerIndex);
        RemoveSelf();
    }
    public override void OnExplode(Explosion explosion, Vector2 normal)
    {
        RemoveSelf();
    }

    public override void OnShock(ShockCircle shock)
    {
        RemoveSelf();
    }

}
