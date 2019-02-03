#if ENABLE_FSHARP_CORE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace StrubT {

	public static class FSharpExtensions {

		// OPTION //

		public static bool HasValue<T>(this FSharpOption<T> option) => FSharpOption<T>.get_IsSome(option);

		public static T? AsNullable<T>(this FSharpOption<T> option) where T : struct => option.HasValue() ? option.Value : default;

		public static T GetValueOrDefault<T>(this FSharpOption<T> option, T @default = default) => option.HasValue() ? option.Value : @default;

		public static FSharpOption<T> AsOption<T>(this T value) => value != null ? FSharpOption<T>.Some(value) : FSharpOption<T>.None;

		public static FSharpOption<T> AsOption<T>(this T? value) where T : struct => value != null ? FSharpOption<T>.Some(value.Value) : FSharpOption<T>.None;

		// CHOICE //

		static bool TryGetChoiceHelper<T>(T choice, int n, out object item) {

			var type = typeof(T);
			if (!type.FullName.StartsWith("Microsoft.FSharp.Core.FSharpChoice")) throw new ArgumentException("Not an F# choice type.", nameof(choice));

			var nofChoices = type.GenericTypeArguments.Length;
			if (n < 1 || n > nofChoices) throw new ArgumentOutOfRangeException("Not an acceptable choice.", nameof(n));

			var choiceType = type.GetNestedType($"Choice{n}Of{nofChoices}").MakeGenericType(type.GenericTypeArguments);
			if (choice.GetType() != choiceType) {
				item = null;
				return false;
			} else {
				item = choiceType.GetProperty("Item").GetValue(choice);
				return true;
			}
		}

		public static bool TryGetChoice<T1, T2>(this FSharpChoice<T1, T2> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2>(this FSharpChoice<T1, T2> choice, int n) => TryGetChoiceHelper(choice, n, out var item) ? item : null;

		public static T1 GetChoice1<T1, T2>(this FSharpChoice<T1, T2> choice) => choice is FSharpChoice<T1, T2>.Choice1Of2 c ? c.Item : default;

		public static T2 GetChoice2<T1, T2>(this FSharpChoice<T1, T2> choice) => choice is FSharpChoice<T1, T2>.Choice2Of2 c ? c.Item : default;

		#region more choices

		public static bool TryGetChoice<T1, T2, T3>(this FSharpChoice<T1, T2, T3> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2, T3>(this FSharpChoice<T1, T2, T3> choice, int n) => TryGetChoiceHelper(choice, n, out var item) ? item : null;

		public static T1 GetChoice1<T1, T2, T3>(this FSharpChoice<T1, T2, T3> choice) => choice is FSharpChoice<T1, T2, T3>.Choice1Of3 c ? c.Item : default;

		public static T2 GetChoice2<T1, T2, T3>(this FSharpChoice<T1, T2, T3> choice) => choice is FSharpChoice<T1, T2, T3>.Choice2Of3 c ? c.Item : default;

		public static T3 GetChoice3<T1, T2, T3>(this FSharpChoice<T1, T2, T3> choice) => choice is FSharpChoice<T1, T2, T3>.Choice3Of3 c ? c.Item : default;

		public static bool TryGetChoice<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice, int n) => TryGetChoiceHelper(choice, n, out var item) ? item : null;

		public static T1 GetChoice1<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice) => choice is FSharpChoice<T1, T2, T3, T4>.Choice1Of4 c ? c.Item : default;

		public static T2 GetChoice2<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice) => choice is FSharpChoice<T1, T2, T3, T4>.Choice2Of4 c ? c.Item : default;

		public static T3 GetChoice3<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice) => choice is FSharpChoice<T1, T2, T3, T4>.Choice3Of4 c ? c.Item : default;

		public static T4 GetChoice4<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice) => choice is FSharpChoice<T1, T2, T3, T4>.Choice4Of4 c ? c.Item : default;

		public static bool TryGetChoice<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice, int n) => TryGetChoiceHelper(choice, n, out var item) ? item : null;

		public static T1 GetChoice1<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5>.Choice1Of5 c ? c.Item : default;

		public static T2 GetChoice2<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5>.Choice2Of5 c ? c.Item : default;

		public static T3 GetChoice3<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5>.Choice3Of5 c ? c.Item : default;

		public static T4 GetChoice4<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5>.Choice4Of5 c ? c.Item : default;

		public static T5 GetChoice5<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5>.Choice5Of5 c ? c.Item : default;

		public static bool TryGetChoice<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice, int n) => TryGetChoiceHelper(choice, n, out var item) ? item : null;

		public static T1 GetChoice1<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice1Of6 c ? c.Item : default;

		public static T2 GetChoice2<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice2Of6 c ? c.Item : default;

		public static T3 GetChoice3<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice3Of6 c ? c.Item : default;

		public static T4 GetChoice4<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice4Of6 c ? c.Item : default;

		public static T5 GetChoice5<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice5Of6 c ? c.Item : default;

		public static T6 GetChoice6<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice6Of6 c ? c.Item : default;

		public static bool TryGetChoice<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice, int n) => TryGetChoiceHelper(choice, n, out var item) ? item : null;

		public static T1 GetChoice1<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice1Of7 c ? c.Item : default;

		public static T2 GetChoice2<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice2Of7 c ? c.Item : default;

		public static T3 GetChoice3<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice3Of7 c ? c.Item : default;

		public static T4 GetChoice4<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice4Of7 c ? c.Item : default;

		public static T5 GetChoice5<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice5Of7 c ? c.Item : default;

		public static T6 GetChoice6<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice6Of7 c ? c.Item : default;

		public static T7 GetChoice7<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice7Of7 c ? c.Item : default;
		#endregion

		// FSHARPSET / ISET //

		public static ISet<T> AsReadOnlySet<T>(this FSharpSet<T> set) => new SetProxy<T>(set);

		#region set proxies

		class SetProxy<T> : ISet<T> {

			readonly FSharpSet<T> FSharpSet;
			readonly Lazy<HashSet<T>> HashSet;

			public int Count => FSharpSet.Count;

			bool ICollection<T>.IsReadOnly => true;

			public SetProxy(FSharpSet<T> set) => (FSharpSet, HashSet) = (set, new Lazy<HashSet<T>>(() => new HashSet<T>(FSharpSet), LazyThreadSafetyMode.ExecutionAndPublication));

			public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)FSharpSet).GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public bool Contains(T item) => FSharpSet.Contains(item);

			void ICollection<T>.CopyTo(T[] array, int arrayIndex) => HashSet.Value.CopyTo(array, arrayIndex);

			bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other) => HashSet.Value.IsProperSubsetOf(other);

			bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other) => HashSet.Value.IsProperSupersetOf(other);

			bool ISet<T>.IsSubsetOf(IEnumerable<T> other) => HashSet.Value.IsSubsetOf(other);

			bool ISet<T>.IsSupersetOf(IEnumerable<T> other) => HashSet.Value.IsSupersetOf(other);

			bool ISet<T>.Overlaps(IEnumerable<T> other) => HashSet.Value.Overlaps(other);

			bool ISet<T>.SetEquals(IEnumerable<T> other) => HashSet.Value.SetEquals(other);

			public override int GetHashCode() => FSharpSet.GetHashCode();

			public override bool Equals(object obj) => FSharpSet.Equals(obj);

			public override string ToString() => FSharpSet.ToString();

			#region read-only proxies

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
#endif
