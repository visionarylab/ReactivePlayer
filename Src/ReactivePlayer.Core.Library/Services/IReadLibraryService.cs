using DynamicData;
using ReactivePlayer.Core.Library.Models;
using System;

namespace ReactivePlayer.Core.Library.Services
{
    // TODO: this should be named ITracksService, find a better name for the others that support domain operations
    public interface IReadLibraryService : IDisposable
    {
        //IObservableCache<Track, uint> TracksChanges { get; }
        //IObservableCache<PlaylistBase, uint> Playlists { get; }

        IObservable<IChangeSet<Track, uint>> TracksChanges { get; }
        IObservable<IChangeSet<PlaylistBase, uint>> PlaylistsChanges { get; }
    }
}