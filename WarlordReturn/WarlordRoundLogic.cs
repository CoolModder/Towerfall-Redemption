using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using FortRise;
using System.Security.Policy;

namespace Warlord;
[CustomRoundLogic("WarlordMode")]
public class TowerBallRoundLogic : CustomVersusRoundLogic
{
	public Vector2 helmPos;

	public Alarm spawnAlarm;

    private Counter endDelay;

	public List<Arrow> arrowQueue;

	public int lastThrower;

    public TowerBallRoundLogic(Session session)
		: base(session, false)
	{
		CanMiasma = false;
		arrowQueue = new List<Arrow>();
        this.endDelay = new Counter();
        this.endDelay.Set(90);
    }

    public static RoundLogicInfo Create()
    {
        return new RoundLogicInfo
        {
            Name = "Warlord",
            Icon = ExampleModModule.Atlas["gameModes/warlord"],
            RoundType = RoundLogicType.FFA
        };
    }

    public override void OnLevelLoadFinish()
	{
		base.OnLevelLoadFinish();
        base.Session.CurrentLevel.Add(new VersusStart(base.Session));
        base.Players = TFGame.PlayerAmount;
		List<Vector2> xMLPositions = Session.CurrentLevel.GetXMLPositions("BigTreasureChest");
        foreach (Vector2 pos in xMLPositions)
        {
            helmPos = pos;
        }
        DropHelm(null, helmPos, Facing.Left);
        SpawnPlayersFFA();
        this.endDelay.Set(90);
        Session.CurrentLevel.Ending = false;
    }

	public override void OnUpdate()
	{
		for (int i = 0; i < arrowQueue.Count; i++)
		{
			base.Session.CurrentLevel.Add(arrowQueue[i]);
		}
		arrowQueue.Clear();
        SessionStats.TimePlayed += Engine.DeltaTicks;
        base.OnUpdate();
        if (!base.RoundStarted || !base.Session.CurrentLevel.Ending || !base.Session.CurrentLevel.CanEnd)
        {
            return;
        }
        
        if ((bool)spawnAlarm)
        {
            spawnAlarm.Update();
        }
        if (base.RoundStarted && base.Session.CurrentLevel.Ending)
        {
            if (this.endDelay)
            {
                this.endDelay.Update();
                return;
            }
            base.Session.EndRound();
            InsertCrownEvent();
        }
	}
    public override void OnPlayerDeath(Player player, PlayerCorpse corpse, int playerIndex, DeathCause cause, Vector2 position, int killerIndex)
    {
        base.OnPlayerDeath(player, corpse, playerIndex, cause, position, killerIndex);
        if (FFACheckForAllButOneDead())
        {
            Session.CurrentLevel.Ending = true;
            int WinPlayerIndex = 0;
            foreach (Player item in base.Session.CurrentLevel[GameTags.Player])
            {
                if (!item.Dead)
                {
                    WinPlayerIndex = item.PlayerIndex;
                }
            }
            if (MyPlayer.HasWarlordHelm[playerIndex] > 0 && killerIndex != playerIndex && killerIndex >= 0)
            { 
                base.Session.Scores[killerIndex]++;
            }
            else if (MyPlayer.HasWarlordHelm[WinPlayerIndex] > 0)
            {
                base.Session.Scores[WinPlayerIndex]++;
            }
        }
    }
    public void DropHelm(Player player, Vector2 position, Facing Facing)
	{
        Entity helm = new WarlordHelm(position,Facing.ToString() == "Left", null, 0);
        Session.CurrentLevel.Add(helm);
	}
    public override void OnRoundStart()
    {
        base.OnRoundStart();
        SpawnTreasureChestsVersus();
    }
    public void IncreaseScore(Player player)
    {
        if (Session.CurrentLevel.Ending == false)
        {
            base.Session.Scores[player.PlayerIndex]++;
            Session.CurrentLevel.Ending = true;
        }
    }
}
