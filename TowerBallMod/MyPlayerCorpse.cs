using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using On.TowerFall;
using IL.TowerFall;

namespace TowerBall;

public class MyPlayerCorpse : TowerFall.PlayerCorpse
{
	private static Counter removeCounter;

    public MyPlayerCorpse(Vector2 position, TowerFall.ArcherData archerData, Allegiance teamColor, Facing facing, int playerIndex, int killerIndex) : base(position, archerData, teamColor, facing, playerIndex, killerIndex)
    {
    }

    public static TowerFall.PlayerCorpse Corspe { get; private set; }

    public static void ctor_string_Allegiance_Vector2_Facing_int_int(On.TowerFall.PlayerCorpse.orig_ctor_string_Allegiance_Vector2_Facing_int_int orig, global::TowerFall.PlayerCorpse self, string corpseSpriteID, Allegiance teamColor, Vector2 position, Facing facing, int playerIndex, int killerIndex)
    {
        removeCounter = new Counter(150);
        orig(self, corpseSpriteID,teamColor, position, facing, playerIndex, killerIndex);
    }

	public static void MyUpdate(On.TowerFall.PlayerCorpse.orig_Update orig, global::TowerFall.PlayerCorpse self)
	{
		orig(self);
		if ((bool)removeCounter)
		{
			removeCounter.Update();
			if (!removeCounter)
			{
				Corspe = self;
				self.Flash(60, RemoveMe);
			}
		}
	}

	public static void RemoveMe()
	{
		Corspe.ArrowCushion.ReleaseArrows(new Vector2(0,2));
		Corspe.RemoveSelf();
	} 

	public static void Load()
	{
        On.TowerFall.PlayerCorpse.ctor_string_Allegiance_Vector2_Facing_int_int += MyPlayerCorpse.ctor_string_Allegiance_Vector2_Facing_int_int;
        On.TowerFall.PlayerCorpse.Update += MyPlayerCorpse.MyUpdate;
    }
	public static void Unload()
	{
        On.TowerFall.PlayerCorpse.ctor_string_Allegiance_Vector2_Facing_int_int -= MyPlayerCorpse.ctor_string_Allegiance_Vector2_Facing_int_int;
        On.TowerFall.PlayerCorpse.Update -= MyPlayerCorpse.MyUpdate;
    }
}
