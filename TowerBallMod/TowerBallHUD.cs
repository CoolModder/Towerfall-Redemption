using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace TowerBall;

public class TowerBallHUD : Entity
{
	private TowerBallRoundLogic roundLogic;

	private Image teamA;

	private Image teamB;

	private Color colorA;

	private Color colorB;

	public float flashAlpha;

	public TowerBallHUD(TowerBallRoundLogic rL)
		: base(3)
	{
		roundLogic = rL;
		teamA = new OutlineImage(TFGame.MenuAtlas["teamA"]);
		teamB = new OutlineImage(TFGame.MenuAtlas["teamB"]);
		teamA.Y = (teamB.Y = 0f);
		teamA.X = 5f;
		teamB.X = 282f;
		colorA = new Color(0, 64, 88);
		colorB = new Color(136, 20, 0);
		flashAlpha = 0f;
	}

	public override void Update()
	{
		if (flashAlpha > 0f)
		{
			flashAlpha -= 0.04f * Engine.TimeMult;
		}
		base.Update();
	}

	public override void Render()
	{
		if (flashAlpha > 0f && !SaveData.Instance.Options.RemoveScreenFlashEffects)
		{
			Draw.Rect(0f, 0f, 320f, 240f, Color.White * flashAlpha);
		}
		teamA.Render();
		teamB.Render();
		Draw.Rect(38f, 5f, 34f, 20f, Color.Black);
		Draw.Rect(248f, 5f, 34f, 20f, Color.Black);
		Draw.Rect(39f, 6f, 32f, 18f, Color.White);
		Draw.Rect(249f, 6f, 32f, 18f, Color.White);
		Draw.Rect(40f, 7f, 30f, 16f, colorA);
		Draw.Rect(250f, 7f, 30f, 16f, colorB);
		Draw.OutlineTextCentered(TFGame.Font, roundLogic.Session.Scores[0].ToString(), new Vector2(56f, 16f), Color.White, 2f);
		Draw.OutlineTextCentered(TFGame.Font, roundLogic.Session.Scores[1].ToString(), new Vector2(266f, 16f), Color.White, 2f);
	}
}
