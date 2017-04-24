using System;
using System.Collections.Generic;

namespace StrubT {

	public class MyComparer<T> : Comparer<T> {

		public Func<T, T, int> Comparer { get; }

		public MyComparer(Func<T, T, int> comparer) => Comparer = comparer;

		public static MyComparer<T> Create<U>(Func<T, U> selector) where U : IComparable<U> => new MyComparer<T>((x, y) => selector(x).CompareTo(selector(y)));

		public override int Compare(T first, T second) {

			if (ReferenceEquals(second, null)) return -1;
			if (ReferenceEquals(first, second)) return 0;
			if (ReferenceEquals(first, null)) return 1;

			return Comparer(first, second);
		}
	}

	public class MyEqualityComparer<T> : EqualityComparer<T> {

		static Func<T, int> HashZero { get; } = _ => 0;

		public static MyEqualityComparer<T> DefaultWithoutHash { get; } = new MyEqualityComparer<T>((x, y) => x.Equals(y));

		public Func<T, T, bool> Comparer { get; }

		public Func<T, int> HashFunction { get; }

		public MyEqualityComparer(Func<T, T, bool> comparer) : this(comparer, HashZero) { }

		public MyEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hashFunction) {

			Comparer = comparer;
			HashFunction = hashFunction;
		}

		public static MyEqualityComparer<T> Create<U>(Func<T, U> selector, bool hashFunction = true) {

			if (!hashFunction)
				return new MyEqualityComparer<T>((x, y) => selector(x).Equals(selector(y)));
			return new MyEqualityComparer<T>((x, y) => selector(x).Equals(selector(y)), o => selector(o).GetHashCode());
		}

		public override int GetHashCode(T obj) => !ReferenceEquals(obj, null) ? HashFunction(obj) : throw new ArgumentNullException(nameof(obj), "Argument cannot be null.");

		public override bool Equals(T first, T second) {

			if (ReferenceEquals(first, second)) return true;
			if (ReferenceEquals(first, null) || ReferenceEquals(second, null)) return false;

			return Comparer(first, second);
		}
	}
}
