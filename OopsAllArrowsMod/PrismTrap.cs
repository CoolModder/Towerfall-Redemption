using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;

public class Listener : Component
{
    public Action OnEntityRemoved;
    
    public Listener() : base(true, false)
    {
    }

    public override void EntityRemoved()
    {
        base.EntityRemoved();
        OnEntityRemoved?.Invoke();
    }
}

public class PrismTrap : Actor
{
    public override bool FinishPushOnSquishRiding => true;
    public int OwnerIndex { get; private set; }
    
    private FlashingImage image;
    private Listener listener;
    private Solid riding;
    private bool isFalling;
    private float fallSpeed;
    
    public PrismTrap(Vector2 position, float rotation, Solid platform) : base(position)
    {
        Tag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider);
        
        Position = position;
        Depth = 150;
        ScreenWrap = true;
        Pushable = false;
        isFalling = false;
        
        riding = platform;
        if (riding != null)
        {
            listener = new Listener();
            listener.OnEntityRemoved += StartFalling;
            riding.Add(listener);
        }
        
        Collider = new WrapHitbox(8f, 8f, -4f, -4f);
        
        image = new FlashingImage(OopsArrowsModModule.ArrowAtlas["PrismTrap"]);
        image.CenterOrigin();
        image.Rotation = rotation;
        Add(image);
    }

    private void StartFalling()
    {
        riding = null;
        isFalling = true;
        listener.OnEntityRemoved -= StartFalling;
        listener = null;
    }

    public static IEnumerator CreatePrismTrap(Solid platform, Level level, Vector2 at, float rotation, int ownerIndex, Action onComplete)
    {
        PrismTrap MyPrismTrap = new PrismTrap(at, rotation, platform);
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

    public override void Update()
    {
        base.Update();
        
        if (!isFalling && riding != null && (!riding.Collidable || riding.MarkedForRemoval))
           StartFalling(); 

        if (isFalling)
        {
          if (!CheckBelow())
            fallSpeed = Calc.Approach(fallSpeed, 3.6f, 0.2f * Engine.TimeMult);
          image.Rotation = Calc.Approach(image.Rotation, 1.570796f, Engine.TimeMult * 0.1f);
          MoveV(fallSpeed * Engine.TimeMult, HitFloor);
        }
    }

    private void HitFloor(Platform solid)
    {
        fallSpeed = 0;
        isFalling = false;
        
        riding = solid as Solid;
        if (riding != null)
        {
            listener = new Listener();
            listener.OnEntityRemoved += StartFalling;
            riding.Add(listener);
        }

        Position.Y += 3;
        image.Rotation = 1.570796f;
    }

    public override bool IsRiding(Solid solid)
    {
        return riding == solid;
    }
    
    public override bool IsRiding(JumpThru jumpThru)
    {
        return false;
    }

    public override void Removed()
    {
        riding = null;
    }

    public override void OnPlayerCollide(Player player)
    {
        Collidable = false;
        player.StartPrism(OwnerIndex);
        RemoveSelf();
    }
    
    public override void OnExplode(Explosion explosion, Vector2 normal)
    {
        Untag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider);
        RemoveSelf();
    }

    public override void OnShock(ShockCircle shock)
    {
        Untag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider);
        RemoveSelf();
    }
}
