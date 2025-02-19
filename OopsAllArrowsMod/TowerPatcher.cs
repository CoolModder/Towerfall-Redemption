using FortRise;

namespace OopsAllArrowsMod
{
    public class Darkfang : TowerPatch
    {
         public override void VersusPatch(VersusTowerPatchContext Patcher)
         {
             Patcher.IncreaseTreasureRates(ModRegisters.PickupType<IceArrowPickup>());
         }
    }
    public class Thornwood : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<SlimeArrowPickup>());
        }
    }
    public class Moonstone : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<BaitArrowPickup>());
        }
    }
    public class Ascension : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<PrismTrapArrowPickup>());
        }
    }
    public class Towerforge : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<LandMineArrowPickup>());
        }
    }
    public class KingsCourt : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<MissleArrowPickup>());
        }
    }
    public class Cataclysm : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<FreakyArrowPickup>());
        }
    }
    public class Flight : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<TornadoArrowPickup>());
        }
    }
    public class Backfire : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<MechArrowPickup>());
        }
    }
    public class Dreadwood : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<BoomerangArrowPickup>());
        }
    }
    public class TwilightSpire : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<NyanArrowPickup>());
        }
    }
} 
