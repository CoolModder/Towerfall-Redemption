using System.Reflection;
using System.Runtime.CompilerServices;
using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;

// [CustomArrowPickup("OopsAllArrowsMod/ShockArrow", typeof(ShockArrow))]
public class ShockArrowPickup : ArrowTypePickup
{
    public ShockArrowPickup(Vector2 position, Vector2 targetPosition, ArrowTypes type) : base(position, targetPosition, type)
    {
        Name = "Shock";
        Color = Calc.HexToColor("F1F6FF");
        ColorB = Calc.HexToColor("A4C6FF");

        var graphic = new Sprite<int>(OopsArrowsModModule.ArrowAtlas["ShockArrowPickup"], 12, 12, 0);
        graphic.Add(0, 0.3f, new int[2] { 0, 0 });
        graphic.Play(0, false);
        graphic.CenterOrigin();
        AddGraphic(graphic);
    }
}

//[CustomArrows("OopsAllArrowsMod/Shock", nameof(CreateHud))]
public class ShockArrow : Arrow
{
    // This is automatically been set by the mod loader
    public override ArrowTypes ArrowType { get; set; }
    private bool used, canDie;
    private Sprite<int> normalImage;
    private Sprite<int> buriedImage;


    public static Subtexture CreateHud() 
    {
        return OopsArrowsModModule.ArrowAtlas["ShockArrowHud"];
    }

    public ShockArrow() : base()
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
        normalImage = new Sprite<int>(OopsArrowsModModule.ArrowAtlas["ShockArrow"], 13, 4, 0);
        normalImage.Add(0, 0.3f, new int[2] { 0, 1 });
        normalImage.Play(0, false);
        normalImage.Origin = new Vector2(13f, 3f);
        buriedImage = new Sprite<int>(OopsArrowsModModule.ArrowAtlas["ShockArrowBuried"], 13, 14, 0);
        buriedImage.Add(0, 0.3f, new int[2] { 0, 1 });
        buriedImage.Play(0, false);
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
    protected override void HitWall(Platform platform)
    {
        if (!used && !(bool)BuriedIn) 
        {
            this.used = true;
            Add(new Coroutine(Shock.CreateShock(Level, Position, buriedImage.Rotation, PlayerIndex, () => canDie = true)));
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