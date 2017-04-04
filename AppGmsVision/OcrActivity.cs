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

namespace AppGmsVision
{
    [Activity(Label = "OcrActivity")]
    public class OcrActivity : Activity
    {
        class OcrTakePictureCallback : Java.Lang.Object, CameraSource.IPictureCallback
        {
            //TextRecognizer _detector;

            public OcrTakePictureCallback()
            {
                //var a = new TextRecognizer.Builder();
            }

            public void OnPictureTaken(byte[] data)
            {

            }
        }

        private CameraSourcePreview _cameraPreview;
        private CameraSource _cameraSource;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Ocr);

            _cameraPreview = FindViewById<CameraSourcePreview>(Resource.Id.ocrCameraPreview);

            var detector = new DummyDetector();
            detector.SetProcessor(new DummyProcessor());

            _cameraSource = new CameraSource.Builder(this, detector)
                .SetAutoFocusEnabled(true)
                //.SetRequestedPreviewSize(640, 480)
                .SetFacing(CameraFacing.Back)
                .SetRequestedFps(15.0f)
                .Build();

            _cameraPreview.Click += _cameraPreview_Click;
        }

        private void _cameraPreview_Click(object sender, EventArgs e)
        {

        }

        protected override void OnResume()
        {
            base.OnResume();
            _cameraPreview.Start(_cameraSource, null);
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