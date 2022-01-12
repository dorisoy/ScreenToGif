using System;
using System.Threading.Tasks;
using ScreenToGif.Model;

namespace ScreenToGif.Capture;

public interface ICapture : IAsyncDisposable, IDisposable
{
    bool WasFrameCaptureStarted { get; set; }
    int FrameCount { get; set; }
    int MinimumDelay { get; set; }
    int Width { get; set; }
    int Height { get; set; }
    ProjectInfo Project { get; set; }

    Action<Exception> OnError {get;set;}

    void Start(int delay, int width, int height, double dpi, ProjectInfo project);
    void ResetConfiguration();
    int Capture(FrameInfo frame);
    Task<int> CaptureAsync(FrameInfo frame);
    int ManualCapture(FrameInfo frame);
    Task<int> ManualCaptureAsync(FrameInfo frame);
    void Save(FrameInfo info);
    Task Stop();
}