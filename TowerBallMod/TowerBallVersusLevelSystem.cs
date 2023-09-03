using System.Collections.Generic;
using System.IO;
using System.Xml;
using FortRise;
using Monocle;
using MonoMod.Utils;
using TowerFall;

namespace TowerBall;

// From Terria:
// This class is instantiate through MyTowerMapData hooks.
// What this class does is just mimic the VersusLevelSystem but it uses your content system
// which loads your Content/Levels instead of the vanilla. Leveraging this technique we can also
// support WiderSetMod as well.
public class TowerBallVersusLevelSystem : VersusLevelSystem
{
    private DynamicData data;

    // not prefixing it with 'data_' or anything else causes Stack overFlow.
    private List<string> data_levels => data.Get<List<string>>("levels");
    private string data_lastLevel 
    {
        get => data.Get<string>("lastLevel");
        set => data.Set("lastLevel", value);
    } 

    public TowerBallVersusLevelSystem(VersusTowerData tower) : base(tower)
    {
        data = DynamicData.For(this);
    }

    public override XmlElement GetNextRoundLevel(MatchSettings matchSettings, int roundIndex, out int randomSeed)
    {
        if (VersusTowerData.GetLevelSet() != "TowerFall") 
        {
            var xml = GetFromAdventureLevels(matchSettings, roundIndex, out randomSeed);
            if (xml != null)
                return xml;
        }
        if (data_levels.Count == 0)
        {
            data.Invoke("GenLevels", matchSettings);
        }
		data_lastLevel = data_levels[0];
		data_levels.RemoveAt(0);
        randomSeed = 0;
        foreach (char c in data_lastLevel)
        {
            randomSeed += (int)c;
        }
        string fixedLastLevel = data_lastLevel.Replace('\\', '/').Replace("Levels", "TowerBallLevels");
        using var fs = ExampleModModule.Instance.Content.MapResource[TransformToTowerBallLevel(fixedLastLevel)].Stream;
        return Calc.LoadXML(fs)["level"];
    }

    private string TransformToTowerBallLevel(string lastLevel) 
    {
        var level = lastLevel.Substring(0, lastLevel.Length - 6);
        return level += "00.oel";
    }

    // This function speaks for itself
    private XmlElement GetFromAdventureLevels(MatchSettings matchSettings, int roundIndex, out int randomSeed) 
    {
		if (data_levels.Count == 0)
		{
            data.Invoke("GenLevels", matchSettings);
		}
		data_lastLevel = TransformToTowerBallLevel(data_levels[0]);
		data_levels.RemoveAt(0);
		randomSeed = 0;
		foreach (char c in data_lastLevel)
		{
			randomSeed += (int)c;
		}
		RiseCore.Resource resource;
		if (RiseCore.ResourceTree.TreeMap.TryGetValue(data_lastLevel, out resource))
		{
			using (Stream stream = resource.Stream)
			{
				return Calc.LoadXML(stream)["level"];
			}
		}
		Logger.Error("[VERSUSLEVELSYSTEM][" + data_lastLevel + "] Path not found!", 57);
        return null;
    }
}