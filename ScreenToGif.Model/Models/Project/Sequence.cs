using ScreenToGif.Domain.Enums;
using ScreenToGif.Domain.Models.Project.Sequences.Effects;
using System.Windows.Media;

namespace ScreenToGif.Domain.Models.Project;

public class Sequence
{
    public int Id { get; set; }

    public SequenceTypes Type { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public double Opacity { get; set; }

    public Brush Background { get; set; }

    public List<Shadow> Effects { get; set; }
}