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
using Android.Gms.Vision.Barcodes;
using Android.Gms.Vision;
using Java.Lang;

namespace AppGmsVision
{
    public class BarcodeGraphicTracker : Tracker
    {
        private GraphicOverlay _overlay;
        private BarcodeGraphic _graphic;

        public BarcodeGraphicTracker(GraphicOverlay overlay, BarcodeGraphic graphic)
        {
            _overlay = overlay;
            _graphic = graphic;
        }

        #region Tracker‚ÌŽÀ‘•

        public override void OnNewItem(int id, Java.Lang.Object item)
        {
            _graphic.Id = id;
        }

        public override void OnUpdate(Detector.Detections detections, Java.Lang.Object item)
        {
            var barcode = item as Barcode;
            if (barcode == null) { return; }

            _overlay.Add(_graphic);
            _graphic.UpdateItem(barcode);
        }

        public override void OnMissing(Detector.Detections detections)
        {
            _overlay.Remove(_graphic);
        }
        #endregion
    }
}