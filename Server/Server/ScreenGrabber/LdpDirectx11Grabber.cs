using Server.WindowsUtils;
using SharpDX;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.ScreenGrabber
{
    sealed class LdpDirectx11Grabber : LdpScreenGrabberBase
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

        #region DX11Constants
        private const int NEXT_FRAME_TIMEOUT = 4000;
        private static ushort MAX_TRYING_ATTEMPS = 0;
        private static SharpDX.Direct3D11.Device dx11Device;
        private static SharpDX.DXGI.Factory1 dx11Factory;
        private static SharpDX.Direct3D11.Texture2DDescription dx11Texture2Ddescr;
        private static SharpDX.DXGI.Output1 dx11Output;
        private static SharpDX.DXGI.OutputDuplication dx11DuplicatedOutput;
        private SharpDX.Direct3D11.Texture2D dx11ScreenTexture;
        private OutputDuplicateFrameInformation dx11DuplFrameInfo;
        private SharpDX.DXGI.Resource dx11ScreenResource;
        private SharpDX.DXGI.Surface dx11ScreenSurface;
        private DataRectangle dx11Map;
        #endregion

        public LdpDirectx11Grabber()
        {
            InitGrabber();
        }

        private void InitGrabber()
        {
            try
            {
                this.pixelFormat = PixelFormat.Format32bppRgb;
                boundsRect = new System.Drawing.Rectangle(0, 0, WIDTH, HEIGHT);

                uint numAdapter = 0;   // # of graphics card adapter
                uint numOutput = 0;    // # of output device (i.e. monitor)

                // create device and factory
                dx11Device = new SharpDX.Direct3D11.Device(SharpDX.Direct3D.DriverType.Hardware);
                dx11Factory = new Factory1();
                dx11Output = new Output1(dx11Factory.Adapters1[numAdapter].Outputs[numOutput].NativePointer);

                // creating CPU-accessible texture resource
                dx11Texture2Ddescr = new SharpDX.Direct3D11.Texture2DDescription();
                dx11Texture2Ddescr.CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.Read;
                dx11Texture2Ddescr.BindFlags = SharpDX.Direct3D11.BindFlags.None;
                dx11Texture2Ddescr.Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm;
                dx11Texture2Ddescr.Height = HEIGHT;
                dx11Texture2Ddescr.Width = WIDTH;
                dx11Texture2Ddescr.OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None;
                dx11Texture2Ddescr.MipLevels = 1;
                dx11Texture2Ddescr.ArraySize = 1;
                dx11Texture2Ddescr.SampleDescription.Count = 1;
                dx11Texture2Ddescr.SampleDescription.Quality = 0;
                dx11Texture2Ddescr.Usage = SharpDX.Direct3D11.ResourceUsage.Staging;
                dx11ScreenTexture = new SharpDX.Direct3D11.Texture2D(dx11Device, dx11Texture2Ddescr);

                // duplicate output stuff

                dx11DuplicatedOutput = dx11Output.DuplicateOutput(dx11Device);
            }
            catch (SharpDX.SharpDXException dxe)
            {
                string error = "Directx 11 initializer error.\n" + dxe.Message;
                LdpLog.Error(error);
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                string error = "Directx 11 initializer error.\n" + ex.Message;
                LdpLog.Error(error);
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override Bitmap GetScreenShot()
        {
            return GetDX11ScreenShot();
        }

        private Bitmap GetDX11ScreenShot()
        {
            try
            {
                screenShot = null;
                screenShot = new Bitmap(WIDTH, HEIGHT, this.pixelFormat);
                dx11DuplicatedOutput.AcquireNextFrame(NEXT_FRAME_TIMEOUT,
                    out dx11DuplFrameInfo, out dx11ScreenResource);

                if (dx11ScreenResource != null)
                    dx11Device.ImmediateContext
                        .CopyResource(dx11ScreenResource.QueryInterface<SharpDX.Direct3D11.Resource>(),
                        dx11ScreenTexture);
                // cast from texture to surface, so we can access its bytes
                dx11ScreenSurface = dx11ScreenTexture.QueryInterface<SharpDX.DXGI.Surface>();
                // map the resource to access it
                dx11Map = dx11ScreenSurface.Map(SharpDX.DXGI.MapFlags.Read);
                bmpData = screenShot.LockBits(boundsRect, ImageLockMode.WriteOnly, screenShot.PixelFormat);
                var sourcePtr = dx11Map.DataPointer;
                var destPtr = bmpData.Scan0;
                for (int y = 0; y < HEIGHT; y++)
                {
                    // Copy a single line 
                    Utilities.CopyMemory(destPtr, sourcePtr, ARGB_WIDTH);
                    // Advance pointers
                    sourcePtr = IntPtr.Add(sourcePtr, dx11Map.Pitch);
                    destPtr = IntPtr.Add(destPtr, bmpData.Stride);
                }

                screenShot.UnlockBits(bmpData);
                dx11ScreenSurface.Unmap();
                dx11ScreenSurface.Dispose();
                dx11ScreenResource.Dispose();
                dx11DuplicatedOutput.ReleaseFrame();
                dx11ScreenSurface = null;
                bmpData = null;
                GC.Collect();

                return screenShot;
            }
            catch (SharpDX.SharpDXException e)
            {
                if (e.ResultCode.Code == SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
                {
                    //screen does not changed
                    LdpLog.Warning("DX11 surface timeout.. Recursion is coming:)");
                    return GetDX11ScreenShot();
                }
                else
                {
                    if (MAX_TRYING_ATTEMPS < 5)
                    {
                        MAX_TRYING_ATTEMPS++;
                        LdpLog.Error("GetDX11ScreenShot\n" + e.Message + "\nMaxAttemps=" + MAX_TRYING_ATTEMPS);
                        return GetDX11ScreenShot();
                    }
                    else
                        return null;

                }
            }

        }

        public override void Dispose()
        {
            if (dx11Device != null)
                dx11Device.Dispose();
            if (dx11Factory != null)
                dx11Factory.Dispose();
            if (dx11Output != null)
                dx11Output.Dispose();
            if (dx11DuplicatedOutput != null)
                dx11DuplicatedOutput.Dispose();
            if (dx11ScreenTexture != null)
                dx11ScreenTexture.Dispose();
            if (dx11ScreenResource != null)
                dx11ScreenResource.Dispose();
            if (dx11ScreenSurface != null)
                dx11ScreenSurface.Dispose();

            if (screenShot != null)
                screenShot.Dispose();

            dx11Device = null;
            dx11Factory = null;
            dx11Output = null;
            dx11DuplicatedOutput = null;
            dx11ScreenTexture = null;
            dx11ScreenResource = null;
            dx11ScreenSurface = null;
            screenShot = null;

            bmpData = null;
            GC.SuppressFinalize(this);
        }
    }
}
