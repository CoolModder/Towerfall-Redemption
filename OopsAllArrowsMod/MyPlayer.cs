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
                if (PlayerData.Get("InMud") != null)
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
        public static void Load()
        {
            Debugger.Launch();
            Hook_MyPlayerRun = new Hook(typeof(Player).GetProperty("MaxRunSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetGetMethod(true), MyMaxPlayerRunningSpeed);

            On.TowerFall.Player.Update += NormalUpdate;
        }
        public static void Unload()
        {
            
            Hook_MyPlayerRun.Dispose();
            On.TowerFall.Player.Update -= NormalUpdate;
        }
    }
}
