using System.Reflection;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using TowerBall;
using TowerFall;
using FortRise;
using System.Collections.Generic;

internal class MyPlayer : Player
{

    public static Dictionary<int, bool> touchedGroundSinceCollect = new Dictionary<int, bool>(16);

    public static Dictionary<int, float> currentHoldFrames = new Dictionary<int, float>(16);


    public static Dictionary<int, int> HasBasketBall = new Dictionary<int, int>(16);
    public static Dictionary<int, ArrowList> PlayerArrows = new Dictionary<int, ArrowList>(16);
    public static Dictionary<int, Image> BasketBallImages = new Dictionary<int, Image>(16);
    public MyPlayer(int playerIndex, Vector2 position, Allegiance allegiance, Allegiance teamColor, PlayerInventory inventory, HatStates hatState, bool frozen, bool flash, bool indicator)
        : base(playerIndex, position, allegiance, teamColor, inventory, hatState, frozen, flash, indicator)
    {
    }

 
    public static void ctor(On.TowerFall.Player.orig_Added orig, TowerFall.Player self)
    {
        orig(self);
        if(self.Level.Session.MatchSettings.Mode == ModRegisters.GameModeType<TowerBall.TowerBall>())
        {
            ExampleModModule.TowerBallMode = true;
        }
        else
        {
            ExampleModModule.TowerBallMode = false;
        }
        HasBasketBall[self.PlayerIndex] = 0;
        touchedGroundSinceCollect[self.PlayerIndex] = true;
        currentHoldFrames[self.PlayerIndex] = 0f;
        BasketBallImages[self.PlayerIndex] = new OutlineImage(ExampleModModule.Atlas["towerball/ball"])
        {
            Origin = new Vector2(5f, 5f)
        };
    }

    
    public static void ShootArrow(On.TowerFall.Player.orig_ShootArrow orig, global::TowerFall.Player self)
    {
        if (ExampleModModule.TowerBallMode) { 
            PlayerArrows[self.PlayerIndex] = self.Arrows;
            if (HasBasketBall[self.PlayerIndex] > 0)
            {
                var arrowsRegistry = RiseCore.ArrowsRegistry["TowerBall/BasketBall"];
                DynamicData.For(self).Set("Arrows", new ArrowList(new ArrowTypes[] { arrowsRegistry.Types }));
                ((TowerBallRoundLogic)self.Level.Session.RoundLogic).lastThrower = self.PlayerIndex;
                HasBasketBall[self.PlayerIndex] = 0;
            }
        }
        orig(self);
        if (ExampleModModule.TowerBallMode)
        {
            DynamicData.For(self).Set("Arrows", PlayerArrows[self.PlayerIndex]);
        }
    }

    public static bool CollectArrows(On.TowerFall.Player.orig_CollectArrows orig, global::TowerFall.Player self, ArrowTypes[] arrows)
    { 
        var arrowsRegistry = RiseCore.ArrowsRegistry["TowerBall/BasketBall"];
        if (arrows != null && arrows.Length == 1 && arrows[0] ==  arrowsRegistry.Types && ExampleModModule.TowerBallMode)
        {
            HasBasketBall[self.PlayerIndex] = 1;
            touchedGroundSinceCollect[self.PlayerIndex] = true;
            
            return true;
        }
        return orig(self, arrows);
    }

    public static PlayerCorpse Die(On.TowerFall.Player.orig_Die_DeathCause_int_bool_bool orig, global::TowerFall.Player self, DeathCause deathCause, int killerIndex, bool brambled, bool laser)
    {
        
        if (ExampleModModule.TowerBallMode)
        {
            Level level = self.Level;
            while (HasBasketBall[self.PlayerIndex] > 0)
            {
                if (deathCause == DeathCause.JumpedOn && level.GetPlayer(killerIndex) != null)
                {
                    HasBasketBall[killerIndex]++;
                }
                else
                {
                    ((TowerBallRoundLogic)level.Session.RoundLogic).DropBall(self, self.Position + Player.ArrowOffset, self.Facing);
                }
                HasBasketBall[self.PlayerIndex]--;
            }
            if (currentHoldFrames[self.PlayerIndex] > 0f)
            {
                currentHoldFrames[self.PlayerIndex] = 0f;
            }
        }
        return orig(self, deathCause, killerIndex, brambled, laser);
    }

    public static void Update(On.TowerFall.Player.orig_Update orig, global::TowerFall.Player self)
    {
        orig(self);
        if (ExampleModModule.TowerBallMode)
        {   
            BasketBallImages[self.PlayerIndex].Position = self.Position + Player.ArrowOffset + new Vector2((float)self.Facing * 4f, 2f);
            if ((self.CharacterIndex == 8 && self.AltSelect == ArcherData.ArcherTypes.Normal) || (self.CharacterIndex == 6 && self.AltSelect == ArcherData.ArcherTypes.Alt) || (self.CharacterIndex == 7 && self.AltSelect == ArcherData.ArcherTypes.Alt))
            {
                (DynamicData.For(self).Get("bowSprite") as Sprite<string>).Visible = HasBasketBall[self.PlayerIndex] <= 0 && self.Aiming;
            }
            else
            {
                (DynamicData.For(self).Get("bowSprite") as Sprite<string>).Visible = HasBasketBall[self.PlayerIndex] <= 0;
            }
            if (TFGame.PlayerInputs[self.PlayerIndex].MenuAlt2)
            {
            }
            if (self.State == PlayerStates.LedgeGrab)
            {
                touchedGroundSinceCollect[self.PlayerIndex] = false;
            }
            else
            {
                touchedGroundSinceCollect[self.PlayerIndex] &= !self.OnGround;
            }
            if (HasBasketBall[self.PlayerIndex] > 0)
            {
                currentHoldFrames[self.PlayerIndex] += Engine.TimeMult;
            }
            else if (currentHoldFrames[self.PlayerIndex] > 0f)
            {
                currentHoldFrames[self.PlayerIndex] = 0f;
            }
        }
    }

    public static int DuckingUpdate(On.TowerFall.Player.orig_DuckingUpdate orig, global::TowerFall.Player self)
    {
        Level level = self.Level;
        var __state = false;
        if (HasBasketBall[self.PlayerIndex] > 0 && TFGame.PlayerInputs[self.PlayerIndex].GetState().JumpPressed && self.CollideCheck(GameTags.JumpThru, self.Position + Vector2.UnitY) && !self.CollideCheck(GameTags.Solid, self.Position + Vector2.UnitY * 3f) && ExampleModModule.TowerBallMode)
        {
            Entity entity = self.CollideFirst(GameTags.JumpThru, self.Position + Vector2.UnitY);
            if (entity is BasketBallBasket)
            {
                bool flag2 = self.Position.X > 160f;
                Sounds.sfx_devTimeFinalDummy.Play(self.X);
                TowerBallRoundLogic towerBallRoundLogic = (TowerBallRoundLogic)self.Level.Session.RoundLogic;
                while (HasBasketBall[self.PlayerIndex] > 0)
                {
                    towerBallRoundLogic.IncreaseScore(self.PlayerIndex, towerBallRoundLogic.lastThrower, dunk: true, allyoop: false, clean: false, (!flag2) ? 1 : 0);
                    towerBallRoundLogic.AddDeadBall(self.Position, self.Speed);
                    HasBasketBall[self.PlayerIndex]--;
                }
                towerBallRoundLogic.SpawnBallChest(300);
                Explosion.Spawn(level, entity.Position + new Vector2(7.5f, 0f), self.PlayerIndex, plusOneKill: false, triggerBomb: false, bombTrap: false);
                towerBallRoundLogic.AddSlamNotification(entity.Position + (flag2 ? new Vector2(-10f, 0f) : new Vector2(10f, 0f)));
                if ((bool)level.KingIntro)
                {
                    level.KingIntro.Laugh();
                }
                ((BasketBallBasket)entity).DoNetJump();
                __state = true;
            }
        }
        
        if (__state)
        {
            self.Position.Y += 6f;
        }
        if (__state && self.HasShield)
        {
            self.Position.Y += 10f;
        }
        return orig(self);
    }

    public static void Render(On.TowerFall.Player.orig_HUDRender orig, global::TowerFall.Player self, bool wrapped)
    {
        orig(self, wrapped);
        if (HasBasketBall[self.PlayerIndex] > 0 && ExampleModModule.TowerBallMode)
        {
            BasketBallImages[self.PlayerIndex].Render();
        }
    }
    public static void Load()
    {
        On.TowerFall.Player.Added += ctor;
        On.TowerFall.Player.Die_DeathCause_int_bool_bool += Die;
        On.TowerFall.Player.Update += Update;
        On.TowerFall.Player.ShootArrow += ShootArrow;
        On.TowerFall.Player.DuckingUpdate += DuckingUpdate;
        On.TowerFall.Player.HUDRender += Render;
        On.TowerFall.Player.CollectArrows += CollectArrows;
    }
    public static void Unload()
    {
        On.TowerFall.Player.Added -= ctor;
        On.TowerFall.Player.Die_DeathCause_int_bool_bool -= Die;
        On.TowerFall.Player.Update -= Update;
        On.TowerFall.Player.HUDRender -= Render;
        On.TowerFall.Player.DuckingUpdate -= DuckingUpdate;
        On.TowerFall.Player.ShootArrow -= ShootArrow;
        On.TowerFall.Player.CollectArrows -= CollectArrows;
    }

}

