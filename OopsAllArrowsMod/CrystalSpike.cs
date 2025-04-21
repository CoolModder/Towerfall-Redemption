// TowerFall.Ice
using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;
public class Crystal : Actor
{
    private Sprite<int> graphic;
    public int OwnerIndex { get; private set; }
    private Solid riding;
    private float direction;
    public Crystal(Vector2 position, float rotation) : base(position)
    {
        direction = rotation;
        Position = position;
        base.Depth = 149;
        Tag(GameTags.Solid, GameTags.ExplosionCollider, GameTags.ShockCollider);
        ScreenWrap = true;
        Pushable = false;

        base.Collider = new WrapHitbox(12f, 16f, -0f, -0f);

        graphic = new Sprite<int>(OopsArrowsModModule.ArrowAtlas["CrystalSpike"], 16, 32, 0);
        graphic.Add(0, 0.1f, false, new int[4] { 0,1,2,3 });
        graphic.Play(0, false);
        graphic.Origin = new Vector2(-16f, -0f);
        graphic.Rotation = rotation;
        Add(graphic);
    }
    public static IEnumerator CreateCrystal(Level level, Vector2 at, float rotation, int ownerIndex, Action onComplete)
    {
        Crystal MyCrystal = new Crystal(at, rotation);
        MyCrystal.OwnerIndex = ownerIndex;
        level.Add(MyCrystal);
        yield return 0.000001f;
        onComplete?.Invoke();
    }

    public override void DoWrapRender()
    {
        graphic.DrawOutline();
        base.DoWrapRender();
        base.Collider.Render();
    }
    public override void Update()
    {
        base.Update();
        
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
        player.Hurt(DeathCause.Squish, TopLeft);
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
