using System;
using System.Numerics;

namespace BiArcTutorial
{
    public struct BiArc
    {
        public readonly Arc A1;
        public readonly Arc A2;

        /// <summary>
        /// Calculate parameters of the biarc
        /// </summary>
        /// <param name="P1">Start point</param>
        /// <param name="T1">Tangent vector at P1</param>
        /// <param name="P2">End point</param>
        /// <param name="T2">Tangent vector at P2</param>
        /// <param name="G">Transition point</param>
        public BiArc(Vector2 P1, Vector2 T1, Vector2 P2, Vector2 T2, Vector2 G)
        {
            // Calculate perpendicular lines to the tangent at P1 and P2
            var tl1 = Line.CreatePerpendicularAt(P1, P1 + T1);
            var tl2 = Line.CreatePerpendicularAt(P2, P2 + T2);

            // Calculate the perpendicular bisector of P1T and P2T
            var P1T2 = (P1 + G) / 2;
            var pbP1T = Line.CreatePerpendicularAt(P1T2, G);

            var P2T2 = (P2 + G) / 2;
            var pbP2T = Line.CreatePerpendicularAt(P2T2, G);

            // The origo of the circles are at the intersection points
            var C1 = tl1.Intersection(pbP1T);
            var C2 = tl2.Intersection(pbP2T);

            // Calculate the radii
            var r1 = (C1 - P1).Length();
            var r2 = (C2 - P2).Length();

            // Calculate start and sweep angles
            var startVector1 = P1 - C1;
            var endVector1 = G - C1;
            var startAngle1 = Math.Atan2(startVector1.Y, startVector1.X);
            var sweepAngle1 = Math.Atan2(endVector1.Y, endVector1.X) - startAngle1;

            var startVector2 = G - C2;
            var endVector2 = P2 - C2;
            var startAngle2 = Math.Atan2(startVector2.Y, startVector2.X);
            var sweepAngle2 = Math.Atan2(endVector2.Y, endVector2.X) - startAngle2;

            var cw = Curve.IsClockWise(P1, P2, G);

            // Adjust angles according to the orientation of the curve
            if (cw && sweepAngle1 < 0) sweepAngle1 = 2 * Math.PI + sweepAngle1;
            if (!cw && sweepAngle1 > 0) sweepAngle1 = sweepAngle1 - 2 * Math.PI;
            if (cw && sweepAngle2 < 0) sweepAngle2 = 2 * Math.PI + sweepAngle2;
            if (!cw && sweepAngle2 > 0) sweepAngle2 = sweepAngle2 - 2 * Math.PI;

            A1 = new Arc(C1, r1, (float)startAngle1, (float)sweepAngle1, P1, G);
            A2 = new Arc(C2, r2, (float)startAngle2, (float)sweepAngle2, G, P2);
        }

        /// <summary>
        /// Implement the parametric equation.
        /// </summary>
        /// <param name="t">Parameter of the curve. Must be in [0,1]</param>
        /// <returns></returns>
        public Vector2 PointAt(float t)
        {
            var s = JointAt;

            if (t <= s)
            {
                return A1.PointAt(t / s);
            }
            else
            {
                return A2.PointAt((t - s) / (1 - s));
            }
        }

        public float Length
        {
            get { return A1.Length + A2.Length; }
        }

        /// <summary>
        /// Parameter value for the joint of the two arcs
        /// </summary>
        public float JointAt
        {
            get { return A1.Length / (A1.Length + A2.Length); }
        }
    }
}
