using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
//using Android.Widget;

using Android.Gms.Vision;
using Android.Graphics;
using Android.Util;
using Android.Content.Res;

namespace AppGmsVision
{
    public class CameraSourcePreview
        : ViewGroup
    {
        private Context _context;
        private SurfaceView _surfaceView;
        private CameraSource _cameraSource;
        private GraphicOverlay _overlay;
        private bool _startRequested;
        protected bool SurfaceAvailable { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        public CameraSourcePreview(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            _context = context;
            _startRequested = false;
            SurfaceAvailable = false;

            _surfaceView = new SurfaceView(context);
            _surfaceView.Holder.AddCallback(new SurfaceCallback(this));
            base.AddView(_surfaceView);
        }

        /// <summary>
        /// カメラ起動
        /// </summary>
        /// <param name="cameraSource"></param>
        /// <param name="overlay"></param>
        public void Start(CameraSource cameraSource, GraphicOverlay overlay)
        {
            // パラメータ設定
            _overlay = overlay;
            _cameraSource = cameraSource;
            // フラグオン
            _startRequested = true;
            // カメラ起動
            Start();
        }

        /// <summary>
        /// カメラ起動
        /// </summary>
        private void Start()
        {
            if (_startRequested && SurfaceAvailable)
            {
                // カメラ起動
                _cameraSource.Start(_surfaceView.Holder);
                if (_overlay != null)
                {
                    var size = _cameraSource.PreviewSize;
                    var min = Math.Min(size.Width, size.Height);
                    var max = Math.Max(size.Width, size.Height);
                    if (IsPortraitMode())
                    {
                        _overlay.SetCameraInfo(min, max, _cameraSource.CameraFacing);
                    }
                    else
                    {
                        _overlay.SetCameraInfo(max, min, _cameraSource.CameraFacing);
                    }
                    _overlay.Clear();
                }
                _startRequested = false;
            }
        }

        /// <summary>
        /// カメラ停止
        /// </summary>
        public void Stop()
        {
            _cameraSource.Stop();
        }

        /// <summary>
        /// リソース解放
        /// </summary>
        public void Release()
        {
            _cameraSource.Release();
            _cameraSource = null;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int width = 640;
            int height = 480;
            if (_cameraSource != null)
            {
                var size = _cameraSource.PreviewSize;
                if (size != null)
                {
                    width = size.Width;
                    height = size.Height;
                }
            }

            if (IsPortraitMode())
            {
                int tmp = width;
                width = height;
                height = tmp;
            }

            var layoutWidth = r - l;
            var layoutHeight = b - t;

            int childWidth = layoutWidth;
            int childHeight = (int)(((float)layoutWidth / (float)width) * height);

            if (childHeight > layoutHeight)
            {
                childHeight = layoutHeight;
                childWidth = (int)(((float)layoutHeight / (float)height) * width);
            }

            for (int i = 0; i < ChildCount; ++i)
                GetChildAt(i).Layout(0, 0, childWidth, childHeight);

            Start();
        }

        /// <summary>
        /// 縦向きかどうか
        /// </summary>
        /// <returns></returns>
        private bool IsPortraitMode()
        {
            var orientation = _context.Resources.Configuration.Orientation;
            if (orientation == Orientation.Landscape)
                // 横向き
                return false;
            if (orientation == Orientation.Portrait)
                // 縦向き
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private class SurfaceCallback
            : Java.Lang.Object, ISurfaceHolderCallback
        {
            public CameraSourcePreview Parent { get; private set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="parent"></param>
            public SurfaceCallback(CameraSourcePreview parent)
            {
                Parent = parent;
            }

            /// <summary>
            /// 変更された場合
            /// </summary>
            /// <param name="holder"></param>
            /// <param name="format"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
            {
            }

            /// <summary>
            /// 生成された場合
            /// </summary>
            /// <param name="holder"></param>
            public void SurfaceCreated(ISurfaceHolder holder)
            {
                Parent.SurfaceAvailable = true;
                Parent.Start();
            }

            /// <summary>
            /// 破棄された場合
            /// </summary>
            /// <param name="holder"></param>
            public void SurfaceDestroyed(ISurfaceHolder holder)
            {
                Parent.SurfaceAvailable = false;
            }
        }
    }
}