using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ScreenToGif.Util;
using ScreenToGif.Util.Extensions;

namespace ScreenToGif.Controls.Timeline
{
    public class TimeBar : Control
    {
        /// <summary>
        /// The threshold of mouse wheel delta to enable the wheel event.
        /// </summary>
        private const double MouseWheelSelectionChangeThreshold = 100;

        /// <summary>
        /// The aggregate of mouse wheel delta since the last mouse wheel event.
        /// </summary>
        private double _mouseWheelCumulativeDelta;

        private TimeTickRenderer _rendered;

        #region Properties

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(TimeSpan), typeof(TimeBar),
            new FrameworkPropertyMetadata(TimeSpan.Zero));

        public static readonly DependencyProperty ViewportStartProperty = DependencyProperty.Register(nameof(ViewportStart), typeof(TimeSpan), typeof(TimeBar),
            new FrameworkPropertyMetadata(TimeSpan.Zero, Current_Changed, Current_Coerce));

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(TimeBar),
            new FrameworkPropertyMetadata(0D, Zoom_Changed, Zoom_Coerce));


        public TimeSpan Maximum
        {
            get => (TimeSpan)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
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

        static TimeBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeBar), new FrameworkPropertyMetadata(typeof(TimeBar)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rendered = GetTemplateChild("TimeTickRenderer") as TimeTickRenderer;
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            _mouseWheelCumulativeDelta += e.Delta;

            if (!Math.Abs(_mouseWheelCumulativeDelta).GreaterThan(MouseWheelSelectionChangeThreshold))
            {
                base.OnMouseWheel(e);
                return;
            }

            e.Handled = true;
            base.OnMouseWheel(e);

            _mouseWheelCumulativeDelta = 0;

            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                //Zoom in and out.
                Zoom += e.Delta > 0 ? 1 : -1;
                _rendered.InvalidateVisual();
                return;
            }

            var total = (Maximum > TimeSpan.Zero ? Maximum : TimeSpan.FromSeconds(30)).TotalMilliseconds;
            var increment = (total - 100d) / 200d; //From -100 to 100 is 200 points. The difference between 100ms to total gets divided by these 200 points.
            var inView = increment * ((Zoom * -1) + 100) + 100; //Zoom * increment + the minimum timespan.
            var perPixel = inView / ActualWidth;

            ViewportStart += TimeSpan.FromMilliseconds((e.Delta > 0 ? 1 : -1) * perPixel);
            _rendered.InvalidateVisual();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            //Get the time based on the CurrentTime + offset of the cursor.
            //Take into consideration the zoom and the scale of the UI.

            //Shift + Click to select time span.
            //Ctrl + Click to drag to select (change cursor to size lateral).
            //Visual indicator of selection needs to be created.

            base.OnMouseDown(e);
        }

        private static void Current_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static object Current_Coerce(DependencyObject d, object baseValue)
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