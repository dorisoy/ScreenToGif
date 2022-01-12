using System;
using System.Windows;
using System.Windows.Controls;

namespace ScreenToGif.Controls.Timeline
{
    public class TimelineSelection : Control
    {
        public static readonly DependencyProperty ViewportStartProperty = DependencyProperty.Register(nameof(ViewportStart), typeof(TimeSpan), typeof(TimelineSelection),
            new FrameworkPropertyMetadata(TimeSpan.Zero, ViewportStart_Changed));

        public static readonly DependencyProperty StartSelectionProperty = DependencyProperty.Register(nameof(StartSelection), typeof(TimeSpan?), typeof(TimelineSelection),
            new FrameworkPropertyMetadata((TimeSpan?)null));

        public static readonly DependencyProperty EndSelectionProperty = DependencyProperty.Register(nameof(EndSelection), typeof(TimeSpan?), typeof(TimelineSelection),
            new FrameworkPropertyMetadata((TimeSpan?)null));

        public TimeSpan ViewportStart
        {
            get => (TimeSpan)GetValue(ViewportStartProperty);
            set => SetValue(ViewportStartProperty, value);
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


        static TimelineSelection()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


        }

        private static void ViewportStart_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}