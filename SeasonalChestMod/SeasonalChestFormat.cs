using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;

namespace SeasonalChestMod
{
    public class SeasonalChestFormat
    {
        public DateTime StartDate;
        public DateTime EndDate;
        public Subtexture Normal;
        public Subtexture Special;
        public Subtexture Big;
        public Subtexture Bottomless;
        public SeasonalChestFormat(DateTime Start, DateTime End, Subtexture NormTexture, Subtexture SpecialTexture, Subtexture BigTexture, Subtexture BottomlessTexture) 
        { 
            StartDate = Start;
            EndDate = End;
            Normal = NormTexture;
            Special = SpecialTexture;
            Big = BigTexture;
            Bottomless = BottomlessTexture;
        }

    }
}
