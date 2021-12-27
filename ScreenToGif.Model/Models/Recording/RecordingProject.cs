using ScreenToGif.Domain.Enums;
using ScreenToGif.Domain.Models.Recording.Events;
using System.Runtime.Serialization;

namespace ScreenToGif.Domain.Models.Recording;

[DataContract]
public class RecordingProject
{
    #region Identity

    /// <summary>
    /// The date of reation of this project.
    /// </summary>
    [DataMember(Order = 1)]
    public DateTime CreationDate { get; set; } = DateTime.Now;

    /// <summary>
    /// The source of this project.
    /// </summary>
    [DataMember(Order = 2)]
    public ProjectSources CreatedBy { get; set; } = ProjectSources.Unknown;

    #endregion

    #region Quality

    /// <summary>
    /// The width of the canvas.
    /// </summary>
    [DataMember(Order = 3)]
    public int Width { get; set; }

    /// <summary>
    /// The height of the canvas.
    /// </summary>
    [DataMember(Order = 4)]
    public int Height { get; set; }

    /// <summary>
    /// The base dpi of the project.
    /// </summary>
    [DataMember(Order = 5)]
    public double Dpi { get; set; } = 96;

    /// <summary>
    /// The number of channels in the captured frames.
    /// 4 is RGBA
    /// 3 is RGB
    /// </summary>
    [DataMember(Order = 6)]
    public int ChannelCount { get; set; } = 4;

    /// <summary>
    /// The bits per channel in the captured frames.
    /// </summary>
    [DataMember(Order = 7)]
    public int BitsPerChannel { get; set; } = 8;

    #endregion

    #region Path

    /// <summary>
    /// A binary cache containing a simple structure with all frames.
    /// </summary>
    public string FramesCachePath { get; set; }

    /// <summary>
    /// A binary cache containing a simple structure with all events (cursors or keys).
    /// </summary>
    public string EventsCachePath { get; set; }
    
    #endregion

    /// <summary>
    /// List of captured frames.
    /// </summary>
    [DataMember(Order = 8)]
    public List<RecordingFrame> Frames { get; set; } = new();

    /// <summary>
    /// List of captured events (cursor, keys, etc).
    /// </summary>
    [DataMember(Order = 9)]
    public List<RecordingEvent> Events { get; set; } = new();
}