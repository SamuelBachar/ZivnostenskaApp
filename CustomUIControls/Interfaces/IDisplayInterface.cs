using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomControlsLibrary.Interfaces;

public interface IDisplayService
{
    (double Width, double Height) GetDisplayDimensions();

    public (double screenWidth, double screenHeight) GetScreenSizes()
    {
        var displayInfo = DeviceDisplay.MainDisplayInfo;

        double screenWidth = displayInfo.Width / displayInfo.Density; // Width in pixels
        double screenHeight = displayInfo.Height / displayInfo.Density; // Height in pixels

        return (screenWidth, screenHeight);
    }

    public (double finalScreenWidth, double finalScreenHeight) GetFinalLayoutSize((double screenWidth, double screenHeight) sizes, double scaleW, double scaleH)
    {
        return (sizes.screenWidth * scaleW, sizes.screenHeight * scaleH);
    }
}