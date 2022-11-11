﻿using System;
using System.Numerics;

namespace BiArcTutorial
{
    /// <summary>
    /// Definition of an Arc. It contains redundant information.
    /// Positive angle direction is clockwise as required by System.Drawing
    /// </summary>
    public struct Arc
    {
        /// <summary>
        /// Center point
        /// </summary>
        public readonly Vector2 C;
        /// <summary>
        /// Radius
        /// </summary>
        public readonly float r;
        /// <summary>
        /// Start angle in radian
        /// </summary>
        public readonly float startAngle;
        /// <summary>
        /// Sweep angle in radian
        /// </summary>
        public readonly float sweepAngle;
        /// <summary>
        /// Start point of the arc
        /// </summary>
        public readonly Vector2 P1;
        /// <summary>
        /// End point of the arc
        /// </summary>
        public readonly Vector2 P2;

        public Arc(Vector2 C, float r, float startAngle, float sweepAngle, Vector2 P1, Vector2 P2)
        {
            this.C = C;
            this.r = r;
            this.startAngle = startAngle;
            this.sweepAngle = sweepAngle;
            this.P1 = P1;
            this.P2 = P2;
        }

        /// <summary>
        /// Orientation of the arc.
        /// </summary>
        public bool IsClockwise
        {
            get { return sweepAngle > 0; }
        }

        /// <summary>
        /// Implement the parametric equation.
        /// </summary>
        /// <param name="t">Parameter of the curve. Must be in [0,1]</param>
        /// <returns></returns>
        public Vector2 PointAt(float t)
        {
            var x = C.X + r * Math.Cos(startAngle + t * sweepAngle);
            var y = C.Y + r * Math.Sin(startAngle + t * sweepAngle);
            return new Vector2((float)x, (float)y);
        }

        public float Length
        {
            get { return r * Math.Abs(sweepAngle); }
        }
    }
}
