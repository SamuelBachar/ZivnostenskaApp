using UIKit;
using Microsoft.Maui.Controls;

using CustomControlsLibrary.Interfaces;
using CustomUIControls;

using CustomControlsLibrary.Platforms.iOS;

[assembly: Dependency(typeof(DisplayService))]
namespace CustomControlsLibrary.Platforms.iOS
{

    // All the code in this file is only included on iOS.
    public class PlatformClass1
    {
    }

    public class DisplayService : IDisplayService
    {
        public (double Width, double Height) GetDisplayDimensions()
        {
            var screenBounds = UIScreen.MainScreen.Bounds;
            return (screenBounds.Width, screenBounds.Height);
        }
    }
}
