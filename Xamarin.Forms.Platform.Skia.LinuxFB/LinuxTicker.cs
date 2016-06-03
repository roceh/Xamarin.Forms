using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Skia
{
    internal class LinuxTicker : Ticker
    {
        readonly System.Timers.Timer _timer;

        public LinuxTicker()
        {
            _timer = new System.Timers.Timer(15);
            _timer.Elapsed += (sender, args) => Device.BeginInvokeOnMainThread(() => SendSignals());
        }

        protected override void DisableTimer()
        {
            _timer.Stop();
        }

        protected override void EnableTimer()
        {
            _timer.Start();
        }
    }
}