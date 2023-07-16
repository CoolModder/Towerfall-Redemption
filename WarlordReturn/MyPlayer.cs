using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using TowerFall;
using FortRise;
using Warlord;

internal class MyPlayer : Player
{
    public static Dictionary<int, int> HasWarlordHelm = new Dictionary<int, int>(16);
    public static Dictionary<int, int> HoldWarlordHelm = new Dictionary<int, int>(16);
    public static Dictionary<int, OutlineImage> SkullHead = new Dictionary<int, OutlineImage>();

    public MyPlayer(int playerIndex, Vector2 position, Allegiance allegiance, Allegiance teamColor, PlayerInventory inventory, HatStates hatState, bool frozen, bool flash, bool indicator)
        : base(playerIndex, position, allegiance, teamColor, inventory, hatState, frozen, flash, indicator)
    {
   
    }


    public static void ctor(On.TowerFall.Player.orig_Added orig, TowerFall.Player self) { 
        orig(self);
        HasWarlordHelm[self.PlayerIndex] = 0;
        HoldWarlordHelm[self.PlayerIndex] = 0;
        SkullHead[self.PlayerIndex] = new OutlineImage(ExampleModModule.Atlas["warlord/helmPlayer"])
        {
            Origin = new Vector2(0f, 0f)
        };
    }
    public static PlayerCorpse Die(On.TowerFall.Player.orig_Die_DeathCause_int_bool_bool orig, global::TowerFall.Player self, DeathCause deathCause, int killerIndex, bool brambled, bool laser)
    {
       
        Level level = self.Level;
        while (HasWarlordHelm[self.PlayerIndex] > 0)
        {
            if (deathCause == DeathCause.JumpedOn && level.GetPlayer(killerIndex) != null)
            {
                HasWarlordHelm[killerIndex]++;
            }
            else
            {
                ((TowerBallRoundLogic)level.Session.RoundLogic).DropHelm(self, self.Position + Player.ArrowOffset, self.Facing);
            }
            HasWarlordHelm[self.PlayerIndex]--;
        }
        return orig(self, deathCause, killerIndex, brambled, laser);
    }


    public static void Update(On.TowerFall.Player.orig_Update orig, global::TowerFall.Player self)
    {
        Level level = self.Level;
        Entity entity = self.CollideFirst(GameTags.Hat);
        if (entity != null)
        {
            if (entity is WarlordHelm)
            { 
                HasWarlordHelm[self.PlayerIndex]++;
                entity.RemoveSelf();
            }
        }
        
       
        if(HasWarlordHelm[self.PlayerIndex] > 0)
        {
            
            HoldWarlordHelm[self.PlayerIndex]++;

            if (HoldWarlordHelm[self.PlayerIndex] >= 1000 * ExampleModModule.Settings.TimeToScore)
            {
                ((TowerBallRoundLogic)level.Session.RoundLogic).IncreaseScore(self);
            }
        }
        orig(self);
    }
    public static void Render(On.TowerFall.Player.orig_HUDRender orig, global::TowerFall.Player self, bool wrapped)
    {
        orig(self, wrapped);
        if (HasWarlordHelm[self.PlayerIndex] > 0)
        {
            SkullHead[self.PlayerIndex].Position = new Vector2(self.Position.X - 8f, self.Position.Y - 38f);
            SkullHead[self.PlayerIndex].Render();
        }
    }
    public static void Load()
    {
        On.TowerFall.Player.Added += ctor;
        On.TowerFall.Player.Die_DeathCause_int_bool_bool += Die;
        On.TowerFall.Player.Update += Update;
        On.TowerFall.Player.HUDRender += Render;
    }
    public static void Unload()
    {
        On.TowerFall.Player.Added -= ctor;
        On.TowerFall.Player.Die_DeathCause_int_bool_bool -= Die;
        On.TowerFall.Player.Update -= Update;
        On.TowerFall.Player.HUDRender -= Render;
    }
}


