using System;
using Android.Content;
using Android.Graphics;
using Android.Renderscripts;
using Android.Runtime;
using Android.Views.Animations;
using Square.Picasso;

namespace InPowerApp.Common
{
    public class CircleTransformation : Java.Lang.Object, ITransformation
    {
        public string Key
        {
            get
            {
                return "circle";
            }
        }

        public Bitmap Transform(Bitmap source)
        {
            int size = Math.Min(source.Width, source.Height);

            int x = (source.Width - size) / 2;
            int y = (source.Height - size) / 2;

            Bitmap squaredBitmap = Bitmap.CreateBitmap(source, x, y, size, size);
            if (squaredBitmap != source)
            {
                source.Recycle();
            }

            Bitmap bitmap = Bitmap.CreateBitmap(size, size, source.GetConfig());

            Canvas canvas = new Canvas(bitmap);
            Paint paint = new Paint();
            BitmapShader shader = new BitmapShader(squaredBitmap,
                    BitmapShader.TileMode.Clamp, BitmapShader.TileMode.Clamp);
            paint.SetShader(shader);
            paint.AntiAlias = (true);

            float r = size / 2f;
            canvas.DrawCircle(r, r, r, paint);

            squaredBitmap.Recycle();
            return bitmap;
        }

    }
    public class CropSquareTransformation : Java.Lang.Object, ITransformation
    {
        public Bitmap Transform(Bitmap source)
        {
            int size = Math.Min(source.Width, source.Height);
            int x = (source.Width - size) / 2;
            int y = (source.Height - size) / 2;
            Bitmap result = Bitmap.CreateBitmap(source, x, y, size, size);
            if (result != source)
            {
                source.Recycle();
            }
            return result;
        }

        public string Key
        {
            get { return "square()"; }
        }
    }
    public class BlurTransformation : Java.Lang.Object, ITransformation
    {
        /**
       * Max blur Radius supported by the Renderscript library
       **/
        protected static int MaxRadius = 25;
        /**
        * Min blur Radius supported by the Renderscript library
        **/
        protected static int MinRadius = 1;
        /**
         * Application Context to instantiate the Renderscript
         **/
        protected Context Context;
        /**
        * Selected Radius
         **/
        protected int Radius;

        /**
        * Creates a new Blur transformation
        * 
        * @param Context Application Context to instantiate the Renderscript
        **/

        public BlurTransformation(Context context, int radius)
        {
            this.Context = context;
            this.Radius = radius < MinRadius
                ? MinRadius
                : radius > MaxRadius ? MaxRadius : radius;
        }

        public Bitmap Transform(Bitmap source)
        {
            // Load a clean bitmap and work from that.
            var originalBitmap = source;

            // Create another bitmap that will hold the results of the filter.
            Bitmap blurredBitmap = null;
            blurredBitmap = Bitmap.CreateBitmap(originalBitmap);

            // Create the Renderscript instance that will do the work.
            var rs = RenderScript.Create(Context);

            // Allocate memory for Renderscript to work with
            var input = Allocation.CreateFromBitmap(rs, originalBitmap, Allocation.MipmapControl.MipmapFull,
                AllocationUsage.Script);
            var output = Allocation.CreateTyped(rs, input.Type);

            // Load up an instance of the specific script that we want to use.
            var script = ScriptIntrinsicBlur.Create(rs, Element.U8_4(rs));
            script.SetInput(input);
            // Set the blur radius
            script.SetRadius(Radius);

            // Start the ScriptIntrinisicBlur
            script.ForEach(output);
            // Copy the output to the blurred bitmap
            output.CopyTo(blurredBitmap);

            source.Recycle();
            return blurredBitmap;
        }

        public string Key => "blurred";
    }
}