namespace ScreenToGif.Domain.Models.Recording;

public class RecordingFrame
{
    /// <summary>
    /// Frame delay in milliseconds.
    /// </summary>
    public uint Delay { get; set; }

    /// <summary>
    /// The capture content.
    /// </summary>
    public byte[] Pixels { get; set; }

    /// <summary>
    /// The number of bytes of the capture content.
    /// </summary>
    public ulong DataLength { get; set; }

    /// <summary>
    /// For some reason, the frame capture failed.
    /// </summary>
    public bool WasFrameSkipped { get; set; }
}