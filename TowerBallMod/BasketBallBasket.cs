using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using FortRise;
using System.Xml;

namespace TowerBall;


public class BasketBallBasket : JumpThru
{
	public OutlineImage rim;

	public OutlineImage net;

	public Tween netJump;

	public BasketBallBasket(XmlElement xml, Vector2 position)
		: base(position, 15)
	{
		bool flag = Position.X > 160f;
		Position.X -= 10f;
		Position.Y += 10f;
		if (flag)
		{
			Position.X += 5f;
		}
		base.Depth++;
		rim = new OutlineImage(ExampleModModule.Atlas["towerball/rim"])
		{
			Origin = new Vector2(1f, 1f),
			FlipX = flag
		};
		net = new OutlineImage(ExampleModModule.Atlas["towerball/net"])
		{
			Origin = new Vector2(-3f, -2f),
			FlipX = flag
		};
		if (flag)
		{
			rim.Position.X = 1f;
			net.Position.X = -2f;
			rim.OutlineColor = new Color(248, 120, 88);
		}
		else
		{
			rim.OutlineColor = new Color(164, 228, 252);
		}
		Logger.Log("[TowerBall] BASKETS LOADED");
		Add(net);
		Add(rim);
	}

	public override void Update()
	{
		base.Update();
		if (netJump != null)
		{
			netJump.Update();
			net.Scale.Y = netJump.Eased * 0.5f + 0.5f;
		}
		else
		{
			net.Scale.Y = 1f;
		}
	}

	public void DoNetJump()
	{
		netJump = Tween.Create(Tween.TweenMode.Oneshot, Ease.BounceOut, 40, start: true);
	}
}
