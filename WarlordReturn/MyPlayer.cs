using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using TowerFall;
using FortRise;
using Warlord;

[HarmonyPatch]
internal class MyPlayer : Player
{
    public static Dictionary<int, int> HasWarlordHelm = new Dictionary<int, int>(16);
    public static Dictionary<int, int> HoldWarlordHelm = new Dictionary<int, int>(16);
    public static Dictionary<int, OutlineImage> SkullHead = new Dictionary<int, OutlineImage>();

    public MyPlayer(int playerIndex, Vector2 position, Allegiance allegiance, Allegiance teamColor, PlayerInventory inventory, HatStates hatState, bool frozen, bool flash, bool indicator)
        : base(playerIndex, position, allegiance, teamColor, inventory, hatState, frozen, flash, indicator)
    {
   
    }

    [HarmonyPatch(typeof(Player), "Added")]
    [HarmonyPostfix]
    public static void ctor(Player __instance)
    {
        HasWarlordHelm[__instance.PlayerIndex] = 0;
        HoldWarlordHelm[__instance.PlayerIndex] = 0;
        SkullHead[__instance.PlayerIndex] = new OutlineImage(ExampleModModule.Atlas["warlord/helmPlayer"])
        {
            Origin = new Vector2(0f, 0f)
        };
    }

    [HarmonyPatch(typeof(Player), "Die", new Type[] {typeof(DeathCause), typeof(int), typeof(bool), typeof(bool)})]
    [HarmonyPrefix]
    public static void Die(Player __instance, DeathCause deathCause, int killerIndex, bool brambled = false, bool laser = false)
    {
        Level level = __instance.Level;
        while (HasWarlordHelm[__instance.PlayerIndex] > 0)
        {
            if (deathCause == DeathCause.JumpedOn && level.GetPlayer(killerIndex) != null)
            {
                HasWarlordHelm[killerIndex]++;
            }
            else
            {
                ((TowerBallRoundLogic)level.Session.RoundLogic).DropHelm(__instance, __instance.Position + Player.ArrowOffset, __instance.Facing);
            }
            HasWarlordHelm[__instance.PlayerIndex]--;
        }
    }


    [HarmonyPatch(typeof(Player), "Update")]
    [HarmonyPrefix]
    public static void Update(Player __instance, ref Sprite<string> ___bowSprite)
    {
        Level level = __instance.Level;
        Entity entity = __instance.CollideFirst(GameTags.Hat);
        if (entity != null)
        {
            if (entity is WarlordHelm)
            { 
                HasWarlordHelm[__instance.PlayerIndex]++;
                entity.RemoveSelf();
            }
        }
        
       
        if(HasWarlordHelm[__instance.PlayerIndex] > 0)
        {
            
            HoldWarlordHelm[__instance.PlayerIndex]++;

            if (HoldWarlordHelm[__instance.PlayerIndex] >= 1000 * ExampleModModule.Settings.TimeToScore)
            {
                ((TowerBallRoundLogic)level.Session.RoundLogic).IncreaseScore(__instance);
            }
        }
    }
    [HarmonyPatch(typeof(Player), "HUDRender")]
    [HarmonyPrefix]
    [HarmonyFinalizer]
    public static void Render(Player __instance)
    {
        if (HasWarlordHelm[__instance.PlayerIndex] > 0)
        {
            SkullHead[__instance.PlayerIndex].Position = new Vector2(__instance.Position.X - 8f, __instance.Position.Y - 38f);
            SkullHead[__instance.PlayerIndex].Render();
        }
    }
}


