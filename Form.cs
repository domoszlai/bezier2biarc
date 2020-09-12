using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace BiArcTutorial
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        public static PointF AsPointF(Vector2 v)
        {
            return new PointF(v.X, v.Y);
        }

        // Test curves
        // http://polymathprogrammer.com/2011/02/06/bezier-curve-inflection-points/

        // No inflexion point
        static CubicBezier b1 = new CubicBezier(
            new Vector2(100, 500), new Vector2(150, 100), new Vector2(500, 150), new Vector2(350, 350));
        static CubicBezier b2 = new CubicBezier(
            new Vector2(100, 500), new Vector2(250, 350), new Vector2(450, 350), new Vector2(500, 500));

        // One inflexion point
        static CubicBezier b3 = new CubicBezier(
            new Vector2(150, 500), new Vector2(100, 100), new Vector2(500, 350), new Vector2(350, 150));

        // Two inflexion point
        static CubicBezier b4 = new CubicBezier(
            new Vector2(100, 500), new Vector2(350, 100), new Vector2(100, 200), new Vector2(500, 400));

        // Corner case 1
        static CubicBezier bt1 = new CubicBezier(
            new Vector2(233.89831f, 285.0169000000001f), new Vector2(233.89831f, 293.0169000000001f),
            new Vector2(230.5649766666668f, 301.0169000000001f), new Vector2(223.89831f, 309.0169000000001f));

        // Corner case 2
        static CubicBezier bt2 = new CubicBezier(
            new Vector2(204.89831f, 328.0169f), new Vector2(198.89831f, 334.0169f),
            new Vector2(191.2316433333334f, 337.0169f), new Vector2(181.89831f, 337.0169f));

        static CubicBezier bt3 = new CubicBezier(
            new Vector2(199, 135), new Vector2(199, 134),
            new Vector2(210, 134), new Vector2(211, 134));

        // Start- and endpoint are very close
        static CubicBezier bt4a = new CubicBezier(
            new Vector2(100, 500), new Vector2(150, 100), new Vector2(500, 150), new Vector2(100, 501));

        // Start- and endpoint are the same
        static CubicBezier bt4b = new CubicBezier(
            new Vector2(100, 500), new Vector2(150, 100), new Vector2(500, 150), new Vector2(100, 500));

        // P1 == C1
        static CubicBezier bt5a = new CubicBezier(
                new Vector2(692.6091876283466f, 499.2999902362205f), new Vector2(692.6091876283466f, 499.2999902362205f),
                new Vector2(707.1606427464568f, 519.8274453543307f), new Vector2(707.1606427464568f, 525.3817351181102f));

        // P2 == C2
        static CubicBezier bt5b = new CubicBezier(
                new Vector2(218.9202733234206f, 270.31665826771655f), new Vector2(214.46111610836238f, 268.53409417322837f),
                new Vector2(187.8873352038994f, 259.04347370078744f), new Vector2(187.8873352038994f, 259.04347370078744f));

        static CubicBezier bt5c = new CubicBezier(
                new Vector2(707.1606427464568f, 525.3817351181102f), new Vector2(707.1606427464568f, 530.9360248818898f),
                new Vector2(705.5107854236222f, 566.1900878740157f), new Vector2(705.5107854236222f, 566.1900878740157f));

        // NaN inflexion points
        static CubicBezier bt6 = new CubicBezier(
                new Vector2(587.889221329134f, 482.32148314960625f), new Vector2(587.889221329134f, 485.32147370078735f),
                new Vector2(585.639221329134f, 486.07146425196845f), new Vector2(584.8892307779529f, 488.32146425196845f));

        // Parallel control lines
        static CubicBezier bt7 = new CubicBezier(
                new Vector2(600, 400), new Vector2(500, 400),
                new Vector2(500, 476), new Vector2(600, 476));

        static CubicBezier[] curveList = new CubicBezier[]
        {
            b1, b2, b3, b4,
            bt1, bt2, bt3, bt4a, bt4b,
            bt5a, bt5b, bt5c, bt6, bt7
        };

        private int Index { get; set; } = 0;
        public CubicBezier Current => curveList[Index % curveList.Length];

        protected override void OnPaint(PaintEventArgs e)
        {
            var bezier = Current;
            var biarcs = Algorithm.ApproxCubicBezier(bezier, 5, 1);

            Graphics g = e.Graphics;

            // The full approximation circles for better understanding
            foreach (var biarc in biarcs)
             {
                 g.DrawEllipse(new Pen(Color.Green, 1),
                     biarc.A1.C.X - biarc.A1.r, biarc.A1.C.Y - biarc.A1.r, 2 * biarc.A1.r, 2 * biarc.A1.r);
                 g.DrawEllipse(new Pen(Color.Green, 1),
                     biarc.A2.C.X - biarc.A2.r, biarc.A2.C.Y - biarc.A2.r, 2 * biarc.A2.r, 2 * biarc.A2.r);
             }

            // Draw the original bezier
            g.DrawBezier(new Pen(Color.Black, 2),
                AsPointF(bezier.P1), AsPointF(bezier.C1), AsPointF(bezier.C2), AsPointF(bezier.P2));

            // The approximation biarcs
            foreach (var biarc in biarcs)
            { 
                g.DrawArc(new Pen(Color.Red, 1),
                    biarc.A1.C.X - biarc.A1.r, biarc.A1.C.Y - biarc.A1.r, 2 * biarc.A1.r, 2 * biarc.A1.r, 
                    biarc.A1.startAngle * 180.0f / (float)Math.PI, biarc.A1.sweepAngle * 180.0f / (float)Math.PI);
                g.DrawArc(new Pen(Color.Red, 1),
                    biarc.A2.C.X - biarc.A2.r, biarc.A2.C.Y - biarc.A2.r, 2 * biarc.A2.r, 2 * biarc.A2.r, 
                    biarc.A2.startAngle * 180.0f / (float)Math.PI, biarc.A2.sweepAngle * 180.0f / (float)Math.PI);
            }    

        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            Index++;
            Refresh();
        }
    }
}
