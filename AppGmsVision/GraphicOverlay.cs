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
using Android.Util;
using Android.Gms.Vision;
using Android.Graphics;

namespace AppGmsVision
{
    public class GraphicOverlay
        : View
    {
        private object _lockObj = new object();
        private int _previewWidth;
        private float _widthScaleFactor = 1.0f;
        private int _previewHeight;
        private float _heightScaleFactor = 1.0f;
        private CameraFacing _facing = CameraFacing.Back;
        private List<Graphic> _graphics = new List<Graphic>();

        public abstract class Graphic
        {
            private GraphicOverlay _overlay;

            public Graphic(GraphicOverlay overlay)
            {
                _overlay = overlay;
            }

            public abstract void Draw(Canvas canvas);

            public float ScaleX(float horizontal)
            {
                return horizontal * _overlay._widthScaleFactor;
            }

            public float ScaleY(float vertical)
            {
                return vertical * _overlay._heightScaleFactor;
            }

            public float TranslateX(float x)
            {
                if (_overlay._facing == CameraFacing.Front)
                {
                    return _overlay.Width - ScaleX(x);
                }
                else
                {
                    return ScaleX(x);
                }
            }

            public float TranslateY(float y)
            {
                return ScaleY(y);
            }

            public void PostInvalidate()
            {
                _overlay.PostInvalidate();
            }
        }

        public GraphicOverlay(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public void Clear()
        {
            lock (_lockObj)
            {
                _graphics.Clear();
            }
            base.PostInvalidate();
        }

        public void Add(Graphic graphic)
        {
            lock (_lockObj)
            {
                if (!_graphics.Contains(graphic))
                    _graphics.Add(graphic);
            }
            base.PostInvalidate();
        }

        public void Remove(Graphic graphic)
        {
            lock (_lockObj)
            {
                _graphics.Remove(graphic);
            }
            base.PostInvalidate();
        }

        public Graphic FindFirst(Func<Graphic, bool> condition)
        {
            if (condition == null)
            {
                return null;
            }

            lock (_lockObj)
            {
                foreach (var item in _graphics)
                {
                    var hit = condition(item);
                    if (hit)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public void SetCameraInfo(int previewWidth, int previewHeight, CameraFacing facing)
        {
            lock (_lockObj)
            {
                _previewWidth = previewWidth;
                _previewHeight = previewHeight;
                _facing = facing;
            }
            base.PostInvalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            lock (_lockObj)
            {
                if (_previewWidth != 0 && _previewHeight != 0)
                {
                    _widthScaleFactor = (float)canvas.Width / (float)_previewWidth;
                    _heightScaleFactor = (float)canvas.Height / (float)_previewHeight;
                }

                foreach (var graphic in _graphics)
                {
                    graphic.Draw(canvas);
                }
            }
        }
    }
}