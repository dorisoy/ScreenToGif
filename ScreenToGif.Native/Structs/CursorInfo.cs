using System.Runtime.InteropServices;

namespace ScreenToGif.Native.Structs;

/// <summary>
/// Contains global cursor information.
/// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-cursorinfo
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CursorInfo
{
    /// <summary>
    /// Specifies the size, in bytes, of the structure. 
    /// </summary>
    public uint Size;

    /// <summary>
    /// Specifies the cursor state.
    /// This parameter can be one of the following values:
    /// CURSOR_SHOWING:     0x00000001
    /// CURSOR_SUPPRESSED:  0x00000002
    /// </summary>
    public uint Flags;

    ///<summary>
    ///Handle to the cursor. 
    ///</summary>
    public IntPtr CursorHandle;

    /// <summary>
    /// A POINT structure that receives the screen coordinates of the cursor. 
    /// </summary>
    public PointW ScreenPosition;

    public CursorInfo(bool? filler) : this()
    {
        Size = (uint)Marshal.SizeOf(typeof(CursorInfo));
    }
}