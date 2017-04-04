using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Gms.Vision.Barcodes;

namespace AppGmsVision
{
    public class BarcodeGraphic : GraphicOverlay.Graphic
    {
        private readonly Color[] COLOR_CHOICES =
        {
            Color.Blue,
            Color.Cyan,
            Color.Green
        };

        private static int _CurrentColorIndex = 0;

        /// <summary>
        /// 範囲枠描画用
        /// </summary>
        private Paint _rectPaint;
        /// <summary>
        /// テキスト描画用
        /// </summary>
        private Paint _textPaint;

        /// <summary>
        /// バーコード追跡ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// バーコード
        /// </summary>
        public Barcode Barcode { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="overlay"></param>
        public BarcodeGraphic(GraphicOverlay overlay) : base(overlay)
        {
            _CurrentColorIndex = (_CurrentColorIndex + 1) % COLOR_CHOICES.Length;
            var selectedColor = COLOR_CHOICES[_CurrentColorIndex];

            _rectPaint = new Paint();
            _rectPaint.Color = selectedColor;
            _rectPaint.SetStyle(Paint.Style.Stroke);
            _rectPaint.StrokeWidth = 5.0f;

            _textPaint = new Paint();
            _textPaint.Color = selectedColor;
            _textPaint.TextSize = 40.0f;
        }

        /// <summary>
        /// バーコードアイテムアップデート
        /// </summary>
        /// <param name="barcode"></param>
        public void UpdateItem(Barcode barcode)
        {
            Barcode = barcode;
            //barcode.CornerPoints
            //barcode.GeoPoint
            //Log.Info("AppGmsVison", string.Format("barcord detect id:{0}", Id));
            //foreach (var p in barcode.CornerPoints)
            //{
            //    //Log.Debug("AppGmsVison", p.ToString());
            //}
            PostInvalidate();
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="canvas"></param>
        public override void Draw(Canvas canvas)
        {
            var barcode = Barcode;
            if (barcode == null)
            {
                return;
            }

            // Draws the bounding box around the barcode.
            RectF rect = new RectF(barcode.BoundingBox);
            rect.Left = TranslateX(rect.Left);
            rect.Top = TranslateY(rect.Top);
            rect.Right = TranslateX(rect.Right);
            rect.Bottom = TranslateY(rect.Bottom);
            canvas.DrawRect(rect, _rectPaint);

            // Draws a label at the bottom of the barcode indicate the barcode value that was detected.
            canvas.DrawText(barcode.RawValue, rect.Left, rect.Bottom, _textPaint);
        }

        public RectF GetDrawRect()
        {
            var barcode = Barcode;
            if (barcode == null)
            {
                return null;
            }

            RectF rect = new RectF(barcode.BoundingBox);
            rect.Left = TranslateX(rect.Left);
            rect.Top = TranslateY(rect.Top);
            rect.Right = TranslateX(rect.Right);
            rect.Bottom = TranslateY(rect.Bottom);

            return rect;
        }
    }
}