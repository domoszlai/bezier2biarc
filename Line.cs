using System;
using System.Numerics;

namespace BiArcTutorial
{
    /// <summary>
    /// Defines a line with the equation y = m*x + b
    /// </summary>
    public struct Line
    {
        /// <summary>
        /// Slope
        /// </summary>
        public readonly float m;
        /// <summary>
        /// Y-intercept
        /// </summary>
        public readonly float b;

        /// <summary>
        /// Define a line by two points
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        public Line(Vector2 P1, Vector2 P2) : this(P1, Slope(P1, P2))
        {
        }

        /// <summary>
        /// Define a line by a point and slope
        /// </summary>
        /// <param name="P"></param>
        /// <param name="m"></param>
        public Line(Vector2 P, float m) : this(m, P.Y - m * P.X)
        {
        }

        /// <summary>
        /// Define a line by slope and y-intercept
        /// </summary>
        /// <param name="m"></param>
        /// <param name="b"></param>
        public Line(float m, float b)
        {
            this.m = m;
            this.b = b;
        }

        /// <summary>
        /// Calculate the intersection point of this line and another one
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public Vector2 Intersection(Line l)
        {
            var x = (this.b - l.b) / (l.m - this.m);
            var y = m * x + b;
            return new Vector2(x, y);
        }

        public double Angle
        {
            get
            {
                var a = Math.Atan(m);
                return a < 0 ? a + Math.PI : a;
            }
        }

        public float y(float x)
        {
            return m * x + b;
        }

        public static float Slope(Vector2 P1, Vector2 P2)
        {
            return (P2.Y - P1.Y) / (P2.X - P1.X);
        }
    }
}
