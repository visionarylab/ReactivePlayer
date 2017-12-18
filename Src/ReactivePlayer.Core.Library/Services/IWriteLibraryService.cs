using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library
{
    public interface IWriteLibraryService
    {
        Task<bool> AddTrack(AddTrackCommand command);
        Task<bool> AddTracks(IReadOnlyList<AddTrackCommand> commands);

        Task<bool> RemoveTrackAsync(RemoveTrackCommand command);
        Task<bool> RemoveTracksAsync(IReadOnlyList<RemoveTrackCommand> commands);

        Task<bool> UpdateTrackAsync(UpdateTrackCommand command);
        Task<bool> UpdateTracksAsync(IReadOnlyList<UpdateTrackCommand> commands);
    }
}