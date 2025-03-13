using System;
using System.Reflection;
using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using TowerFall;

namespace OopsAllArrowsMod;

[CustomArrows("Mech", "CreateGraphicPickup")]
public class MechArrow : TriggerArrow
{
    // This is automatically been set by the mod loader
    public override ArrowTypes ArrowType { get; set; }
    private bool used, canDie;
    private Image normalImage;
    private Image buriedImage;
    private static Action<TriggerArrow, LevelEntity, Vector2, float> BaseInit;
    private Alarm explodeAlarm;
    public bool CanExplode;
    private Counter cannotPickupCounter;
    public static ArrowInfo CreateGraphicPickup() 
    {
        var graphic = new Sprite<int>(OopsArrowsModModule.ArrowAtlas["MechArrowPickup"], 12, 12, 0);
        graphic.Add(0, 0.3f, new int[2] { 0, 0 });
        graphic.Play(0, false);
        graphic.CenterOrigin();
        var arrowInfo = ArrowInfo.Create(graphic, OopsArrowsModModule.ArrowAtlas["MechArrowHud"]);
        arrowInfo.Name = "Mech Arrows";

        return arrowInfo;
    }

    public MechArrow() : base()
    {
    }
    protected override void Init(LevelEntity owner, Vector2 position, float direction)
    {
        cannotPickupCounter = new Counter();
        cannotPickupCounter.Set(0);
        base.Init(owner, position, direction);
        used = (canDie = false);
        CanExplode = false;

        explodeAlarm = Alarm.Create(Alarm.AlarmMode.Persist, Explode, 30);
        explodeAlarm.Start();
        StopFlashing();
    }
    public static void Load()
    {
        BaseInit = CallHelper.CallBaseGen<Arrow, TriggerArrow, LevelEntity, Vector2, float>("Init", BindingFlags.NonPublic | BindingFlags.Instance);
        On.TowerFall.TriggerArrow.SetDetonator_Player += SetDetonatorPlayerPatch;
        On.TowerFall.TriggerArrow.SetDetonator_Enemy += SetDetonatorEnemyPatch;
        On.TowerFall.TriggerArrow.Detonate += DetonatePatch;
        On.TowerFall.TriggerArrow.Init += InitPatch;
        On.TowerFall.TriggerArrow.RemoveDetonator += RemoveDetonatorPatch;
    }

    private static void RemoveDetonatorPatch(On.TowerFall.TriggerArrow.orig_RemoveDetonator orig, TriggerArrow self)
    {
        if (self is MechArrow)
        {
            self.LightVisible = false;
            var dynData = DynamicData.For(self);
            Player player = dynData.Get<Player>("playerDetonator");
            dynData.Set("playerDetonator", null);
            if (player != null)
            {
                player.RemoveTriggerArrow(self);
            }
            dynData.Set("enemyDetonator", null);
            return;
        }
        orig(self);
    }

    private static void InitPatch(On.TowerFall.TriggerArrow.orig_Init orig, TriggerArrow self, LevelEntity owner, Vector2 position, float direction)
    {
        if (self is MechArrow bramble)
        {

            BaseInit(self, owner, position, direction);
            Logger.Log("Working");
            self.LightVisible = true;
            Player playerDetonator = null;
            Enemy enemyDetonator = null;
            var dynData = DynamicData.For(self);
            dynData.Get<Alarm>("primed").Start();
            dynData.Get<Alarm>("enemyDetonateCheck").Stop();
            dynData.Set("playerDetonator", playerDetonator);
            dynData.Set("enemyDetonator", enemyDetonator);
            if (owner is Enemy)
            {
                bramble.SetDetonator(owner as Enemy);
            }

            bramble.used = bramble.canDie = false;
            bramble.StopFlashing();
            return;
        }
        orig(self, owner, position, direction);
    }

    public static void Unload()
    {
        On.TowerFall.TriggerArrow.SetDetonator_Player -= SetDetonatorPlayerPatch;
        On.TowerFall.TriggerArrow.SetDetonator_Enemy -= SetDetonatorEnemyPatch;
        On.TowerFall.TriggerArrow.Detonate -= DetonatePatch;
        On.TowerFall.TriggerArrow.Init -= InitPatch;
        On.TowerFall.TriggerArrow.RemoveDetonator -= RemoveDetonatorPatch;
    }

    protected override void HitWall(TowerFall.Platform platform)
    {

        base.HitWall(platform);
        cannotPickupCounter.Set(15);
    }
    public virtual void Bury(ArrowCushion buryIn, float moveIn = 0f, bool drawHead = false)
    {
        base.Bury(buryIn, moveIn, drawHead);
        cannotPickupCounter.Set(15);
        
        Speed = Vector2.Zero;
    }
    public override bool IsCollectible
    {
        get
        {
            if (!cannotPickupCounter)
            {
                return State >= ArrowStates.Stuck;
            }
            return false;
        }
    }
    private static void DetonatePatch(On.TowerFall.TriggerArrow.orig_Detonate orig, TriggerArrow self)
    {
        if (self is MechArrow brambleSelf)
        {
            DynamicData.For(self).Set("enemyDetonator", null);
            DynamicData.For(self).Set("playerDetonator", null);
            if (self.Scene != null && !self.MarkedForRemoval)
            {
                brambleSelf.Explode();
            }
            return;
        }
        orig(self);
    }

    public override void Render()
    {
        normalImage.DrawOutline();
        buriedImage.DrawOutline();
        normalImage.Render();
        buriedImage.Render();
    }

    private static void SetDetonatorEnemyPatch(On.TowerFall.TriggerArrow.orig_SetDetonator_Enemy orig, TriggerArrow self, Enemy enemy)
    {
        if (self is MechArrow)
        {
            DynamicData.For(self).Set("enemyDetonator", enemy);
            DynamicData.For(self).Get<Alarm>("enemyDetonateCheck").Start();
            return;
        }
        orig(self, enemy);
    }

    private static void SetDetonatorPlayerPatch(On.TowerFall.TriggerArrow.orig_SetDetonator_Player orig, TriggerArrow self, Player player)
    {
        if (self is MechArrow)
        {
            DynamicData.For(self).Set("playerDetonator", player);
            return;
        }
        orig(self, player);
    }
    protected override void CreateGraphics()
    {
        normalImage = new Image(OopsArrowsModModule.ArrowAtlas["MechArrow"]);
        normalImage.Origin = new Vector2(13f, 3f);
        buriedImage = new Image(OopsArrowsModModule.ArrowAtlas["MechArrowBuried"]);
        buriedImage.Origin = new Vector2(10f, 3f);
        Graphics = new Image[2] { normalImage, buriedImage };
        Add(Graphics);
    }

    protected override void InitGraphics()
    {
        normalImage.Visible = true;
        buriedImage.Visible = false;
    }
    public void Explode()
    {
        CanExplode = true;
    }
    
    protected override void SwapToBuriedGraphics()
    {
        Graphics[0].Visible = false;
        Graphics[1].Visible = true;
    }

    protected override void SwapToUnburiedGraphics()
    {
        Graphics[0].Visible = true;
        Graphics[1].Visible = false;
    }

    public override bool CanCatch(LevelEntity catcher)
    {
        return !used && base.CanCatch(catcher);
    }
    public override void Update()
    {
        if ((bool)cannotPickupCounter)
        {
            cannotPickupCounter.Update();
        }
        base.Update();
        if (canDie) 
        {
            RemoveSelf();
        }
        if (explodeAlarm.Active)
        {
            explodeAlarm.Update();
        }
        if (State != ArrowStates.Stuck && State != ArrowStates.Buried && State != ArrowStates.LayingOnGround)
        {
            if (!used && CanExplode)
            {
                if (!Level.Session.MatchSettings.Variants.GetCustomVariant("DoubleSpread"))
                {
                    var middle = Arrow.Create(RiseCore.ArrowsRegistry["MiniMechArrow"].Types, Owner, Position, Direction);
                    var top = Arrow.Create(RiseCore.ArrowsRegistry["MiniMechArrow"].Types, Owner, Position + new Vector2(0, 1), Direction + 0.6011317f);
                    var bottom = Arrow.Create(RiseCore.ArrowsRegistry["MiniMechArrow"].Types, Owner, Position - new Vector2(0, 1), Direction - 0.6011317f);
                    Level.Add(middle, top, bottom);
                    canDie = true;
                    used = true;
                }
                else
                {
                    var corebottom = Arrow.Create(RiseCore.ArrowsRegistry["MiniMechArrow"].Types, Owner, Position - new Vector2(0, 0.5f), Direction - 0.151f);
                    var coretop = Arrow.Create(RiseCore.ArrowsRegistry["MiniMechArrow"].Types, Owner, Position + new Vector2(0, 0.5f), Direction + 0.151f);
                    var top = Arrow.Create(RiseCore.ArrowsRegistry["MiniMechArrow"].Types, Owner, Position + new Vector2(0, 1), Direction + 0.4511317f);
                    var bottom = Arrow.Create(RiseCore.ArrowsRegistry["MiniMechArrow"].Types, Owner, Position - new Vector2(0, 1), Direction - 0.4511317f);
                    var finaltop = Arrow.Create(RiseCore.ArrowsRegistry["MiniMechArrow"].Types, Owner, Position + new Vector2(0, 1.5f), Direction + 0.7011317f);
                    var finalbottom = Arrow.Create(RiseCore.ArrowsRegistry["MiniMechArrow"].Types, Owner, Position - new Vector2(0, 1.5f), Direction - 0.7011317f);
                    Level.Add(corebottom, coretop, finaltop, finalbottom, top, bottom);
                    canDie = true;
                    used = true;
                }
            }
        }
    }
}