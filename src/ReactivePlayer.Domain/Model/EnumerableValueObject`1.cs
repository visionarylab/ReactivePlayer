﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ReactivePlayer.Domain.Model
{
    public class EnumerableValueObject<T> : ValueObject<EnumerableValueObject<T>>, IReadOnlyList<T>
        where T : IEquatable<T>
    {
        #region ctor

        public EnumerableValueObject(IEnumerable<T> data)
        {
            this._data = data.ToList().AsReadOnly();
        }

        #endregion

        #region properties

        private readonly IReadOnlyList<T> _data;

        public int Count => this._data.Count;

        public T this[int index] => this._data[index];

        #endregion

        #region operators

        public static bool operator ==(EnumerableValueObject<T> left, EnumerableValueObject<T> right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
                return false;

            return ReferenceEquals(left, null) || left.Equals(right);
        }

        public static bool operator !=(EnumerableValueObject<T> left, EnumerableValueObject<T> right) => !(left == right);

        public static implicit operator EnumerableValueObject<T>(T[] data) => new EnumerableValueObject<T>(data);

        #endregion

        #region methods

        public IEnumerator<T> GetEnumerator() => this._data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._data.GetEnumerator();

        public override bool Equals(object other) => this.Equals(other as EnumerableValueObject<T>);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region ValueObject

        public override bool Equals(EnumerableValueObject<T> other) =>
            other != null
            && this.Count.Equals(other.Count)
            && this.SequenceEqual(other);

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            foreach (var t in this)
                yield return t;
        }

        #endregion
    }
}