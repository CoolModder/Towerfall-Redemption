using FortRise;
using Monocle;
using MonoMod.ModInterop;
using OopsAllArrowsMod;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using TowerFall;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace OopsAllArrowsMod
{
    public class Darkfang : TowerPatch
    {
         public override void VersusPatch(VersusTowerPatchContext Patcher)
         {
             Patcher.IncreaseTreasureRates(RiseCore.PickupRegistry["Ice"].ID);
         }
    }
    public class Thornwood : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(RiseCore.PickupRegistry["Slime"].ID);
        }
    }
    public class Moonstone : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(RiseCore.PickupRegistry["Bait"].ID);
        }
    }
    public class Ascension : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(RiseCore.PickupRegistry["PrismTrap"].ID);
        }
    }
    public class Towerforge : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(RiseCore.PickupRegistry["LandMine"].ID);
        }
    }
    public class KingsCourt : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(RiseCore.PickupRegistry["Missle"].ID);
        }
    }
    public class Cataclysm : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(RiseCore.PickupRegistry["Freaky"].ID);
        }
    }
    public class Flight : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(RiseCore.PickupRegistry["Tornado"].ID);
        }
    }
    public class Backfire : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(RiseCore.PickupRegistry["Mech"].ID);
        }
    }
    public class Dreadwood : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(RiseCore.PickupRegistry["Boomerang"].ID);
        }
    }
    public class TwilightSpire : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(RiseCore.PickupRegistry["Nyan"].ID);
        }
    }
} 
