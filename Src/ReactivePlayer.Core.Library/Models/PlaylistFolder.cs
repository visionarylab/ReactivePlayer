﻿using DynamicData;
using DynamicData.List;
using DynamicData.Cache;
using DynamicData.Operators;
using DynamicData.PLinq;
using DynamicData.Kernel;
using DynamicData.Aggregation;
using DynamicData.Annotations;
using DynamicData.Binding;
using DynamicData.Diagnostics;
using DynamicData.Experimental;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
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

namespace ReactivePlayer.Core.Library.Models
{
    public class PlaylistFolder : PlaylistBase, IDisposable
    {
        #region constants & fields
        #endregion

        #region ctors

        public PlaylistFolder(
            uint id,
            uint? parentId,
            string name,
            IEnumerable<Playlist> playlists) : base(id, parentId, name)
        {
            this.Name = name;
            this.ParentId = parentId;

            this._playlistsSourceList = new SourceList<PlaylistBase>().DisposeWith(this._disposables);
            this._playlistsSourceList.Edit(cache => cache.Add(playlists));

            this.TrackIds = this._playlistsSourceList.Connect().MergeMany(p => p.TrackIds.Connect()).Distinct().AsObservableList().DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private readonly SourceList<PlaylistBase> _playlistsSourceList;
        public IObservableList<PlaylistBase> Playlists => this._playlistsSourceList;

        public override IObservableList<uint> TrackIds { get; }
        //public override IObservableCache<uint, uint> TrackIds { get; }

        #endregion

        #region methods

        public void Add(PlaylistBase playlistBase)
        {
            this._playlistsSourceList.Add(playlistBase);
        }

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this._isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}