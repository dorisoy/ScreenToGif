using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows;
using ScreenToGif.Model;

namespace ScreenToGif.Capture;

public abstract class BaseCapture : ICapture
{
    private Task _frameConsumerTask;

    #region Properties

    /// <summary>
    /// True if the recording has started.
    /// </summary>
    public bool WasFrameCaptureStarted { get; set; }

    /// <summary>
    /// True if the frame consumer is still accepting data.
    /// No frame should accepted if the consumer is no longer working.
    /// </summary>
    public bool IsAcceptingFrames { get; set; }

    /// <summary>
    /// The total number of frames already captured.
    /// </summary>
    public int FrameCount { get; set; }

    /// <summary>
    /// The minimum capture delay chosen by the user.
    /// </summary>
    public int MinimumDelay { get; set; }

    /// <summary>
    /// The current width of the capture. It can fluctuate, based on the DPI of the current screen.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The current height of the capture. It can fluctuate, based on the DPI of the current screen.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// The starting width of the capture. 
    /// </summary>
    public int StartWidth { get; set; }

    /// <summary>
    /// The starting height of the capture.
    /// </summary>
    public int StartHeight { get; set; }

    /// <summary>
    /// The starting scale of the recording.
    /// </summary>
    public double StartScale { get; set; }

    /// <summary>
    /// The current scale of the recording.
    /// </summary>
    public double Scale { get; set; }

    /// <summary>
    /// The difference in scale from the start frame to the current frame.
    /// </summary>
    public double ScaleDiff => StartScale / Scale;

    public ProjectInfo Project { get; set; }

    public Action<Exception> OnError { get; set; }

    protected BlockingCollection<FrameInfo> FrameConsumer { get; private set; } = new();

    #endregion

    ~BaseCapture()
    {
        Dispose();
    }

    public virtual void Start(int delay, int width, int height, double scale, ProjectInfo project)
    {
        if (WasFrameCaptureStarted)
            throw new Exception("The capture was already started. Stop before trying again.");

        FrameCount = 0;
        MinimumDelay = delay;
        StartWidth = Width = width;
        StartHeight = Height = height;
        StartScale = scale;
        Scale = scale;

        Project = project;
        Project.Width = width;
        Project.Height = height;
        Project.Dpi = 96 * scale;

        ConfigureConsumer();

        WasFrameCaptureStarted = true;
        IsAcceptingFrames = true;
    }

    private void ConfigureConsumer()
    {
        FrameConsumer ??= new BlockingCollection<FrameInfo>();

        //Spin up a Task to consume the frames.
        _frameConsumerTask = Task.Factory.StartNew(() =>
        {
            try
            {
                while (true)
                    Save(FrameConsumer.Take());
            }
            catch (InvalidOperationException)
            {
                //It means that Take() was called on a completed collection.
            }
            catch (Exception e)
            {
                //Uh-oh, fail hard when a frame fails to be saved.
                //This can occur for inumerous reasons, one being lack of disk space.
                Application.Current.Dispatcher.Invoke(() => OnError?.Invoke(e));
            }
        });
    } 

    public virtual void ResetConfiguration()
    { }

    public virtual void Save(FrameInfo info)
    { }

    public virtual int Capture(FrameInfo frame)
    {
        return 0;
    }

    public virtual Task<int> CaptureAsync(FrameInfo frame)
    {
        return null;
    }
    
    public virtual int ManualCapture(FrameInfo frame)
    {
        return Capture(frame);
    }

    public virtual Task<int> ManualCaptureAsync(FrameInfo frame)
    {
        return CaptureAsync(frame);
    }

    public virtual async Task Stop()
    {
        if (!WasFrameCaptureStarted)
            return;

        IsAcceptingFrames = false;

        //Stop the consumer thread.
        FrameConsumer.CompleteAdding();

        await _frameConsumerTask;

        WasFrameCaptureStarted = false;
    }


    internal virtual async Task DisposeInternal()
    {
        if (WasFrameCaptureStarted)
            await Stop();

        _frameConsumerTask?.Dispose();
        _frameConsumerTask = null;

        FrameConsumer?.Dispose();
        FrameConsumer = null;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeInternal();
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        DisposeInternal().Wait();
        GC.SuppressFinalize(this);
    }
}