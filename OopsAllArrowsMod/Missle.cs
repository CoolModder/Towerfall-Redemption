using System.Reflection;
using System.Runtime.CompilerServices;
using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using TowerFall;

namespace OopsAllArrowsMod;

[CustomArrows("MissleArrow", "CreateGraphicPickup")]
public class MissleArrow : Arrow
{
    // This is automatically been set by the mod loader
    public override ArrowTypes ArrowType { get; set; }
    private bool used, canDie;
    
    private const float SEEK_TURN = (float)Math.PI / 160f;

    private const int TURN_START_COOLDOWN = 6;

    private const int TURN_COOLDOWN = 5;

    private bool turning;

    private Counter canTurnCounter;

    private WrapHitbox turnHitbox;

    private Image normalImage;

    private Image buriedImage;
    protected override float SeekMinDistSq
    {
        get
        {
            if (turning)
            {
                return 100f;
            }
            return 0f;
        }
    }

    protected override float SeekRadiusSq
    {
        get
        {
            if (turning)
            {
                return 25600f;
            }
            return 6400f;
        }
    }

    protected override float SeekMaxAngle
    {
        get
        {
            if (turning)
            {
                return (float)Math.PI * 2f / 15f;
            }
            return (float)Math.PI / 6f;
        }
    }
    public int Turns { get; private set; }
    public static ArrowInfo CreateGraphicPickup() 
    {
        var graphic = new Sprite<int>(ExampleModModule.MissleAtlas["MissleArrowPickup"], 12, 12, 0);
        graphic.Add(0, 0.3f, new int[2] { 0, 0 });
        graphic.Play(0, false);
        graphic.CenterOrigin();
        var arrowInfo = ArrowInfo.Create(graphic, ExampleModModule.MissleAtlas["MissleArrowHud"]);
        arrowInfo.Name = "Missles";
        return arrowInfo;
    }

    public MissleArrow() : base()
    {
    }
    protected override void Init(LevelEntity owner, Vector2 position, float direction)
    {
        base.Init(owner, position, direction);
        used = (canDie = false);
        StopFlashing();
        LightRadius = 30f;
        canTurnCounter = new Counter();
        turnHitbox = new WrapHitbox(6f, 6f, -3f, -3f);
        LightVisible = true;
        canTurnCounter.Set(6);

    }
    public override void ShootUpdate()
    {
        LevelEntity levelEntity = FindSeekTarget();
        if ((bool)levelEntity)
        {
            if (!base.Level.Session.MatchSettings.Variants.NoSeekingArrows[base.PlayerIndex])
            {
                float num = (float)Math.PI / 180f * Engine.TimeMult;
                if (base.Level.Session.MatchSettings.Variants.SuperSeekingArrows[base.PlayerIndex])
                {
                    num *= 2f;
                }
                base.Direction += MathHelper.Clamp(Calc.AngleDiff(base.Direction, Calc.Angle(Position, levelEntity.Position + levelEntity.SeekOffset)), 0f - num, num);
                Speed = Calc.AngleToVector(base.Direction, StartSpeed);
            }
            levelEntity.OnArrowSeeking(this);
        }
        else if ((bool)canTurnCounter)
        {
            canTurnCounter.Update();
        }
        else if (!TryTurn((float)Math.PI / 2f))
        {
            TryTurn(-(float)Math.PI / 2f);
        }
    }

    private bool TryTurn(float turnAngle)
    {
        float num = base.Direction;
        base.Direction += turnAngle;
        turning = true;
        LevelEntity levelEntity = FindSeekTarget(1);
        turning = false;
        if ((bool)levelEntity)
        {
            SmallShock smallShock = Cache.Create<SmallShock>();
            smallShock.Init(Position);
            base.Level.Add(smallShock);
            Sounds.sfx_boltArrowTurn.Play(base.X);
            base.Direction = WrapMath.WrapAngle(Position, levelEntity.Position + levelEntity.SeekOffset);
            Speed = Calc.AngleToVector(base.Direction, StartSpeed);
            canTurnCounter.Set(5);
            Turns++;
            return true;
        }
        base.Direction = num;
        return false;
    }

    protected override bool SightCheckTarget(Vector2 seekTo)
    {
        if (turning)
        {
            if (!base.SightCheckTarget(seekTo))
            {
                return false;
            }
            base.Collider = turnHitbox;
            Vector2 vector = WrapMath.Shortest(Position, seekTo);
            vector.Normalize();
            vector *= 3f;
            _ = Position + vector;
            int num = (int)(WrapMath.WrapDistance(Position, seekTo) / 3f);
            for (int i = 2; i < num - 1; i++)
            {
                if (CollideCheck(GameTags.Solid, Position + vector * i))
                {
                    base.Collider = NormalHitbox;
                    return false;
                }
            }
            base.Collider = NormalHitbox;
            return true;
        }
        return base.SightCheckTarget(seekTo);
    }
    protected override void CreateGraphics()
    {
        normalImage = new Image(ExampleModModule.MissleAtlas["MissleArrow"]);
        normalImage.Origin = new Vector2(11f, 3f);
        buriedImage = new Image(ExampleModModule.MissleAtlas["MissleArrowBuried"]);
        buriedImage.Origin = new Vector2(11f, 3f);
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
        if (!used)
        {
            this.used = true;
            Explosion.Spawn(platform.Level, Position, PlayerIndex, true, false, false);
            canDie = true;
        }

        base.HitWall(platform);
    }
    public override void Update()
    {
       
        base.Update();
        if (canDie) 
        {
            RemoveSelf();
        }
        if ((bool)BuriedIn)
        {
            Explosion.Spawn(base.Level, Position, PlayerIndex, true, false, false);
            canDie = true;
        }
    }

    public override void OnPlayerCollide(Player player)
    {
        Explosion.Spawn(player.Level, Position, PlayerIndex, true, false, false);
        canDie = true;
    }
}