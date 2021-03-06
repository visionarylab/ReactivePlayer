using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaybackControlsViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        //private readonly IPlaybackService _playbackService;
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        //private readonly PlaybackQueue _playbackQueue;
        //private readonly PlaybackHistory _playbackHistory;
        //private readonly IReadLibraryService _readLibraryService;

        #endregion

        #region constructors

        public PlaybackControlsViewModel(
            //IPlaybackService playbackService,
            IAudioPlaybackEngine audioPlaybackEngine,
            //PlaybackQueue playbackQueue,
            //PlaybackHistory playbackHistory,
            //IReadLibraryService readLibraryService
            PlaybackTimelineViewModel playbackTimelineViewModel
            )
        {
            // TODO: log
            //this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService));
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            //this._playbackQueue = playbackQueue ?? throw new ArgumentNullException(nameof(playbackQueue));
            //this._playbackHistory = playbackHistory ?? throw new ArgumentNullException(nameof(playbackHistory));
            //this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this.PlaybackTimelineViewModel = playbackTimelineViewModel ?? throw new ArgumentNullException(nameof(playbackTimelineViewModel));

            this.Pause = ReactiveCommand.CreateFromTask(() => this._audioPlaybackEngine.PauseAsync(), this._audioPlaybackEngine.WhenCanPauseChanged).DisposeWith(this._disposables);
            this.Resume = ReactiveCommand.CreateFromTask(() => this._audioPlaybackEngine.ResumeAsync(), this._audioPlaybackEngine.WhenCanResumeChanged).DisposeWith(this._disposables);
            this.Stop = ReactiveCommand.CreateFromTask(() => this._audioPlaybackEngine.StopAsync(), this._audioPlaybackEngine.WhenCanStopChanged).DisposeWith(this._disposables);

            //this.PlayPrevious = ReactiveCommand
            //    .CreateFromTask(async () =>
            //    {
            //        await this._playbackService.StopAsync();
            //        var next = this._playbackQueue.Remove();
            //        if (next != null)
            //        {
            //            await this._playbackService.LoadAsync(next);
            //            await this._playbackService.PlayAsync();
            //        }
            //    }, Observable.CombineLatest(
            //        this._playbackQueue.Items.Connect().IsEmpty(),
            //        this._playbackService.WhenCanStopChanged,
            //        (isEmpty, canStop) => !isEmpty && canStop))
            //    .DisposeWith(this._disposables);
            //this.PlayNext = ReactiveCommand
            //    .CreateFromTask(async () =>
            //    {
            //        await this._playbackService.StopAsync();
            //        var next = this._playbackQueue.Remove();
            //        if (next != null)
            //        {
            //            await this._playbackService.LoadAsync(next);
            //            await this._playbackService.PlayAsync();
            //        }
            //    }, Observable.CombineLatest(
            //        this._playbackQueue.Items.Connect().IsEmpty(),
            //        this._playbackService.WhenCanStopChanged,
            //        (isEmpty, canStop) => !isEmpty && canStop))
            //    .DisposeWith(this._disposables);

            this._volumeOAPH = this._audioPlaybackEngine.WhenVolumeChanged
                .ToProperty(this, nameof(this.Volume))
                .DisposeWith(this._disposables);
            this._canPauseOAPH = this._audioPlaybackEngine.WhenCanPauseChanged
                .ToProperty(this, nameof(this.CanPause))
                .DisposeWith(this._disposables);
            this._canResumeOAPH = this._audioPlaybackEngine.WhenCanResumeChanged
                .ToProperty(this, nameof(this.CanResume))
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private readonly ObservableAsPropertyHelper<float> _volumeOAPH;
        public float Volume
        {
            get => this._volumeOAPH.Value;
            set => this._audioPlaybackEngine.Volume = value;
        }

        private readonly ObservableAsPropertyHelper<bool> _canResumeOAPH;
        public bool CanResume => this._canResumeOAPH.Value;

        private readonly ObservableAsPropertyHelper<bool> _canPauseOAPH;
        public bool CanPause => this._canPauseOAPH.Value;

        public PlaybackTimelineViewModel PlaybackTimelineViewModel { get; }

        #endregion

        #region methods

        public override async void CanClose(Action<bool> callback)
        {
            await this._audioPlaybackEngine.StopAsync()/*.ConfigureAwait(false)*/;
            this._disposables.Dispose();

            base.CanClose(callback);
        }

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> Pause { get; }
        public ReactiveCommand<Unit, Unit> Resume { get; }
        public ReactiveCommand<Unit, Unit> Stop { get; }

        //public ReactiveCommand<Unit, Unit> PlayPrevious { get; }
        //public ReactiveCommand<Unit, Unit> PlayNext { get; }

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}