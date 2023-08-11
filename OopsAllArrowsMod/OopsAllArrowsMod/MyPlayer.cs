using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerFall;
using Monocle;
using MonoMod.Utils;
using MonoMod.RuntimeDetour;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using System.Diagnostics;
using FortRise;
namespace OopsAllArrowsMod
{
    class MyPlayer : Player
    {
        public static Dictionary<int, bool> SlimePlayer = new Dictionary<int, bool>();
        public static IDetour Hook_MyPlayerRun;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public delegate float orig_MaxRunningSpeed(TowerFall.Player self);
   
        public MyPlayer(int playerIndex, Vector2 position, Allegiance allegiance, Allegiance teamColor, PlayerInventory inventory, HatStates hatState, bool frozen, bool flash, bool indicator) : base(playerIndex, position, allegiance, teamColor, inventory, hatState, frozen, flash, indicator)
        {
        }
        public static void NormalUpdate(On.TowerFall.Player.orig_Update orig, global::TowerFall.Player self)
        {
            orig(self);
            var Slime = self.CollideFirst(Monocle.GameTags.Mud);
            
            if (Slime != null)
            {
                if (Slime is Slime)
                {
                    SlimePlayer[self.PlayerIndex] = true;
                }
                else
                {
                    SlimePlayer[self.PlayerIndex] = true;
                }
            }
            else 
            {
                SlimePlayer[self.PlayerIndex] = false;
            }
        }

        public static float MyMaxPlayerRunningSpeed(orig_MaxRunningSpeed orig, Player self)
        {
            var PlayerData = DynamicData.For(self);
            if (SlimePlayer[self.PlayerIndex])
            {
                if (PlayerData.Get("inMud") != null )
                {
                    return 0.2f;
                }
                else
                {
                    return 0.4f;
                }
            }
            return orig(self);
        }
        public static void CollectArrows(On.TowerFall.Player.orig_CatchArrow orig, global::TowerFall.Player self, global::TowerFall.Arrow arrow)
        {
            if (!self.Level.Session.MatchSettings.Variants.GetCustomVariant("InfiniteWarping"))
            {
                var playerdata = DynamicData.For(self);
                Arrow newarrow;
                if (arrow.CanCatch(self) && !arrow.IsCollectible && arrow.CannotHit != self && (!self.HasShield || !arrow.Dangerous) && arrow != playerdata.Get("lastCaught"))
                {
                    if (arrow.ArrowType == RiseCore.ArrowsID["FreakyArrow"])
                    {
                        arrow.OnPlayerCatch(self);
                        Sounds.sfx_cyanWarp.Play();
                        arrow.RemoveSelf();

                    }
                    else { 
                        orig(self, arrow); 
                    }
                }
                else
                {
                    orig(self, arrow);
                }
            }
            else
            {
                orig(self, arrow);
            }
        }
        public static bool CollectArrowPatchs(On.TowerFall.Player.orig_CollectArrows orig, global::TowerFall.Player self, ArrowTypes[] arrows)
        {
            if (arrows != null && arrows.Length == 1 && arrows[0] == RiseCore.ArrowsID["MiniMechArrow"])
            {
                return true;
            }
            return orig(self, arrows);
        }
        public static void AddedHook(On.TowerFall.Player.orig_Added orig, TowerFall.Player self)
        {
            orig(self);
            SlimePlayer[self.PlayerIndex] = false;
            if (self.Level.Session.MatchSettings.Variants.GetCustomVariant("VarietyPack")[self.PlayerIndex])
            {
                ArrowTypes[] arrows = new ArrowTypes[self.Arrows.Count];
                Random rand = new Random();
                for (int i = 0; i < self.Arrows.Count; i++)
                {
                    
                    int ArrowType = rand.Next(9+ExampleModModule.CustomArrowList.Count);
                    if (ArrowType < 10)
                    { 
                        arrows[i] = (ArrowTypes)(ArrowType+1);
                    }
                    else
                    {
                        arrows[i] = ExampleModModule.CustomArrowList[ArrowType-9].CustomArrow;
                    }
                }
                DynamicData.For(self).Set("Arrows", new ArrowList(arrows));
            }
        }
        public static void Load()
        {
            Hook_MyPlayerRun = new Hook(typeof(Player).GetProperty("MaxRunSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetGetMethod(true), MyMaxPlayerRunningSpeed);

            On.TowerFall.Player.Update += NormalUpdate;
            On.TowerFall.Player.CatchArrow += CollectArrows;
            On.TowerFall.Player.Added += AddedHook;
            On.TowerFall.Player.CollectArrows += CollectArrowPatchs;
        }
        public static void Unload()
        {
            
            Hook_MyPlayerRun.Dispose();
            On.TowerFall.Player.Update -= NormalUpdate;
            On.TowerFall.Player.CatchArrow -= CollectArrows;
            On.TowerFall.Player.Added -= AddedHook;
            On.TowerFall.Player.CollectArrows -= CollectArrowPatchs;
        }
    }
}
