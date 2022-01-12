using ScreenToGif.Domain.Enums;

namespace ScreenToGif.Domain.Models.Recording.Events;

public class RecordingEvent
{
    public RecordingEvents Type { get; set; }

    public TimeSpan TimeStamp => TimeSpan.FromTicks(TimeStampInTicks);

    public long TimeStampInTicks { get; set; }
}