using DynamicData;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Joins;
using System.Reactive.Linq;
using System.Reactive.Threading;

namespace ReactivePlayer.Core.Playback.Queue
{
    public class PlaybackQueue : IDisposable
    {
        #region constants & fields

        private readonly IAudioPlaybackEngine _audioPlayer;
        //private readonly Random _random = new Random((int)DateTime.Now.Ticks);

        #endregion

        #region ctor

        public PlaybackQueue(
            IAudioPlaybackEngine audioPlayer
            //, IObservable<IChangeSet<Track> tracksCacheChanges
            )
        {
            this._audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer));

            this._sourcedEntries = new SourceList<PlaybackQueueEntry>().DisposeWith(this._disposables);

            Observable.CombineLatest(
                this._audioPlayer.WhenCanLoadChanged,
                this._audioPlayer.WhenCanPlayChanged,
                (canLoad, canPlay) => canLoad && !canPlay)
                .Where(canLoadButNotPlay => canLoadButNotPlay == true)
                .Subscribe(async (s) =>
                {
                    PlaybackQueueEntry next = null; // = this._playlistEntries.Items.FirstOrDefault();

                    this._playlistEntries.Edit(list =>
                    {
                        next = list.FirstOrDefault();
                        if (next == null)
                        {
                            // unsubscribe from playlist?
                            return;
                        }
                        list.RemoveAt(0);
                    });

                    if (next == null)
                        return;

                    switch (next.Track)
                    {
                        case Track nextTrack:
                            await this._audioPlayer.LoadAsync(nextTrack);
                            await this._audioPlayer.PlayAsync();
                            break;
                    }
                })
                .DisposeWith(this._disposables);

            //this._upNextEntries = new SourceList<PlaybackQueueEntry>().DisposeWith(this._disposables);
            this._playlistEntries = new SourceList<PlaybackQueueEntry>().DisposeWith(this._disposables);

            //this._upNextEntries.Connect().Bind(out this._upNextEntriesROOC);
            this._playlistEntries.Connect().Bind(out this._playlistEntriesROOC);
        }

        #endregion

        #region properties

        private IObservableList<Track> _tracksSource;
        private readonly ISourceList<PlaybackQueueEntry> _sourcedEntries;

        //private readonly SourceList<PlaybackQueueEntry> _upNextEntries = new SourceList<PlaybackQueueEntry>();
        //private readonly ReadOnlyObservableCollection<PlaybackQueueEntry> _upNextEntriesROOC;
        //public ReadOnlyObservableCollection<PlaybackQueueEntry> UpNextEntriesROOC => this._upNextEntriesROOC;

        private readonly ISourceList<PlaybackQueueEntry> _playlistEntries = new SourceList<PlaybackQueueEntry>();
        private readonly ReadOnlyObservableCollection<PlaybackQueueEntry> _playlistEntriesROOC;
        public ReadOnlyObservableCollection<PlaybackQueueEntry> PlaylistEntriesROOC => this._playlistEntriesROOC;

        #endregion

        #region methods

        public void Clear()
        {
            this._sourcedEntries.Clear();
            this._playlistEntries.Clear();
        }

        //public void Enqueue(IEnumerable<PlaybackQueueEntry> trackLocations)
        //{
        //    this._queueEntries.AddRange(trackLocations);
        //}

        public void SetTracksSource(IObservableList<Track> tracksSource)
        {
            this._playlistEntries.Clear();

            this._tracksSource = tracksSource;

            this._tracksSource.Connect()
                .Transform(t => new PlaybackQueueEntry(t))
                .DisposeMany()
                .PopulateInto(this._sourcedEntries)
                .DisposeWith(this._disposables);

            this._tracksSource
                .Connect()
                //.Transform(t => new PlaybackQueueEntry(t))
                .Subscribe(
                null,
                () =>
                {
                    this.Clear();
                });

            // TODO: if the currently playing track is removed from the playlist from which its playback was started, ask if stop playback

            //return Task.CompletedTask;
        }

        public void RemoveFromSourcedQueue(PlaybackQueueEntry queueEntry)
        {
        }

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