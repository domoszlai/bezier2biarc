using System;
using System.Numerics;

namespace BiArcTutorial
{
	public static class Curve
	{
        
        public static bool IsClockWise(Vector2 P1, Vector2 P2, Vector2 P3)
		{
            var sum = 0d;
            sum += (P3.X - P1.X) * (P3.Y + P1.Y);
            sum += (P2.X - P3.X) * (P2.Y + P3.Y);
            sum += (P1.X - P2.X) * (P1.Y + P2.Y);
            return sum < 0;
        }

        public static bool IsClockWise(Vector2 P1, Vector2 P2, Vector2 P3, Vector2 P4)
        {
            var sum = 0d;
            sum += (P2.X - P1.X) * (P2.Y + P1.Y);
            sum += (P3.X - P2.X) * (P3.Y + P2.Y);
            sum += (P4.X - P3.X) * (P4.Y + P3.Y);
            sum += (P1.X - P4.X) * (P1.Y + P4.Y);
            return sum < 0;
        }
	}
}

