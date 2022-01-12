using ScreenToGif.Domain.Enums;
using System.Windows.Input;

namespace ScreenToGif.Domain.Models.Recording.Events;

public class CursorEvent : RecordingEvent
{
    public CursorEvent()
    {
        Type = RecordingEvents.Cursor;
    }

    public CursorEvent(int top, int left, uint width, uint height, MouseButton pressedButtons, byte[] pixels)
    {
        Top = top;
        Left = left;
        Width = width;
        Height = height;
        PressedButtons = pressedButtons;
        Pixels = pixels;
    }

    public int Top { get; set; }

    public int Left { get; set; }

    public uint Width { get; set; }

    public uint Height { get; set; }

    public MouseButton PressedButtons { get; set; }

    public byte[] Pixels { get; set; }
}