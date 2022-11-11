using System;
using System.Numerics;

namespace BiArcTutorial
{
    public class Visualizer : BindableObject, IDrawable
    {
        public static readonly BindableProperty BezierProperty =
                BindableProperty.Create(nameof(Bezier), typeof(CubicBezier), typeof(Visualizer), null);

        public static readonly BindableProperty BiArcsProperty =
                BindableProperty.Create(nameof(BiArcs), typeof(List<BiArc>), typeof(Visualizer), null);

        public static readonly BindableProperty SelectedArcIndexProperty =
                BindableProperty.Create(nameof(SelectedArcIndex), typeof(int), typeof(Visualizer), null);

        public CubicBezier Bezier {
            get => (CubicBezier)GetValue(BezierProperty);
            set => SetValue(BezierProperty, value);
        }

        public List<BiArc> BiArcs
        {
            get => (List<BiArc>)GetValue(BiArcsProperty);
            set => SetValue(BiArcsProperty, value);
        }

        public int SelectedArcIndex
        {
            get => (int)GetValue(SelectedArcIndexProperty);
            set => SetValue(SelectedArcIndexProperty, value);
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            int currArcIdx = 1;
            foreach (var biarc in BiArcs)
            {
                DrawApproxCircle(canvas, currArcIdx++, biarc.A1);
                DrawApproxCircle(canvas, currArcIdx++, biarc.A2);
            }

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 3;

            PathF bezierPath = new PathF();
            bezierPath.MoveTo(Bezier.P1);
            bezierPath.CurveTo(Bezier.C1, Bezier.C2, Bezier.P2);
            canvas.DrawPath(bezierPath);

            currArcIdx = 1;
            foreach (var biarc in BiArcs)
            {
                DrawApproxArc(canvas, currArcIdx++, biarc.A1);
                DrawApproxArc(canvas, currArcIdx++, biarc.A2);
            }
        }

        private void DrawApproxCircle(ICanvas canvas, int currArcIdx, Arc arc)
        {
            if (arc.r != 0.0)
            {
                if (currArcIdx == SelectedArcIndex)
                {
                    canvas.StrokeColor = Colors.LightGreen;
                    canvas.StrokeSize = 3;
                }
                else
                {
                    canvas.StrokeColor = Colors.Green;
                    canvas.StrokeSize = 1;
                }

                canvas.DrawEllipse(arc.C.X - arc.r, arc.C.Y - arc.r, 2 * arc.r, 2 * arc.r);
            }
        }

        private void DrawApproxArc(ICanvas canvas, int currArcIdx, Arc arc)
        {
            if (arc.r != 0.0)
            {
                if (currArcIdx == SelectedArcIndex)
                {
                    canvas.StrokeColor = Colors.Red;
                    canvas.StrokeSize = 3;
                }
                else
                {
                    canvas.StrokeColor = Colors.Blue;
                    canvas.StrokeSize = 3;
                }

                // MAUI seemingly has opposite angle direction than System.Drawing
                var startAngle = -1 * arc.startAngle * 180.0f / (float)Math.PI;
                var endAngle = startAngle - arc.sweepAngle * 180.0f / (float)Math.PI;

                canvas.DrawArc(arc.C.X - arc.r, arc.C.Y - arc.r, 2 * arc.r, 2 * arc.r,
                    startAngle, endAngle, arc.IsClockwise, false);
            }
        }
    }
}

