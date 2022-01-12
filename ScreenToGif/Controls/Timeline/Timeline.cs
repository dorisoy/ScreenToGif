using ScreenToGif.Domain.Models.Project;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ScreenToGif.Controls.Timeline
{
    public class Timeline : Control
    {
        #region Properties

        public static readonly DependencyProperty TracksProperty = DependencyProperty.Register(nameof(Tracks), typeof(ObservableCollection<Track>), typeof(Timeline),
            new FrameworkPropertyMetadata(new ObservableCollection<Track>()));

        public static readonly DependencyProperty StartSelectionProperty = DependencyProperty.Register(nameof(StartSelection), typeof(TimeSpan?), typeof(Timeline),
            new FrameworkPropertyMetadata((TimeSpan?)null));

        public static readonly DependencyProperty EndSelectionProperty = DependencyProperty.Register(nameof(EndSelection), typeof(TimeSpan?), typeof(Timeline),
            new FrameworkPropertyMetadata((TimeSpan?)null));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(TimeSpan), typeof(Timeline),
            new FrameworkPropertyMetadata(TimeSpan.Zero));

        public static readonly DependencyProperty ViewportStartProperty = DependencyProperty.Register(nameof(ViewportStart), typeof(TimeSpan), typeof(Timeline),
            new FrameworkPropertyMetadata(TimeSpan.Zero, ViewportStart_Changed, ViewportStart_Coerce));

        public static readonly DependencyProperty CurrentProperty = DependencyProperty.Register(nameof(Current), typeof(TimeSpan), typeof(Timeline),
            new FrameworkPropertyMetadata(TimeSpan.Zero, Current_Changed, Current_Coerce));

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(Timeline),
            new FrameworkPropertyMetadata(0D, Zoom_Changed, Zoom_Coerce));


        public ObservableCollection<Track> Tracks
        {
            get => (ObservableCollection<Track>)GetValue(TracksProperty);
            set => SetValue(TracksProperty, value);
        }

        public TimeSpan? StartSelection
        {
            get => (TimeSpan?)GetValue(StartSelectionProperty);
            set => SetValue(StartSelectionProperty, value);
        }

        public TimeSpan? EndSelection
        {
            get => (TimeSpan?)GetValue(EndSelectionProperty);
            set => SetValue(EndSelectionProperty, value);
        }

        public TimeSpan Maximum
        {
            get => (TimeSpan)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public TimeSpan Current
        {
            get => (TimeSpan)GetValue(CurrentProperty);
            set => SetValue(CurrentProperty, value);
        }

        public TimeSpan ViewportStart
        {
            get => (TimeSpan)GetValue(ViewportStartProperty);
            set => SetValue(ViewportStartProperty, value);
        }

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        #endregion

        static Timeline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata(typeof(Timeline)));
        }

        //Horizontal viewport (min and max by multiplier).
        //Current view position (left).
        //Current play head (current playback position, time based)
        //Selection start-end (time-based)
        //Maximum time, but allowing users to move tracks further.

        //The view port will need to virtualized horizontally, based on the zoom level, size of the control and center of view, the renderization will happen.

        //Multiple tracks.
        //Fixed track tab (left side), with lock and visibility options.
        //Layer height.
        //Vertical scroll for layers.

        private static void Current_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static object Current_Coerce(DependencyObject d, object baseValue)
        {
            if (baseValue is TimeSpan value)
                return value.TotalMilliseconds < 0 ? TimeSpan.Zero : value;

            return baseValue;
        }

        private static void ViewportStart_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static object ViewportStart_Coerce(DependencyObject d, object baseValue)
        {
            if (baseValue is TimeSpan value)
                return value.TotalMilliseconds < 0 ? TimeSpan.Zero : value;

            return baseValue;
        }

        private static void Zoom_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static object Zoom_Coerce(DependencyObject d, object baseValue)
        {
            if (baseValue is double value)
                return value > 100d ? 100d : value < -100d ? -100d : baseValue;

            return baseValue;
        }
    }
}