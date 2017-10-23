using System;
using System.Collections;
using System.Collections.Generic;

namespace StrubT {

	static class ReadOnlyExtensions {

		public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection) { return collection as IReadOnlyCollection<T> ?? new CollectionProxy<T>(collection); }

		public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list) { return list as IReadOnlyList<T> ?? new ListProxy<T>(list); }

		public static ISet<T> AsReadOnlySet<T>(this ISet<T> set) { return set.IsReadOnly ? set : new SetProxy<T>(set); }

		public static IReadOnlyDictionary<K, V> AsReadOnly<K, V>(this IDictionary<K, V> dictionary) { return dictionary as IReadOnlyDictionary<K, V> ?? new DictionaryProxy<K, V>(dictionary); }

		#region read-only proxies

		class CollectionProxy<T> : IReadOnlyCollection<T> {

			readonly ICollection<T> collection;

			public CollectionProxy(ICollection<T> collection) { this.collection = collection; }

			IEnumerator IEnumerable.GetEnumerator() { return collection.GetEnumerator(); }

			IEnumerator<T> IEnumerable<T>.GetEnumerator() { return collection.GetEnumerator(); }

			int IReadOnlyCollection<T>.Count { get { return collection.Count; } }

			public override int GetHashCode() { return collection.GetHashCode(); }

			public override bool Equals(object obj) { return collection.Equals(obj); }

			public override string ToString() { return collection.ToString(); }
		}

		class ListProxy<T> : CollectionProxy<T>, IReadOnlyList<T> {

			readonly IList<T> list;

			public ListProxy(IList<T> list) : base(list) { this.list = list; }

			T IReadOnlyList<T>.this[int index] { get { return list[index]; } }
		}

		class SetProxy<T> : ISet<T> {

			readonly ISet<T> set;

			public SetProxy(ISet<T> set) { this.set = set; }

			IEnumerator<T> IEnumerable<T>.GetEnumerator() { return set.GetEnumerator(); }

			IEnumerator IEnumerable.GetEnumerator() { return set.GetEnumerator(); }

			void ICollection<T>.Add(T item) { throw new NotSupportedException("This collection is read-only."); }

			void ICollection<T>.Clear() { throw new NotSupportedException("This collection is read-only."); }

			bool ICollection<T>.Contains(T item) { return set.Contains(item); }

			void ICollection<T>.CopyTo(T[] array, int arrayIndex) { throw new NotSupportedException("This collection is read-only."); }

			int ICollection<T>.Count { get { return set.Count; } }

			bool ICollection<T>.IsReadOnly { get { return true; } }

			bool ICollection<T>.Remove(T item) { throw new NotSupportedException("This collection is read-only."); }

			bool ISet<T>.Add(T item) { throw new NotSupportedException("This collection is read-only."); }

			void ISet<T>.ExceptWith(IEnumerable<T> other) { throw new NotSupportedException("This collection is read-only."); }

			void ISet<T>.IntersectWith(IEnumerable<T> other) { throw new NotSupportedException("This collection is read-only."); }

			bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other) { return set.IsProperSubsetOf(other); }

			bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other) { return set.IsProperSupersetOf(other); }

			bool ISet<T>.IsSubsetOf(IEnumerable<T> other) { return set.IsSubsetOf(other); }

			bool ISet<T>.IsSupersetOf(IEnumerable<T> other) { return set.IsSupersetOf(other); }

			bool ISet<T>.Overlaps(IEnumerable<T> other) { return set.Overlaps(other); }

			bool ISet<T>.SetEquals(IEnumerable<T> other) { return set.SetEquals(other); }

			void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) { throw new NotSupportedException("This collection is read-only."); }

			void ISet<T>.UnionWith(IEnumerable<T> other) { throw new NotSupportedException("This collection is read-only."); }

			public override int GetHashCode() { return set.GetHashCode(); }

			public override bool Equals(object obj) { return set.Equals(obj); }

			public override string ToString() { return set.ToString(); }
		}

		class DictionaryProxy<K, V> : CollectionProxy<KeyValuePair<K, V>>, IReadOnlyDictionary<K, V> {

			readonly IDictionary<K, V> dictionary;

			public DictionaryProxy(IDictionary<K, V> dictionary) : base(dictionary) { this.dictionary = dictionary; }

			IEnumerable<K> IReadOnlyDictionary<K, V>.Keys { get { return dictionary.Keys; } }

			IEnumerable<V> IReadOnlyDictionary<K, V>.Values { get { return dictionary.Values; } }

			V IReadOnlyDictionary<K, V>.this[K key] { get { return dictionary[key]; } }

			bool IReadOnlyDictionary<K, V>.ContainsKey(K key) { return dictionary.ContainsKey(key); }

			bool IReadOnlyDictionary<K, V>.TryGetValue(K key, out V value) { return dictionary.TryGetValue(key, out value); }
		}
		#endregion
	}
}
