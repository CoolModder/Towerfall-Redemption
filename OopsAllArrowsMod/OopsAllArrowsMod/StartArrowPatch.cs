using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerFall;
using MonoMod;
using FortRise;
using Microsoft.Xna.Framework;
using MonoMod.Utils;

namespace OopsAllArrowsMod
{
    class StartArrowPatch : MatchVariants
    {
      
        public static ArrowTypes StartArrowTypes(On.TowerFall.MatchVariants.orig_GetStartArrowType orig, global::TowerFall.MatchVariants self, int playerIndex, ArrowTypes randomType)
        {
            foreach (CustomArrowFormat customArrow in ExampleModModule.CustomArrowList)
            {
                if (self.GetCustomVariant(customArrow.StartWith)[playerIndex])
                {
                    return customArrow.CustomArrow;
                }
            }
            if (self.StartWithRandomArrows[playerIndex])
            {
                    ArrowTypes arrows = new ArrowTypes();
                    Random rand = new Random();
                    int ArrowType = rand.Next(9 + ExampleModModule.CustomArrowList.Count);
                    if (ArrowType < 10)
                    {
                            arrows = (ArrowTypes)(ArrowType + 1);
                    }
                    else
                    {
                            arrows = ExampleModModule.CustomArrowList[ArrowType - 9].CustomArrow;
                    }

                return arrows;
            }
            return orig(self, playerIndex, randomType);
        }

        public static void Load()
        {
            On.TowerFall.MatchVariants.GetStartArrowType += StartArrowTypes;

        }
        public static void Unload() 
        {
            On.TowerFall.MatchVariants.GetStartArrowType -= StartArrowTypes;

        }


    }
}
