using Avalonia.Animation.Easings;
using Avalonia.Media;
using System;
using System.Diagnostics;

namespace AvaloniaInside.Shell.Platform.Android;

public class EmphasizedEasing : Easing
{
    private PathGeometry _pathGeometry;

    public EmphasizedEasing()
    {
        _pathGeometry = PathGeometry.Parse("M 0,0 C 0.05, 0, 0.133333, 0.06, 0.166666, 0.4 C 0.208333, 0.82, 0.25, 1, 1, 1");
    }

    public override double Ease(double input)
    {
        // Clamp input within [0, 1]
        input = Math.Max(0, Math.Min(1, input));

        if (!_pathGeometry.TryGetPointAtDistance(_pathGeometry.ContourLength * input, out var point))
        {
            // Handle the case where TryGetPointAtDistance fails (if needed)
        }
        Debug.WriteLine(point.ToString());

        return point.Y;
    }
}
