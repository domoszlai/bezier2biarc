using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace BiArcTutorial
{
    public class Algorithm
    {

        const float EPS = 0.0001f;
        const float MAXITER = 10;

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
                if (T1.m == T2.m)
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
                // D.J. Walton*, D.S. Meek, Approximation of a planar cubic B6zier spiral by circular arcs (1996)

                // Calculate parameter value of the bezier that corresponds to the biarc joint point
                var tj = RadialDirectionIntersection(bezier, biarc, biarc.JointAt);
                if (tj != -1)
                {
                    var dj = (bezier.PointAt(tj) - biarc.PointAt(biarc.JointAt)).Length();

                    // Valid in (0,tj]
                    var g0 = new Func<float, float>(u =>
                    {
                        return Vector2.Dot(bezier.PointAt(u) - biarc.A1.C, bezier.FirstDerivativeAt(u));
                    });

                    var g0d = new Func<float, float>(u =>
                    {
                        var d = bezier.FirstDerivativeAt(u);
                        return Vector2.Dot(d, d) +
                               Vector2.Dot(bezier.PointAt(u) - biarc.A1.C, bezier.SecondDerivativeAt(u));
                    });

                    // Valid in [tj,1)
                    var g1 = new Func<float, float>(u =>
                    {
                        return Vector2.Dot(bezier.PointAt(u) - biarc.A2.C, bezier.FirstDerivativeAt(u));
                    });

                    var g1d = new Func<float, float>(u =>
                    {
                        var d = bezier.FirstDerivativeAt(u);
                        return Vector2.Dot(d, d) +
                               Vector2.Dot(bezier.PointAt(u) - biarc.A2.C, bezier.SecondDerivativeAt(u));
                    });

                    var tb0 = FindRoot(g0, g0d, 0 + EPS, tj);
                    var d0 = 0f;
                    if (tb0 != -1)
                    {
                        var vb0 = bezier.PointAt(tb0);
                        d0 = Math.Abs((vb0 - biarc.A1.C).Length() - biarc.A1.r);
                    }

                    var tb1 = FindRoot(g1, g1d, tj, 1 - EPS);
                    var d1 = 0f;
                    if (tb1 != -1)
                    {
                        var vb1 = bezier.PointAt(tb1);
                        d1 = Math.Abs((vb1 - biarc.A2.C).Length() - biarc.A2.r);
                    }

                    var maxDistance = Math.Max(dj, Math.Max(d0, d1));
                    var maxDistanceAt = tj;

                    if (d0 > d1 && d0 > dj)
                    {
                        maxDistanceAt = tb0;
                    }
                    else if(d1 >= d0 && d1 > dj)
                    {
                        maxDistanceAt = tb1;
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
                        biarcs.Add(new Approx(bezier, biarc));
                    }
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

            var df = new Func<float, float>(u =>
            {
                return Vector2.Dot(bezier.FirstDerivativeAt(u), H);
            });

            return FindRoot(f, df, 0, 1);
        }

        /// <summary>
        /// Tries to find the root of f in interval [lowerBound,upperBound] using a combination of
        /// Newton and bisection methods.
        /// It is supposed to have at most one solution. If no solution is found, returns -1
        /// </summary>
        public static float FindRoot(Func<float, float> f, Func<float, float> df, float lowerBound, float upperBound)
        {
            var fmin = f(lowerBound);
            var fmax = f(upperBound);

            if (fmin * fmax >= 0) return -1;

            var root = (lowerBound + upperBound) / 2;
            var fx = f(root);

            for (var i=0; i<MAXITER && Math.Abs(fx) >= EPS; i++)
            {
                var h = f(root) / df(root);

                // overshoot or undershoot -> switch to bisection
                if (root - h < lowerBound || root - h > upperBound)
                {
                    if (fmin * fx < 0)
                    {
                        upperBound = root;
                        fmax = fx;
                    }
                    else if (fmax * fx < 0)
                    {
                        lowerBound = root;
                        fmin = fx;
                    }
                    root = (lowerBound + upperBound) / 2;
                }
                else
                {
                    root = root - h;
                }

                fx = f(root);
            }

            // If i==0, we may not reached tolarence yet, but hopefully it is close enough
            return root;
        }
    }
}