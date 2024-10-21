using Android.Content;
using Microsoft.Maui.Controls;

using CustomControlsLibrary.Interfaces;
using CustomUIControls;

[assembly: Dependency(typeof(DisplayService))]
namespace CustomUIControls;

// All the code in this file is only included on Android.
public class PlatformClass1
{
}

public class DisplayService : IDisplayService
{
    public (double Width, double Height) GetDisplayDimensions()
    {
        var metrics = Android.App.Application.Context.Resources.DisplayMetrics;
        return (metrics.WidthPixels / metrics.Density, metrics.HeightPixels / metrics.Density);
    }
}