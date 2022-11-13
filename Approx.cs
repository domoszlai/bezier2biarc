using System;
using System.Numerics;

namespace BiArcTutorial
{
	public class Approx
	{
		public Approx(CubicBezier bezier, BiArc biarc, List<Tuple<Vector2, Vector2, Color>> debugLines = null)
		{
			this.Bezier = bezier;
			this.BiArc = biarc;
			this.DebugLines = debugLines;
		}

		public CubicBezier Bezier
		{
			get; private set;
		}

        public BiArc BiArc
        {
            get; private set;
        }

		public List<Tuple<Vector2, Vector2, Color>> DebugLines
		{
			get; private set;
		}

    }
}

