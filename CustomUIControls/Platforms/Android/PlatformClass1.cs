using Android.Content;
using Microsoft.Maui.Controls;

using CustomControlsLibrary.Interfaces;

using CustomControlsLibrary.Platforms.Droid;

[assembly: Dependency(typeof(DisplayService))]
namespace CustomControlsLibrary.Platforms.Droid
{

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
}