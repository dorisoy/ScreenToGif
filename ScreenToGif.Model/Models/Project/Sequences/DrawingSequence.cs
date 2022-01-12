using ScreenToGif.Domain.Enums;
using System.Windows.Ink;

namespace ScreenToGif.Domain.Models.Project.Sequences;

public class DrawingSequence : Sequence
{
    public StrokeCollection Strokes { get; set; }


    public DrawingSequence()
    {
        Type = SequenceTypes.Drawing;
    }
}