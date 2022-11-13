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
        /// Based on: M.A. Sabin, The use of piecewise forms for the numerical representation of shape (1977)
        /// </summary>
        /// <param name="bezier">The bezier curve to be approximated.</param>
        /// <param name="nrPointsToCheck">The number of points used for calculating the approximation error.</param>
        /// <param name="tolerance">The approximation is accepted if the maximum devation at the sampling points is smaller than this number.</param>
        /// <returns></returns>
        public static List<Approx> ApproxCubicBezier(CubicBezier bezier, int nrPointsToCheck, float tolerance)
        {
            // The result will be put here
            List<Approx> biarcs = new List<Approx>();

            // The bezier curves to approximate
            var curves = new Stack<CubicBezier>();
            curves.Push(bezier);

            // ---------------------------------------------------------------------------
            // First, calculate the inflexion points and split the bezier at them (if any)

            var toSplit = curves.Pop();

            // Degenerate curve: P1 == P2 -> Split bezier
            if (bezier.P1 == bezier.P2)
            {
                var bs = bezier.Split(0.5f);
                curves.Push(bs.Item2);
                curves.Push(bs.Item1);
            }
            // Degenerate curve: P1 == C1 || P2 == C2 -> no inflexion points
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
                // Calculate the BiARC 

                // V: Intersection point of tangent lines
                var C1 = bezier.P1 == bezier.C1 ? bezier.C2 : bezier.C1;
                var C2 = bezier.P2 == bezier.C2 ? bezier.C1 : bezier.C2;

                var T1 = new Line(bezier.P1, C1);
                var T2 = new Line(bezier.P2, C2);

                // Control lines are parallel -> can't calculate triangle for biarc
                if(T1.m == T2.m)
                {
                    var bs = bezier.Split(0.5f);
                    curves.Push(bs.Item2);
                    curves.Push(bs.Item1);
                    continue;
                }

                var V = T1.Intersection(T2);

                // Biarc triangle has the wrong orientation
                // Curve looks like this: https://pomax.github.io/bezierinfo/images/chapters/decasteljau/df92f529841f39decf9ad62b0967855a.png
                if (bezier.IsClockWise != Curve.IsClockWise(bezier.P1, bezier.P2, V))
                {
                    var bs = bezier.Split(0.5f);
                    curves.Push(bs.Item2);
                    curves.Push(bs.Item1);
                    continue;
                }

                // G: incenter point of the triangle (P1, V, P2)
                // http://www.mathopenref.com/coordincenter.html
                var dP2V = Vector2.Distance(bezier.P2, V);
                var dP1V = Vector2.Distance(bezier.P1, V);
                var dP1P2 = Vector2.Distance(bezier.P1, bezier.P2);
                var G = (dP2V * bezier.P1 + dP1V * bezier.P2 + dP1P2 * V) / (dP2V + dP1V + dP1P2);

                BiArc biarc = new BiArc(bezier.P1, (bezier.P1 - C1), bezier.P2, (bezier.P2 - C2), G);

                // ---------------------------------------------------------------------------
                // Calculate the maximum error along the radial direction
                // TODO: D.J. Walton*, D.S. Meek, Approximation of a planar cubic B6zier spiral by circular arcs (1996)

                var maxDistance = 0f;
                var maxDistanceAt = 0f;

                var parameterStep = 1f / (nrPointsToCheck + 1);

                for (int i = 1; i <= nrPointsToCheck; i++)
                {
                    var t = parameterStep * i;
                    var bt = RadialDirectionIntersection(bezier, biarc, t);                    
                    if(bt != -1)
                    {
                        var distance = (bezier.PointAt(bt) - biarc.PointAt(t)).Length();

                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            maxDistanceAt = bt;
                        }
                    }
                }
                
                // Calculate parameter value on the bezier that corresponds to the biarc joint point
                var tj = RadialDirectionIntersection(bezier, biarc, biarc.JointAt);
                var dj = tj != -1 ? (bezier.PointAt(tj) - biarc.PointAt(biarc.JointAt)).Length() : 0;
                if (tj == -1)
                {
                    var jt2 = RadialDirectionIntersection(bezier, biarc, biarc.JointAt);
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
                    biarcs.Add(new Approx(bezier, biarc, null));
                }
            }

            return biarcs;
        }

        /// <summary>
        /// Takes a paramater `t` fore the `biarc` and calculates the the related parameter for
        /// the `bezier` (which is the intersection point in the radial direction)
        /// </summary>
        public static float RadialDirectionIntersection(CubicBezier bezier, BiArc biarc, float t)
        {
            if(t == 0 || t == 1)
            {
                return t;
            }

            var P = biarc.PointAt(t);
            var C = t <= biarc.JointAt ? biarc.A1.C : biarc.A2.C;
            var M = P - C;
            var H = Vector2.Normalize(new Vector2(-M.Y, M.X));

            var f = new Func<float, float>(u =>
            {
                return Vector2.Dot(bezier.PointAt(u) - P, H);
            });

            // https://proofwiki.org/wiki/Derivative_of_Dot_Product_of_Vector-Valued_Functions
            var df = new Func<float, float>(u =>
            {
                return Vector2.Dot(bezier.FirstDerivativePointAt(u), H);
            });

            return FindRoot(f, df, 0, 1);
        }

        /// <summary>
        /// Tries to find the root of f in interval [a,b] using the bisection method.
        /// It is supposed to have at most one solution. If no solution is found, returns -1
        /// </summary>
        public static float FindRoot(Func<float, float> f, float a, float b)
        {
            if (f(a) * f(b) >= 0) return -1;

            var maxiter = 100;
            var eps = 0.001;

            float x = default(float);
            float v = default(float);

            while (maxiter > 0)
            {
                x = (a + b) / 2;
                v = f(x);

                if (Math.Abs(v) < eps)
                {
                    return x;
                }

                if(f(a) * v < 0)
                {
                    b = x;
                }
                else if(f(b) * v < 0)
                {
                    a = x;
                }
                else
                {
                    return -1;
                }

                maxiter -= 1;
            }

            // We must be close enough now
            return x;
        }


        /// <summary>
        /// Tries to find the root of f in interval [a,b] using the Newton method.
        /// It is supposed to have at most one solution. If no solution is found, returns -1
        /// </summary>
        public static float FindRoot(Func<float, float> f, Func<float, float> d, float a, float b)
        {
            if (f(a) * f(b) >= 0) return -1;

            var maxiter = 100;
            var eps = 0.001;

            var x = (a + b) / 2;
            var h = f(a) / d(a);

            while (maxiter > 0 && Math.Abs(h) >= eps)
            {
                h = f(x) / d(x);
                x = x - h;
                maxiter -= 1;
            }

            return x;
        }
    }
}
