using Monocle;
using TowerFall;

namespace TowerBall;

public class PlayerRespawner
{
	public int playerIndex;

	public Allegiance team;

	public TowerBallRoundLogic roundLogic;

	public Counter alarm;

	public PlayerRespawner(int pI, Allegiance t, TowerBallRoundLogic rL)
	{
		playerIndex = pI;
		team = t;
		roundLogic = rL;
		alarm = new Counter(180);
	}

	public bool Update()
	{
		alarm.Update();
		if (!alarm)
		{
			roundLogic.RespawnPlayer(playerIndex, team);
			return true;
		}
		return false;
	}
}
