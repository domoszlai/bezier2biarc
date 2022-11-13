using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BiArcTutorial
{
    public class ViewModel : INotifyPropertyChanged
    {
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

        // NaN inflexion point
        static CubicBezier bt8 = new CubicBezier(
                new Vector2(460, 365), new Vector2(440, 365),
                new Vector2(400, 466), new Vector2(400, 365));

        public List<LabeledItem<CubicBezier>> LabeledCurves => new List<LabeledItem<CubicBezier>>
        {
            LabeledItem.Create("#1",b1),
            LabeledItem.Create("#2",b2),
            LabeledItem.Create("#3",b3),
            LabeledItem.Create("#4",b4),
            LabeledItem.Create("#5",bt1),
            LabeledItem.Create("#6",bt2),
            LabeledItem.Create("#7",bt3),
            LabeledItem.Create("#8",bt4a),
            LabeledItem.Create("#9",bt4b),
            LabeledItem.Create("#10",bt5a),
            LabeledItem.Create("#11",bt5b),
            LabeledItem.Create("#12",bt5c),
            LabeledItem.Create("#13",bt6),
            LabeledItem.Create("#14",bt7),
            LabeledItem.Create("#15",bt8)
        };

        private int _selectedArcIndex = 1;
        public int SelectedArcIndex
        {
            get
            {
                return _selectedArcIndex;
            }
            set
            {
                if (_selectedArcIndex != value)
                {
                    _selectedArcIndex = value;
                    OnPropertyChanged(nameof(SelectedArcIndex));
                }
            }
        }

        public int NumberOfArcs => Approx == null ? 0 : Approx.Count * 2;

        private int _selectedCurveIndex = 0;
        public int SelectedCurveIndex {
            get
            {
                return _selectedCurveIndex;
            }
            set
            {
                if(_selectedCurveIndex != value)
                {
                    _selectedCurveIndex = value;
                    OnPropertyChanged(nameof(SelectedCurveIndex));
                }
            }
        }

        private int _maxError = 1;
        public int MaxError
        {
            get
            {
                return _maxError;
            }
            set
            {
                if(_maxError != value)
                {
                    _maxError = value;
                    OnPropertyChanged(nameof(SelectedCurveIndex));
                }
            }
        }

        public int NrArcs { get; private set; }

        public CubicBezier SelectedCurve => LabeledCurves[SelectedCurveIndex].Item;
        public List<Approx> Approx { get; private set; }

        public ICommand NextArcCommand { get; private set; }

        public ViewModel()
        {
            PropertyChanged += ViewModel_PropertyChanged;
            OnPropertyChanged(nameof(SelectedCurveIndex));

            NextArcCommand = new Command<string>((key) => {
                SelectedArcIndex = (SelectedArcIndex + 1) % Approx.Count;
                OnPropertyChanged(nameof(SelectedArcIndex));
            });
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SelectedCurveIndex))
            {
                ApproximateBezier();
                OnPropertyChanged(nameof(SelectedCurve));
            }
            else if(e.PropertyName == nameof(MaxError))
            {
                ApproximateBezier();
            }
        }

        private void ApproximateBezier()
        {
            Approx = Algorithm.ApproxCubicBezier(SelectedCurve, 20, MaxError);
            SelectedArcIndex = 1;
            NrArcs = Approx.Count * 2;
            OnPropertyChanged(nameof(NrArcs));
            OnPropertyChanged(nameof(Approx));
            OnPropertyChanged(nameof(NumberOfArcs));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

