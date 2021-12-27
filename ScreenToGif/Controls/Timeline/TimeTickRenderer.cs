using System;
using System.Windows;
using System.Windows.Media;

namespace ScreenToGif.Controls.Timeline
{
    public class TimeTickRenderer : FrameworkElement
    {
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(TimeSpan), typeof(TimeTickRenderer),
            new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CurrentProperty = DependencyProperty.Register(nameof(Current), typeof(TimeSpan), typeof(TimeTickRenderer),
            new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(TimeTickRenderer),
            new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TickBrushProperty = DependencyProperty.Register(nameof(TickBrush), typeof(Brush), typeof(TimeTickRenderer), new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(TimeTickRenderer), new PropertyMetadata(Brushes.Black));


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

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        public Brush TickBrush
        {
            get => (Brush)GetValue(TickBrushProperty);
            set => SetValue(TickBrushProperty, value);
        }

        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            var pen = new Pen(TickBrush, 1);

            //Make it smoother.
            //Zoom should actually increase and decrease visually.
            //Zoom based on mouse position (Adjust time).

            //00:01:30.000 (00:30.000 if empty)
            // 100% = 30000ms
            //   0% = 15050ms
            //-100% =   100ms

            //Maybe instead of defaulting to 30s, I'll use a timeSpan that equates to 0% zoom on the current display width.
            var total = (Maximum > TimeSpan.Zero ? Maximum : TimeSpan.FromSeconds(30)).TotalMilliseconds;
            var increment = (total - 100d) / 200d; //From -100 to 100 is 200 points. The difference between 100ms to total gets divided by these 200 points.
            var inView = increment * (Zoom * -1 + 100) + 100; //Zoom * increment + the minimum timespan.
            var perPixel = inView / ActualWidth;

            //Based on viewSpan, decide when to show each tick. Should be dynamic.

            // < 200ms in view:
            //Large tick: Every 1000ms
            //Medium tick: Every 500ms
            //Small tick: Every 100ms
            //Very small tick: Every 10ms
            //Timestamp: Every 500ms

            //< 15000ms in view:
            //Large tick: Every 10.000ms
            //Medium tick: Every 5.000ms
            //Small tick: Every 1.000ms
            //Very small tick: Every 100ms
            //Timestamp: Every 5.000ms

            //Large
            var largeTickFrequency = 1000 / perPixel;
            var offset = Current.TotalMilliseconds % 1000;
            var offsetSize = offset / perPixel;

            Console.WriteLine($"{Zoom}% • {inView}ms = {Current} ~ {offset} = {offsetSize}px");

            for (var x = offsetSize; x < ActualWidth; x += largeTickFrequency)
                drawingContext.DrawLine(pen, new Point(x, 0), new Point(x, 16));

            if (perPixel > 9)
            {
                base.OnRender(drawingContext);
                return;
            }

            //Medium, printed for each 500ms.
            var mediumTickFrequency = 500 / perPixel;
            offset = Current.TotalMilliseconds % 500;
            offsetSize = offset / perPixel;

            for (var x = offsetSize; x < ActualWidth; x += mediumTickFrequency)
                drawingContext.DrawLine(pen, new Point(x, 0), new Point(x, 12));

            if (perPixel > 5)
            {
                base.OnRender(drawingContext);
                return;
            }

            //Small, printed for each 100ms
            var smallTickFrequency = 100 / perPixel;
            offset = Current.TotalMilliseconds % 100;
            offsetSize = offset / perPixel;

            for (var x = offsetSize; x < ActualWidth; x += smallTickFrequency)
                drawingContext.DrawLine(pen, new Point(x, 0), new Point(x, 8));

            if (perPixel > 1.5)
            {
                base.OnRender(drawingContext);
                return;
            }

            //Very small, printed for each 10ms.
            var verySmallTickFrequency = 10 / perPixel;
            offset = Current.TotalMilliseconds % 10;
            offsetSize = offset / perPixel;

            for (var x = offsetSize; x < ActualWidth; x += verySmallTickFrequency)
                drawingContext.DrawLine(pen, new Point(x, 0), new Point(x, 4));

            base.OnRender(drawingContext);
        }
    }
}