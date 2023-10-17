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
using System.Reflection;
using IL.TowerFall.Editor;

namespace SeasonChestMod
{


    class MyTreasueChest : TreasureChest
    {
        
        public MyTreasueChest(Vector2 position, Types graphic, AppearModes mode, Pickups[] pickups, int timer = 0) : base(position, graphic, mode, pickups, timer)
        {
        }
       
        public static void MyTreasureChestCtor(On.TowerFall.TreasureChest.orig_ctor_Vector2_Types_AppearModes_PickupsArray_int orig, TreasureChest self, Vector2 position, Types graphic, AppearModes mode, Pickups[] pickups, int timer = 0)
        {
            var ChestData = DynamicData.For(self);
            orig(self, position, graphic, mode, pickups, timer);
            if (SeasonChestModModule.seasonalChest != null && SeasonChestModModule.Settings.SeasonalChestOn)
            {
                if (graphic == Types.Bottomless)
                {
                   
                    ((Sprite<int>)ChestData.Get("sprite")).SwapSubtexture(SeasonChestModModule.seasonalChest.Bottomless);

                    if (SeasonChestModModule.seasonalChest.Bottomless == SeasonChestModModule.ChestAtlas["catBottomless"])
                    {
                        var NewSprite = new Sprite<int>(SeasonChestModModule.seasonalChest.Bottomless, 20, 20);
                        NewSprite.Add(0, 0.2f, true, new int[3] { 0, 1, 2 }); 
                        NewSprite.Add(1, 0.2f, false, new int[5] { 3, 4, 5, 6, 7 });
                        NewSprite.Position = Vector2.UnitY * 5f;
                        NewSprite.Origin = new Vector2(10f, 20f);
                        NewSprite.Play(0);
                        self.Remove((Sprite<int>)ChestData.Get("sprite"));
                        self.Add(NewSprite);
                        ChestData.Set("sprite", NewSprite);
                    }
                }
                else if (graphic == Types.Large)
                {
                    ((Sprite<int>)ChestData.Get("sprite")).SwapSubtexture(SeasonChestModModule.seasonalChest.Big);
                    if (SeasonChestModModule.seasonalChest.Big == SeasonChestModModule.ChestAtlas["catBig"])
                    {
                        var NewSprite = new Sprite<int>(SeasonChestModModule.seasonalChest.Big, 20, 20);
                        NewSprite.Add(0, 0.2f, true, new int[3] { 0, 1, 2 });
                        NewSprite.Add(1, 0.2f, false, new int[5] { 3, 4, 5, 6, 7 });
                        NewSprite.Position = Vector2.UnitY * 5f;
                        NewSprite.Origin = new Vector2(10f, 20f);
                        NewSprite.Play(0);
                        self.Remove((Sprite<int>)ChestData.Get("sprite"));
                        self.Add(NewSprite);
                        ChestData.Set("sprite", NewSprite);
                    }
                }
                else if (graphic == Types.Special)
                {
                    ((Sprite<int>)ChestData.Get("sprite")).SwapSubtexture(SeasonChestModModule.seasonalChest.Special);
                }
                else
                {
                    ((Sprite<int>)ChestData.Get("sprite")).SwapSubtexture(SeasonChestModModule.seasonalChest.Normal);
                }
            } 

        }
        public static void Load()
        {
            On.TowerFall.TreasureChest.ctor_Vector2_Types_AppearModes_PickupsArray_int += MyTreasureChestCtor;
        }
        public static void Unload()
        {
            On.TowerFall.TreasureChest.ctor_Vector2_Types_AppearModes_PickupsArray_int -= MyTreasureChestCtor;
        }

      
    
    }
}
 