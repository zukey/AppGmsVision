using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Graphics;
using Android.Runtime;
using System;

namespace AppGmsVision
{
    [Activity(Label = "AppGmsVision", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var btn = FindViewById<Button>(Resource.Id.buttonDetectView);
            btn.Click += (sender, e) =>
            {
                StartActivity(typeof(DetectActivity));
            };

            var btn2 = FindViewById<Button>(Resource.Id.buttonOcrView);
            btn2.Click += (s, e) =>
            {
                StartActivity(typeof(OcrActivity));
            };
        }
    }
}

