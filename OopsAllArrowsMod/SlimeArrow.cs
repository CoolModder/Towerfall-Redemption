using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using FortRise;
namespace OopsAllArrowsMod;

[CustomArrowPickup("OopsAllArrowsMod/SlimeArrow", typeof(SlimeArrow))]
public class SlimeArrowPickup : ArrowTypePickup
{
    public SlimeArrowPickup(Vector2 position, Vector2 targetPosition, ArrowTypes type) : base(position, targetPosition, type)
    {
        Name = "Slime Arrows";

        var graphic = new Sprite<int>(OopsArrowsModModule.ArrowAtlas["SlimeArrowPickup"], 12, 12, 0);
        graphic.Add(0, 0.3f, new int[2] { 0, 0 });
        graphic.Play(0, false);
        graphic.CenterOrigin();
        AddGraphic(graphic);
    }
}

[CustomArrows("OopsAllArrowsMod/Slime", nameof(CreateHud))]
public class SlimeArrow : Arrow
{
    // This is automatically been set by the mod loader
    public override ArrowTypes ArrowType { get; set; }
    private bool used, canDie;
    private Image normalImage;

    private Image buriedImage;


    public static Subtexture CreateHud() 
    {
        return OopsArrowsModModule.ArrowAtlas["SlimeArrowHud"];
    }

    public SlimeArrow() : base()
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
        normalImage = new Image(OopsArrowsModModule.ArrowAtlas["SlimeArrow"]);
        normalImage.Origin = new Vector2(13f, 3f);
        buriedImage = new Image(OopsArrowsModModule.ArrowAtlas["SlimeArrowBuried"]);
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
        if (!used)
        {
            this.used = true;
            Add(new Coroutine(Slime.CreateSlime(Level, Position, PlayerIndex, () => canDie = true)));
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
        if (Fire.OnFire)
        {
            Explosion.Spawn(Level, Position, PlayerIndex, false, false, false);
            canDie = true;
        }
    }
}