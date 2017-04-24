using System.Collections;
using System.Collections.Generic;

namespace StrubT {

	public class CachedEnumerable<T> : IEnumerable<T> {

		IEnumerator<T> enumerator;
		bool? isEmpty;
		T current;
		CachedEnumerable<T> next;

		public CachedEnumerable() {

			isEmpty = true;
		}

		public CachedEnumerable(IEnumerable<T> enumerable) : this(enumerable.GetEnumerator()) { }

		public CachedEnumerable(IEnumerator<T> enumerator) {

			this.enumerator = enumerator;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<T> GetEnumerator() {

			var cache = this;
			while (true) {
				lock (cache)
					if (!cache.isEmpty.HasValue && !(cache.isEmpty = !cache.enumerator.MoveNext()).Value) {
						cache.current = cache.enumerator.Current;
						cache.next = new CachedEnumerable<T>(cache.enumerator);
						cache.enumerator = null;
					}

				if (cache.isEmpty.Value)
					yield break;

				yield return cache.current;
				cache = cache.next;
			}
		}
	}

	public static class CachedEnumerableExtensions {

		public static IEnumerable<T> AsCached<T>(this IEnumerable<T> enumerable) => enumerable as CachedEnumerable<T> ?? new CachedEnumerable<T>(enumerable);

		public static IEnumerable<T> AsCached<T>(this IEnumerator<T> enumerator) => new CachedEnumerable<T>(enumerator);
	}
}
