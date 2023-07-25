using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerFall;
using MonoMod;
using FortRise;
using Microsoft.Xna.Framework;

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
