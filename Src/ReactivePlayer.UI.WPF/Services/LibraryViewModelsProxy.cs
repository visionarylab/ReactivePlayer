﻿using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactivePlayer.UI.WPF.Services
{
    public class LibraryViewModelsProxy :
        //ReactiveObject, 
        IDisposable
    {
        private readonly IReadLibraryService _readLibraryService;
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        private readonly IDialogService _dialogService;
        private readonly Func<Track, TrackViewModel> _trackViewModelFactoryMethod;
        private readonly Func<Track, EditTrackTagsViewModel> _editTrackTagsViewModelFactoryMethod;
        private SerialDisposable _tracksSubscription;
        private SerialDisposable _playlistsSubscription;

        public LibraryViewModelsProxy(
            IReadLibraryService readLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod,
            Func<Track, EditTrackTagsViewModel> editTrackTagsViewModelFactoryMethod)
        {
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod));
            this._editTrackTagsViewModelFactoryMethod = editTrackTagsViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editTrackTagsViewModelFactoryMethod));

            //this._readLibraryService.TracksChanges.DeferUntilLoaded().Subscribe().DisposeWith(this._disposables);
            //this._readLibraryService.PlaylistsChanges.DeferUntilLoaded().Subscribe().DisposeWith(this._disposables);

            this._tracksSubscription = new SerialDisposable().DisposeWith(this._disposables);
            this._playlistsSubscription = new SerialDisposable().DisposeWith(this._disposables);

            this.TrackViewModelsChangeSets = this._readLibraryService.TracksChanges
                .Transform(track => this._trackViewModelFactoryMethod.Invoke(track))
                .DisposeMany()
                .Multicast(new ReplaySubject<IChangeSet<TrackViewModel, uint>>())
                .AutoConnect(1, d => this._tracksSubscription.Disposable = d);
            //.RefCount();

            this.PlaylistViewModelsChanges = this._readLibraryService.PlaylistsChanges
                .Transform(playlist => this.CreatePlaylistViewModel(playlist))
                .DisposeMany()
                .Multicast(new ReplaySubject<IChangeSet<PlaylistBaseViewModel, uint>>())
                .AutoConnect(1, d => this._playlistsSubscription.Disposable = d);
            //.RefCount();

            //this.TrackViewModelsChangeSets.DeferUntilLoaded().Subscribe().DisposeWith(this._disposables);
            //this.PlaylistViewModelsChanges.DeferUntilLoaded().Subscribe().DisposeWith(this._disposables);

            this.AllTracksViewModel = new AllTracksViewModel(
                this._audioPlaybackEngine,
                //this._readLibraryService,
                this._dialogService,
                this._editTrackTagsViewModelFactoryMethod,
                this.TrackViewModelsChangeSets);
        }

        #region properties

        public IObservable<IChangeSet<TrackViewModel, uint>> TrackViewModelsChangeSets { get; }
        public IObservable<IChangeSet<PlaylistBaseViewModel, uint>> PlaylistViewModelsChanges { get; }

        //private bool _holdTrackViewModels;
        //public bool HoldTrackViewModels
        //{
        //    get { return this._holdTrackViewModels; }
        //    set { this.RaiseAndSetIfChanged(ref this._holdTrackViewModels, value); }
        //}

        public AllTracksViewModel AllTracksViewModel { get; }

        #endregion

        #region methods

        //private PlaylistBaseViewModel<PlaylistBase> CreatePlaylistViewModel(PlaylistBase playlistBase)
        private PlaylistBaseViewModel CreatePlaylistViewModel(PlaylistBase playlistBase)
        {
            switch (playlistBase)
            {
                case SimplePlaylist simplePlaylist:
                    return new SimplePlaylistViewModel(
                        this._audioPlaybackEngine,
                        //this._readLibraryService,
                        this._dialogService,
                        this._editTrackTagsViewModelFactoryMethod,
                        this.TrackViewModelsChangeSets,
                        simplePlaylist)
                        //as PlaylistBaseViewModel<PlaylistBase>
                        ;

                case FolderPlaylist folderPlaylist:
                    return new FolderPlaylistViewModel(
                        this._audioPlaybackEngine,
                        //this._readLibraryService,
                        this._dialogService,
                        this.TrackViewModelsChangeSets,
                        this._editTrackTagsViewModelFactoryMethod,
                        folderPlaylist,
                        this.CreatePlaylistViewModel)
                        //as PlaylistBaseViewModel<PlaylistBase>
                        ;

                default:
                    throw new NotSupportedException(playlistBase.GetType().FullName + " is not a supported " + nameof(PlaylistBase) + " implementation.");
            }
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