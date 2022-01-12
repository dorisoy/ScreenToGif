using ScreenToGif.Domain.Enums;
using System.Windows.Media;

namespace ScreenToGif.Domain.Models.Project.Sequences.Effects;

public class Shadow : Effect
{
    public Shadow()
    {
        Type = EffectTypes.Shadow;
    }

    public Color Color { get;set; } 
        
    public double Direction { get; set; }
        
    public double BlurRadius { get; set; }

    public double Opacity { get; set; }

    public double Depth { get; set; }
}