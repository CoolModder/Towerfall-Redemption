using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using FortRise;
using System.Xml;
using System;

namespace TowerBall;


public class BasketBallTreasureChest : TreasureChest
{
	private Counter openCounter;

	private Counter spawnCounter;

	public BasketBallTreasureChest(XmlElement xml, Vector2 position)
		: base(position, Types.Special, AppearModes.Normal, new Pickups[0])
	{
		openCounter = new Counter();
		openCounter.Set(60);
		spawnCounter = new Counter();
		Flash(15);
        
    }

	public bool ReadyToAppear()
	{
		return true;
	}

	public override void Update()
	{
		base.Update();
		if ((bool)spawnCounter)
		{
			spawnCounter.Update();
			if ((bool)spawnCounter)
			{
				return;
			}
			Player player = null;
			for (int i = 0; i < 4; i++)
			{
				player = Level.GetPlayer(i);
				if (player != null)
				{
					break;
				}
			}
			if (player != null)
			{
				Allegiance allegiance = player.Allegiance;
				player.Allegiance = Allegiance.Neutral;
				Arrow entity = Arrow.Create(ModRegisters.ArrowType<BasketBall>(), player, Position, (float)Math.PI / 2f);
				player.Allegiance = allegiance;
				Level.Add(entity);
			}
		}
		else if ((bool)openCounter && Level.Session.RoundLogic.RoundStarted)
		{
			openCounter.Update();
			if (!openCounter)
			{
				Flash(30, RemoveSelf);
				spawnCounter.Set(1);
			}
		}
	}

	public override bool OnArrowHit(Arrow arrow)
	{
		return false;
	}

	public override void OnPlayerCollide(Player player)
	{
	}

	public override void OnPlayerGhostCollide(PlayerGhost player)
	{
	}
}
