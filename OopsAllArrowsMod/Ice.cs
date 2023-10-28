// TowerFall.Ice
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;
public class Ice : Actor
{
	[Flags]
	private enum Corners
	{
		None = 0,
		BottomLeft = 1,
		BottomRight = 2,
		TopLeft = 4,
		TopRight = 8
	}

	private static int nextID = 0;

	private static Vector2[] Checks = new Vector2[5]
	{
		Vector2.Zero,
		Vector2.UnitX * 4f,
		Vector2.UnitX * -4f,
		Vector2.UnitY * 4f,
		Vector2.UnitY * -4f
	};

	private static Stack<Ice> cached = new Stack<Ice>();

	public const int BRAMBLE_LIFE = 600;

	public const int BRAMBLE_SHORT_LIFE = 180;

	private const int HALF_SIZE = 4;

	private const int WIGGLE = 4;

	private List<Ice> brothers;

	private FlashingImage image;

	private Solid riding;

	private bool soundPlayed;

	private WrapHitbox hitbox;

	private Alarm delayAlarm;

	private Alarm deathAlarm;

	private bool tweenedOut;

	public int ID { get; private set; }

	public FireControl Fire { get; private set; }

	public int OwnerIndex { get; private set; }

	public static IEnumerator CreateIce(Level level, Vector2 at, int ownerIndex, Action onComplete, int spread = 3, bool shortTime = false)
	{
		int id = nextID;
		nextID++;
		Queue<Vector2> toCheck = new Queue<Vector2>();
		Queue<int> spreads = new Queue<int>();
		List<Vector2> made = new List<Vector2>();
		toCheck.Enqueue(at);
		spreads.Enqueue(spread);
		List<Ice> created = new List<Ice>();
		int waited = 0;
		bool soundPlayed = false;
		float timeWaited = 0f;
		while (toCheck.Count > 0)
		{
			Vector2 vector = toCheck.Dequeue();
			int num = spreads.Dequeue();
			if (!made.Contains(vector))
			{
				Ice Ice = Create(id, vector, ownerIndex, 8 + (spread - num) * 2 - waited, shortTime);
				Ice.Depth += num;
				created.Add(Ice);
				level.Add(Ice);
				made.Add(vector);
				if (num > 0)
				{
					TrySpread(level, toCheck, spreads, vector - Vector2.UnitX * 8f, num);
					TrySpread(level, toCheck, spreads, vector + Vector2.UnitX * 8f, num);
					TrySpread(level, toCheck, spreads, vector - Vector2.UnitY * 8f, num);
					TrySpread(level, toCheck, spreads, vector + Vector2.UnitY * 8f, num);
				}
				waited++;
				timeWaited += 0.6f;
				yield return 0.6f;
				if (made.Count > 0 && timeWaited >= 0.5f && !soundPlayed)
				{
					soundPlayed = true;
					Sounds.pu_brambleGrow.Play(at.X);
				}
			}
		}
		if (made.Count > 0 && !soundPlayed)
		{
			Sounds.pu_brambleGrow.Play(at.X);
		}
		foreach (Ice item in created)
		{
			item.SetBrothers(created);
		}
		toCheck.Clear();
		spreads.Clear();
		made.Clear();
		onComplete?.Invoke();
	}

	private static void TrySpread(Level level, Queue<Vector2> toCheck, Queue<int> spreads, Vector2 to, int spread)
	{
		Vector2[] checks = Checks;
		foreach (Vector2 vector in checks)
		{
			if (CanSpreadTo(level, to + vector))
			{
				toCheck.Enqueue(to + vector);
				spreads.Enqueue(spread - 1);
				break;
			}
		}
	}

	public static bool CanSpreadTo(Level level, Vector2 to)
	{
		int num = 0;
		if (level.CollideCheck(WrapMath.ApplyWrap(to + Vector2.UnitX * 4f), GameTags.Solid))
		{
			num++;
		}
		if (level.CollideCheck(WrapMath.ApplyWrap(to - Vector2.UnitX * 4f), GameTags.Solid))
		{
			num++;
		}
		if (num == 1)
		{
			return true;
		}
		if (level.CollideCheck(WrapMath.ApplyWrap(to + Vector2.UnitY * 4f), GameTags.Solid))
		{
			num++;
		}
		if (num == 2 || num == 1)
		{
			return true;
		}
		if (level.CollideCheck(WrapMath.ApplyWrap(to - Vector2.UnitY * 4f), GameTags.Solid))
		{
			num++;
		}
		if (num > 0)
		{
			return num < 4;
		}
		return false;
	}

	public static Ice Create(int id, Vector2 position, int ownerIndex, int delay, bool shortTime)
	{
		Ice Ice = ((cached.Count <= 0) ? new Ice() : cached.Pop());
		Ice.Init(id, position, ownerIndex, Math.Max(delay, 1), shortTime);
		return Ice;
	}

	public static void GetBrambleColors(int ownerIndex, bool teamsMode, Allegiance teamColor, out Color colorA, out Color colorB)
	{
		if (teamsMode)
		{
			colorA = ArcherData.GetColorA(ownerIndex, teamColor);
			colorB = ArcherData.GetColorB(ownerIndex, teamColor);
		}
		else
		{
			colorA = ArcherData.GetColorA(ownerIndex);
			colorB = ArcherData.GetColorB(ownerIndex);
		}
	}

	private Ice(): base(Vector2.Zero)
	{
		base.Depth = 0;
		Tag(GameTags.Ice, GameTags.LavaCollider, GameTags.ExplosionCollider, GameTags.ShockCollider);
		ScreenWrap = true;
		Pushable = false;
		Fire = new FireControl(foreground: true, Vector2.Zero, Vector2.One * 5f, 30);
		Fire.Interval = 3;
		Fire.OnFinish = base.RemoveSelf;
		Add(Fire);
		base.Collider = (hitbox = new WrapHitbox(8f, 8f, -4f, -4f));
		ExplosionCheckOffsets = new Vector2[4]
		{
			base.Collider.TopCenter,
			base.Collider.BottomCenter,
			base.Collider.CenterLeft,
			base.Collider.CenterRight
		};
		image = new FlashingImage(OopsArrowsModModule.ArrowAtlas["Ice"]);
		image.CenterOrigin();
		Add(image);
		Add(delayAlarm = Alarm.Create(Alarm.AlarmMode.Persist, TweenIn));
		Add(deathAlarm = Alarm.Create(Alarm.AlarmMode.Persist, TweenOut));
	}

	private void Init(int id, Vector2 position, int ownerIndex, int delay, bool shortTime)
	{
		Position = position;
		OwnerIndex = ownerIndex;
		ID = id;
		delayAlarm.Start(delay);
		if (shortTime)
		{
			deathAlarm.Start(delay + 180);
		}
		else
		{
			deathAlarm.Start(delay + 600);
		}
		image.Rotation = Calc.Random.NextAngle();
		image.Scale = Vector2.Zero;
		Visible = (Collidable = false);
		soundPlayed = false;
		tweenedOut = false;
		Fire.Cancel();
		riding = null;
	}

	public override void Added()
	{
		base.Added();
		riding = CollideFirst(GameTags.Solid) as Solid;
		GetBrambleColors(OwnerIndex, base.Level.Session.MatchSettings.TeamMode, base.Level.Session.MatchSettings.Teams[OwnerIndex], out var colorA, out var colorB);
		image.StartFlashing(4, colorA, colorB);
		CollideDo(GameTags.Ice, OtherBrambleCheck);
		if (OwnerIndex != -1 && base.Level.Session.MatchSettings.Variants.InfiniteBrambles[OwnerIndex])
		{
			deathAlarm.Stop();
		}
	}

	private void OtherBrambleCheck(Entity e)
	{
		if (e is not TowerFall.Ice && e is Ice)
		{
			if ((e as Ice).ID != ID)
			{
				(e as Ice).TweenOutNoSound();
			}
		}
	}

	public override void Removed()
	{
		base.Removed();
		Remove<Tween>();
		brothers = null;
		riding = null;
		cached.Push(this);
	}

	private void SetBrothers(List<Ice> brothers)
	{
		this.brothers = brothers;
	}

	private void TweenIn()
	{
		Visible = true;
		image.Rotation = 0;
		float startAngle = image.Rotation;
		Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.BackOut, 20, start: true);
		tween.OnUpdate = delegate(Tween t)
		{
			image.Scale = Vector2.One * t.Eased * 0.8f;
			if (t.Eased > 0.1f && t.Eased < 0.8f && Scene.OnInterval(1))
			{
				Level.ParticlesBG.Emit(Particles.IcicleBreak, Position);
			}
		};
		tween.OnComplete = delegate
		{
			Collidable = true;
		};
		Add(tween);
	}

	public void TweenOutNoSound()
	{
		if (tweenedOut)
		{
			return;
		}
		Remove<Tween>();
		tweenedOut = true;
		Collidable = false;
		float startAngle = image.Rotation;
		Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.BackIn, 20, start: true);
		tween.OnUpdate = delegate(Tween t)
		{
			image.Scale = Vector2.One * (1f - t.Eased);
			if (t.Eased > 0.1f && t.Eased < 0.8f && Level.OnInterval(1))
			{
				Level.ParticlesBG.Emit(Particles.IcicleBreak, Position);
			}
		};
		tween.OnComplete = delegate
		{
			RemoveSelf();
		};
		Add(tween);
	}

	public void TweenOut()
	{
		if (tweenedOut)
		{
			return;
		}
		TweenOutNoSound();
		if (brothers == null)
		{
			return;
		}
		bool flag = false;
		foreach (Ice brother in brothers)
		{
			if (brother.soundPlayed)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			soundPlayed = true;
			Sounds.pu_brambleDisappear.Play(base.X);
		}
	}

	public override void OnExplode(Explosion explosion, Vector2 normal)
	{
		RemoveSelf();
	}

	public override void OnShock(ShockCircle shock)
	{
		RemoveSelf();
	}

	public override void Update()
	{
		base.Update();
		if (base.Level.OnInterval(4) && !CanSpreadTo(base.Level, Position))
		{
			RemoveSelf();
		}
	}

	public override void DoWrapRender()
	{
		image.DrawOutline();
		base.DoWrapRender();
	}
	public override bool IsRiding(Solid solid)
	{
		return riding == solid;
	}

	public override bool IsRiding(JumpThru jumpThru)
	{
		return false;
	}

	public override void MoveExactH(int move, Action<Platform> onCollide = null)
	{
		base.X += move;
	}

	public override void MoveExactV(int move, Action<Platform> onCollide = null)
	{
		base.Y += move;
	}
}
