// TowerFall.Slime
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;
public class Slime : Actor
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

	private static Stack<Slime> cached = new Stack<Slime>();

	public const int BRAMBLE_LIFE = 600;

	public const int BRAMBLE_SHORT_LIFE = 180;

	private const int HALF_SIZE = 4;

	private const int WIGGLE = 4;

	private List<Slime> brothers;

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

	public static IEnumerator CreateSlime(Level level, Vector2 at, int ownerIndex, Action onComplete, int spread = 3, bool shortTime = false)
	{
		int id = nextID;
		nextID++;
		Queue<Vector2> toCheck = new Queue<Vector2>();
		Queue<int> spreads = new Queue<int>();
		List<Vector2> made = new List<Vector2>();
		toCheck.Enqueue(at);
		spreads.Enqueue(spread);
		List<Slime> created = new List<Slime>();
		int waited = 0;
		bool soundPlayed = false;
		float timeWaited = 0f;
		while (toCheck.Count > 0)
		{
			Vector2 vector = toCheck.Dequeue();
			int num = spreads.Dequeue();
			if (!made.Contains(vector))
			{
				Slime Slime = Create(id, vector, ownerIndex, 8 + (spread - num) * 2 - waited, shortTime);
				Slime.Depth += num;
				created.Add(Slime);
				level.Add(Slime);
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
		foreach (Slime item in created)
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

	public static Slime Create(int id, Vector2 position, int ownerIndex, int delay, bool shortTime)
	{
		Slime Slime = ((cached.Count <= 0) ? new Slime() : cached.Pop());
		Slime.Init(id, position, ownerIndex, Math.Max(delay, 1), shortTime);
		return Slime;
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

	private Slime(): base(Vector2.Zero)
	{
		base.Depth = -149;
		Tag(GameTags.Mud, GameTags.LavaCollider, GameTags.ExplosionCollider, GameTags.ShockCollider);
		ScreenWrap = true;
		Pushable = false;
		Fire = new FireControl(foreground: true, Vector2.Zero, Vector2.One * 5f, 30);
		Fire.Interval = 3;
		Fire.OnFinish = base.RemoveSelf;
		Add(Fire);
		base.Collider = (hitbox = new WrapHitbox(10f, 10f, -4f, -4f));
		ExplosionCheckOffsets = new Vector2[4]
		{
			base.Collider.TopCenter,
			base.Collider.BottomCenter,
			base.Collider.CenterLeft,
			base.Collider.CenterRight
		};
		image = new FlashingImage(ExampleModModule.SlimeAtlas["Slime"]);
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
		CollideDo(GameTags.Mud, OtherBrambleCheck);
		if (OwnerIndex != -1 && base.Level.Session.MatchSettings.Variants.InfiniteBrambles[OwnerIndex])
		{
			deathAlarm.Stop();
		}
	}

	private void OtherBrambleCheck(Entity e)
	{
        if (e is not Mud && e is Slime)
		{ 
			if ((e as Slime).ID != ID)
			{
				(e as Slime).TweenOutNoSound();
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

	private void SetBrothers(List<Slime> brothers)
	{
		this.brothers = brothers;
	}

	private void TweenIn()
	{
		Visible = true;
		float startAngle = image.Rotation;
		Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.BackOut, 20, start: true);
		tween.OnUpdate = delegate(Tween t)
		{
            image.Rotation = MathHelper.Lerp(startAngle, startAngle + 40f, t.Eased);
            image.Scale = Vector2.One * t.Eased * 0.8f;
			if (t.Eased > 0.1f && t.Eased < 0.8f && Scene.OnInterval(1))
			{
				Level.ParticlesBG.Emit(Particles.MudPlayerRun, Position);
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
            image.Rotation = MathHelper.Lerp(startAngle, startAngle + 40f, t.Eased);
            image.Scale = Vector2.One * (1f - t.Eased);
			if (t.Eased > 0.1f && t.Eased < 0.8f && Level.OnInterval(1))
			{
				Level.ParticlesBG.Emit(Particles.MudPlayerRun, Position);
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
		foreach (Slime brother in brothers)
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
