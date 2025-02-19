using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using FortRise;
using System;

namespace TowerBall;


[CustomArrows("TowerBall/BasketBall", "CreateGraphicPickup")]
public class BasketBall : ToyArrow
{
	public int AssistIndex;

	private float prevXSpeed;

	private bool fallModeEntered;

	private bool cleanShot;

	public bool allyOop;

	private Image ballImage;

	public Allegiance ThrownTeam;

	public Wiggler scaleWiggler;

	public Counter cantCollectCounter;

	public Counter slamming;

	public Hitbox ballColliderNormal;

	public Hitbox ballColliderOtherArrow;

    public override ArrowTypes ArrowType { get; set; }
    private bool used, canDie;
    private static Action<TriggerArrow, LevelEntity, Vector2, float> BaseInit;
    public override bool IsCollectible
	{
		get
		{
			if ((bool)cantCollectCounter)
			{
				return false;
			}
			return Speed.Length() < 2f;
		}
	}

	protected override void CreateGraphics()
	{
		ballImage = new Image(ExampleModModule.Atlas["towerball/ball"])
		{
			Origin = new Vector2(5f, 5f)
		};
		Graphics = new Image[1] { ballImage };
		Add(Graphics);
	}

	protected override void InitGraphics()
	{
		ballImage.Visible = true;
	}


	public BasketBall()
	{
		LightRadius = 80f;
		cleanShot = true;
		ThrownTeam = Allegiance.Neutral;
		cantCollectCounter = new Counter(0);
		slamming = new Counter(15);
		base.Collider = new Hitbox(8f, 8f, -4f, -4f);
		TargetCollider = new Hitbox(8f, 8f, -4f, -4f);
		ballColliderNormal = new Hitbox(8f, 8f, -4f, -4f);
		ballColliderOtherArrow = new Hitbox(12f, 12f, -6f, -6f);
		Action<float> onChange = delegate(float v)
		{
			ballImage.Scale = Vector2.One * (1f + 0.2f * v);
		};
		scaleWiggler = Wiggler.Create(20, 4f, null, onChange);
		Add(scaleWiggler);

    }

	public void JustSpawned()
	{
		cantCollectCounter.Set(10);
	}


	protected override bool CheckForTargetCollisions()
	{
		if ((bool)cantCollectCounter)
		{
			return false;
		}
		for (int i = 0; i < 4; i++)
		{
			Player player = base.Level.GetPlayer(i);
			if (player != null && player.ArrowCheck(this) && player != CannotHit)
			{
				if ((int)cantCollectCounter <= 0 && (player.Allegiance == ThrownTeam || ThrownTeam == Allegiance.Neutral))
				{
					player.CollectArrows(ModRegisters.ArrowType<BasketBall>());
					RemoveSelf();
					return false;
				}
			}
		}
		return false;
	}

	private void forceState()
	{
		base.State = ArrowStates.Shooting;
		bool value = false;
		if (PlayerIndex >= 0)
		{
			value = base.Level.Session.MatchSettings.Variants.NoSeekingArrows[PlayerIndex];
			base.Level.Session.MatchSettings.Variants.NoSeekingArrows[PlayerIndex] = true;
		}

		base.ShootUpdate();
		if (PlayerIndex >= 0)
		{
			base.Level.Session.MatchSettings.Variants.NoSeekingArrows[PlayerIndex] = value;
		}
	}

	protected override void OnCollideH(Platform platform)
	{
		if (platform.Collidable)
		{
			if (Math.Abs(Speed.X) > 1.5f)
			{
				Sounds.Play("TowerBall/BOUNCYBALL", X, Math.Min(Math.Abs(Speed.Y / 5f), 1f));
            }
			prevXSpeed *= -1f;
			int num = (int)Math.Round(Math.Abs(Speed.X)) / 2;
			int num2 = -Math.Sign(Speed.X);
			if (num > 0)
			{
				base.Level.Particles.Emit(Particles.PlayerDust[3], num, Position + new Vector2(4 * -num2, 0f), new Vector2(1f, 3f), new Vector2(num2, -0.5f).Angle());
			}
			forceState();
			cleanShot = false;
		}
	}

	protected override void OnCollideV(Platform platform)
	{
		if (platform is BasketBallBasket)
		{
			bool flag = Position.X > 160f;
			if ((flag && Position.X <= platform.Position.X) || (!flag && Position.X >= platform.Position.X + 15f))
			{
				if (Math.Abs(Speed.Y) > 1.5f)
				{
					Sounds.Play("TowerBall/BOUNCYBALL", X, Math.Min(Math.Abs(Speed.Y / 5f), 1f));
				}
				float length = Speed.Length();
				float angleRadians = Calc.Angle(platform.Position + new Vector2((!flag) ? 15 : 0, 0f), Position);
				Speed = Calc.AngleToVector(angleRadians, length);
				prevXSpeed = Speed.X + (flag ? (-0.1f) : 0.1f);
				Speed.Y *= -0.9f;
				cleanShot = false;
			}
			else
			{
				if (!(Speed.Y > 0f))
				{
					return;
				}
				Sounds.sfx_devTimeFinalDummy.Play(base.X);
				TowerBallRoundLogic towerBallRoundLogic = (TowerBallRoundLogic)base.Level.Session.RoundLogic;
				towerBallRoundLogic.IncreaseScore(PlayerIndex, AssistIndex, slamming, allyOop, cleanShot, (!flag) ? 1 : 0);
				towerBallRoundLogic.SpawnBallChest(300);
				if ((flag && Position.X - 5f < platform.Position.X) || (!flag && Position.X + 5f >= platform.Position.X + 15f))
				{
					Speed.X *= -1f;
				}
				else
				{
					Speed.X = MathHelper.Lerp(0f, prevXSpeed, Math.Max(Speed.Y / 2f, 1f));
				}
				towerBallRoundLogic.AddDeadBall(Position, Speed);
				if ((bool)slamming)
				{
					Explosion.Spawn(base.Level, platform.Position + new Vector2(7.5f, 0f), base.PlayerIndex, plusOneKill: false, triggerBomb: false, bombTrap: false);
					towerBallRoundLogic.AddSlamNotification(platform.Position + (flag ? new Vector2(-10f, 0f) : new Vector2(10f, 0f)));
					if ((bool)Level.KingIntro)
					{
						Level.KingIntro.Laugh();
					}
				}
				((BasketBallBasket)platform).DoNetJump();
				RemoveSelf();
			}
		}
		else
		{
			if (!platform.Collidable)
			{
				return;
			}
			if (Math.Abs(Speed.Y) > 1.5f)
			{
				Sounds.Play("TowerBall/BOUNCYBALL", X, Math.Min(Math.Abs(Speed.Y / 5f), 1f));
            }
			Speed.Y *= -0.9f;
			if (Math.Abs(Speed.Y) < 1f)
			{
				Speed.Y = 0f;
			}
			else if (Math.Abs(Speed.Y) > 1.75f)
			{
				int num = (int)Math.Round(Math.Abs(Speed.Y)) / 2;
				if (num > 0)
				{
					base.Level.Particles.Emit(Particles.PlayerDust[3], num, Position + new Vector2(0f, 4f), Vector2.One * 3f);
				}
			}
			forceState();
			cleanShot = false;
		}
	}

    public static Subtexture CreateGraphicPickup()
    {
		return ExampleModModule.Atlas["towerball/ball"];
    }

    public override void EnterFallMode(bool bounce = true, bool zeroX = false, bool sound = true)
	{
		fallModeEntered = true;
		base.EnterFallMode(bounce, zeroX, sound);
		Speed.X *= 2f;
		if (Speed.X == 0f)
		{
			if (X < 160f)
			{
				Speed.X = 1f;
			}
			else
			{
				Speed.X = -1f;
			}
		}
	}

	
	public override void Update()
	{
		prevXSpeed = Speed.X;
		float rotation = ballImage.Rotation;
		if ((bool)slamming)
		{
			slamming.Update();
			if (Speed.Y <= 0.1f)
			{
				slamming.Set(0);
			}
		}
		if (cleanShot && Math.Abs(Speed.X) == 0f)
		{
			cleanShot = false;
		}
		if ((bool)cantCollectCounter)
		{
			cantCollectCounter.Update();
		}
		bool value = false;
		if (PlayerIndex >= 0) 
		{
			value = base.Level.Session.MatchSettings.Variants.NoSeekingArrows[PlayerIndex];
			base.Level.Session.MatchSettings.Variants.NoSeekingArrows[PlayerIndex] = true;
		}

		fallModeEntered = false;
		base.Update();
		if (PlayerIndex >= 0)
		{
			base.Level.Session.MatchSettings.Variants.NoSeekingArrows[PlayerIndex] = value;
		}
		if (fallModeEntered)
		{
			prevXSpeed = Speed.X;
		}
		forceState();
		if (base.State == ArrowStates.Gravity)
		{
			Speed.X = Calc.Approach(prevXSpeed, 0f, 0.025f * Engine.TimeMult);
		}
		else
		{
			Speed.X = prevXSpeed;
		}
		ballImage.Rotation = rotation + Speed.X * 0.002f;
	}

	public override void Render()
	{
		base.Render();
	}

    public override void HitLava()
    {
    }
}
