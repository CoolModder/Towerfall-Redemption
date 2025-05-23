using System.Reflection;
using System.Runtime.CompilerServices;
using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;

[CustomArrowPickup("OopsAllArrowsMod/PrismTrapArrow", typeof(PrismTrapArrow))]
public class PrismTrapArrowPickup : ArrowTypePickup
{
    public PrismTrapArrowPickup(Vector2 position, Vector2 targetPosition, ArrowTypes type) : base(position, targetPosition, type)
    {
        Name = "Prism Trap";
        Color = Calc.HexToColor("D399CC");
        ColorB = Calc.HexToColor("6C0D9C");

        var graphic = new Sprite<int>(OopsArrowsModModule.ArrowAtlas["PrismTrapArrowPickup"], 12, 12, 0);
        graphic.Add(0, 0.3f, new int[2] { 0, 0 });
        graphic.Play(0, false);
        graphic.CenterOrigin();
        AddGraphic(graphic);
    }
}

[CustomArrows("OopsAllArrowsMod/PrismTrap", nameof(CreateHud))]
public class PrismTrapArrow : Arrow
{
    // This is automatically been set by the mod loader
    public override ArrowTypes ArrowType { get; set; }
    private bool used, canDie;
    private Image normalImage;
    private Image buriedImage;


    public static Subtexture CreateHud() 
    {
        return OopsArrowsModModule.ArrowAtlas["PrismTrapArrowHud"];
    }

    public PrismTrapArrow() : base()
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
    protected override void HitWall(TowerFall.Platform platform)
    {
        if (!used)
        {
            this.used = true;
            Add(new Coroutine(PrismTrap.CreatePrismTrap(Level, Position, buriedImage.Rotation, PlayerIndex, () => canDie = true)));
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