using System;
using System.Collections;
using System.Collections.Generic;

namespace StrubT {

	static class ReadOnlyExtensions {

		public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection) => collection as IReadOnlyCollection<T> ?? new CollectionProxy<T>(collection);

		public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list) => list as IReadOnlyList<T> ?? new ListProxy<T>(list);

		public static ISet<T> AsReadOnlySet<T>(this ISet<T> set) => set.IsReadOnly ? set : new SetProxy<T>(set);

		public static IReadOnlyDictionary<K, V> AsReadOnly<K, V>(this IDictionary<K, V> dictionary) => dictionary as IReadOnlyDictionary<K, V> ?? new DictionaryProxy<K, V>(dictionary);

		#region read-only proxies

		class CollectionProxy<T> : IReadOnlyCollection<T> {

			ICollection<T> Collection { get; }

			public CollectionProxy(ICollection<T> collection) => Collection = collection;

			IEnumerator IEnumerable.GetEnumerator() => Collection.GetEnumerator();

			IEnumerator<T> IEnumerable<T>.GetEnumerator() => Collection.GetEnumerator();

			int IReadOnlyCollection<T>.Count => Collection.Count;

			public override int GetHashCode() => Collection.GetHashCode();

			public override bool Equals(object obj) => Collection.Equals(obj);

			public override string ToString() => Collection.ToString();
		}

		class ListProxy<T> : CollectionProxy<T>, IReadOnlyList<T> {

			IList<T> List { get; }

			public ListProxy(IList<T> list) : base(list) => List = list;

			T IReadOnlyList<T>.this[int index] => List[index];
		}

		class SetProxy<T> : ISet<T> {

			ISet<T> Set { get; }

			public SetProxy(ISet<T> set) => Set = set;

			IEnumerator<T> IEnumerable<T>.GetEnumerator() => Set.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => Set.GetEnumerator();

			void ICollection<T>.Add(T item) => throw new NotSupportedException("This collection is read-only.");

			void ICollection<T>.Clear() => throw new NotSupportedException("This collection is read-only.");

			bool ICollection<T>.Contains(T item) => Set.Contains(item);

			void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotSupportedException("This collection is read-only.");

			int ICollection<T>.Count => Set.Count;

			bool ICollection<T>.IsReadOnly => true;

			bool ICollection<T>.Remove(T item) => throw new NotSupportedException("This collection is read-only.");

			bool ISet<T>.Add(T item) => throw new NotSupportedException("This collection is read-only.");

			void ISet<T>.ExceptWith(IEnumerable<T> other) => throw new NotSupportedException("This collection is read-only.");

			void ISet<T>.IntersectWith(IEnumerable<T> other) => throw new NotSupportedException("This collection is read-only.");

			bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other) => Set.IsProperSubsetOf(other);

			bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other) => Set.IsProperSupersetOf(other);

			bool ISet<T>.IsSubsetOf(IEnumerable<T> other) => Set.IsSubsetOf(other);

			bool ISet<T>.IsSupersetOf(IEnumerable<T> other) => Set.IsSupersetOf(other);

			bool ISet<T>.Overlaps(IEnumerable<T> other) => Set.Overlaps(other);

			bool ISet<T>.SetEquals(IEnumerable<T> other) => Set.SetEquals(other);

			void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) => throw new NotSupportedException("This collection is read-only.");

			void ISet<T>.UnionWith(IEnumerable<T> other) => throw new NotSupportedException("This collection is read-only.");

			public override int GetHashCode() => Set.GetHashCode();

			public override bool Equals(object obj) => Set.Equals(obj);

			public override string ToString() => Set.ToString();
		}

		class DictionaryProxy<K, V> : CollectionProxy<KeyValuePair<K, V>>, IReadOnlyDictionary<K, V> {

			IDictionary<K, V> Dictionary { get; }

			public DictionaryProxy(IDictionary<K, V> dictionary) : base(dictionary) => Dictionary = dictionary;

			IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => Dictionary.Keys;

			IEnumerable<V> IReadOnlyDictionary<K, V>.Values => Dictionary.Values;

			V IReadOnlyDictionary<K, V>.this[K key] => Dictionary[key];

			bool IReadOnlyDictionary<K, V>.ContainsKey(K key) => Dictionary.ContainsKey(key);

			bool IReadOnlyDictionary<K, V>.TryGetValue(K key, out V value) => Dictionary.TryGetValue(key, out value);
		}
		#endregion
	}
}
