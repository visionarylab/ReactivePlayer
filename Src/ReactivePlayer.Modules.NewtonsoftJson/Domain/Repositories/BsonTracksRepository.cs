﻿using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using ReactivePlayer.Domain.Entities;
using ReactivePlayer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Json.Repositories
{
    public sealed class BsonTracksRepository : ITracksRepository // TODO: IDisposable to close FileStream????
    {
        private readonly Uri _dbFileLocation;
        private FileStream _dbFileStream;
        private List<Track> _tracks = new List<Track>();
        //private SortedList<Guid, Track> _tracks = new SortedList<Guid, Track>();

        public BsonTracksRepository(Uri dbFileLocation)
        {
            this._dbFileLocation = (dbFileLocation ?? throw new ArgumentNullException(nameof(dbFileLocation))).IsFile ? dbFileLocation : throw new UriFormatException(); // TODO: maybe it's not the perfect Exception
            this._dbFileStream = File.Open(this._dbFileLocation.LocalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            //this._tracks = new List<Track>();
            ReloadTracksFromBson();
        }

        #region ITracksRepository

        public Task<Track> AddAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Track>> BulkAddAsync(IEnumerable<Track> entities)
        {
            bool result;

            try
            {
                if (entities.Any(e => this._tracks.Select(t => t.Id).Contains(e.Id)))
                {
                    throw new Exception("Duplicate Id");
                }

                this._tracks.AddRange(entities);

                result = await Commit();
            }
            catch (Exception)
            {
                result = false;
            }

            return result ? entities.ToList().AsReadOnly() : null;
        }

        public Task<IReadOnlyList<Track>> BulkRemoveAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> BulkUpdateAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> CountAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Track> FirstAsync(Func<Track, bool> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Track> RemoveAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<Track> UpdateAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region support methods

        private Task<bool> ReloadTracksFromBson()
        {
            bool result;

            try
            {
                using (BsonReader bsonReader = new BsonReader(this._dbFileStream)
                {
                    ReadRootValueAsArray = true,
                    CloseInput = false
                })
                {
                    JsonSerializer serializer = new JsonSerializer();
                    this._tracks = serializer.Deserialize<List<Track>>(bsonReader);
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }

            return Task.FromResult(result);
        }

        private Task<bool> Commit()
        {
            return this.WriteTracksToBson();
        }

        private Task<bool> WriteTracksToBson()
        {
            bool result;

            try
            {
                using (BsonWriter bsonWriter = new BsonWriter(this._dbFileStream)
                {
                    CloseOutput = false,
                    Formatting = Formatting.None
                })
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(bsonWriter, this._tracks);
                }

                result = true;
            }
            catch (Exception)
            {
                result = false;
                throw;
            }

            return Task.FromResult(result);
        }

        #endregion
    }
}