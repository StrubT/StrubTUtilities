using System;
using System.Collections;
using System.Collections.Generic;

namespace StrubT {

	public enum ReadOnlyBehaviour { CastIfPossible, CreateProxy, CopyElements }

	public static class ReadOnlyExtensions {

		public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection, ReadOnlyBehaviour behaviour = ReadOnlyBehaviour.CastIfPossible) {
			switch (behaviour) {
				case ReadOnlyBehaviour.CastIfPossible: return collection as IReadOnlyCollection<T> ?? new CollectionProxy<T>(collection);
				case ReadOnlyBehaviour.CreateProxy: return new CollectionProxy<T>(collection);
				case ReadOnlyBehaviour.CopyElements: return new List<T>(collection);
				default: throw new ArgumentException();
			}
		}

		public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list, ReadOnlyBehaviour behaviour = ReadOnlyBehaviour.CastIfPossible) {
			switch (behaviour) {
				case ReadOnlyBehaviour.CastIfPossible: return list as IReadOnlyList<T> ?? new ListProxy<T>(list);
				case ReadOnlyBehaviour.CreateProxy: return new ListProxy<T>(list);
				case ReadOnlyBehaviour.CopyElements: return new List<T>(list);
				default: throw new ArgumentException();
			}
		}

		public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, ReadOnlyBehaviour behaviour = ReadOnlyBehaviour.CastIfPossible) {
			switch (behaviour) {
				case ReadOnlyBehaviour.CastIfPossible: return dictionary as IReadOnlyDictionary<TKey, TValue> ?? new DictionaryProxy<TKey, TValue>(dictionary);
				case ReadOnlyBehaviour.CreateProxy: return new DictionaryProxy<TKey, TValue>(dictionary);
				case ReadOnlyBehaviour.CopyElements: return new Dictionary<TKey, TValue>(dictionary);
				default: throw new ArgumentException();
			}
		}

		public static ISet<T> AsReadOnlySet<T>(this ISet<T> set, ReadOnlyBehaviour behaviour = ReadOnlyBehaviour.CreateProxy) {
			switch (behaviour) {
				case ReadOnlyBehaviour.CastIfPossible:
				case ReadOnlyBehaviour.CreateProxy: return new SetProxy<T>(set);
				case ReadOnlyBehaviour.CopyElements: return new SetProxy<T>(new HashSet<T>(set));
				default: throw new ArgumentException();
			}
		}

		#region read-only proxies

		class CollectionProxy<T> : IReadOnlyCollection<T> {

			readonly ICollection<T> Collection;

			public int Count => Collection.Count;

			public CollectionProxy(ICollection<T> collection) => Collection = collection;

			public IEnumerator<T> GetEnumerator() => Collection.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public override int GetHashCode() => Collection.GetHashCode();

			public override bool Equals(object obj) => Collection.Equals(obj);

			public override string ToString() => Collection.ToString();
		}

		class ListProxy<T> : CollectionProxy<T>, IReadOnlyList<T> {

			readonly IList<T> List;

			public T this[int index] => List[index];

			public ListProxy(IList<T> list) : base(list) => List = list;
		}

		class DictionaryProxy<TKey, TValue> : CollectionProxy<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue> {

			readonly IDictionary<TKey, TValue> Dictionary;

			public IEnumerable<TKey> Keys => Dictionary.Keys;

			public IEnumerable<TValue> Values => Dictionary.Values;

			public TValue this[TKey key] => Dictionary[key];

			public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);

			public bool TryGetValue(TKey key, out TValue value) => Dictionary.TryGetValue(key, out value);

			public DictionaryProxy(IDictionary<TKey, TValue> dictionary) : base(dictionary) => Dictionary = dictionary;
		}

		class SetProxy<T> : CollectionProxy<T>, ISet<T> {

			readonly ISet<T> Set;

			bool ICollection<T>.IsReadOnly => true;

			public SetProxy(ISet<T> set) : base(set) => Set = set;

			public bool Contains(T item) => Set.Contains(item);

			public void CopyTo(T[] array, int arrayIndex) => Set.CopyTo(array, arrayIndex);

			public bool IsProperSubsetOf(IEnumerable<T> other) => Set.IsProperSubsetOf(other);

			public bool IsProperSupersetOf(IEnumerable<T> other) => Set.IsProperSupersetOf(other);

			public bool IsSubsetOf(IEnumerable<T> other) => Set.IsSubsetOf(other);

			public bool IsSupersetOf(IEnumerable<T> other) => Set.IsSupersetOf(other);

			public bool Overlaps(IEnumerable<T> other) => Set.Overlaps(other);

			public bool SetEquals(IEnumerable<T> other) => Set.SetEquals(other);

			#region not supported, read-only

			void ICollection<T>.Add(T item) => throw new NotSupportedException("This collection is read-only.");

			void ICollection<T>.Clear() => throw new NotSupportedException("This collection is read-only.");

			bool ICollection<T>.Remove(T item) => throw new NotSupportedException("This collection is read-only.");

			bool ISet<T>.Add(T item) => throw new NotSupportedException("This collection is read-only.");

			void ISet<T>.ExceptWith(IEnumerable<T> other) => throw new NotSupportedException("This collection is read-only.");

			void ISet<T>.IntersectWith(IEnumerable<T> other) => throw new NotSupportedException("This collection is read-only.");

			void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) => throw new NotSupportedException("This collection is read-only.");

			void ISet<T>.UnionWith(IEnumerable<T> other) => throw new NotSupportedException("This collection is read-only.");
			#endregion
		}
		#endregion
	}
}
