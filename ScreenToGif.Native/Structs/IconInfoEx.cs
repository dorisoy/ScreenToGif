using System.Runtime.InteropServices;

namespace ScreenToGif.Native.Structs;

/// <summary>
/// Contains information about an icon or a cursor. Extends ICONINFO. Used by GetIconInfoEx.
/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-_iconinfoexa
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct IconInfoEx
{
    /// <summary>
    /// <para>Type: <c>DWORD</c></para>
    /// <para>The size, in bytes, of this structure.</para>
    /// </summary>
    public uint cbSize;

    /// <summary/>
    [MarshalAs(UnmanagedType.Bool)]
    public bool fIcon;

    /// <summary>
    /// <para>Type: <c>DWORD</c></para>
    /// <para>
    /// The x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot spot is always in the center of the icon,
    /// and this member is ignored.
    /// </para>
    /// </summary>
    public uint xHotspot;

    /// <summary>
    /// <para>Type: <c>DWORD</c></para>
    /// <para>
    /// The y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot spot is always in the center of the
    /// icon, and this member is ignored.
    /// </para>
    /// </summary>
    public uint yHotspot;

    /// <summary>
    /// <para>Type: <c>HBITMAP</c></para>
    /// <para>
    /// The icon bitmask bitmap. If this structure defines a black and white icon, this bitmask is formatted so that the upper half
    /// is the icon AND bitmask and the lower half is the icon XOR bitmask. Under this condition, the height should be an even
    /// multiple of two. If this structure defines a color icon, this mask only defines the AND bitmask of the icon.
    /// </para>
    /// </summary>
    public IntPtr hbmMask;

    /// <summary>
    /// <para>Type: <c>HBITMAP</c></para>
    /// <para>
    /// A handle to the icon color bitmap. This member can be optional if this structure defines a black and white icon. The AND
    /// bitmask of <c>hbmMask</c> is applied with the <c>SRCAND</c> flag to the destination; subsequently, the color bitmap is
    /// applied (using XOR) to the destination by using the <c>SRCINVERT</c> flag.
    /// </para>
    /// </summary>
    public IntPtr hbmColor;

    /// <summary>
    /// <para>Type: <c>WORD</c></para>
    /// <para>
    /// The icon or cursor resource bits. These bits are typically loaded by calls to the LookupIconIdFromDirectoryEx and
    /// LoadResource functions.
    /// </para>
    /// </summary>
    public ushort wResID;

    /// <summary>
    /// <para>Type: <c>TCHAR[MAX_PATH]</c></para>
    /// <para>The fully qualified path of the module.</para>
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.MaxPath)]
    public string szModName;

    /// <summary>
    /// <para>Type: <c>TCHAR[MAX_PATH]</c></para>
    /// <para>The fully qualified path of the resource.</para>
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.MaxPath)]
    public string szResName;
}