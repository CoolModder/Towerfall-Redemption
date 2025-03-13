using FortRise;

namespace OopsAllArrowsMod
{
    [TowerPatcher("Darkfang")]
    public class Darkfang : TowerPatch
    {
         public override void VersusPatch(VersusTowerPatchContext Patcher)
         {
             Patcher.IncreaseTreasureRates(ModRegisters.PickupType<IceArrowPickup>());
         }
    }

    [TowerPatcher("Thornwood")]
    public class Thornwood : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<SlimeArrowPickup>());
        }
    }

    [TowerPatcher("Moonstone")]
    public class Moonstone : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<BaitArrowPickup>());
        }
    }

    [TowerPatcher("Ascension")]
    public class Ascension : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<PrismTrapArrowPickup>());
        }
    }

    [TowerPatcher("Towerforge")]
    public class Towerforge : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<LandMineArrowPickup>());
        }
    }

    [TowerPatcher("King's Court")]
    public class KingsCourt : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<MissleArrowPickup>());
        }
    }

    [TowerPatcher("Cataclysm")]
    public class Cataclysm : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<FreakyArrowPickup>());
        }
    }

    [TowerPatcher("Flight")]
    public class Flight : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<TornadoArrowPickup>());
        }
    }

    [TowerPatcher("Backfire")]
    public class Backfire : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<MechArrowPickup>());
        }
    }

    [TowerPatcher("Dreadwood")]
    public class Dreadwood : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<BoomerangArrowPickup>());
        }
    }

    [TowerPatcher("Twilight Spire")]
    public class TwilightSpire : TowerPatch
    {
        public override void VersusPatch(VersusTowerPatchContext Patcher)
        {
            Patcher.IncreaseTreasureRates(ModRegisters.PickupType<NyanArrowPickup>());
        }
    }
} 
