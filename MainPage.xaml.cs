using System.Numerics;

namespace BiArcTutorial;

public partial class MainPage : ContentPage
{

    public MainPage()
	{
        InitializeComponent();

        var ViewModel = BindingContext as ViewModel;
        ViewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == "SelectedCurve" ||
                e.PropertyName == "SelectedArcIndex" ||
                e.PropertyName == "BiArc")
            {
                (FindByName("graphicsView") as GraphicsView).Invalidate();
            }
        };
    }
}


