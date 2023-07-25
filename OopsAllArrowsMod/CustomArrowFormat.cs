
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerFall;
namespace OopsAllArrowsMod
{
    public class CustomArrowFormat
    {
        public ArrowTypes CustomArrow;
        public string StartWith;
        public string Exclude;
        public string Level;
        public int PickupID;
        public CustomArrowFormat(ArrowTypes arrow, string start, string exclude, string spawnLevel, int Pickup)
        {
            CustomArrow = arrow;
            StartWith = start;
            Exclude = exclude;
            Level = spawnLevel;
            PickupID = Pickup;

        }
    }
}
