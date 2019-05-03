﻿using Caliburn.Micro.ReactiveUI;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public abstract class TracksSubsetViewModel : TracksFilterViewModel, IDisposable
    {
        #region constants & fields

        private readonly IObservableList<TrackViewModel> _trackVeiwModelsSource;

        #endregion

        #region ctors

        public TracksSubsetViewModel(IObservableList<TrackViewModel> trackVeiwModelsSource)
        {
            this._trackVeiwModelsSource = trackVeiwModelsSource ?? throw new ArgumentNullException(nameof(trackVeiwModelsSource));

            this._trackVeiwModelsSource
                .Connect(this.FilterCallback)
                .Bind(out this._sortedFilteredTrackViewModelsROOC)
                .Subscribe()
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        public IObservableList<TrackViewModel> SortedFilteredTrackViewModelsOL { get; }

        private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC { get; }

        #endregion

        #region methods
        #endregion

        #region commands
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