using System.Collections;
using System.Collections.Generic;

namespace StrubT {

	public static class ReadOnlyExtensions {

		public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection) => collection as IReadOnlyCollection<T> ?? new CollectionProxy<T>(collection);

		public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list) => list as IReadOnlyList<T> ?? new ListProxy<T>(list);

		public static IReadOnlyDictionary<K, V> AsReadOnly<K, V>(this IDictionary<K, V> dictionary) => dictionary as IReadOnlyDictionary<K, V> ?? new DictionaryProxy<K, V>(dictionary);

		class CollectionProxy<T> : IReadOnlyCollection<T> {

			ICollection<T> Collection { get; }

			public CollectionProxy(ICollection<T> collection) => Collection = collection;

			IEnumerator IEnumerable.GetEnumerator() => Collection.GetEnumerator();

			IEnumerator<T> IEnumerable<T>.GetEnumerator() => Collection.GetEnumerator();

			int IReadOnlyCollection<T>.Count => Collection.Count;
		}

		class ListProxy<T> : CollectionProxy<T>, IReadOnlyList<T> {

			IList<T> List { get; }

			public ListProxy(IList<T> list) : base(list) => List = list;

			T IReadOnlyList<T>.this[int index] => List[index];
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
	}
}
