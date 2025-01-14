using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;
public class LandMine : Actor
{
    public override bool FinishPushOnSquishRiding => true;
    public int OwnerIndex { get; private set; }
    
    private FlashingImage image;
    private Listener listener;
    
    private Solid riding;
    private bool used;
    private bool isFalling;
    private float fallSpeed;
    
    public LandMine(Vector2 position, float rotation, Solid platform) : base(position)
    {
        Tag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider, GameTags.LavaCollider, GameTags.Target, GameTags.LightSource);
        
        Position = position;
        Depth = 150;
        ScreenWrap = true;
        Pushable = false;
        isFalling = false;
        used = false;
        Seek = true;
        
        riding = platform;
        if (riding != null)
        {
            listener = new Listener();
            listener.OnEntityRemoved += StartFalling;
            riding.Add(listener);
        }
            
        Collider = new WrapHitbox(8f, 8f, -4f, -4f);
        
        image = new FlashingImage(OopsArrowsModModule.ArrowAtlas["LandMine"]);
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

    public static IEnumerator CreateLandMine(Solid platform, Level level, Vector2 at, float rotation, int ownerIndex, Action onComplete)
    {
        LandMine MyLandMine = new LandMine(at, rotation, platform);
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
    
    public override bool IsRiding(JumpThru jumpThru)
    {
        return false;
    }

    public override void Removed()
    {
        riding = null;
    }

    private void Use(Level level)
    {
        used = true;
        Collidable = false;
        Explosion.Spawn(level, Position, OwnerIndex, false, false, false);
        Untag(GameTags.PlayerCollider, GameTags.ExplosionCollider, GameTags.ShockCollider, GameTags.LavaCollider, GameTags.Target, GameTags.LightSource);
        RemoveSelf();
    }
    
    public override void OnPlayerCollide(Player player)
    {
        if (!used)
        {
            Use(player.Level);
        }
    }
    
    public override void OnExplode(Explosion explosion, Vector2 normal)
    {
        if (!used)
        {
            Use(explosion.Level);
        }
    }
    public override bool OnArrowHit(Arrow arrow)
    {   if (!used)
        {
            Use(arrow.Level);
        }
        return true;
    }
    public override void OnShock(ShockCircle shock)
    {
        if (!used)
        {
            Use(shock.Level);
        }
    }

    public override void OnLavaCollide(Lava lava)
    {
        if (!used)
        {
            Use(lava.Level);
        }
    }

}
