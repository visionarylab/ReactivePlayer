﻿using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data
{
    public class AlbumDto
    {
        public AlbumDto() { }

        public AlbumDto(Album album)
        {
            if (album == null) throw new ArgumentNullException(nameof(album));

            this.Name = album.Name;
            this.Authors = album.Authors.Select(a => new ArtistDto(a)).ToImmutableArray();
            this.Year = album.Year;
            this.TracksCount = album.TracksCount;
            this.DiscsCount = album.DiscsCount;
        }

        public string Name { get; }
        public IReadOnlyList<ArtistDto> Authors { get; }
        public uint? Year { get; }
        public uint? TracksCount { get; }
        public uint? DiscsCount { get; }
    }
}