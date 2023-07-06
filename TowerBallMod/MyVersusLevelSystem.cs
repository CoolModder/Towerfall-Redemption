using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortRise;
using MonoMod;
using TowerFall;
using TowerBall;
using MonoMod.Utils;

namespace TowerBall
{
    public static class MyVersusLevelSystem
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static void VersusLevelSystem_GenLevels_patch(On.TowerFall.VersusLevelSystem.orig_GenLevels orig, VersusLevelSystem self, MatchSettings matchSettings)
        {
            Console.Write(matchSettings.CurrentModeName);
            if (matchSettings.CurrentModeName != "TowerBallRoundLogic")
            {
                orig(self, matchSettings);
                return;
            }
            var levelSystem = DynamicData.For(self);
            var lastLevel = levelSystem.Get<string>("lastlevel");
            var fake = FakeVersusTowerballData.Chapters[self.ID.X];
            var levels = fake.GetLevels(matchSettings);
            if (self.VersusTowerData.FixedFirst && lastLevel == null)
            {
                string text = levels[0];
                levels.RemoveAt(0);
                levels.Shuffle();
                levels.Insert(0, text);
                levelSystem.Set("levels", levels);
                return;
            }
            levels.Shuffle();
            if (levels[0] == lastLevel)
            {
                levels.RemoveAt(0);
                levels.Add(lastLevel);
            }
            levelSystem.Set("levels", levels);
        }

        public static void Load()
        {
            On.TowerFall.VersusLevelSystem.GenLevels += VersusLevelSystem_GenLevels_patch;
        }
        public static void Unload()
        {
            On.TowerFall.VersusLevelSystem.GenLevels -= VersusLevelSystem_GenLevels_patch;
        }
    }
}
