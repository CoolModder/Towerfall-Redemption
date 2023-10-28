// TowerFall.Ice
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace OopsAllArrowsMod;
public class Shock : Actor
{
    private Sprite<int> sprite;
    public int OwnerIndex { get; private set; }
    private Solid riding;

    public const int SHOCK_LIFE = 600;
    private Alarm deathAlarm;
    private Vector2[] CollisionOffsets;
    private Direction direction;
    private float speed = 0.05f;
    private Side side;
    public enum Direction
    {
        Up, Down, Left, Right
    }
    public enum Side
    {
        Up, Down, Left, Right
    }
    public Shock(Vector2 position, Direction newdirection, Side newside) : base(position)
    {
        Position = position;
        base.Depth = -100;
        direction = newdirection;
        side = newside;
        Tag(GameTags.PlayerCollider, GameTags.LightSource);
        ScreenWrap = true;
        Pushable = false;
        base.Collider = new WrapHitbox(12f, 12f, -4f, -4f);
        CollisionOffsets = new Vector2[4]
        {
            base.Collider.TopLeft,
            base.Collider.TopCenter,
            base.Collider.BottomCenter,
            base.Collider.Center
        };
        sprite = OopsArrowsModModule.SpriteData.GetSpriteInt("Shock");
        sprite.Play(0, false);
        sprite.CenterOrigin();
        Add(sprite);
        Add(deathAlarm = Alarm.Create(Alarm.AlarmMode.Persist, RemoveSelf));
        deathAlarm.Start(SHOCK_LIFE + 180);
    }
    public static IEnumerator CreateShock(Level level, Vector2 at, float rotation, int ownerIndex, Action onComplete)
    {

        float Angle = rotation / ((float)Math.PI / 180) - 180;
        Console.WriteLine(Angle);
        Vector2 OffSet = new Vector2(0, 0);
        Direction Test = Direction.Down;
        Side side = Side.Left;
        if (-45 < Angle && Angle >= -135f)
        {

            Entity testentity = level.CollideFirst(new Rectangle((int)at.X, (int)at.Y, 12, 12), GameTags.Solid);
            if (testentity != null)
            {
                OffSet -= new Vector2(0.5f, 0);
            }
        }
        else if (-135f < Angle && Angle >= -225f)
        {
            Test = Direction.Right;
            side = Side.Down;
            Entity testentity = level.CollideFirst(new Rectangle((int)at.X, (int)at.Y, 12, 12), GameTags.Solid);
            if (testentity != null)
            {
                OffSet -= new Vector2(0, 0.5f);

            }
        }
        else if (-225f < Angle && Angle >= -315f)
        {
            Test = Direction.Up;
            side = Side.Right;
            Entity testentity = level.CollideFirst(new Rectangle((int)at.X, (int)at.Y, 12, 12), GameTags.Solid);
            if (testentity != null)
            {
                OffSet += new Vector2(0, 0.5f);
            }
        }
        else
        {
            Test = Direction.Left;
            side = Side.Up;
            Entity testentity = level.CollideFirst(new Rectangle((int)at.X, (int)at.Y, 12, 12), GameTags.Solid);
            if (testentity != null)
            {
                OffSet += new Vector2(0.5f, 0);
            }
        }
        Shock MyShock = new Shock(at + OffSet, Test, side);
        MyShock.OwnerIndex = ownerIndex;
        level.Add(MyShock);
        yield return 0.000001f;
        onComplete?.Invoke();
    }
    public override void DoWrapRender()
    {
        sprite.DrawOutline();
        sprite.Render();
        base.DoWrapRender();

    }
    public override bool IsRiding(Solid solid)
    {
        return riding == solid;
    }

    public override void Update()
    {
        base.Update();
        Vector2 Offset = new Vector2(0, 0);
        if (direction == Direction.Up)
        {
            Offset += new Vector2(0, -speed);
        }
        else if (direction == Direction.Down)
        {
            Offset += new Vector2(0, speed);
        }
        else if (direction == Direction.Left)
        {
            Offset += new Vector2(-speed, 0);
        }
        else
        {
            Offset += new Vector2(speed, 0);
        }

        Entity testentity = Level.CollideFirst(new Rectangle((int)Position.X + ((int)Offset.X), (int)Position.Y + ((int)Offset.Y), 12, 12), GameTags.Solid);
        if (testentity == null)
        {
            if (direction == Direction.Up)
            {
                if (side == Side.Right)
                {
                    direction = Direction.Right;
                }
                else
                {
                    direction = Direction.Left;
                }
                side = Side.Up;
            }
            else if (direction == Direction.Down)
            {
                if (side == Side.Right)
                {
                    direction = Direction.Right;
                }
                else
                {
                    direction = Direction.Left;
                }
                side = Side.Down;
            }
            else if (direction == Direction.Left)
            {
                if (side == Side.Up)
                {
                    direction = Direction.Up;

                }
                else
                {
                    direction = Direction.Down;
                }
                side = Side.Left;
            }
            else
            {
                if (side == Side.Up)
                {
                    direction = Direction.Up;
                }
                else
                {
                    direction = Direction.Down;
                }
                side = Side.Right;
            }
        }
        else
        {
            if (side == Side.Up)
            {
                testentity = Level.CollideFirst(new Rectangle((int)Position.X + ((int)Offset.X), (int)Position.Y + ((int)Offset.Y - (int)0.05), 12, 12), GameTags.Solid);
            }
            else if (side == Side.Down)
            {
                testentity = Level.CollideFirst(new Rectangle((int)Position.X + ((int)Offset.X), (int)Position.Y + ((int)Offset.Y + (int)0.05), 12, 12), GameTags.Solid);
            }
            else if (side == Side.Left)
            {
                testentity = Level.CollideFirst(new Rectangle((int)Position.X + ((int)Offset.X) - (int)0.05, (int)Position.Y + ((int)Offset.Y), 12, 12), GameTags.Solid);
            }
            else if (side == Side.Right)
            {
                testentity = Level.CollideFirst(new Rectangle((int)Position.X + ((int)Offset.X)  + (int)0.05, (int)Position.Y + ((int)Offset.Y), 12, 12), GameTags.Solid);
            }
            if (testentity != null)
            {
                if (direction == Direction.Up)
                {
                    if (side == Side.Left)
                    {
                        direction = Direction.Right;
                        side = Side.Up;
                    }
                    else
                    {
                        direction = Direction.Left;
                        side = Side.Up;
                    }
                }
                else if (direction == Direction.Down)
                {

                    if (side == Side.Right)
                    {
                        direction = Direction.Left;
                        side = Side.Down;
                    }
                    else
                    {
                        direction = Direction.Right;
                        side = Side.Down;
                    }
                }
                else if (direction == Direction.Left)
                {
                    if (side == Side.Down)
                    {
                        direction = Direction.Up;

                    }
                    else
                    {
                        direction = Direction.Down;
                    }
                    side = Side.Left;
                }
                else
                {
                    if (side == Side.Down)
                    {
                        direction = Direction.Up;
                    }
                    else
                    {
                        direction = Direction.Down;
                    }
                    side = Side.Right;
                }
            }
        }

        if (direction == Direction.Up)
        {
            Offset = new Vector2(0, -speed);
        }
        else if (direction == Direction.Down)
        {
            Offset = new Vector2(0, speed);
        }
        else if (direction == Direction.Left)
        {
            Offset = new Vector2(-speed, 0);
        }
        else
        {
            Offset = new Vector2(speed, 0);
        }
        Position += Offset;
        Console.WriteLine(direction);
    }
    public override void Render()
    {
        base.Render();
        sprite.DrawOutline();
        sprite.Render();
    }
    public override void Removed()
    {
        riding = null;
    }

    public override bool IsRiding(JumpThru jumpThru)
    {
        Sounds.sfx_boltArrowExplode.Play();
        RemoveSelf();
        return false;
    }
    public override void OnPlayerCollide(Player player)
    {
        if (base.Level.Session.CanHurtPlayer(OwnerIndex, player.PlayerIndex))
        {
            player.Hurt(DeathCause.Shock, Position, OwnerIndex, Sounds.sfx_boltArrowExplode);
        }
    }
}
