using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using System;
namespace TowerBall;

public class TowerBallHUD : Entity
{
	private TowerBallRoundLogic roundLogic;
	private Image teamA;
	private Image teamB;
	private Color colorA;
	private Color colorB;
    private Color colorC;
    public float flashAlpha;

	public TowerBallHUD(TowerBallRoundLogic rL)
		: base(3)
	{
		roundLogic = rL;
		teamA = new OutlineImage(TFGame.MenuAtlas["teamA"]);
		teamB = new OutlineImage(TFGame.MenuAtlas["teamB"]);
		teamA.Y = (teamB.Y = 0f);
		teamA.X = 5f;
		teamB.X = EigthPlayerImport.IsEightPlayer != null ? EigthPlayerImport.IsEightPlayer() ? 382f : 282f : 282f;
		colorA = new Color(0, 64, 88);
		colorB = new Color(136, 20, 0);
		colorC = new Color(255, 255, 90);
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
		
		if (EigthPlayerImport.IsEightPlayer != null && EigthPlayerImport.IsEightPlayer()) 
		{
			if (flashAlpha > 0f && !SaveData.Instance.Options.RemoveScreenFlashEffects)
			{
				Draw.Rect(0f, 0f, 420f, 240f, Color.White * flashAlpha);
			}
			teamA.Render();
			teamB.Render();
			Draw.Rect(38f, 5f, 34f, 20f, Color.Black);
			Draw.Rect(348f, 5f, 34f, 20f, Color.Black);
			Draw.Rect(39f, 6f, 32f, 18f, Color.White);
			Draw.Rect(349f, 6f, 32f, 18f, Color.White);
			Draw.Rect(40f, 7f, 30f, 16f, colorA);
			Draw.Rect(350f, 7f, 30f, 16f, colorB);
            if (roundLogic.timed)
            {
                Draw.Rect(196f, 5f, 54f, 20f, Color.Black);
                Draw.Rect(197f, 6f, 52f, 18f, Color.White);
                Draw.Rect(198f, 7f, 50f, 16f, colorC);
                TimeSpan time = TimeSpan.FromSeconds(roundLogic.secondsleft);
                Draw.OutlineTextCentered(TFGame.Font, time.ToString(@"mm\:ss"), new Vector2(209f, 16f), Color.White, 2f);
                if (roundLogic.overtime)
                {
                    Draw.OutlineTextCentered(TFGame.Font, "OVERTIME!", new Vector2(209f, 40f), Color.White, 2f);
                }
            }
	
            Draw.OutlineTextCentered(TFGame.Font, roundLogic.Session.Scores[0].ToString(), new Vector2(56f, 16f), Color.White, 2f);
			Draw.OutlineTextCentered(TFGame.Font, roundLogic.Session.Scores[1].ToString(), new Vector2(366f, 16f), Color.White, 2f);
			return;
		}
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
        if (roundLogic.timed)
        {
            Draw.Rect(131f, 5f, 54f, 20f, Color.Black);
            Draw.Rect(132f, 6f, 52f, 18f, Color.White);
            Draw.Rect(133f, 7f, 50f, 16f, colorC);
            TimeSpan time = TimeSpan.FromSeconds(roundLogic.secondsleft);
            Draw.OutlineTextCentered(TFGame.Font, time.ToString(@"mm\:ss"), new Vector2(159f, 16f), Color.White, 2f);
            if (roundLogic.overtime)
            {
                Draw.OutlineTextCentered(TFGame.Font, "OVERTIME!", new Vector2(159f, 40f), Color.White, 2f);
            }
        }
        
        Draw.OutlineTextCentered(TFGame.Font, roundLogic.Session.Scores[0].ToString(), new Vector2(56f, 16f), Color.White, 2f);
		Draw.OutlineTextCentered(TFGame.Font, roundLogic.Session.Scores[1].ToString(), new Vector2(266f, 16f), Color.White, 2f);
	}
}
