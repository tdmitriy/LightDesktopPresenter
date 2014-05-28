using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Server.ScreenGrabber
{
    class LdpScreenProcessingUtils
    {
        public static long IMAGE_QUALITY_VALUE = 80L;
        private static EncoderParameters encoderParams = 
            new EncoderParameters();
        private static EncoderParameter qualityParam = 
            new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, IMAGE_QUALITY_VALUE);

        private static ImageCodecInfo encoder = GetEncoder(ImageFormat.Jpeg);

        public static byte[] ImageToByteArray(Bitmap imageIn)
        {
            using (var ms = new MemoryStream())
            {
                encoderParams.Param[0] = qualityParam;
                imageIn.Save(ms, encoder, encoderParams);
                //imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static Rectangle GetBoundingBoxForChanges(Bitmap newBitmap, Bitmap previousBitmap)
        {
            // The search algorithm starts by looking
            //	for the top and left bounds. The search
            //	starts in the upper-left corner and scans
            //	left to right and then top to bottom. It uses
            //	an adaptive approach on the pixels it
            //	searches. Another pass is looks for the
            //	lower and right bounds. The search starts
            //	in the lower-right corner and scans right
            //	to left and then bottom to top. Again, an
            //	adaptive approach on the search area is used.
            //

            // Notice: The GetPixel member of the Bitmap class
            //	is too slow for this purpose. This is a good
            //	case of using unsafe code to access pointers
            //	to increase the speed.
            //

            // If the previous image does not exist, then
            //	in essence everything has changed.
            //
            if (previousBitmap == null)
            {
                return new Rectangle(0, 0, newBitmap.Width, newBitmap.Height);
            }

            // Validate the images are the same shape and type.
            //	If they are not the same size, then in essence
            //	everything has changed.
            //

            /*if (previousBitmap.Width != newBitmap.Width ||
                previousBitmap.Height != newBitmap.Height ||
                previousBitmap.PixelFormat != newBitmap.PixelFormat)
            {
                // Not the same shape...can't do the search.
                //
                return new Rectangle(0, 0, newBitmap.Width, newBitmap.Height);
            }*/

            // Init the search parameters.
            //

            int width = newBitmap.Width;
            int height = newBitmap.Height;
            int left = width;
            int right = 0;
            int top = height;
            int bottom = 0;

            BitmapData bmNewData = null;
            BitmapData bmPrevData = null;
            try
            {
                // Lock the bits into memory.
                //

                bmNewData = newBitmap.LockBits(
                    new Rectangle(0, 0, newBitmap.Width, newBitmap.Height),
                    ImageLockMode.ReadOnly, newBitmap.PixelFormat);

                bmPrevData = previousBitmap.LockBits(
                    new Rectangle(0, 0, previousBitmap.Width, previousBitmap.Height),
                    ImageLockMode.ReadOnly, previousBitmap.PixelFormat);

                // The images are ARGB (4 bytes)
                //
                const int numBytesPerPixel = 4;

                // Get the number of integers (4 bytes) in each row
                //	of the image.
                //
                int strideNew = bmNewData.Stride / numBytesPerPixel;
                int stridePrev = bmPrevData.Stride / numBytesPerPixel;

                // Get a pointer to the first pixel.
                //
                // Notice: Another speed up implemented is that I don't
                //	need the ARGB elements. I am only trying to detect
                //	change. So this algorithm reads the 4 bytes as an
                //	integer and compares the two numbers.
                //
                IntPtr scanNew0 = bmNewData.Scan0;
                IntPtr scanPrev0 = bmPrevData.Scan0;

                // Enter the unsafe code.
                //
                unsafe
                {
                    // Cast the safe pointers into unsafe pointers.
                    //
                    int* pNew = (int*)(void*)scanNew0;
                    int* pPrev = (int*)(void*)scanPrev0;

                    // First Pass - Find the left and top bounds
                    //	of the minimum bounding rectangle. Adapt the
                    //	number of pixels scanned from left to right so
                    //	we only scan up to the current bound. We also
                    //	initialize the bottom & right. This helps optimize
                    //	the second pass.
                    //
                    // For all rows of pixels (top to bottom)
                    //
                    for (int y = 0; y < height; ++y)
                    {
                        // For pixels up to the current bound (left to right)
                        //
                        for (int x = 0; x < left; ++x)
                        {
                            // Use pointer arithmetic to index the
                            //	next pixel in this row.
                            //
                            if ((pNew + x)[0] != (pPrev + x)[0])
                            {
                                // Found a change.
                                //
                                if (x < left)
                                {
                                    left = x;
                                }
                                if (x > right)
                                {
                                    right = x;
                                }
                                if (y < top)
                                {
                                    top = y;
                                }
                                if (y > bottom)
                                {
                                    bottom = y;
                                }
                            }
                        }

                        // Move the pointers to the next row.
                        //
                        pNew += strideNew;
                        pPrev += stridePrev;
                    }

                    // If we did not find any changed pixels
                    //	then no need to do a second pass.
                    //
                    if (left != width)
                    {
                        // Second Pass - The first pass found at
                        //	least one different pixel and has set
                        //	the left & top bounds. In addition, the
                        //	right & bottom bounds have been initialized.
                        //	Adapt the number of pixels scanned from right
                        //	to left so we only scan up to the current bound.
                        //	In addition, there is no need to scan past
                        //	the top bound.
                        //

                        // Set the pointers to the first element of the
                        //	bottom row.
                        //
                        pNew = (int*)(void*)scanNew0;
                        pPrev = (int*)(void*)scanPrev0;
                        pNew += (height - 1) * strideNew;
                        pPrev += (height - 1) * stridePrev;

                        // For each row (bottom to top)
                        //
                        for (int y = height - 1; y > top; y--)
                        {
                            // For each column (right to left)
                            //
                            for (int x = width - 1; x > right; x--)
                            {
                                // Use pointer arithmetic to index the
                                //	next pixel in this row.
                                //
                                if ((pNew + x)[0] != (pPrev + x)[0])
                                {
                                    // Found a change.
                                    //
                                    if (x > right)
                                    {
                                        right = x;
                                    }
                                    if (y > bottom)
                                    {
                                        bottom = y;
                                    }
                                }
                            }

                            // Move up one row.
                            //
                            pNew -= strideNew;
                            pPrev -= stridePrev;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                // Unlock the bits of the image.
                //
                if (bmNewData != null)
                {
                    newBitmap.UnlockBits(bmNewData);
                }
                if (bmPrevData != null)
                {
                    previousBitmap.UnlockBits(bmPrevData);
                }
            }

            // Validate we found a bounding box. If not
            //	return an empty rectangle.
            //
            int diffImgWidth = right - left + 1;
            int diffImgHeight = bottom - top + 1;
            if (diffImgHeight < 0 || diffImgWidth < 0)
            {
                // Nothing changed
                return Rectangle.Empty;
            }

            // Return the bounding box.
            //
            //return new Rectangle(left, top, diffImgWidth, diffImgHeight);
            return new Rectangle(left - 20, top - 20, diffImgWidth + 40, diffImgHeight + 40);
        }

        public static Bitmap ImageFromByteArray(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return (Bitmap)Image.FromStream(ms);
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static Bitmap CopyRegion(Bitmap srcBitmap, Rectangle bounds)
        {
            Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(srcBitmap, 0, 0, bounds, GraphicsUnit.Pixel);
                gr.Dispose();
            }
            return bmp;
        }

        #region MarshalArray
        public static byte[] MarshalBitmapToArray(Bitmap image, PixelFormat pixelFormat)
        {
            var imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                                ImageLockMode.ReadWrite, pixelFormat);
            var bytes = new byte[imageData.Height * imageData.Stride];
            try
            {
                Marshal.Copy(imageData.Scan0, bytes, 0, bytes.Length);
            }
            finally
            {
                image.UnlockBits(imageData);
            }
            return bytes;
        }

        public static Bitmap MarshalArrayToBitmap(byte[] bytes, int width, int height, PixelFormat pixelFormat)
        {
            var image = new Bitmap(width, height, pixelFormat);
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                                ImageLockMode.ReadWrite, pixelFormat);
            try
            {
                Marshal.Copy(bytes, 0, imageData.Scan0, bytes.Length);
            }
            finally
            {
                image.UnlockBits(imageData);
            }
            return image;
        }
        #endregion
    }
}
