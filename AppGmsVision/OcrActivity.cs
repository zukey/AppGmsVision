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
using Android.Gms.Vision.Texts;
using Android.Graphics;
using Android.Util;
using Google.Apis.Vision.v1;
using Google.Apis.Services;
using Google.Apis.Vision.v1.Data;

namespace AppGmsVision
{
    [Activity(Label = "OcrActivity")]
    public class OcrActivity : Activity
    {
        class OcrTakePictureCallback : Java.Lang.Object, CameraSource.IPictureCallback
        {
            Context _context;
            TextRecognizer _detector;

            public OcrTakePictureCallback(Context context)
            {
                _context = context;
                _detector = new TextRecognizer.Builder(context).Build();
            }

            public void OnPictureTaken(byte[] data)
            {
                OcrCloudApi(data);
            }

            private void OcrLocalLib(byte[] data)
            {
                try
                {
                    var bitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                    var frame = new Frame.Builder()
                        .SetBitmap(bitmap)
                        .Build();

                    var texts = _detector.Detect(frame);
                    for (int i = 0; i < texts.Size(); i++)
                    {
                        var v = texts.ValueAt(i);
                        var text = v as TextBlock;
                        if (text != null)
                        {
                            Log.Debug("AppGmsVison", text.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("AppGmsVison", ex.ToString());
                }
            }

            private void OcrCloudApi(byte[] data)
            {
                try
                {
                    var imageContent = Convert.ToBase64String(data);

                    var vs = new VisionService(new BaseClientService.Initializer
                    {
                        ApiKey = "AIzaSyAoVrO5IZL0JccFr8aLHAiKq9aH9JnM9yo",
                        GZipEnabled = false
                    });

                    var response = vs.Images.Annotate(new Google.Apis.Vision.v1.Data.BatchAnnotateImagesRequest()
                    {
                        Requests = new[] {
                            new AnnotateImageRequest() {
                                Features = new[] { new Feature() { Type = "TEXT_DETECTION" } },
                                Image = new Image() { Content = imageContent }
                            }
                        }
                    }).Execute();

                    
                    foreach (var item in response.Responses)
                    {
                        if (item.TextAnnotations == null) { continue; }

                        //Log.Debug("AppGmsVison", "Texts");
                        //foreach (var text in item.TextAnnotations)
                        //{
                        //    Log.Debug("AppGmsVison", text.Description);

                        //}

                        if (item.FullTextAnnotation != null)
                        {
                            Log.Debug("AppGmsVison", "FullText");
                            Log.Debug("AppGmsVison", item.FullTextAnnotation.Text);

                            var words = new List<string>();
                            foreach (var page in item.FullTextAnnotation.Pages)
                            {
                                foreach (var block in page.Blocks)
                                {
                                    foreach (var paragraph in block.Paragraphs)
                                    {
                                        foreach (var word in paragraph.Words)
                                        {
                                            var sb = new StringBuilder();
                                            foreach (var symbol in word.Symbols)
                                            {
                                                sb.Append(symbol.Text);
                                            }
                                            var s = sb.ToString();
                                            Log.Debug("AppGmsVison", s);
                                            words.Add(s);
                                        }
                                    }
                                }
                            }
                            var intent = new Intent(_context, typeof(OcrResultActivity));
                            intent.PutExtra(OcrResultActivity.INTENTKEY_WORDS, words.ToArray());
                            _context.StartActivity(intent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("AppGmsVison", ex.ToString());
                }
            }
        }

        private CameraSourcePreview _cameraPreview;
        private CameraSource _cameraSource;
        private OcrTakePictureCallback _takeCallback;

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

            _takeCallback = new OcrTakePictureCallback(this);

            _cameraPreview.Click += _cameraPreview_Click;
        }

        private void _cameraPreview_Click(object sender, EventArgs e)
        {
            try
            {
                _cameraSource.TakePicture(null, _takeCallback);
            }
            catch (Exception ex)
            {
                Log.Warn("AppGmsVison", ex.ToString());
            }
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