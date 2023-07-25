using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TowerFall;
using Monocle;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Collections;

namespace OopsAllArrowsMod
{

    class MyTreasueChest : TreasureChest
    {
        public static Dictionary<TreasureChest, float[]> MyPickup = new Dictionary<TreasureChest, float[]>();
        public MyTreasueChest(Vector2 position, Types graphic, AppearModes mode, Pickups[] pickups, int timer = 0) : base(position, graphic, mode, pickups, timer)
        {
        }

        public static void MyTreasureChestCtor(On.TowerFall.TreasureChest.orig_ctor_Vector2_Types_AppearModes_PickupsArray_int orig, TreasureChest self, Vector2 position, Types graphic, AppearModes mode, Pickups[] pickups, int timer = 0)
        {
            var ChestData = DynamicData.For(self);
            orig(self, position, graphic, mode, pickups, timer);
           
        }
        public static void MyTreasureChestOpen(On.TowerFall.TreasureChest.orig_OpenChest orig, TreasureChest self, int PlayerIndex)
        {
            var ChestData = DynamicData.For(self);
            MyPickup[self] = new float[ExampleModModule.CustomArrowList.Count];

            if (((ChestData.Get("pickups") as List<Pickups>).First() == Pickups.Bomb && (ChestData.Get("pickups") as List<Pickups>).Count == 1))
            {
                orig(self, PlayerIndex);
            }
            if (!self.Level.Session.MatchSettings.Variants.GetCustomVariant("SpawnInTowers"))
            {
                orig(self, PlayerIndex);
            }
            else
            {
                if (self.State >= States.Opening)
                {
                    return;
                }
                self.Untag(GameTags.PlayerCollider, GameTags.PlayerGhostCollider);
                self.Seek = false;
                ChestData.Set("State", States.Opening);
                (ChestData.Get("sprite") as Sprite<int>).Play(1);
                if ((Types)ChestData.Get("type") == Types.Special)
                {
                    Sounds.sfx_chestSpecOpen.Play();
                }
                else
                {
                    Sounds.sfx_unlockChest.Play();
                }
                self.LightVisible = false;
                self.Level.Add(Cache.Create<LightFade>().Init(self));
                foreach (CustomArrowFormat customArrow in ExampleModModule.CustomArrowList)
                {

                     if (self.Level.Session.MatchSettings.Variants.IgnoreTowerItemSet || self.Level.Session.MatchSettings.LevelSystem.Theme.Name == customArrow.Level)
                     {
                            MyPickup[self][customArrow.PickupID-1] = 1f;
                            Console.Write("Can Spawn: " + (customArrow.PickupID - 1).ToString());
                     }
                     if (self.Level.Session.MatchSettings.Variants.GetCustomVariant(customArrow.Exclude))
                     {
                            MyPickup[self][customArrow.PickupID-1] = 0f;
                            Console.Write("Excluded: " + (customArrow.PickupID - 1).ToString());
                     }
                }
                Pickup pickup = null;
                Random rand = new Random();
                var MyFixedPickups = new List<CustomArrowFormat>();
                for (int i = 0; i < MyPickup[self].Length; i++)
                {
                    if (MyPickup[self][i] == 1f)
                    {
                        foreach (CustomArrowFormat customArrow in ExampleModModule.CustomArrowList)
                        {
                            if (customArrow.PickupID-1 == i)
                            {
                                MyFixedPickups.Add(customArrow);
                            }
                        }
                    }
                }
                if (rand.Next((ChestData.Get("pickups") as List<Pickups>).Count + MyFixedPickups.Count + 1) <= (ChestData.Get("pickups") as List<Pickups>).Count)
                {
                    for (int i = 0; i < (ChestData.Get("pickups") as List<Pickups>).Count; i++)
                    {

                        Vector2 zero = Vector2.Zero;
                        if (i % 2 == 0)
                        {
                            zero.Y = -16 * (i / 2);
                        }
                        else
                        {
                            zero.Y = 16 * ((i + 1) / 2);
                        } 

                        if ((ChestData.Get("pickups") as List<Pickups>)[i] == Pickups.Gem)
                        {
                            self.Level.Add(pickup = new GemPickup(self.Position, Pickup.GetTargetPositionFromChest(self.Level, self.Position + zero - Vector2.UnitY)));
                        }
                        else
                        {
                            self.Level.Add(pickup = Pickup.CreatePickup(self.Position, Pickup.GetTargetPositionFromChest(self.Level, self.Position + zero - Vector2.UnitY), (ChestData.Get("pickups") as List<Pickups>)[i], PlayerIndex));
                        }
                    }
                }
                else
                {
                    MyFixedPickups.Shuffle();
                    Vector2 zero = Vector2.Zero;
                    zero.Y = -16;
                    self.Level.Add(pickup = new ArrowTypePickup(self.Position, Pickup.GetTargetPositionFromChest(self.Level, self.Position + zero), MyFixedPickups[0].CustomArrow));
                }
                object Why;
                if ((Types)ChestData.Get("type") == Types.Bottomless)
                {
                    pickup.StartTimeout();
                    self.Add(new Coroutine((IEnumerator)ChestData.Invoke("BottomlessLogic", pickup)));
                }

                (ChestData.Get("light") as Sprite<int>).Active = ((ChestData.Get("light") as Sprite<int>).Visible = true);
                (ChestData.Get("sprite") as Sprite<int>).Scale.X = 0.8f;
                (ChestData.Get("sprite") as Sprite<int>).Scale.Y = 1.2f;
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, 40, start: true);
                tween.OnUpdate = delegate (Tween t)
                {
                    (ChestData.Get("light") as Sprite<int>).Color = Color.Lerp(Color.White, Color.Transparent, t.Eased);
                    float amount = Ease.BackIn(t.Percent);
                    (ChestData.Get("sprite") as Sprite<int>).Scale.X = MathHelper.Lerp(0.8f, 1f, amount);
                    (ChestData.Get("sprite") as Sprite<int>).Scale.Y = MathHelper.Lerp(1.2f, 1f, amount);
                };
                tween.OnComplete = delegate
                {
                    (ChestData.Get("light") as Sprite<int>).Visible = ((ChestData.Get("light") as Sprite<int>).Active = false);
                    ChestData.Set("State", States.Opened);
                };
                self.Add(tween);
            }
        }
        public static void Load()
        {
            On.TowerFall.TreasureChest.ctor_Vector2_Types_AppearModes_PickupsArray_int += MyTreasureChestCtor;
            On.TowerFall.TreasureChest.OpenChest += MyTreasureChestOpen;
        }
        public static void Unload()
        {
            On.TowerFall.TreasureChest.ctor_Vector2_Types_AppearModes_PickupsArray_int -= MyTreasureChestCtor;
            On.TowerFall.TreasureChest.OpenChest -= MyTreasureChestOpen;
        }
    }
}
 