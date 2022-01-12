using ScreenToGif.Domain.Models.Project;
using ScreenToGif.Domain.ViewModels;
using System.Collections.ObjectModel;

namespace ScreenToGif.ViewModel;

public class TrackViewModel : BaseViewModel
{
    private int _id = 0;
    private bool _isVisible = true;
    private bool _isLocked = false;
    private string _name = "";
    private ObservableCollection<SequenceViewModel> _sequences = new();

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public bool IsLocked
    {
        get => _isLocked;
        set => SetProperty(ref _isLocked, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// A track can have multiple sequences of the same type.
    /// </summary>
    public ObservableCollection<SequenceViewModel> Sequences
    {
        get => _sequences;
        set => SetProperty(ref _sequences, value);
    }

    public static TrackViewModel FromModel(Track track)
    {
        return new TrackViewModel
        {
            Id = track.Id,
            IsVisible = track.IsVisible,
            IsLocked = track.IsLocked,
            Name = track.Name,
            Sequences = new ObservableCollection<SequenceViewModel>(track.Sequences.Select(SequenceViewModel.FromModel))
        };
    }
}