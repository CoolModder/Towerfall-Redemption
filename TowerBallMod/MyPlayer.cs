using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using TowerBall;
using TowerFall;
using FortRise;
[HarmonyPatch]
internal class MyPlayer : Player
{

    public static Dictionary<int, bool> touchedGroundSinceCollect = new Dictionary<int, bool>(16);

    public static Dictionary<int, float> currentHoldFrames = new Dictionary<int, float>(16);

    private static ConstructorInfo base_Player;

    private static DynamicData BaseData;

    public static Dictionary<int, int> HasBasketBall = new Dictionary<int, int>(16);
    public static Dictionary<int, ArrowList> PlayerArrows = new Dictionary<int, ArrowList>(16);
    public static Dictionary<int, Image> BasketBallImages = new Dictionary<int, Image>(16);
    public MyPlayer(int playerIndex, Vector2 position, Allegiance allegiance, Allegiance teamColor, PlayerInventory inventory, HatStates hatState, bool frozen, bool flash, bool indicator)
        : base(playerIndex, position, allegiance, teamColor, inventory, hatState, frozen, flash, indicator)
    {
    }

    [HarmonyPatch(typeof(Player), "Added")]
    [HarmonyPostfix]
    public static void ctor(Player __instance)
    {
        if(__instance.Level.Session.MatchSettings.CurrentModeName == "TowerBallRoundLogic")
        {

             ExampleModModule.TowerBallMode = true;
        }
        else
        {
            ExampleModModule.TowerBallMode = false;
        }
        HasBasketBall[__instance.PlayerIndex] = 0;
        touchedGroundSinceCollect[__instance.PlayerIndex] = true;
        currentHoldFrames[__instance.PlayerIndex] = 0f;
        BasketBallImages[__instance.PlayerIndex] = new OutlineImage(ExampleModModule.Atlas["towerball/ball"])
        {
            Origin = new Vector2(5f, 5f)
        };
    }

    [HarmonyPatch(typeof(Player), "ShootArrow")]
    [HarmonyPrefix]
    public static void ShootArrow(Player __instance, bool __state)
    {
        if (ExampleModModule.TowerBallMode) { 
            PlayerArrows[__instance.PlayerIndex] = __instance.Arrows;
            if (HasBasketBall[__instance.PlayerIndex] > 0)
            {
                DynamicData.For(__instance).Set("Arrows", new ArrowList(new ArrowTypes[] { RiseCore.ArrowsID["BasketBall"] }));
                ((TowerBallRoundLogic)__instance.Level.Session.RoundLogic).lastThrower = __instance.PlayerIndex;
                HasBasketBall[__instance.PlayerIndex] = 0;
            }
        }
    }

    [HarmonyPatch(typeof(Player), "ShootArrow")]
    [HarmonyPostfix]
    public static void PostShootArrow(Player __instance)
    {
        if (ExampleModModule.TowerBallMode)
        {
            DynamicData.For(__instance).Set("Arrows", PlayerArrows[__instance.PlayerIndex]);
        }
    }

    [HarmonyPatch(typeof(Player), "CollectArrows")]
    [HarmonyPrefix]
    public static bool CollectArrows(Player __instance, bool __result, params ArrowTypes[] arrows)
    { 
        if (arrows != null && arrows.Length == 1 && arrows[0] == RiseCore.ArrowsID["BasketBall"] && ExampleModModule.TowerBallMode)
        {
            Console.WriteLine();
            HasBasketBall[__instance.PlayerIndex] = 1;
            touchedGroundSinceCollect[__instance.PlayerIndex] = true;
            __result = true;
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(Player), "Die", new Type[] {typeof(DeathCause), typeof(int), typeof(bool), typeof(bool)})]
    [HarmonyPrefix]
    public static void Die(Player __instance, DeathCause deathCause, int killerIndex, bool brambled = false, bool laser = false)
    {
        if (ExampleModModule.TowerBallMode)
        {
            Level level = __instance.Level;
            while (HasBasketBall[__instance.PlayerIndex] > 0)
            {
                if (deathCause == DeathCause.JumpedOn && level.GetPlayer(killerIndex) != null)
                {
                    HasBasketBall[killerIndex]++;
                }
                else
                {
                    ((TowerBallRoundLogic)level.Session.RoundLogic).DropBall(__instance, __instance.Position + Player.ArrowOffset, __instance.Facing);
                }
                HasBasketBall[__instance.PlayerIndex]--;
            }
            if (currentHoldFrames[__instance.PlayerIndex] > 0f)
            {
                currentHoldFrames[__instance.PlayerIndex] = 0f;
            }
        }
    }

    [HarmonyPatch(typeof(Player), "OnExplode")]
    [HarmonyPrefix]
    public static void OnExplode(Player __instance, Explosion explosion, Vector2 normal)
    {
        if (ExampleModModule.TowerBallMode) { 
            while (HasBasketBall[__instance.PlayerIndex] > 0)
            {
                ((TowerBallRoundLogic)__instance.Level.Session.RoundLogic).DropBall(__instance, __instance.Position, __instance.Facing);
                HasBasketBall[__instance.PlayerIndex]--;
            }      
        }
    }

    [HarmonyPatch(typeof(Player), "Update")]
    [HarmonyPrefix]
    public static void Update(Player __instance, ref Sprite<string> ___bowSprite)
    {
        if (ExampleModModule.TowerBallMode)
        {
            BasketBallImages[__instance.PlayerIndex].Position = __instance.Position + Player.ArrowOffset + new Vector2((float)__instance.Facing * 4f, 2f);
            if ((__instance.CharacterIndex == 8 && __instance.AltSelect == ArcherData.ArcherTypes.Normal) || (__instance.CharacterIndex == 6 && __instance.AltSelect == ArcherData.ArcherTypes.Alt) || (__instance.CharacterIndex == 7 && __instance.AltSelect == ArcherData.ArcherTypes.Alt))
            {
                ___bowSprite.Visible = HasBasketBall[__instance.PlayerIndex] <= 0 && __instance.Aiming;
            }
            else
            {
                ___bowSprite.Visible = HasBasketBall[__instance.PlayerIndex] <= 0;
            }
            if (TFGame.PlayerInputs[__instance.PlayerIndex].MenuAlt2)
            {
            }
            if (__instance.State == PlayerStates.LedgeGrab)
            {
                touchedGroundSinceCollect[__instance.PlayerIndex] = false;
            }
            else
            {
                touchedGroundSinceCollect[__instance.PlayerIndex] &= !__instance.OnGround;
            }
            if (HasBasketBall[__instance.PlayerIndex] > 0)
            {
                currentHoldFrames[__instance.PlayerIndex] += Engine.TimeMult;
            }
            else if (currentHoldFrames[__instance.PlayerIndex] > 0f)
            {
                currentHoldFrames[__instance.PlayerIndex] = 0f;
            }
        }
    }

    [HarmonyPatch(typeof(Player), "DuckingUpdate")]
    [HarmonyPrefix]
    public static void DuckingUpdate(Player __instance, bool __state)
    {
        Level level = __instance.Level;
        __state = false;
        if (HasBasketBall[__instance.PlayerIndex] > 0 && TFGame.PlayerInputs[__instance.PlayerIndex].GetState().JumpPressed && __instance.CollideCheck(GameTags.JumpThru, __instance.Position + Vector2.UnitY) && !__instance.CollideCheck(GameTags.Solid, __instance.Position + Vector2.UnitY * 3f) && ExampleModModule.TowerBallMode)
        {
            Entity entity = __instance.CollideFirst(GameTags.JumpThru, __instance.Position + Vector2.UnitY);
            if (entity is BasketBallBasket)
            {
                bool flag2 = __instance.Position.X > 160f;
                Sounds.sfx_devTimeFinalDummy.Play(__instance.X);
                TowerBallRoundLogic towerBallRoundLogic = (TowerBallRoundLogic)__instance.Level.Session.RoundLogic;
                while (HasBasketBall[__instance.PlayerIndex] > 0)
                {
                    towerBallRoundLogic.IncreaseScore(__instance.PlayerIndex, towerBallRoundLogic.lastThrower, dunk: true, allyoop: false, clean: false, (!flag2) ? 1 : 0);
                    towerBallRoundLogic.AddDeadBall(__instance.Position, __instance.Speed);
                    HasBasketBall[__instance.PlayerIndex]--;
                }
                towerBallRoundLogic.SpawnBallChest(300);
                Explosion.Spawn(level, entity.Position + new Vector2(7.5f, 0f), __instance.PlayerIndex, plusOneKill: false, triggerBomb: false, bombTrap: false);
                towerBallRoundLogic.AddSlamNotification(entity.Position + (flag2 ? new Vector2(-10f, 0f) : new Vector2(10f, 0f)));
                if ((bool)level.KingIntro)
                {
                    level.KingIntro.Laugh();
                }
                ((BasketBallBasket)entity).DoNetJump();
                __state = true;
            }
        }
    }
    [HarmonyPatch(typeof(Player), "DuckingUpdate")]
    [HarmonyPostfix]
    public static void DuckingUpdatePost(Player __instance, bool __state)
    { 
        if (__state)
        {
            __instance.Position.Y += 6f;
        }
        if (__state && __instance.HasShield)
        {
            __instance.Position.Y += 10f;
        }
    }

    [HarmonyPatch(typeof(Player), "HUDRender")]
    [HarmonyPrefix]
    [HarmonyFinalizer]
    public static void Render(Player __instance)
    {
        if (HasBasketBall[__instance.PlayerIndex] > 0 && ExampleModModule.TowerBallMode)
        {
            BasketBallImages[__instance.PlayerIndex].Render();
        }
    }

}

