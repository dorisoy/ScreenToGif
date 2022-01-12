using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ScreenToGif.Domain.Enums.Native;
using ScreenToGif.Model;
using ScreenToGif.Native.External;
using ScreenToGif.Native.Structs;
using ScreenToGif.Util;
using ScreenToGif.Util.Settings;
using System.Windows.Media.Imaging;

namespace ScreenToGif.Capture;

internal class CachedCapture : ImageCapture
{
    #region Variables

    private FileStream _fileStream;
    private BufferedStream _bufferedStream;
    private DeflateStream _compressStream;

    private BitmapInfoHeader _infoHeader;
    private long _byteLength;

    #endregion

    public override void Start(int delay, int left, int top, int width, int height, double scale, ProjectInfo project)
    {
        base.Start(delay, left, top, width, height, scale, project);

        _infoHeader = new BitmapInfoHeader();
        _infoHeader.Size = (uint)Marshal.SizeOf(_infoHeader);
        _infoHeader.BitCount = 24; //Without alpha channel.
        _infoHeader.ClrUsed = 0;
        _infoHeader.ClrImportant = 0;
        _infoHeader.Compression = 0;
        _infoHeader.Height = -StartHeight; //Negative, so the Y-axis will be positioned correctly.
        _infoHeader.Width = StartWidth;
        _infoHeader.Planes = 1;

        //This was working with 32 bits: 3L * Width * Height;
        _byteLength = (StartWidth * _infoHeader.BitCount + 31) / 32 * 4 * StartHeight;

        //Due to a strange behavior with the GetDiBits method while the cursor is IBeam, it's best to use 24 bits, to ignore the alpha values.
        //This capture mode ignores the alpha value.
        project.BitDepth = 24;

        _fileStream = new FileStream(project.CachePath, FileMode.Create, FileAccess.Write, FileShare.None);
        _bufferedStream = new BufferedStream(_fileStream, UserSettings.All.MemoryCacheSize * 1048576); //Each 1 MB has 1_048_576 bytes.
        _compressStream = new DeflateStream(_bufferedStream, UserSettings.All.CaptureCompression, true);
    }

    public override int Capture(FrameInfo frame)
    {
        try
        {
            //var success = Native.BitBlt(CompatibleDeviceContext, 0, 0, Width, Height, WindowDeviceContext, Left, Top, Native.CopyPixelOperation.SourceCopy | Native.CopyPixelOperation.CaptureBlt);
            var success = Gdi32.StretchBlt(CompatibleDeviceContext, 0, 0, StartWidth, StartHeight, WindowDeviceContext, Left, Top, Width, Height, CopyPixelOperations.SourceCopy | CopyPixelOperations.CaptureBlt);

            if (!success)
                return FrameCount;

            //Set frame details.
            FrameCount++;
            frame.Path = $"{Project.FullPath}{FrameCount}.png";
            frame.Delay = FrameRate.GetMilliseconds();
            frame.DataLength = _byteLength;
            frame.Data = new byte[_byteLength];

            if (Gdi32.GetDIBits(WindowDeviceContext, CompatibleBitmap, 0, (uint)StartHeight, frame.Data, ref _infoHeader, DibColorModes.RgbColors) == 0)
                frame.FrameSkipped = true;

            if (IsAcceptingFrames)
                FrameConsumer.Add(frame);
        }
        catch (Exception)
        {
            //LogWriter.Log(ex, "Impossible to get screenshot of the screen");
        }

        return FrameCount;
    }

    public override int CaptureWithCursor(FrameInfo frame)
    {
        try
        {
            //var success = Native.BitBlt(CompatibleDeviceContext, 0, 0, Width, Height, WindowDeviceContext, Left, Top, Native.CopyPixelOperation.SourceCopy | Native.CopyPixelOperation.CaptureBlt);
            var success = Gdi32.StretchBlt(CompatibleDeviceContext, 0, 0, StartWidth, StartHeight, WindowDeviceContext, Left, Top, Width, Height, CopyPixelOperations.SourceCopy | CopyPixelOperations.CaptureBlt);

            if (!success)
                return FrameCount;

            FallbackCursorCapture(frame);

            #region Cursor

            try
            {
                var cursorInfo = new CursorInfo(false);

                if (User32.GetCursorInfo(out cursorInfo))
                {
                    if (cursorInfo.Flags == Native.Constants.CursorShowing)
                    {
                        var hicon = User32.CopyIcon(cursorInfo.CursorHandle);

                        if (hicon != IntPtr.Zero)
                        {
                            if (User32.GetIconInfo(hicon, out var iconInfo))
                            {
                                frame.CursorX = cursorInfo.ScreenPosition.X - Left;
                                frame.CursorY = cursorInfo.ScreenPosition.Y - Top;

                                //If the cursor rate needs to be precisely captured.
                                //https://source.winehq.org/source/dlls/user32/cursoricon.c#2325
                                //int rate = 0, num = 0;
                                //var ok1 = User32.GetCursorFrameInfo(cursorInfo.hCursor, IntPtr.Zero, 17, ref rate, ref num);

                                //CursorStep
                                var ok = User32.DrawIconEx(CompatibleDeviceContext, frame.CursorX - iconInfo.XHotspot, frame.CursorY - iconInfo.YHotspot, cursorInfo.CursorHandle, 0, 0, CursorStep, IntPtr.Zero, 0x0003);

                                //var ok2 = Gdi32.GetDIBits(WindowDeviceContext, iconInfo.hbmColor, 0, 32, new byte[]{}, ref _infoHeader, DibColorModes.RgbColors);

                                if (!ok)
                                {
                                    CursorStep = 0;
                                    User32.DrawIconEx(CompatibleDeviceContext, frame.CursorX - iconInfo.XHotspot, frame.CursorY - iconInfo.YHotspot, cursorInfo.CursorHandle, 0, 0, CursorStep, IntPtr.Zero, 0x0003);
                                }
                                else
                                    CursorStep++;

                                //var ic = System.Drawing.Icon.FromHandle(hicon);
                                //var bmp = ic.ToBitmap();
                                
                                //Set to fix all alpha bits back to 255.
                                //frame.RemoveAnyTransparency = iconInfo.hbmMask != IntPtr.Zero;
                            }

                            Gdi32.DeleteObject(iconInfo.Color);
                            Gdi32.DeleteObject(iconInfo.Mask);
                        }

                        User32.DestroyIcon(hicon);
                    }

                    Gdi32.DeleteObject(cursorInfo.CursorHandle);
                }
            }
            catch (Exception e)
            {
                //LogWriter.Log(e, "Impossible to get the cursor");
            }

            #endregion

            //Set frame details.
            FrameCount++;
            frame.Path = $"{Project.FullPath}{FrameCount}.png";
            frame.Delay = FrameRate.GetMilliseconds();
            frame.DataLength = _byteLength;
            frame.Data = new byte[_byteLength];

            if (Gdi32.GetDIBits(WindowDeviceContext, CompatibleBitmap, 0, (uint)StartHeight, frame.Data, ref _infoHeader, DibColorModes.RgbColors) == 0)
                frame.FrameSkipped = true;

            if (IsAcceptingFrames)
                FrameConsumer.Add(frame);
        }
        catch (Exception e)
        {
            //LogWriter.Log(ex, "Impossible to get the screenshot of the screen");
        }

        return FrameCount;
    }

    public override void Save(FrameInfo info)
    {
        if (UserSettings.All.PreventBlackFrames && info.Data != null && !info.FrameSkipped && info.Data[0] == 0)
        {
            if (!info.Data.Any(a => a > 0))
                info.FrameSkipped = true;
        }

        //If the frame skipped, just increase the delay to the previous frame.
        if (info.FrameSkipped || info.Data == null)
        {
            info.Data = null;

            //Pass the duration to the previous frame, if any.
            if (Project.Frames.Count > 0)
                Project.Frames[Project.Frames.Count - 1].Delay += info.Delay;

            return;
        }

        _compressStream.WriteBytes(info.Data);
        info.Data = null;

        Project.Frames.Add(info);
    }

    public override async Task Stop()
    {
        if (!WasFrameCaptureStarted)
            return;

        //Stop the recording first.
        await base.Stop();

        //Then close the streams.
        //_compressStream.Flush();
        await _compressStream.DisposeAsync();

        await _bufferedStream.FlushAsync();
        await _fileStream.FlushAsync();

        await _bufferedStream.DisposeAsync();
        await _fileStream.DisposeAsync();
    }

    [Obsolete("Only for test")]
    public void Other()
    {
        var hDc = User32.GetWindowDC(IntPtr.Zero);
        var hMemDc = Gdi32.CreateCompatibleDC(hDc);

        var bi = new BitmapInfoHeader();
        bi.Size = (uint)Marshal.SizeOf(bi);
        bi.BitCount = 24; //Creating RGB bitmap. The following three members don't matter  
        bi.ClrUsed = 0;
        bi.ClrImportant = 0;
        bi.Compression = 0;
        bi.Height = Height;
        bi.Width = Width;
        bi.Planes = 1;

        var cb = (int)(bi.Height * bi.Width * bi.BitCount / 8); //8 is bits per byte.  
        bi.SizeImage = (uint)(((((bi.Width * bi.BitCount) + 31) & ~31) / 8) * bi.Height);
        //bi.biXPelsPerMeter = XPelsPerMeter;
        //bi.biYPelsPerMeter = YPelsPerMeter;
        bi.XPelsPerMeter = 96;
        bi.YPelsPerMeter = 96;

        var pBits = IntPtr.Zero;
        //Allocate memory for bitmap bits  
        var pBI = Kernel32.LocalAlloc((uint)LocalMemoryFlags.LPTR, new UIntPtr(bi.Size));
        // Not sure if this needed - simply trying to keep marshaller happy  
        Marshal.StructureToPtr(bi, pBI, false);
        //This will return IntPtr to actual DIB bits in pBits  
        var hBmp = Gdi32.CreateDIBSection(hDc, ref pBI, 0, out pBits, IntPtr.Zero, 0);
        //Marshall back - now we have BitmapInfoHeader correctly filled in Marshal.PtrToStructure(pBI, bi);

        var biNew = (BitmapInfoHeader)Marshal.PtrToStructure(pBI, typeof(BitmapInfoHeader));
        //Usual stuff  
        var hOldBitmap = Gdi32.SelectObject(hMemDc, hBmp);
        //Grab bitmap  
        var nRet = Gdi32.BitBlt(hMemDc, 0, 0, bi.Width, bi.Height, hDc, Left, Top, CopyPixelOperations.SourceCopy | CopyPixelOperations.CaptureBlt);

        // Allocate memory for a copy of bitmap bits  
        var realBits = new byte[cb];
        // And grab bits from DIBSestion data  
        Marshal.Copy(pBits, realBits, 0, cb);

        //This simply creates valid bitmap file header, so it can be saved to disk  
        var bfh = new BitmapFileHeader();
        bfh.bfSize = (uint)cb + 0x36; // Size of header + size of Native.BitmapInfoHeader size of bitmap bits
        bfh.bfType = 0x4d42; //BM  
        bfh.bfOffBits = 0x36; //  
        var hdrSize = 14;
        var header = new byte[hdrSize];

        BitConverter.GetBytes(bfh.bfType).CopyTo(header, 0);
        BitConverter.GetBytes(bfh.bfSize).CopyTo(header, 2);
        BitConverter.GetBytes(bfh.bfOffBits).CopyTo(header, 10);
        //Allocate enough memory for complete bitmap file  
        var data = new byte[cb + bfh.bfOffBits];
        //BITMAPFILEHEADER  
        header.CopyTo(data, 0);

        //BitmapInfoHeader  
        header = new byte[Marshal.SizeOf(bi)];
        var pHeader = Kernel32.LocalAlloc((uint)LocalMemoryFlags.LPTR, new UIntPtr((uint)Marshal.SizeOf(bi)));
        Marshal.StructureToPtr(biNew, pHeader, false);
        Marshal.Copy(pHeader, header, 0, Marshal.SizeOf(bi));
        Kernel32.LocalFree(pHeader);
        header.CopyTo(data, hdrSize);
        //Bitmap bits  
        realBits.CopyTo(data, (int)bfh.bfOffBits);

        //Native.SelectObject(_compatibleDeviceContext, _oldBitmap);
        //Native.DeleteObject(_compatibleBitmap);
        //Native.DeleteDC(_compatibleDeviceContext);
        //Native.ReleaseDC(_desktopWindow, _windowDeviceContext);

        Gdi32.SelectObject(hMemDc, hOldBitmap);
        Gdi32.DeleteObject(hBmp);
        Gdi32.DeleteDC(hMemDc);
        User32.ReleaseDC(IntPtr.Zero, hDc);
    }

    private void FallbackCursorCapture(FrameInfo frame)
    {
        try
        {
            //ReSharper disable once RedundantAssignment, disable once InlineOutVariableDeclaration
            var cursorInfo = new CursorInfo(false);

            if (!User32.GetCursorInfo(out cursorInfo))
                return;

            if (cursorInfo.Flags != Native.Constants.CursorShowing)
            {
                Gdi32.DeleteObject(cursorInfo.CursorHandle);
                return;
            }

            var iconHandle = User32.CopyIcon(cursorInfo.CursorHandle);

            if (iconHandle == IntPtr.Zero)
            {
                User32.DestroyIcon(iconHandle);
                Gdi32.DeleteObject(cursorInfo.CursorHandle);
                return;
            }

            if (!User32.GetIconInfo(iconHandle, out var iconInfo))
            {
                Gdi32.DeleteObject(iconInfo.Color);
                Gdi32.DeleteObject(iconInfo.Mask);

                User32.DestroyIcon(iconHandle);
                Gdi32.DeleteObject(cursorInfo.CursorHandle);
                return;
            }

            var iconInfoEx = new IconInfoEx();
            iconInfoEx.cbSize = (uint) Marshal.SizeOf(iconInfoEx);

            if (!User32.GetIconInfoEx(iconHandle, ref iconInfoEx))
            {

            }

            //Color.
            var colorHeader = new BitmapInfoHeader(false);

            Gdi32.GetDIBits(WindowDeviceContext, iconInfo.Color, 0, 0, null, ref colorHeader, DibColorModes.RgbColors);

            if (colorHeader.Height != 0)
            {
                var channels = (int) colorHeader.SizeImage / (colorHeader.Width * colorHeader.Height);
                var colorBuffer = new byte[colorHeader.SizeImage];

                //Trial.
                //var size = (int) (Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Cursors", "CursorBaseSize", 32) ?? 32);
                //colorHeader.Width = size;
                //colorHeader.Height = size;
                //colorHeader.Compression = BitmapCompressionModes.Rgb;
                //colorBuffer = new byte[colorHeader.Height * colorHeader.Width * channels * (colorHeader.BitCount / 8)];
                //colorHeader.SizeImage = (uint) colorBuffer.Length;

                colorHeader.Height *= -1;
                Gdi32.GetDIBits(WindowDeviceContext, iconInfo.Color, 0, (uint)(colorHeader.Height * -1), colorBuffer, ref colorHeader, DibColorModes.RgbColors);

                //Temporary.
                var img = ImageUtil.ImageMethods.FromArray(colorBuffer, colorHeader.Width, colorHeader.Height * -1, channels, colorHeader.BitCount);

                using var fileStream = new FileStream(@"C:\Users\nicke\Desktop\Output\Color.png", FileMode.Create);
                var bmpEncoder = new BmpBitmapEncoder();
                bmpEncoder.Frames.Add(BitmapFrame.Create(img));
                bmpEncoder.Save(fileStream);

                var image2 = System.Drawing.Image.FromHbitmap(iconInfo.Color);
                image2.Save(@"C:\Users\nicke\Desktop\Output\Color2.png");
            }

            //Mask.
            var maskHeader = new BitmapInfoHeader(false);

            Gdi32.GetDIBits(WindowDeviceContext, iconInfo.Mask, 0, 0, null, ref maskHeader, DibColorModes.RgbColors);

            if (maskHeader.Height != 0)
            {
                var maskBuffer = new byte[maskHeader.SizeImage];

                maskHeader.Height *= -1;
                Gdi32.GetDIBits(WindowDeviceContext, iconInfo.Mask, 0, (uint) maskHeader.Height, maskBuffer, ref maskHeader, DibColorModes.RgbColors);

                //Temporary.
                var image = ImageUtil.ImageMethods.FromArray(maskBuffer, maskHeader.Width, maskHeader.Height * -1, 1, maskHeader.BitCount);
                
                using var fileStream = new FileStream(@"C:\Users\nicke\Desktop\Output\Mask.png", FileMode.Create);
                var bmpEncoder = new PngBitmapEncoder();
                bmpEncoder.Frames.Add(BitmapFrame.Create(image));
                bmpEncoder.Save(fileStream);

                var image2 = System.Drawing.Image.FromHbitmap(iconInfo.Mask);
                image2.Save(@"C:\Users\nicke\Desktop\Output\Mask2.png");
            }

            //TODO:
            //Set the cursor bytes and properties to the be saved.
            //Width, Height, Stride?, XHotspot, YHotspot, Type?, Color?, Mask?
            //Ignore masks with 32x32, since they are useless. When that happens, use just color.
            //How to save this data, passing to the base class?
            //Maybe call a special register method, passing the byte array and save there, leaving data for the next events.
            //Maybe controlling with a semaphore.

            Gdi32.DeleteObject(iconInfo.Color);
            Gdi32.DeleteObject(iconInfo.Mask);
            User32.DestroyIcon(iconHandle);
            Gdi32.DeleteObject(cursorInfo.CursorHandle);
        }
        catch (Exception e)
        {
            LogWriter.Log(e, "Impossible to get the cursor");
        }
    }

    private void FallbackCursorCapture2(FrameInfo frame)
    {
        //https://stackoverflow.com/a/6374151/1735672
        var infoHeader = new BitmapInfoHeader();
        infoHeader.Size = (uint)Marshal.SizeOf(infoHeader);
        infoHeader.ClrUsed = 0;
        infoHeader.ClrImportant = 0;
        infoHeader.Compression = 0;
        infoHeader.Planes = 1;

        try
        {
            var cursorInfo = new CursorInfo(false);

            if (!User32.GetCursorInfo(out cursorInfo))
                return;

            if (cursorInfo.Flags != Native.Constants.CursorShowing)
            {
                Gdi32.DeleteObject(cursorInfo.CursorHandle);
                return;
            }

            var hicon = User32.CopyIcon(cursorInfo.CursorHandle);

            if (hicon == IntPtr.Zero)
            {
                User32.DestroyIcon(hicon);
                Gdi32.DeleteObject(cursorInfo.CursorHandle);
                return;
            }

            if (User32.GetIconInfo(hicon, out var iconInfo))
            {
                frame.CursorX = cursorInfo.ScreenPosition.X - Left;
                frame.CursorY = cursorInfo.ScreenPosition.Y - Top;

                //https://microsoft.public.vc.mfc.narkive.com/H1CZeqUk/how-can-i-get-bitmapinfo-object-from-bitmap-or-hbitmap

                //Color.
                var bitmapColor = new Bitmap();
                var handleColor = GCHandle.Alloc(bitmapColor, GCHandleType.Pinned);
                var addressColor = handleColor.AddrOfPinnedObject();

                Gdi32.GetObject(iconInfo.Color, Marshal.SizeOf<Bitmap>(), addressColor);
                bitmapColor = Marshal.PtrToStructure<Bitmap>(addressColor);
                var r = Gdi32.DeleteObject(addressColor);
                handleColor.Free();

                //Mask.
                var bitmapMask = new Bitmap();
                var handleMask = GCHandle.Alloc(bitmapMask, GCHandleType.Pinned);
                var addressMask = handleMask.AddrOfPinnedObject();

                Gdi32.GetObject(iconInfo.Mask, Marshal.SizeOf<Bitmap>(), addressMask);
                bitmapMask = Marshal.PtrToStructure<Bitmap>(addressMask);
                r = Gdi32.DeleteObject(addressMask);
                handleMask.Free();

                //Color.
                infoHeader.Height = bitmapColor.bmHeight * -1;
                infoHeader.Width = bitmapColor.bmWidth;
                infoHeader.BitCount = bitmapColor.bmBitsPixel;
                infoHeader.Planes = bitmapColor.bmPlanes;

                var w = (bitmapColor.bmWidth * bitmapColor.bmWidthBytes + 31) / 8; //(bitmapColor.bmWidth * bitmapColor.bmBitsPixel + 31) / 8;
                var cursorShapeBuffer = new byte[w * bitmapColor.bmHeight];

                var windowDeviceContext = User32.GetWindowDC(IntPtr.Zero);
                //var compatibleBitmap = Gdi32.CreateCompatibleBitmap(windowDeviceContext, Width, Height);

                var a = Gdi32.GetDIBits(windowDeviceContext, iconInfo.Color, 0, (uint)(infoHeader.Height * -1), cursorShapeBuffer, ref infoHeader, DibColorModes.RgbColors);

                //Mask.
                infoHeader.Height = bitmapMask.bmHeight;// * -1;
                infoHeader.Width = bitmapMask.bmWidth;
                infoHeader.BitCount = bitmapMask.bmBitsPixel;
                infoHeader.Planes = bitmapMask.bmPlanes;

                var cursorMask = new byte[bitmapMask.bmWidth * bitmapMask.bmHeight];

                Gdi32.GetDIBits(windowDeviceContext, iconInfo.Mask, 0, (uint)(infoHeader.Height * -1), null, ref infoHeader, DibColorModes.RgbColors);

                var b = Gdi32.GetDIBits(windowDeviceContext, iconInfo.Mask, 0, (uint)(infoHeader.Height * -1), cursorMask, ref infoHeader, DibColorModes.RgbColors);

                if (bitmapColor.bmHeight != 0)
                {
                    var img = ImageUtil.ImageMethods.FromArray(cursorShapeBuffer.ToList(), bitmapColor.bmWidth, bitmapColor.bmHeight, 4);

                    using (var fileStream = new FileStream(@"C:\Users\nicke\Desktop\Output\Color.png", FileMode.Create))
                    {
                        var bmpEncoder = new BmpBitmapEncoder();
                        bmpEncoder.Frames.Add(BitmapFrame.Create(img));
                        bmpEncoder.Save(fileStream);
                    }
                }
                
                var img2 = ImageUtil.ImageMethods.FromArray(cursorMask.ToList(), bitmapMask.bmWidth, bitmapMask.bmHeight, 1);
                
                using (var fileStream = new FileStream(@"C:\Users\nicke\Desktop\Output\Mask.png", FileMode.Create))
                {
                    var bmpEncoder = new BmpBitmapEncoder();
                    bmpEncoder.Frames.Add(BitmapFrame.Create(img2));
                    bmpEncoder.Save(fileStream);
                }

                var d = User32.ReleaseDC(IntPtr.Zero, windowDeviceContext);

                //CursorShapeInfo = new OutputDuplicatePointerShapeInformation();
                //CursorShapeInfo.Type = (int)OutputDuplicatePointerShapeType.Color;
                //CursorShapeInfo.Width = bitmap.bmWidth;
                //CursorShapeInfo.Height = bitmap.bmHeight;
                //CursorShapeInfo.Pitch = w;
                //CursorShapeInfo.HotSpot = new RawPoint(0, 0);

                //if (frame.CursorX > 0 && frame.CursorY > 0)
                //    Native.DrawIconEx(_compatibleDeviceContext, frame.CursorX - iconInfo.xHotspot, frame.CursorY - iconInfo.yHotspot, cursorInfo.hCursor, 0, 0, 0, IntPtr.Zero, 0x0003);

                //Native.SelectObject(CompatibleDeviceContext, OldBitmap);
                //Native.DeleteObject(compatibleBitmap);
                //Native.DeleteDC(CompatibleDeviceContext);
                //Native.ReleaseDC(IntPtr.Zero, windowDeviceContext);
            }

            Gdi32.DeleteObject(iconInfo.Color);
            Gdi32.DeleteObject(iconInfo.Mask);
            User32.DestroyIcon(hicon);
            Gdi32.DeleteObject(cursorInfo.CursorHandle);
        }
        catch (Exception e)
        {
            LogWriter.Log(e, "Impossible to get the cursor");
        }
    }
}