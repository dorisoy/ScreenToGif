using System.Windows;
using System.Windows.Controls;

namespace ScreenToGif.Controls.Timeline
{
    /// <summary>
    /// Track that displays sequences for the timeline.
    /// </summary>
    public class TimelineTrack : Control
    {
        public static readonly DependencyProperty TrackNameProperty = DependencyProperty.Register(nameof(TrackName), typeof(string), typeof(TimelineTrack), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty IsTrackVisibleProperty = DependencyProperty.Register(nameof(IsTrackVisible), typeof(bool), typeof(TimelineTrack), new PropertyMetadata(true));

        public static readonly DependencyProperty IsTrackLockedProperty = DependencyProperty.Register(nameof(IsTrackLocked), typeof(bool), typeof(TimelineTrack), new PropertyMetadata(false));


        public string TrackName
        {
            get => (string)GetValue(TrackNameProperty);
            set => SetValue(TrackNameProperty, value);
        }

        public bool IsTrackVisible
        {
            get => (bool)GetValue(IsTrackVisibleProperty);
            set => SetValue(IsTrackVisibleProperty, value);
        }

        public bool IsTrackLocked
        {
            get => (bool)GetValue(IsTrackLockedProperty);
            set => SetValue(IsTrackLockedProperty, value);
        }




        static TimelineTrack()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineTrack), new FrameworkPropertyMetadata(typeof(TimelineTrack)));
        }

        //Header > content
        //Allow dragging sequences
        //Render based on ViewportStart, Zoom
        //
    }
}