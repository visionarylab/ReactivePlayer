﻿using ReactivePlayer.UI.WPF.Services;
using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ReactivePlayer.UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for PlaybackTimelineView.xaml
    /// </summary>
    public partial class PlaybackTimelineView : UserControl, IViewFor<PlaybackTimelineViewModel>, IDisposable
    {
        // TODO: move to VM?
        //private readonly WaveformTimelineSoundPlayer _waveformTimelineSoundPlayer;

        public PlaybackTimelineView(
            //WaveformTimelineSoundPlayer waveformTimelineSoundPlayer
            )
        {
            var whenDataContextChangedAndViewLoaded =
                this.Events().DataContextChanged
                //.Do(dc =>
                //{
                //    var ndctn = (dc.NewValue != null) ? dc.NewValue.ToString() : "null";
                //    Debug.WriteLine($"dc changed: {ndctn}");
                //})
                .And(this.Events().Loaded.Repeat())
                .Then((dataContextChangedEventArgs, loadedEventArgs) => dataContextChangedEventArgs);

            Observable
                .When(whenDataContextChangedAndViewLoaded)
                .Subscribe(dataContextChangedEventArgs =>
                {
                    if (dataContextChangedEventArgs.OldValue != null)
                    {
                        // TODO: remove event handler from old viewmodel
                    }

                    if (this.ViewModel != null)
                    {
                        this.AttachSeekingCommandsToSlider(this.PlaybackPositionSlider);
                    }
                })
                .DisposeWith(this._disposables);

            this.InitializeComponent();

            //Observable.Interval(TimeSpan.FromSeconds(2))
            //    .Select(x =>
            //    {
            //        string name = null;

            //        switch (x % 3)
            //        {
            //            case 0:
            //                name = $"dio";
            //                break;
            //            case 1:
            //                name = $"cane";
            //                break;
            //            case 2:
            //                name = $"ladro";
            //                break;
            //        }

            //        return $"{name}-{x}";
            //    })
            //    .ObserveOnDispatcher()
            //    .Subscribe(x =>
            //    {
            //        this.DataContext = x;
            //    });
        }

        #region IViewFor

        public PlaybackTimelineViewModel ViewModel
        {
            get => this.DataContext as PlaybackTimelineViewModel;
            set => this.DataContext = value ?? throw new ArgumentNullException(nameof(value)); // TODO: localize
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as PlaybackTimelineViewModel);
        }

        #endregion

        #region time slider

        private IConnectableObservable<Unit> _whenDragStartedSubscriber;
        private IConnectableObservable<long> _whenDragDeltaSubscriber;
        private IConnectableObservable<long> _whenDragCompletedSubscriber;
        //private IDisposable _dragDeltaSubscription;
        //private IDisposable _dragCompletedSubscription;

        private void AttachSeekingCommandsToSlider(Slider slider)
        {
            Track sliderTrack = null;
            Thumb sliderThumb = null;

            sliderTrack = slider.Template.FindName("PART_Track", slider) as Track;
            sliderThumb = sliderTrack?.Thumb;

            if (sliderThumb == null)
                return;

            this._whenDragStartedSubscriber = Observable
                .FromEventPattern<DragStartedEventHandler, DragStartedEventArgs>(
                h => sliderThumb.DragStarted += h,
                h => sliderThumb.DragStarted -= h)
                .Do(x => Debug.WriteLine("Drag started"))
                //.Select(_ => Convert.ToInt64(slider.Value))
                .Select(_ => Unit.Default)
                .Publish();

            this._whenDragDeltaSubscriber = Observable
                .FromEventPattern<DragDeltaEventHandler, DragDeltaEventArgs>(
                h => sliderThumb.DragDelta += h,
                h => sliderThumb.DragDelta -= h)
                .Do(x => Debug.WriteLine("Drag delta"))
                .Select(_ => Convert.ToInt64(slider.Value))
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Publish();

            this._whenDragCompletedSubscriber = Observable
                .FromEventPattern<DragCompletedEventHandler, DragCompletedEventArgs>(
                h => sliderThumb.DragCompleted += h,
                h => sliderThumb.DragCompleted -= h)
                .Do(x => Debug.WriteLine("Drag completed"))
                .Select(_ => Convert.ToInt64(slider.Value))
                //.Select(_ => Unit.Default)
                //.Take(1)
                .Publish();

            // ------------------

            //this._whenDragStartedSubscriber
            //    .Subscribe(_ => this._dragDeltaSubscription = this._whenDragDeltaSubscriber.Connect().DisposeWith(this._disposables))
            //    .DisposeWith(this._disposables);
            //this._whenDragStartedSubscriber
            //    .Subscribe(_ => this._dragCompletedSubscription = this._whenDragCompletedSubscriber.Connect().DisposeWith(this._disposables))
            //    .DisposeWith(this._disposables);

            // --------------

            this._whenDragStartedSubscriber
                .InvokeCommand(this.ViewModel, vm => vm.StartSeeking)
                .DisposeWith(this._disposables);

            this._whenDragDeltaSubscriber
               .InvokeCommand(this.ViewModel, vm => vm.SeekTo)
               .DisposeWith(this._disposables);

            //this._whenDragCompletedSubscriber
            //    .InvokeCommand(this.ViewModel, vm => vm.SeekTo)
            //    .DisposeWith(this._disposables);

            this._whenDragCompletedSubscriber
                .InvokeCommand(this.ViewModel, vm => vm.EndSeeking)
                .DisposeWith(this._disposables);

            this._whenDragStartedSubscriber.Connect();
            //this._whenDragDeltaSubscriber.Connect();
            this._whenDragCompletedSubscriber.Connect();

            //this._whenDragCompletedSubscriber
            //    .Subscribe(_ => this._dragDeltaSubscription = null)
            //    .DisposeWith(this._disposables);

            //this._whenDragCompletedSubscriber
            //    .Subscribe(_ =>
            //    {
            //        this._dragDeltaSubscription?.Dispose();
            //        this._dragCompletedSubscription = null;
            //    })
            //    .DisposeWith(this._disposables);            
        }

        #endregion

        #region waveform timeline

        //private void AttachPlaybackServiceToTimeline()
        //{
        //    if (this.WaveformTimeline == null)
        //        return;

        //    this.WaveformTimeline.RegisterSoundPlayer(this._waveformTimelineSoundPlayer);
        //}

        #endregion

        #region IDisposable Support

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this.disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}