using ScreenToGif.Domain.Enums;
using ScreenToGif.Domain.Models.Project;
using ScreenToGif.Domain.ViewModels;

namespace ScreenToGif.ViewModel;

public class SequenceViewModel : BaseViewModel
{
    public static SequenceViewModel FromModel(Sequence sequence)
    {
        //TODO: Copy all data.

        switch (sequence.Type)
        {
            case SequenceTypes.Brush:
                break;
            case SequenceTypes.Raster:
                break;
            case SequenceTypes.Text:
                break;
            case SequenceTypes.Shape:
                break;
            case SequenceTypes.Drawing:
                break;
            case SequenceTypes.Key:
                break;
            case SequenceTypes.Cursor:
                break;
            case SequenceTypes.Progress:
                break;
            case SequenceTypes.TitleFrame:
                break;
            case SequenceTypes.Obfuscation:
                break;
            case SequenceTypes.Cinemagraph:
                break;
        }

        return new SequenceViewModel
        {

        };
    }
}