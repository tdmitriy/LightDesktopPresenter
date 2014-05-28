using Server.WindowsUtils;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ScreenGrabber
{
    sealed class LdpDirectx9Grabber : LdpScreenGrabberBase
    {
        #region Variables
        private Bitmap screenShot;
        private System.Drawing.Imaging.PixelFormat pixelFormat;
        private System.Drawing.Rectangle boundsRect;
        private BitmapData bmpData;
        private static readonly int WIDTH = LdpUtils.SCREEN_WIDTH;
        private static readonly int HEIGHT = LdpUtils.SCREEN_HEIGHT;
        private static readonly int ARGB_WIDTH = WIDTH * 4;
        #endregion

        #region DX9Constants
        private SharpDX.Direct3D9.Device dx9Device;
        private SharpDX.Direct3D9.Surface dx9ScreenSurface;
        private DataRectangle dx9Map;
        #endregion

        public LdpDirectx9Grabber()
        {
            InitGrabber();
        }

        private void InitGrabber()
        {
            try
            {
                this.pixelFormat = PixelFormat.Format32bppRgb;
                boundsRect = new System.Drawing.Rectangle(0, 0, WIDTH, HEIGHT);

                var format = SharpDX.Direct3D9.Format.A8R8G8B8;
                SharpDX.Direct3D9.PresentParameters present_params = new SharpDX.Direct3D9.PresentParameters();
                present_params.Windowed = true;
                present_params.BackBufferFormat = format;
                present_params.SwapEffect = SharpDX.Direct3D9.SwapEffect.Discard;
                present_params.BackBufferWidth = WIDTH;
                present_params.BackBufferHeight = HEIGHT;
                dx9Device = new SharpDX.Direct3D9.Device(new SharpDX.Direct3D9.Direct3D(),
                                                      0,
                                                      SharpDX.Direct3D9.DeviceType.Hardware,
                                                      IntPtr.Zero,
                                                      SharpDX.Direct3D9.CreateFlags.HardwareVertexProcessing,
                                                      present_params);

                dx9Device.SetRenderState(RenderState.CullMode, Cull.None);
                dx9Device.SetRenderState(RenderState.Lighting, false);
                dx9Device.SetRenderState(RenderState.AntialiasedLineEnable, false);
            }
            catch (SharpDX.SharpDXException dxe)
            {
                LdpLog.Error("SharpDX InitializeDX9\n" + dxe.Message);
            }
            catch (Exception ex)
            {
                LdpLog.Error("InitializeDX9\n" + ex.Message);
            }
        }

        public override Bitmap GetScreenShot()
        {
            return GetDX9ScreenShot();
        }

        private Bitmap GetDX9ScreenShot()
        {
            screenShot = null;
            screenShot = new System.Drawing.Bitmap(WIDTH, HEIGHT, pixelFormat);
            dx9ScreenSurface = SharpDX.Direct3D9.Surface.CreateOffscreenPlain(
            dx9Device,
            WIDTH,
            HEIGHT,
            SharpDX.Direct3D9.Format.A8R8G8B8,
            Pool.SystemMemory);

            dx9Device.GetFrontBufferData(0, dx9ScreenSurface);

            dx9Map = dx9ScreenSurface.LockRectangle(LockFlags.None);
            bmpData = screenShot.LockBits(boundsRect,
                System.Drawing.Imaging.ImageLockMode.WriteOnly, screenShot.PixelFormat);

            var sourcePtr = dx9Map.DataPointer;
            var destPtr = bmpData.Scan0;
            for (int y = 0; y < HEIGHT; y++)
            {
                // Copy a single line 
                Utilities.CopyMemory(destPtr, sourcePtr, ARGB_WIDTH);
                // Advance pointers
                sourcePtr = IntPtr.Add(sourcePtr, dx9Map.Pitch);
                destPtr = IntPtr.Add(destPtr, bmpData.Stride);
            }

            screenShot.UnlockBits(bmpData);
            dx9ScreenSurface.UnlockRectangle();
            dx9ScreenSurface.Dispose();
            bmpData = null;
            return screenShot;
        }

        public override void Dispose()
        {
            if (dx9Device != null)
                dx9Device.Dispose();
            if (dx9ScreenSurface != null)
                dx9ScreenSurface.Dispose();
            if (screenShot != null)
                screenShot.Dispose();
            bmpData = null;
        }
    }
}
