﻿using DynamicData;
using DynamicData.List;
using DynamicData.Cache;
using DynamicData.Operators;
using DynamicData.PLinq;
using DynamicData.ReactiveUI;
using DynamicData.Kernel;
using DynamicData.Aggregation;
using DynamicData.Annotations;
using DynamicData.Binding;
using DynamicData.Diagnostics;
using DynamicData.Experimental;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public abstract class PlaylistViewModel : PlaylistViewModelBase, IDisposable
    {
        #region constants & fields

        private readonly Playlist _playlist;
        private readonly IObservableCache<uint, uint> _playlistIdsCache;
        private readonly IObservable<IChangeSet<TrackViewModel, uint>> _filteredSortedViewModelsChangeSet;

        #endregion

        #region ctors

        public PlaylistViewModel(
            IObservableCache<TrackViewModel, uint> allTrackViewModelsSourceCache,
            Playlist playlist) 
            : base(
                  allTrackViewModelsSourceCache,
                  playlist)
        {
            this._playlist = playlist;

            this._playlistIdsCache = this._playlist.TrackIds
                .Connect()
                .AddKey(x => x)
                .AsObservableCache()
                .DisposeWith(this._disposables);

            this._filteredSortedViewModelsChangeSet = this._playlistIdsCache
                .Connect()
                .LeftJoin(
                    this._allTrackViewModelsSourceCache.Connect(),
                    vm => vm.Id,
                    (id, joinedVm) => joinedVm.Value)
                .Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime));

            this.SortedFilteredTrackViewModelsOC = this._filteredSortedViewModelsChangeSet
                .AsObservableCache()
                .DisposeWith(this._disposables);

            this._filteredSortedViewModelsChangeSet
                .Bind(out this._sortedFilteredTrackViewModelsROOC)
                .Subscribe()
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        // TODO: make return lazy, so if noone will subscribe theres no reason to create the observable cache
        public override IObservableCache<TrackViewModel, uint> SortedFilteredTrackViewModelsOC { get; }

        private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public override ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC => this._sortedFilteredTrackViewModelsROOC;

        #endregion

        #region methods

        //private bool Filter(TrackViewModel trackViewModel)
        //{
        //    return this._playlistIdsCache.Lookup(trackViewModel.Id).HasValue;
        //}

        #endregion

        #region commands
        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected override void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            base.Dispose(isDisposing);
        }

        #endregion
    }
}