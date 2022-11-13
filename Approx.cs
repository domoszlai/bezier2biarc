using System;
namespace BiArcTutorial
{
	public class Approx
	{
		public Approx(CubicBezier bezier, BiArc biarc)
		{
			this.Bezier = bezier;
			this.BiArc = biarc;
		}

		public CubicBezier Bezier
		{
			get; private set;
		}

        public BiArc BiArc
        {
            get; private set;
        }
    }
}

