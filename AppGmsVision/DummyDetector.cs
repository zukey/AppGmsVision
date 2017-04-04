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
using Android.Util;

namespace AppGmsVision
{
    class DummyDetector : Detector
    {
        public override SparseArray Detect(Frame frame)
        {
            return null;
        }
    }
}