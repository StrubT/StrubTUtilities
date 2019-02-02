using System;
using System.Collections;
using System.Collections.Generic;

namespace StrubT {

	class CachedEnumerable<T> : IEnumerable<T> {

		const CachingStrategy DefaultStrategy = CachingStrategy.SinglyLinkedList;

		readonly IEnumerable<T> Enumerable;

		public CachedEnumerable() => Enumerable = null;

		public CachedEnumerable(IEnumerable<T> enumerable, CachingStrategy strategy = DefaultStrategy) : this(enumerable.GetEnumerator(), strategy) { }

		public CachedEnumerable(IEnumerator<T> enumerator, CachingStrategy strategy = DefaultStrategy) {

			switch (strategy) {
				case CachingStrategy.SinglyLinkedList: Enumerable = new Impl_SinglyLinkedList(enumerator); break;
				case CachingStrategy.ArrayList: Enumerable = new Impl_ArrayList(enumerator); break;
				default: throw new ArgumentException();
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<T> GetEnumerator() => Enumerable.GetEnumerator();

		#region strategy implementations

		class Impl_SinglyLinkedList : IEnumerable<T> {

			readonly object @lock;
			IEnumerator<T> enumerator;
			bool? isEmpty;
			T current;
			Impl_SinglyLinkedList next;

			public Impl_SinglyLinkedList(IEnumerator<T> enumerator, object @lock = null) => (this.enumerator, this.@lock) = (enumerator, @lock ?? new object());

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public IEnumerator<T> GetEnumerator() {

				var cache = this;
				while (true) {
					lock (@lock)
						if (!cache.isEmpty.HasValue && !(cache.isEmpty = !cache.enumerator.MoveNext()).Value) {
							cache.current = cache.enumerator.Current;
							cache.next = new Impl_SinglyLinkedList(cache.enumerator, @lock);
							cache.enumerator = null;
						}

					if (cache.isEmpty.Value)
						yield break;

					yield return cache.current;
					cache = cache.next;
				}
			}
		}

		class Impl_ArrayList : IEnumerable<T> {

			readonly IEnumerator<T> enumerator;
			readonly List<T> list = new List<T>();

			public Impl_ArrayList(IEnumerator<T> enumerator) => this.enumerator = enumerator;

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public IEnumerator<T> GetEnumerator() {

				var i = 0;
				while (true) {
					lock (enumerator)
						if (list.Count <= i) {
							if (!enumerator.MoveNext())
								yield break;

							list.Add(enumerator.Current);
						}

					yield return list[i++];
				}
			}
		}
		#endregion
	}

	static class CachedEnumerableExtensions {

		public static IEnumerable<T> AsCached<T>(this IEnumerable<T> enumerable) => enumerable as CachedEnumerable<T> ?? new CachedEnumerable<T>(enumerable);

		public static IEnumerable<T> AsCached<T>(this IEnumerator<T> enumerator) => new CachedEnumerable<T>(enumerator);
	}

	public enum CachingStrategy {

		SinglyLinkedList,
		ArrayList
	}
}
