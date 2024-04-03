using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using FortRise;
using STimer = System.Timers.Timer;

namespace TowerBall;

public class TowerBall : CustomGameMode
{
    public override RoundLogic CreateRoundLogic(Session session)
    {
		return new TowerBallRoundLogic(session);
    }

    public override void Initialize()
    {
		Name = "TowerBall";
		NameColor = Color.OrangeRed;
		Icon = ExampleModModule.MenuAtlas["gamemodes/TowerBall"];
		TeamMode = true;
    }

    public override void InitializeSounds() {}
}

public class TowerBallRoundLogic : RoundLogic
{
	public Vector2 ballPos;

	public Alarm spawnAlarm;

	public PlayerRespawner[] respawns;

	private Counter endDelay;

	public TowerBallHUD hud;

	public List<Arrow> arrowQueue;

	public int lastThrower;

	public STimer roundTimer;

	public bool overtime = false;

	public bool timed = false;

	public int secondsleft;
	public TowerBallRoundLogic(Session session)
		: base(session, false)
	{
		CanMiasma = false;
		respawns = new PlayerRespawner[4];
		arrowQueue = new List<Arrow>();
		this.endDelay = new Counter();
		this.endDelay.Set(90);
	}

    public override void OnRoundStart()
    {
        base.OnRoundStart();
        SpawnTreasureChestsVersus();
        if (timed)
		{
            roundTimer.Enabled = true;
        }
    }
    public override void OnLevelLoadFinish()
	{
		base.OnLevelLoadFinish();
		base.Session.CurrentLevel.Add(new VersusStart(base.Session));
		SpawnPlayersTeams();
		base.Players = TFGame.PlayerAmount;
		List<Vector2> xMLPositions = Session.CurrentLevel.GetXMLPositions("BigTreasureChest");
		ballPos = xMLPositions[new Random().Next(xMLPositions.Count)];
		SpawnBallChest();
		List<Vector2> xMLPositions2 = Session.CurrentLevel.GetXMLPositions("Basket");
		base.Session.CurrentLevel.Add(new BasketBallBasket(null, xMLPositions2[0]));
		base.Session.CurrentLevel.Add(new BasketBallBasket(null, xMLPositions2[1]));
		hud = base.Session.CurrentLevel.Add(new TowerBallHUD(this));
		Session.MatchSettings.Variants.ReturnAsGhosts.Value = false;
		Session.MatchSettings.Variants.TriggerCorpses.Value = false;
		overtime = false;
		Session.MatchSettings.Variants.TeamRevive.Value = false;
		if (base.Session.MatchSettings.Variants.GetCustomVariant("TimedRounds"))
		{
			roundTimer = new STimer(1000);
			roundTimer.AutoReset = true;
			secondsleft = 30 * Session.MatchSettings.GoalScore;
			roundTimer.Elapsed += TimerMinus;
			

			timed = true;
		}
		else
		{
			timed = false;
		}
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
	
        if (secondsleft <= 0 && timed && !base.Session.CurrentLevel.Ending)
        {
            RoundEndTimed();
        } 
        if (!base.RoundStarted || !base.Session.CurrentLevel.Ending || !base.Session.CurrentLevel.CanEnd)
		{
			return;
		}
		
		if ((bool)spawnAlarm)
		{
			spawnAlarm.Update();
		}
		if (base.RoundStarted && base.Session.CurrentLevel.Ending && base.Session.CurrentLevel.CanEnd)
		{
			if (this.endDelay)
			{
				this.endDelay.Update();
				return;
			}
			
			base.Session.EndRound();
		}
		else
		{
			InsertCrownEvent();
		}
	}


	private void TimerMinus(Object source, System.Timers.ElapsedEventArgs e)
	{
		secondsleft--;
	}
	public Player RespawnPlayer(int playerIndex, Allegiance team)
	{
		List<Vector2> list = ((team != 0) ? Session.CurrentLevel.GetXMLPositions("TeamSpawnB") : Session.CurrentLevel.GetXMLPositions("TeamSpawnA"));
		Player player = new Player(playerIndex, list[new Random().Next(list.Count)], team, team, Session.GetPlayerInventory(playerIndex), Session.GetSpawnHatState(playerIndex), frozen: false, flash: false, indicator: true);
		Session.CurrentLevel.Add(player);
		player.Flash(120);
		Alarm.Set(player, 60, player.RemoveIndicator);
		return player;
	}
	public void RoundEndTimed()
	{
		if (Session.Scores[1] == Session.Scores[0])
		{
			overtime = true;
			return;
		}
		Allegiance allegiance = Allegiance.Blue;
		if (Session.Scores[1] == Session.GetHighestScore())
		{
			allegiance = Allegiance.Red;
		}
		List<LevelEntity> list = new List<LevelEntity>();
		for (int i = 0; i < 4; i++)
		{
			if (TFGame.Players[i] && Session.MatchSettings.Teams[i] == allegiance)
			{
				Session.MatchStats[i].GotWin = true;
				LevelEntity playerOrCorpse = Session.CurrentLevel.GetPlayerOrCorpse(i);
				if (playerOrCorpse != null)
				{
					list.Add(playerOrCorpse);
				}
			}
		}
		Session.CurrentLevel.LightingLayer.SetSpotlight(list.ToArray());
		Session.CurrentLevel.OrbLogic.DoSlowMoKill();
		Session.MatchSettings.LevelSystem.StopVersusMusic();
		hud.RemoveSelf();
		base.Session.CurrentLevel.CanEnd = true;
		base.Session.CurrentLevel.Ending = true;
	}
    public override void OnPlayerDeath(Player player, PlayerCorpse corpse, int playerIndex, DeathCause cause, Vector2 position, int killerIndex)
	{
		Entity entity = new Entity();
		Alarm.Set(entity, 70, () => {
			RespawnPlayer(playerIndex, player.Allegiance);
			entity.RemoveSelf();
		});
		Session.CurrentLevel.Add(entity);
		if ((bool)Session.CurrentLevel.KingIntro)
		{
			Session.CurrentLevel.KingIntro.Laugh();
		}
		if ((bool)Session.MatchSettings.Variants.GunnStyle)
		{
			Session.CurrentLevel.Add(new GunnStyle(corpse));
		}
		if (killerIndex != -1 && killerIndex != playerIndex)
		{
			Kills[killerIndex]++;
		}
		if (killerIndex != -1 && !Session.CurrentLevel.IsPlayerAlive(killerIndex))
		{
			Session.MatchStats[killerIndex].KillsWhileDead = Session.MatchStats[killerIndex].KillsWhileDead + 1;
		}
		if (killerIndex != -1 && killerIndex != playerIndex)
		{
			Session.MatchStats[killerIndex].RegisterFastestKill(Time);
		}
		DeathType deathType = DeathType.Normal;
		if (killerIndex == playerIndex)
		{
			deathType = DeathType.Self;
		}
		else if (killerIndex != -1 && Session.MatchSettings.TeamMode && Session.MatchSettings.GetPlayerAllegiance(playerIndex) == Session.MatchSettings.GetPlayerAllegiance(killerIndex))
		{
			deathType = DeathType.Team;
		}
		if (killerIndex != -1 && deathType == DeathType.Normal && Session.WasWinningAtStartOfRound(playerIndex))
		{
			Session.MatchStats[killerIndex].WinnerKills = Session.MatchStats[killerIndex].WinnerKills + 1;
		}
		DeathType type = deathType;
		int characterIndex = ((killerIndex == -1) ? (-1) : TFGame.Characters[killerIndex]);
		Session.MatchStats[playerIndex].Deaths.Add(type, cause, characterIndex);
		SaveData.Instance.Stats.Deaths.Add(deathType, cause, TFGame.Characters[playerIndex]);
		if (!Session.MatchSettings.SoloMode)
		{
			SessionStats.RegisterVersusKill(killerIndex, playerIndex, deathType == DeathType.Team);
		}
	}
	
	public void IncreaseScore(int playerIndex, int assistPlayerIndex, bool dunk, bool allyoop, bool clean, int teamIndex)
	{
		if (endDelay.Value != 90)
		{
			return;
		}
		AddScore(teamIndex, 1);
        SpawnBallChest();
        bool flag = assistPlayerIndex != -1 && assistPlayerIndex != playerIndex && Session.MatchSettings.Teams[playerIndex] == Session.MatchSettings.Teams[assistPlayerIndex];
		TFGame.PlayerInputs[playerIndex].Rumble(1f, 10);
		hud.flashAlpha = 1f;
		if (!timed)
		{
			if (Session.GetHighestScore() < Session.MatchSettings.GoalScore)
			{
                if (base.Session.MatchSettings.Variants.GetCustomVariant("HoopTreasure"))
                {
                    SpawnTreasureChestsVersus();
                }
                return;
			}
		}
		else
		{
			if (!overtime)
			{
                if (base.Session.MatchSettings.Variants.GetCustomVariant("HoopTreasure"))
                {
                    SpawnTreasureChestsVersus();
                }
                return;
			}
		}
		Allegiance allegiance = Allegiance.Blue;
		if (Session.Scores[1] == Session.GetHighestScore())
		{
			allegiance = Allegiance.Red;
		}
		List<LevelEntity> list = new List<LevelEntity>();
		for (int i = 0; i < 4; i++)
		{
			if (TFGame.Players[i] && Session.MatchSettings.Teams[i] == allegiance)
			{
				Session.MatchStats[i].GotWin = true;
				LevelEntity playerOrCorpse = Session.CurrentLevel.GetPlayerOrCorpse(i);
				if (playerOrCorpse != null)
				{
					list.Add(playerOrCorpse);
				}
			}
		}
		Session.CurrentLevel.LightingLayer.SetSpotlight(list.ToArray());
		Session.CurrentLevel.OrbLogic.DoSlowMoKill();
		Session.MatchSettings.LevelSystem.StopVersusMusic();
		Sounds.sfx_finalKill.Play();
        hud.RemoveSelf();
        base.Session.CurrentLevel.CanEnd = true;
        base.Session.CurrentLevel.Ending = true;
    }

	public void AddDeadBall(Vector2 pos, Vector2 s)
	{
		base.Session.CurrentLevel.Add(new DeadBall(pos, s));
	}

	public void AddSlamNotification(Vector2 pos)
	{
		base.Session.CurrentLevel.Add(new SlamNotification(pos.X + 7f, pos.Y));
	}

	public void SpawnBallChest()
	{
		base.Session.CurrentLevel.Add(new BasketBallTreasureChest(null, ballPos));
		lastThrower = -1;
	}

	public void SpawnBallChest(int delay)
	{
		spawnAlarm = Alarm.Create(Alarm.AlarmMode.Oneshot, SpawnBallChest, delay, start: true);
	}

	public void DropBall(Player p, Vector2 pos, Facing face)
	{
		var arrowsRegistry = RiseCore.ArrowsRegistry["TowerBall/BasketBall"];
		Arrow arrow = Arrow.Create(arrowsRegistry.Types, p, pos, (face == Facing.Right) ? 0f : ((float)Math.PI));
		arrow.Drop((int)face);
		arrow.Speed.Y *= 0.5f;
		arrowQueue.Add(arrow);
	}

	public void DropBall(Player p, Vector2 pos)
	{
		var arrowsRegistry = RiseCore.ArrowsRegistry["TowerBall/BasketBall"];
		Arrow arrow = Arrow.Create(arrowsRegistry.Types, p, pos, 4.712389f);
		arrow.Drop(0);
		arrow.Speed.X = (arrow.Speed.Y = 0f);
		arrowQueue.Add(arrow);
	}
}
