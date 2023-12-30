
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;
public class NyanTrail : LevelEntity
{
    private Sprite<int> image;
    public NyanTrail(Vector2 position, float rotation) : base(position)
    {
        Position = position;
        image = OopsArrowsModModule.SpriteData.GetSpriteInt("NyanTrail");
        base.Collider = new Hitbox(1f, 1f, -4f, -4f);
        base.Collidable = false;
        image.CenterOrigin();
        image.Rotation = rotation;
        image.Play(0, false);
        Add(image);
        image.Play(0, false);
    }

    public static IEnumerator CreateNyanTrail(Level level, Vector2 at, float rotation)
    {
        NyanTrail MyNyanTrail = new NyanTrail(at, rotation);
        level.Add(MyNyanTrail);
        yield return 1;

        level.Remove(MyNyanTrail);  
    }
}
