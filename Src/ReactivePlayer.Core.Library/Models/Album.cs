using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.Core.Library.Models
{
    // TODO: add special version field (e.g. IsDeluxe, ...)
    public class Album : ValueObject<Album>
    {
        #region ctor

        // TODO: update ctor constraints: might not have all tags
        public Album(
            string title,
            IEnumerable<Artist> authors,
            uint? year,
            uint? tracksCount,
            uint? discsCount)
        {
            this.Title = title.TrimmedOrNull(); // ?? throw new ArgumentNullException(nameof(name), $"An {this.GetType().Name}'s {nameof(Name)} cannot be null."); // TODO: localize ;
            this.Authors = authors.EmptyIfNull().ToArray(); // TODO: this is not completely an IReadOnlyList<> since array items can be replaced via index access!!
            this.Year = year.ThrowIf(v => v > DateTime.Now.Year, () => throw new ArgumentOutOfRangeException(nameof(year)));
            this.TracksCount = tracksCount.NullIf(v => v <= 0);
            this.DiscsCount = discsCount.NullIf(v => v <= 0);
        }

        #endregion

        #region properties

        public string Title { get; }
        public IReadOnlyList<Artist> Authors { get; }
        public uint? Year { get; }
        public uint? TracksCount { get; }
        public uint? DiscsCount { get; }

        #endregion

        #region ValueObject

        protected override bool EqualsCore(Album other) =>
            this.Title.Equals(other.Title)
            && this.Authors.SequenceEqual(other.Authors)
            && this.Year.Equals(other.Year)
            && this.TracksCount.Equals(other.TracksCount)
            && this.DiscsCount.Equals(other.DiscsCount);

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            yield return this.Title;
            foreach (var a in this.Authors)
                yield return a;
            yield return this.Year;
            yield return this.TracksCount;
            yield return this.DiscsCount;
        }

        #endregion
    }
}