using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVERIZE
{
    public class Direction
    {
        public enum Horizontal { Left = -1, Middle, Right }
        public enum Vertical { Up = -1, Middle, Down }
        (Horizontal Horizontal, Vertical Vertical) bearing;

        Direction() => bearing = (Horizontal.Middle, Vertical.Middle);
        Direction(Horizontal horizontal) => bearing = (horizontal, Vertical.Middle);
        Direction(Vertical vertical) => bearing = (Horizontal.Middle, vertical);
        Direction(Horizontal horizontal, Vertical vertical) => bearing = (horizontal, vertical);

        public static implicit operator Horizontal(Direction direction) => direction.bearing.Horizontal;
        public static implicit operator Direction(Horizontal horizontal) => new Direction(horizontal);
        public static implicit operator Vertical(Direction direction) => direction.bearing.Vertical;
        public static implicit operator Direction(Vertical vertical) => new Direction(vertical);
        public static implicit operator Vector2(Direction direction) => new Vector2((int)direction.bearing.Horizontal, (int)direction.bearing.Vertical);
        public static implicit operator Direction(Vector2 vector2) => new Direction((Horizontal)OVERMath.Sign(vector2.x), (Vertical)OVERMath.Sign(vector2.y));
    }
}
