﻿using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class LibrarySidebarViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        //private readonly LibraryViewModel _libraryViewModel;

        #endregion

        #region ctors

        public LibrarySidebarViewModel(LibraryViewModel libraryViewModel)
        {
        }

        #endregion

        #region properties

        private ReadOnlyObservableCollection<PlaylistBaseViewModel<PlaylistBase>> _playlistViewModelsROOC;
        public ReadOnlyObservableCollection<PlaylistBaseViewModel<PlaylistBase>> PlaylistViewModelsROOC
        {
            get { return this._playlistViewModelsROOC; }
            set { this.RaiseAndSetIfChanged(ref this._playlistViewModelsROOC, value); }
        }

        #endregion

        #region methods

        //private void ConnectPlaylists()
        //{
        //    // TODO: make lazy, so if view doesnt request it, it's not subscribed
        //    this._libraryViewModelsProxy.PlaylistViewModels.Bind(out var playlistsRooc);
        //    this.PlaylistViewModelsROOC = playlistsRooc;
        //}

        protected override void OnActivate()
        {
            base.OnActivate();

            //this.ConnectPlaylists();
        }


        #endregion

        #region commands
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
