
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerFall;
namespace OopsAllArrowsMod
{
    public class CustomPickupFormat
    {
        public Pickups CustomPickup;
        public string Exclude;
        public string SpawnVariant;
        public CustomPickupFormat(Pickups pickup, string exclude, string spawnVariant = "SpawnInTowers")
        {
            CustomPickup = pickup;
            Exclude = exclude;
            SpawnVariant = spawnVariant;
        }
    }
}
