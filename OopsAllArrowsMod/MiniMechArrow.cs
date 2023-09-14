using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;

[CustomArrows("MiniMechArrow", "CreateGraphicPickup")]
public class MiniMechArrow : Arrow
{
    // This is automatically been set by the mod loader
    public override ArrowTypes ArrowType { get; set; }
    private bool used, canDie;
    private Image normalImage;
    private Image buriedImage;

    public static ArrowInfo CreateGraphicPickup() 
    {
        var graphic = new Sprite<int>(ExampleModModule.MechAtlas["MechArrowPickup"], 12, 12, 0);
        graphic.Add(0, 0.3f, new int[2] { 0, 0 });
        graphic.Play(0, false);
        graphic.CenterOrigin();
        var arrowInfo = ArrowInfo.Create(graphic, ExampleModModule.MechAtlas["MechArrowHud"]);
        arrowInfo.Name = "MiniMech Arrows";

        return arrowInfo;
    }

    public MiniMechArrow() : base()
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
        normalImage = new Image(ExampleModModule.MechAtlas["MiniMechArrow"]);
        normalImage.Origin = new Vector2(13f, 3f);
        buriedImage = new Image(ExampleModModule.MechAtlas["MiniMechArrowBuried"]);
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
    public override void Update()
    {
       
        base.Update();
        if (canDie) 
        {
            RemoveSelf();
        }
    }
}