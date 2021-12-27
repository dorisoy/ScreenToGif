using ScreenToGif.Domain.Enums;
using System.Windows.Input;

namespace ScreenToGif.Domain.Models.Recording.Events;

public class KeyEvent : RecordingEvent
{
    public KeyEvent()
    {
        Type = RecordingEvents.Key;
    }

    public Key Key { get; set; }

    public ModifierKeys Modifiers { get; set; }
}