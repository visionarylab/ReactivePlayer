using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Domain.Library.Models
{
    // TODO: review controversy: if 2 tracks have the same TrackAlbumAssociation, can they be treated as the same track? NO!! Is this a weak design??
    public class TrackAlbumAssociation : ValueObject<TrackAlbumAssociation>
    {
        public TrackAlbumAssociation(Album album, uint? trackNumber, uint? discNumber)
        {
            this.Album = album ?? throw new ArgumentNullException(nameof(album));
            this.TrackNumber = trackNumber;
            this.DiscNumber = discNumber;
        }

        public Album Album { get; }
        public uint? TrackNumber { get; }
        public uint? DiscNumber { get; }

        protected override bool EqualsCore(TrackAlbumAssociation other) =>
            this.Album.Equals(other.Album)
            && this.TrackNumber.Equals(other.TrackNumber)
            && this.DiscNumber.Equals(other.DiscNumber);

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            yield return this.Album;
            yield return this.TrackNumber;
            yield return this.DiscNumber;
        }
    }
}