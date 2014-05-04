using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using SharpDX;
using SharpDX.DXGI;
using System.Diagnostics;
using Server.WindowsUtils;
using SharpDX.Direct3D9;


namespace Server.ScreenGrabber
{
    class LdpDirectxGrabber : LdpBaseScreenGrabber
    {
        private static readonly bool WINDOWS7 = LdpUtils.WINDOWS7;
        private static readonly bool WINDOWS8 = LdpUtils.WINDOWS8;
        private Bitmap screenShot;
        private System.Drawing.Imaging.PixelFormat pixelFormat;
        private ScreenshotType screenshotType;
        private System.Drawing.Rectangle boundsRect;
        private BitmapData bmpData;
        private static readonly int WIDTH = LdpUtils.SCREEN_WIDTH;
        private static readonly int HEIGHT = LdpUtils.SCREEN_HEIGHT;
        private static readonly int ARGB_WIDTH = WIDTH * 4;

        #region DX9Constants
        private SharpDX.Direct3D9.Device dx9Device;
        private SharpDX.Direct3D9.Surface dx9ScreenSurface;
        private DataRectangle dx9Map;
        #endregion

        #region DX11Constants
        private const int NEXT_FRAME_TIMEOUT = 4000;
        private static ushort MAX_TRYING_ATTEMPS = 0;
        private static SharpDX.Direct3D11.Device dx11Device;
        private static Factory1 dx11Factory;
        private static SharpDX.Direct3D11.Texture2DDescription dx11Texture2Ddescr;
        private static Output1 dx11Output;
        private static OutputDuplication dx11DuplicatedOutput;
        private SharpDX.Direct3D11.Texture2D dx11ScreenTexture;
        private OutputDuplicateFrameInformation dx11DuplFrameInfo;
        private SharpDX.DXGI.Resource dx11ScreenResource;
        private SharpDX.DXGI.Surface dx11ScreenSurface;
        private DataRectangle dx11Map;
        #endregion


        public LdpDirectxGrabber() 
        {
            this.pixelFormat = PixelFormat.Format32bppRgb;
            boundsRect = new System.Drawing.Rectangle(0, 0, WIDTH, HEIGHT);

            if (WINDOWS7)
            {
                screenshotType = ScreenshotType.DX9_SCREENSHOT;
                InitializeDX9();
            }
            else if (WINDOWS8)
            {
                screenshotType = ScreenshotType.DX11_SCREENSHOT;
                InitializeDX11();
            }
                
        }
        #region DX9Init
        protected override void InitializeDX9()
        {
            try
            {
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
        #endregion

        #region DX11Init
        protected override void InitializeDX11()
        {
            try
            {
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
                LdpLog.Error("SharpDX InitializeDX11\n" + dxe.Message);
            }
            catch (Exception ex)
            {
                LdpLog.Error("InitializeDX11\n" + ex.Message);
            }
        }
        #endregion

        #region DX9Grabber
        protected override Bitmap GetDX9ScreenShot()
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
        #endregion

        #region DX11Grabber
        protected override Bitmap GetDX11ScreenShot()
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
                //// cast from texture to surface, so we can access its bytes
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
                    //screen does not cnhanged
                    LdpLog.Warning("DX11 surface timeout.. Recursion is coming:)");
                    return GetDX11ScreenShot();
                }
                else
                {
                    if (MAX_TRYING_ATTEMPS < 10)
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
        #endregion

        public override Bitmap GetScreenShot()
        {
            switch (screenshotType)
            {
                case ScreenshotType.DX9_SCREENSHOT:
                    return GetDX9ScreenShot();
                case ScreenshotType.DX11_SCREENSHOT:
                    return GetDX11ScreenShot();
                default: return null;
            }
        }

        enum ScreenshotType
        {
            DX9_SCREENSHOT,
            DX11_SCREENSHOT
        }
    }
}
