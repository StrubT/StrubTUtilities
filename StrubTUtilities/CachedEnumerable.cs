using System;
using System.Collections;
using System.Collections.Generic;

namespace StrubT {

	public class CachedEnumerable<T> : IEnumerable<T> {

		const CachingStrategy DefaultStrategy = CachingStrategy.ArrayList;

		readonly IEnumerable<T> Enumerable;

		public CachedEnumerable() => Enumerable = null;

		public CachedEnumerable(IEnumerable<T> enumerable, CachingStrategy strategy = DefaultStrategy) : this(enumerable.GetEnumerator(), strategy) { }

		public CachedEnumerable(IEnumerator<T> enumerator, CachingStrategy strategy = DefaultStrategy) {

			switch (strategy) {
				case CachingStrategy.SinglyLinkedList: Enumerable = new ImplSinglyLinkedList(enumerator); break;
				case CachingStrategy.ArrayList: Enumerable = new ImplArrayList(enumerator); break;
				default: throw new ArgumentException();
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<T> GetEnumerator() => Enumerable.GetEnumerator();

		#region strategy implementations

		class ImplSinglyLinkedList : IEnumerable<T> {

			readonly object Lock;
			IEnumerator<T> Enumerator;
			bool? IsEmpty;
			T Current;
			ImplSinglyLinkedList Next;

			public ImplSinglyLinkedList(IEnumerator<T> enumerator, object @lock = null) => (Enumerator, Lock) = (enumerator, @lock ?? new object());

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public IEnumerator<T> GetEnumerator() {

				var cache = this;
				while (true) {
					lock (Lock)
						if (!cache.IsEmpty.HasValue && !(cache.IsEmpty = !cache.Enumerator.MoveNext()).Value) {
							cache.Current = cache.Enumerator.Current;
							cache.Next = new ImplSinglyLinkedList(cache.Enumerator, Lock);
							cache.Enumerator = null;
						}

					if (cache.IsEmpty.Value)
						yield break;

					yield return cache.Current;
					cache = cache.Next;
				}
			}
		}

		class ImplArrayList : IEnumerable<T> {

			readonly IEnumerator<T> Enumerator;
			readonly List<T> List = new List<T>();

			public ImplArrayList(IEnumerator<T> enumerator) => Enumerator = enumerator;

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public IEnumerator<T> GetEnumerator() {

				var i = 0;
				while (true) {
					lock (Enumerator)
						if (List.Count <= i) {
							if (!Enumerator.MoveNext())
								yield break;

							List.Add(Enumerator.Current);
						}

					yield return List[i++];
				}
			}
		}
		#endregion
	}

	public static class CachedEnumerableExtensions {

		public static IEnumerable<T> AsCached<T>(this IEnumerable<T> enumerable) => enumerable as CachedEnumerable<T> ?? new CachedEnumerable<T>(enumerable);

		public static IEnumerable<T> AsCached<T>(this IEnumerator<T> enumerator) => new CachedEnumerable<T>(enumerator);
	}

	public enum CachingStrategy {

		SinglyLinkedList,
		ArrayList
	}
}
