using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;

[CustomArrows("PrismTrap", "CreateGraphicPickup")]
public class PrismTrapArrow : Arrow
{
    // This is automatically been set by the mod loader
    public override ArrowTypes ArrowType { get; set; }
    private bool used, canDie;
    private Image normalImage;
    private Image buriedImage;

    public PrismTrapArrow() : base()
    {
    }
    
    public static ArrowInfo CreateGraphicPickup() 
    {
        var graphic = new Sprite<int>(OopsArrowsModModule.ArrowAtlas["PrismTrapArrowPickup"], 12, 12, 0);
        graphic.Add(0, 0.3f, new int[2] { 0, 0 });
        graphic.Play(0, false);
        graphic.CenterOrigin();
        var arrowInfo = ArrowInfo.Create(graphic, OopsArrowsModModule.ArrowAtlas["PrismTrapArrowHud"]);
        arrowInfo.Name = "Prism Trap Arrows";
        return arrowInfo;
    }
    
    protected override void Init(LevelEntity owner, Vector2 position, float direction)
    {
        base.Init(owner, position, direction);
        used = canDie = false;
        StopFlashing();
    }
    protected override void CreateGraphics()
    {
        normalImage = new Image(OopsArrowsModModule.ArrowAtlas["PrismTrapArrow"]);
        normalImage.Origin = new Vector2(13f, 3f);
        buriedImage = new Image(OopsArrowsModModule.ArrowAtlas["PrismTrapArrowBuried"]);
        buriedImage.Origin = new Vector2(13f, 3f);
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
    protected override void HitWall(Platform platform)
    {
        if (!used && platform is not JumpThru)
        {
            used = true;
            Add(new Coroutine(PrismTrap.CreatePrismTrap(platform as Solid, Level, Position, buriedImage.Rotation, PlayerIndex, () => canDie = true)));
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
    }
}