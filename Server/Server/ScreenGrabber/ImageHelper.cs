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

namespace TCPServer.ScreenGrabber_SYNC
{
    class ImageHelper
    {
        private static EncoderParameters encoderParams = 
            new EncoderParameters();
        private static EncoderParameter qualityParam = 
            new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);

        private static ImageCodecInfo encoder = GetEncoder(ImageFormat.Jpeg);

        public static List<Bitmap> CropImage(Bitmap img, int numTiles)
        {
            List<Bitmap> tmp = new List<Bitmap>();
            double res = img.Height / numTiles;
            int tileHeight = (int)Math.Round(res);
            int height = img.Height;
            int width = img.Width;
            for (int size = 0, i = 0; size < height; size += tileHeight, i++)
            {
                if (i == numTiles - 1 && (size + tileHeight) != height)
                {
                    size = height - tileHeight;
                    tmp.Add(CutImage(img, new Rectangle(0, size, width, tileHeight)));
                    break;
                }
                tmp.Add(CutImage(img, new Rectangle(0, size, width, tileHeight)));
            }
            return tmp;
        }

        public static List<KeyValuePair<Bitmap, Rectangle>> CropImageIntoRectangles(Bitmap img,
            List<Rectangle> list)
        {
            var tmp = new List<KeyValuePair<Bitmap, Rectangle>>();
            int height = img.Height;
            int width = img.Width;
            
            foreach (var rect in list)
                tmp.Add(new KeyValuePair<Bitmap, Rectangle>(CutImage(img, rect), rect));
            return tmp;
        }

        public static List<Rectangle> DivideIntoRectangles(int w, int h, ushort rectsPerRow)
        {
            ushort DIVIDE_CONST = rectsPerRow;
            int i = w / DIVIDE_CONST;
            int j = h / DIVIDE_CONST;
            int m = 0;
            var rect = new List<Rectangle>();
            Rectangle lRect = Rectangle.Empty;
            for (int k = 0; k < DIVIDE_CONST; k++)
                for (m = 0; m < DIVIDE_CONST; m++)
                {
                    lRect = new Rectangle();
                    lRect.X = (i * k);
                    lRect.Y = (j * m);
                    lRect.Width = i;
                    lRect.Height = j;
                    //Rect = Helper.AlignRectangle(lRect, w, h);
                    rect.Add(lRect);
                }
            return rect;
        }


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


        public static Rectangle GetBoundingBoxForChanges(Bitmap newBmp, Bitmap oldBmp)
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

            // Validate the images are the same shape and type.
            //
            if (oldBmp.Width != newBmp.Width ||
                oldBmp.Height != newBmp.Height ||
                oldBmp.PixelFormat != newBmp.PixelFormat)
            {
                // Not the same shape...can't do the search.
                //
                return Rectangle.Empty;
            }

            // Init the search parameters.
            //
            int width = newBmp.Width;
            int height = newBmp.Height;
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
                bmNewData = newBmp.LockBits(
                    new Rectangle(0, 0, newBmp.Width, newBmp.Height),
                    ImageLockMode.ReadOnly, newBmp.PixelFormat);
                bmPrevData = oldBmp.LockBits(
                    new Rectangle(0, 0, oldBmp.Width, oldBmp.Height),
                    ImageLockMode.ReadOnly, oldBmp.PixelFormat);

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
                    for (int y = 0; y < newBmp.Height; ++y)
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
                        pNew += (newBmp.Height - 1) * strideNew;
                        pPrev += (oldBmp.Height - 1) * stridePrev;

                        // For each row (bottom to top)
                        //
                        for (int y = newBmp.Height - 1; y > top; y--)
                        {
                            // For each column (right to left)
                            //
                            for (int x = newBmp.Width - 1; x > right; x--)
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
                // Do something with this info.
            }
            finally
            {
                // Unlock the bits of the image.
                //
                if (bmNewData != null)
                {
                    newBmp.UnlockBits(bmNewData);
                }
                if (bmPrevData != null)
                {
                    oldBmp.UnlockBits(bmPrevData);
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
            return new Rectangle(left, top, diffImgWidth, diffImgHeight);
        }

        public static Bitmap ImageFromByteArray(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return (Bitmap)Image.FromStream(ms);
            }
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

        public static Bitmap CutImage(Bitmap img, Rectangle cutArea)
        {
            Bitmap bmpCrop = img.Clone(cutArea, img.PixelFormat);
            return bmpCrop;
        }
    }
}
