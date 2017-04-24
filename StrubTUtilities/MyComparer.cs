using System;
using System.Collections.Generic;

namespace StrubT {

	public class MyComparer<T> : Comparer<T> {

		readonly Func<T, T, int> comparer;

		public MyComparer(Func<T, T, int> comparer) { this.comparer = comparer; }

		public static MyComparer<T> Create<U>(Func<T, U> selector) where U : IComparable<U> {

			return new MyComparer<T>((x, y) => selector(x).CompareTo(selector(y)));
		}

		public override int Compare(T first, T second) {

			if (ReferenceEquals(second, null)) return -1;
			if (ReferenceEquals(first, second)) return 0;
			if (ReferenceEquals(first, null)) return 1;

			return comparer(first, second);
		}
	}

	public class MyEqualityComparer<T> : EqualityComparer<T> {

		readonly static Func<T, int> hashZero = _ => 0;
		readonly static MyEqualityComparer<T> _defaultWithoutHash = new MyEqualityComparer<T>((x, y) => x.Equals(y));

		public static MyEqualityComparer<T> DefaultWithoutHash { get { return _defaultWithoutHash; } }

		readonly Func<T, T, bool> _comparer;
		readonly Func<T, int> _hashFunction;

		public Func<T, T, bool> Comparer { get { return _comparer; } }

		public Func<T, int> HashFunction { get { return _hashFunction; } }

		public MyEqualityComparer(Func<T, T, bool> comparer) : this(comparer, hashZero) { }

		public MyEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hashFunction) {

			_comparer = comparer;
			_hashFunction = hashFunction;
		}

		public static MyEqualityComparer<T> Create<U>(Func<T, U> selector, bool hashFunction = true) {

			if (!hashFunction)
				return new MyEqualityComparer<T>((x, y) => selector(x).Equals(selector(y)));
			return new MyEqualityComparer<T>((x, y) => selector(x).Equals(selector(y)), o => selector(o).GetHashCode());
		}

		public override int GetHashCode(T obj) {

			if (ReferenceEquals(obj, null))
				throw new ArgumentNullException("obj", "Argument cannot be null.");
			return HashFunction(obj);
		}

		public override bool Equals(T first, T second) {

			if (ReferenceEquals(first, second)) return true;
			if (ReferenceEquals(first, null) || ReferenceEquals(second, null)) return false;

			return Comparer(first, second);
		}
	}
}
