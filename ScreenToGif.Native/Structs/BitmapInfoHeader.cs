using System.Runtime.InteropServices;
using ScreenToGif.Domain.Enums.Native;

namespace ScreenToGif.Native.Structs;

/// <summary>
/// The BITMAPINFOHEADER structure contains information about the dimensions and color format of a device-independent bitmap (DIB).
/// https://docs.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapinfoheader
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct BitmapInfoHeader
{
    /// <summary>
    /// Specifies the number of bytes required by the structure.
    /// This value does not include the size of the color table or the size of the color masks,
    /// if they are appended to the end of structure. See Remarks.
    /// </summary>
    public uint Size;

    /// <summary>
    /// Specifies the width of the bitmap, in pixels.
    /// For information about calculating the stride of the bitmap, see Remarks.
    /// </summary>
    public int Width;

    /// <summary>
    /// Specifies the height of the bitmap, in pixels.
    /// For uncompressed RGB bitmaps, if biHeight is positive, the bitmap is a bottom-up DIB with the origin at the lower left corner.
    /// If biHeight is negative, the bitmap is a top-down DIB with the origin at the upper left corner.
    /// For YUV bitmaps, the bitmap is always top-down, regardless of the sign of biHeight.
    /// Decoders should offer YUV formats with positive biHeight, but for backward compatibility they should accept YUV formats
    /// with either positive or negative biHeight.
    /// For compressed formats, Height must be positive, regardless of image orientation.
    /// </summary>
    public int Height;

    /// <summary>
    /// Specifies the number of planes for the target device.
    /// This value must be set to 1.
    /// </summary>
    public ushort Planes;

    /// <summary>
    /// Specifies the number of bits per pixel (bpp).
    /// For uncompressed formats, this value is the average number of bits per pixel.
    /// For compressed formats, this value is the implied bit depth of the uncompressed image, after the image has been decoded.
    /// </summary>
    public ushort BitCount;

    /// <summary>
    /// Compression mode of the image.
    /// </summary>
    public BitmapCompressionModes Compression;

    /// <summary>
    /// Specifies the size, in bytes, of the image. This can be set to 0 for uncompressed RGB bitmaps.
    /// </summary>
    public uint SizeImage;

    /// <summary>
    /// Specifies the horizontal resolution, in pixels per meter, of the target device for the bitmap.
    /// </summary>
    public int XPelsPerMeter;

    /// <summary>
    /// Specifies the vertical resolution, in pixels per meter, of the target device for the bitmap.
    /// </summary>
    public int YPelsPerMeter;

    /// <summary>
    /// Specifies the number of color indices in the color table that are actually used by the bitmap.
    /// </summary>
    public uint ClrUsed;

    /// <summary>
    /// Specifies the number of color indices that are considered important for displaying the bitmap.
    /// If this value is zero, all colors are important.
    /// </summary>
    public uint ClrImportant;

    public BitmapInfoHeader(bool? filler) : this()
    {
        Size = (uint)Marshal.SizeOf(typeof(MemoryStatusEx));
    }
}