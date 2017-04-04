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
using Android.Gms.Vision.Barcodes;
using Android.Util;
using Android.Graphics;

namespace AppGmsVision
{
    [Activity(Label = "DetectActivity")]
    public class DetectActivity : Activity
    {
        private CameraSourcePreview _cameraPreview;
        private GraphicOverlay _overlay;
        private CameraSource _cameraSource;

        public static float ConvertPx2Dp(int px, Context context)
        {
            DisplayMetrics metrics = context.Resources.DisplayMetrics;
            return px / metrics.Density;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Detect);

            _cameraPreview = FindViewById<CameraSourcePreview>(Resource.Id.cameraSourcePreview1);
            _overlay = FindViewById<GraphicOverlay>(Resource.Id.barcodeOverlay);

            // バーコード検出器を生成
            var detector = new BarcodeDetector.Builder(Application.Context)
                //.SetBarcodeFormats(BarcodeFormat.QrCode)
                .Build();
            detector.SetProcessor(new MultiProcessor.Builder(new BarcodeTrackerFactory(_overlay)).Build());

            // カメラソースを生成
            _cameraSource = new CameraSource.Builder(this, detector)
                        .SetAutoFocusEnabled(true)
                        //.SetRequestedPreviewSize(640, 480)
                        .SetFacing(CameraFacing.Back)
                        .SetRequestedFps(15.0f)
                        .Build();

            _overlay.Touch += _overlay_Touch;
        }

        private void _overlay_Touch(object sender, View.TouchEventArgs e)
        {
            try
            {
                var x = e.Event.GetX();
                var y = e.Event.GetY();
                var rawX = e.Event.RawX;
                var rawY = e.Event.RawY;
                var dpX = ConvertPx2Dp((int)x, Application.Context);
                var dpY = ConvertPx2Dp((int)y, Application.Context);

                var m = new StringBuilder();
                m.AppendFormat("OverlayTouch X:{0} Y:{1} rawX:{2} rawY:{3} dpX:{4} dpY:{5}", x, y, rawX, rawY, dpX, dpY);

                Log.Debug("AppGmsVison", m.ToString());

                TouchedFunction(x, y);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Log.Error("AppGmsVison", ex.ToString());
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            //try
            //{
            //    var prvX = _cameraPreview.GetX();
            //    var prvY = _cameraPreview.GetY();
            //    var prvdpx = ConvertPx2Dp((int)prvX, Application.Context);
            //    var prvdpy = ConvertPx2Dp((int)prvY, Application.Context);
            //    Log.Debug("AppGmsVison", string.Format("Preview X:{0} Y:{1} dpX:{2} dpY:{3}", prvX, prvY, prvdpx, prvdpy));

            //    var x = e.RawX;
            //    var y = e.RawY;
            //    var dpx = ConvertPx2Dp((int)x, Application.Context);
            //    var dpy = ConvertPx2Dp((int)y, Application.Context);
            //    Log.Debug("AppGmsVison", string.Format("Touch X:{0} Y:{1} dpX:{2} dpY:{3}", x, y, dpx, dpy));

            //    var offsetDpX = dpx - prvdpx;
            //    var offsetDpY = dpy - prvdpy;
            //    Log.Debug("AppGmsVison", string.Format("TouchOffsetDP X:{0} Y:{1}", offsetDpX, offsetDpY));

            //    if (TouchedFunction((int)offsetDpX, (int)offsetDpY))
            //    {
            //        return true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Log.Error("AppGmsVison", ex.ToString());
            //}

            return base.OnTouchEvent(e);
        }

        private bool TouchedFunction(int offsetDpX,int offsetDpY)
        {
            var target = _overlay.FindFirst(graphic =>
            {
                try
                {
                    var barcordGraphic = graphic as BarcodeGraphic;
                    var box = barcordGraphic?.Barcode?.BoundingBox;
                    var corners = barcordGraphic.Barcode?.CornerPoints;
                    if (box == null)
                    {
                        return false;
                    }
                    var scaleBox = barcordGraphic.GetDrawRect();

                    var m = new StringBuilder();
                    m.AppendFormat("box[{0},{1},{2},{3}] ", box.Top, box.Left, box.Right, box.Bottom);
                    m.AppendFormat("scalebox[{0},{1},{2},{3}] ", scaleBox.Top, scaleBox.Left, scaleBox.Right, scaleBox.Bottom);
                    Log.Debug("AppGmsVison", m.ToString());

                    if (box.Contains(offsetDpX, offsetDpY))
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Log.Error("AppGmsVison", ex.ToString());
                    return false;
                }
            }) as BarcodeGraphic;

            try
            {
                var barcodeValue = target?.Barcode?.DisplayValue;
                if (barcodeValue != null)
                {
                    var clipItem = new ClipData.Item(barcodeValue);
                    var clipDescription = new ClipDescription("", new string[] { ClipDescription.MimetypeTextPlain });
                    var clipData = new ClipData(clipDescription, clipItem);

                    var clipManager = GetSystemService(ClipboardService) as ClipboardManager;
                    if (clipManager != null)
                    {
                        clipManager.PrimaryClip = clipData;

                        this.RunOnUiThread(() => Toast.MakeText(this, "コピーしました", ToastLength.Long).Show());

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("AppGmsVison", ex.ToString());
            }
            return false;
        }

        private bool TouchedFunction(float x, float y)
        {
            var target = _overlay.FindFirst(graphic =>
            {
                try
                {
                    var barcordGraphic = graphic as BarcodeGraphic;
                    var box = barcordGraphic?.Barcode?.BoundingBox;
                    var corners = barcordGraphic.Barcode?.CornerPoints;
                    if (box == null)
                    {
                        return false;
                    }
                    var scaleBox = barcordGraphic.GetDrawRect();

                    var m = new StringBuilder();
                    m.AppendFormat("box[{0},{1},{2},{3}] ", box.Top, box.Left, box.Right, box.Bottom);
                    m.AppendFormat("scalebox[{0},{1},{2},{3}] ", scaleBox.Top, scaleBox.Left, scaleBox.Right, scaleBox.Bottom);
                    Log.Debug("AppGmsVison", m.ToString());

                    if (scaleBox.Contains(x, y))
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Log.Error("AppGmsVison", ex.ToString());
                    return false;
                }
            }) as BarcodeGraphic;

            try
            {
                var barcodeValue = target?.Barcode?.DisplayValue;
                if (barcodeValue != null)
                {
                    var clipItem = new ClipData.Item(barcodeValue);
                    var clipDescription = new ClipDescription("", new string[] { ClipDescription.MimetypeTextPlain });
                    var clipData = new ClipData(clipDescription, clipItem);

                    var clipManager = GetSystemService(ClipboardService) as ClipboardManager;
                    if (clipManager != null)
                    {
                        clipManager.PrimaryClip = clipData;

                        this.RunOnUiThread(() => Toast.MakeText(this, "コピーしました", ToastLength.Long).Show());

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("AppGmsVison", ex.ToString());
            }
            return false;
        }

        protected override void OnResume()
        {
            base.OnResume();
            _cameraPreview.Start(_cameraSource, _overlay);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _cameraPreview.Stop();
        }

        protected override void OnDestroy()
        {
            _cameraPreview.Release();
            base.OnDestroy();
        }
    }
}