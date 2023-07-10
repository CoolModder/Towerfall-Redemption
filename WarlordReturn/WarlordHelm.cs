using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerFall;
using Monocle;
namespace Warlord
{
    public class WarlordHelm : Actor
    {
        private const float GRAVITY = 0.3f;

        private const float MAX_FALL = 3f;

        private const float FRICTION = 0.2f;

        private ArcherData archerData;
        
        private Image image;

        private float spin;

        private Action<Platform> onCollideH;

        private Action<Platform> onCollideV;

        public Vector2 Speed;

        public WarlordHelm(Vector2 position, bool flipped, Arrow arrow, int ownerIndex) : base(position)
        {
            Position = position;
            image = new Image(ExampleModModule.Atlas["warlord/helmNoPlayer"]);
            image.CenterOrigin();
            image.FlipX = flipped;
            Add(image);
            base.Collider = new Hitbox(2f, 2f, -1f);
            spin = Calc.Random.Range(4f, 12f) * (float)Calc.Random.Choose<int>(1, -1) * ((float)Math.PI / 180f);
            if (arrow != null)
            {
                Speed = arrow.Speed + Vector2.UnitY * Calc.Random.Range(-1f, -1.2f);
                Speed.Y = MathHelper.Clamp(Speed.Y, -2f, 2f);
            }
            else
            {
                Speed.X = Calc.Random.Range(0.8f, 0.8f) * (float)Calc.Random.Choose<int>(1, -1);
                Speed.Y = Calc.Random.Range(-1f, -1.2f);
            }

            onCollideH = CollideH;
            onCollideV = CollideV;
            ScreenWrap = true;
            Tag(GameTags.Hat);
        }
        private void CollideH(Platform platform)
        {
            Speed.X *= -0.5f;
            ResetSpin();
            Sounds.sfx_endropStaffHead.Play(base.X);
        }

        private void CollideV(Platform platform)
        {
            ResetSpin();
            if (Speed.Y >= 2f)
            {
                Speed.X += Calc.Random.Range(-1f, 1f);
                Speed.Y *= -0.5f;
            }
            else
            {
                Speed.Y = 0f;
            }

            Sounds.sfx_endropStaffHead.Play(base.X);
        }

        public bool CanPickUp(ArcherData archerData)
        {
            return false;
        }

        public override void Update()
        {
            if (CheckBelow())
            {
                Speed.X = Calc.Approach(Speed.X, 0f, 0.2f * Engine.TimeMult);
                float radiansB = Calc.ShorterAngleDifference(image.Rotation, 0f, (float)Math.PI);
                image.Rotation += MathHelper.Clamp(Calc.AngleDiff(image.Rotation, radiansB), -(float)Math.PI / 30f, (float)Math.PI / 30f) * Engine.TimeMult;
            }
            else
            {
                Speed.Y = Math.Min(Speed.Y + 0.3f * ((Math.Abs(Speed.Y) <= 0.5f) ? 0.5f : 1f) * Engine.TimeMult, 3f);
                image.Rotation += spin * Engine.TimeMult;
            }
            MoveH(Speed.X * Engine.TimeMult, onCollideH);
            MoveV(Speed.Y * Engine.TimeMult, onCollideV);
            base.Update();
        }
        private void ResetSpin()
        {
            spin = (float)(Calc.Random.Range(4, 12) * -Math.Sign(spin)) * ((float)Math.PI / 180f);
        }
    }
}
