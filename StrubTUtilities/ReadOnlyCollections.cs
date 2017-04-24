using System.Collections;
using System.Collections.Generic;

namespace StrubT {

	public static class ReadOnlyExtensions {

		public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection) { return collection as IReadOnlyCollection<T> ?? new CollectionProxy<T>(collection); }

		public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list) { return list as IReadOnlyList<T> ?? new ListProxy<T>(list); }

		public static IReadOnlyDictionary<K, V> AsReadOnly<K, V>(this IDictionary<K, V> dictionary) { return dictionary as IReadOnlyDictionary<K, V> ?? new DictionaryProxy<K, V>(dictionary); }

		class CollectionProxy<T> : IReadOnlyCollection<T> {

			readonly ICollection<T> collection;

			public CollectionProxy(ICollection<T> collection) {

				this.collection = collection;
			}

			IEnumerator IEnumerable.GetEnumerator() { return collection.GetEnumerator(); }

			IEnumerator<T> IEnumerable<T>.GetEnumerator() { return collection.GetEnumerator(); }

			int IReadOnlyCollection<T>.Count { get { return collection.Count; } }
		}

		class ListProxy<T> : CollectionProxy<T>, IReadOnlyList<T> {

			readonly IList<T> list;

			public ListProxy(IList<T> list) : base(list) {

				this.list = list;
			}

			T IReadOnlyList<T>.this[int index] { get { return list[index]; } }
		}

		class DictionaryProxy<K, V> : CollectionProxy<KeyValuePair<K, V>>, IReadOnlyDictionary<K, V> {

			readonly IDictionary<K, V> dictionary;

			public DictionaryProxy(IDictionary<K, V> dictionary) : base(dictionary) {

				this.dictionary = dictionary;
			}

			IEnumerable<K> IReadOnlyDictionary<K, V>.Keys { get { return dictionary.Keys; } }

			IEnumerable<V> IReadOnlyDictionary<K, V>.Values { get { return dictionary.Values; } }

			V IReadOnlyDictionary<K, V>.this[K key] { get { return dictionary[key]; } }

			bool IReadOnlyDictionary<K, V>.ContainsKey(K key) { return dictionary.ContainsKey(key); }

			bool IReadOnlyDictionary<K, V>.TryGetValue(K key, out V value) { return dictionary.TryGetValue(key, out value); }
		}
	}
}
