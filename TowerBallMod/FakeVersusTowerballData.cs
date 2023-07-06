using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using TowerBall;
using Monocle;
using TowerFall;

namespace TowerFall
{
    public class FakeVersusTowerballData 
    {
        internal static Dictionary<int, FakeVersusTowerballData> Chapters = new();
        public List<VersusLevelData> Levels = new();

        public static void Load(int chapter, string directory)
        {
            var fakeVersusTowerData = new FakeVersusTowerballData();
            foreach (var text in Directory.EnumerateFiles(directory, "*.oel", SearchOption.TopDirectoryOnly))
            {
                fakeVersusTowerData.Levels.Add(new VersusLevelData(text));
            }
            Chapters.Add(chapter, fakeVersusTowerData);
        }

        public List<string> GetLevels(MatchSettings matchSettings)
        {
            List<string> list = new List<string>();
            if (matchSettings.Mode == Modes.LevelTest)
            {
                foreach (VersusLevelData level in Levels)
                {
                    list.Add(level.Path);
                }
                return list;
            }
            if (matchSettings.Mode == Modes.TeamDeathmatch)
            {
                int maxTeamSize = matchSettings.GetMaxTeamSize();
                {
                    foreach (VersusLevelData level2 in Levels)
                    {
                        if (level2.TeamSpawns >= maxTeamSize)
                        {
                            list.Add(level2.Path);
                        }
                    }
                    return list;
                }
            }
            foreach (VersusLevelData level3 in Levels)
            {
                if (level3.PlayerSpawns >= TFGame.PlayerAmount)
                {
                    list.Add(level3.Path);
                }
            }
            return list;
        }
    }
}