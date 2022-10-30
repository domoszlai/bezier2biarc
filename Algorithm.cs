using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace BiArcTutorial
{
    public class Algorithm
    {

        /// <summary>
        /// Algorithm to approximate a bezier curve with biarcs
        /// </summary>
        /// <param name="bezier">The bezier curve to be approximated.</param>
        /// <param name="nrPointsToCheck">The number of points used for calculating the approximation error.</param>
        /// <param name="tolerance">The approximation is accepted if the maximum devation at the sampling points is smaller than this number.</param>
        /// <returns></returns>
        public static List<BiArc> ApproxCubicBezier(CubicBezier bezier, int nrPointsToCheck, float tolerance)
        {
            // The result will be put here
            List<BiArc> biarcs = new List<BiArc>();

            // The bezier curves to approximate
            var curves = new Stack<CubicBezier>();
            curves.Push(bezier);

            // ---------------------------------------------------------------------------
            // First, calculate the inflexion points and split the bezier at them (if any)

            var toSplit = curves.Pop();

            // Edge case: P1 == P2 -> Split bezier
            if (bezier.P1 == bezier.P2)
            {
                var bs = bezier.Split(0.5f);
                curves.Push(bs.Item2);
                curves.Push(bs.Item1);
            }
            // Edge case -> no inflexion points
            else if (toSplit.P1 == toSplit.C1 || toSplit.P2 == toSplit.C2)
            {
                curves.Push(toSplit);
            }
            else
            {
                var inflex = toSplit.InflexionPoints;

                var i1 = inflex.Count > 0;
                var i2 = inflex.Count > 1;

                if (i1 && !i2)
                {
                    var splited = toSplit.Split((float)inflex[0].Real);
                    curves.Push(splited.Item2);
                    curves.Push(splited.Item1);
                }
                else if (!i1 && i2)
                {
                    var splited = toSplit.Split((float)inflex[1].Real);
                    curves.Push(splited.Item2);
                    curves.Push(splited.Item1);
                }
                else if (i1 && i2)
                {
                    var t1 = (float)inflex[0].Real;
                    var t2 = (float)inflex[1].Real;

                    // I'm not sure if I need, but it does not hurt to order them
                    if (t1 > t2)
                    {
                        var tmp = t1;
                        t1 = t2;
                        t2 = tmp;
                    }

                    // Make the first split and save the first new curve. The second one has to be splitted again
                    // at the recalculated t2 (it is on a new curve)

                    var splited1 = toSplit.Split(t1);

                    t2 = (1 - t1) * t2;

                    toSplit = splited1.Item2;
                    var splited2 = toSplit.Split(t2);

                    curves.Push(splited2.Item2);
                    curves.Push(splited2.Item1);
                    curves.Push(splited1.Item1);
                }
                else
                {
                    curves.Push(toSplit);
                }
            }

            // ---------------------------------------------------------------------------
            // Second, approximate the curves until we run out of them

            while (curves.Count > 0)
            {
                bezier = curves.Pop();

                // ---------------------------------------------------------------------------
                // Calculate the transition point for the BiArc 

                // V: Intersection point of tangent lines
                var C1 = bezier.P1 == bezier.C1 ? bezier.C2 : bezier.C1;
                var C2 = bezier.P2 == bezier.C2 ? bezier.C1 : bezier.C2;

                var T1 = new Line(bezier.P1, C1);
                var T2 = new Line(bezier.P2, C2);

                // Edge case: control lines are parallel
                if(T1.m == T2.m)
                {
                    var bs = bezier.Split(0.5f);
                    curves.Push(bs.Item2);
                    curves.Push(bs.Item1);
                    continue;
                }

                var V = T1.Intersection(T2);

                // G: incenter point of the triangle (P1, V, P2)
                // http://www.mathopenref.com/coordincenter.html
                var dP2V = Vector2.Distance(bezier.P2, V);
                var dP1V = Vector2.Distance(bezier.P1, V);
                var dP1P2 = Vector2.Distance(bezier.P1, bezier.P2);
                var G = (dP2V * bezier.P1 + dP1V * bezier.P2 + dP1P2 * V) / (dP2V + dP1V + dP1P2);

                // ---------------------------------------------------------------------------
                // Calculate the BiArc

                BiArc biarc = new BiArc(bezier.P1, (bezier.P1 - C1), bezier.P2, (bezier.P2 - C2), G);

                // ---------------------------------------------------------------------------
                // Calculate the maximum error

                var maxDistance = 0f;
                var maxDistanceAt = 0f;

                var parameterStep = 1f / nrPointsToCheck;

                for (int i = 0; i <= nrPointsToCheck; i++)
                {
                    var t = parameterStep * i;
                    var u1 = biarc.PointAt(t);
                    var u2 = bezier.PointAt(t);
                    var distance = (u1 - u2).Length();

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        maxDistanceAt = t;
                    }
                }

                // Check if the two curves are close enough
                if (maxDistance > tolerance)
                {
                    // If not, split the bezier curve the point where the distance is the maximum
                    // and try again with the two halfs
                    var bs = bezier.Split(maxDistanceAt);
                    curves.Push(bs.Item2);
                    curves.Push(bs.Item1);
                }
                else
                {
                    // Otherwise we are done with the current bezier
                    biarcs.Add(biarc);
                }
            }

            return biarcs;
        }

    }
}
