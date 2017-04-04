using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Vision;
using Java.Lang;

namespace AppGmsVision
{
    public class BarcodeTrackerFactory : Java.Lang.Object, MultiProcessor.IFactory
    {
        private GraphicOverlay _graphicOverlay;

        public BarcodeTrackerFactory(GraphicOverlay graphicOverlay)
        {
            _graphicOverlay = graphicOverlay;
        }

        public Tracker Create(Java.Lang.Object item)
        {
            BarcodeGraphic graphic = new BarcodeGraphic(_graphicOverlay);
            return new BarcodeGraphicTracker(_graphicOverlay, graphic);
        }
    }
}