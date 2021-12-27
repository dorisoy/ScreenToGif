namespace ScreenToGif.Domain.Enums;

public enum SequenceTypes : int
{
    /// <summary>
    /// A sequence that contains a single brush data.
    /// </summary>
    Brush,

    /// <summary>
    /// A sequence that holds raster image data.
    /// It can be the actual frame image from a recording or an overlay.
    /// </summary>
    Raster,

    /// <summary>
    /// A sequence that holds text data.
    /// </summary>
    Text,

    /// <summary>
    /// A sequence that holds a shape.
    /// </summary>
    Shape,

    /// <summary>
    /// A sequence that holds strokes (drawings).
    /// </summary>
    Drawing,

    /// <summary>
    /// A sequence that holds all keys events.
    /// </summary>
    Key,

    /// <summary>
    /// A sequence that holds all cursor events.
    /// </summary>
    Cursor,


    Progress,
    TitleFrame, //? Maybe it should be a layer type of Frame.
    Obfuscation,
    Cinemagraph
}