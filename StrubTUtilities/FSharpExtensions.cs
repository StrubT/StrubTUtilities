﻿#if ENABLE_FSHARP_CORE

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace StrubT {

	public static class FSharpExtensions {

		// OPTION //

		public static bool HasValue<T>(this FSharpOption<T> option) => FSharpOption<T>.get_IsSome(option);

		public static T? AsNullable<T>(this FSharpOption<T> option) where T : struct => option.HasValue() ? option.Value : default(T?);

		public static T GetValueOrDefault<T>(this FSharpOption<T> option, T @default = default(T)) => option.HasValue() ? option.Value : @default;

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

		static object GetChoiceHelper<T>(T choice, int n) {

			object item;
			return TryGetChoiceHelper(choice, n, out item) ? item : null;
		}

		public static bool TryGetChoice<T1, T2>(this FSharpChoice<T1, T2> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2>(this FSharpChoice<T1, T2> choice, int n) => GetChoiceHelper(choice, n);

		public static T1 GetChoice1<T1, T2>(this FSharpChoice<T1, T2> choice) => choice is FSharpChoice<T1, T2>.Choice1Of2 ? ((FSharpChoice<T1, T2>.Choice1Of2)choice).Item : default(T1);

		public static T2 GetChoice2<T1, T2>(this FSharpChoice<T1, T2> choice) => choice is FSharpChoice<T1, T2>.Choice2Of2 ? ((FSharpChoice<T1, T2>.Choice2Of2)choice).Item : default(T2);

		#region other choices

		public static bool TryGetChoice<T1, T2, T3>(this FSharpChoice<T1, T2, T3> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2, T3>(this FSharpChoice<T1, T2, T3> choice, int n) => GetChoiceHelper(choice, n);

		public static T1 GetChoice1<T1, T2, T3>(this FSharpChoice<T1, T2, T3> choice) => choice is FSharpChoice<T1, T2, T3>.Choice1Of3 ? ((FSharpChoice<T1, T2, T3>.Choice1Of3)choice).Item : default(T1);

		public static T2 GetChoice2<T1, T2, T3>(this FSharpChoice<T1, T2, T3> choice) => choice is FSharpChoice<T1, T2, T3>.Choice2Of3 ? ((FSharpChoice<T1, T2, T3>.Choice2Of3)choice).Item : default(T2);

		public static T3 GetChoice3<T1, T2, T3>(this FSharpChoice<T1, T2, T3> choice) => choice is FSharpChoice<T1, T2, T3>.Choice3Of3 ? ((FSharpChoice<T1, T2, T3>.Choice3Of3)choice).Item : default(T3);

		public static bool TryGetChoice<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice, int n) => GetChoiceHelper(choice, n);

		public static T1 GetChoice1<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice) => choice is FSharpChoice<T1, T2, T3, T4>.Choice1Of4 ? ((FSharpChoice<T1, T2, T3, T4>.Choice1Of4)choice).Item : default(T1);

		public static T2 GetChoice2<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice) => choice is FSharpChoice<T1, T2, T3, T4>.Choice2Of4 ? ((FSharpChoice<T1, T2, T3, T4>.Choice2Of4)choice).Item : default(T2);

		public static T3 GetChoice3<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice) => choice is FSharpChoice<T1, T2, T3, T4>.Choice3Of4 ? ((FSharpChoice<T1, T2, T3, T4>.Choice3Of4)choice).Item : default(T3);

		public static T4 GetChoice4<T1, T2, T3, T4>(this FSharpChoice<T1, T2, T3, T4> choice) => choice is FSharpChoice<T1, T2, T3, T4>.Choice4Of4 ? ((FSharpChoice<T1, T2, T3, T4>.Choice4Of4)choice).Item : default(T4);

		public static bool TryGetChoice<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice, int n) => GetChoiceHelper(choice, n);

		public static T1 GetChoice1<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5>.Choice1Of5 ? ((FSharpChoice<T1, T2, T3, T4, T5>.Choice1Of5)choice).Item : default(T1);

		public static T2 GetChoice2<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5>.Choice2Of5 ? ((FSharpChoice<T1, T2, T3, T4, T5>.Choice2Of5)choice).Item : default(T2);

		public static T3 GetChoice3<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5>.Choice3Of5 ? ((FSharpChoice<T1, T2, T3, T4, T5>.Choice3Of5)choice).Item : default(T3);

		public static T4 GetChoice4<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5>.Choice4Of5 ? ((FSharpChoice<T1, T2, T3, T4, T5>.Choice4Of5)choice).Item : default(T4);

		public static T5 GetChoice5<T1, T2, T3, T4, T5>(this FSharpChoice<T1, T2, T3, T4, T5> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5>.Choice5Of5 ? ((FSharpChoice<T1, T2, T3, T4, T5>.Choice5Of5)choice).Item : default(T5);

		public static bool TryGetChoice<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice, int n) => GetChoiceHelper(choice, n);

		public static T1 GetChoice1<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice1Of6 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice1Of6)choice).Item : default(T1);

		public static T2 GetChoice2<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice2Of6 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice2Of6)choice).Item : default(T2);

		public static T3 GetChoice3<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice3Of6 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice3Of6)choice).Item : default(T3);

		public static T4 GetChoice4<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice4Of6 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice4Of6)choice).Item : default(T4);

		public static T5 GetChoice5<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice5Of6 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice5Of6)choice).Item : default(T5);

		public static T6 GetChoice6<T1, T2, T3, T4, T5, T6>(this FSharpChoice<T1, T2, T3, T4, T5, T6> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice6Of6 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6>.Choice6Of6)choice).Item : default(T6);

		public static bool TryGetChoice<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice, int n, out object item) => TryGetChoiceHelper(choice, n, out item);

		public static object GetChoice<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice, int n) => GetChoiceHelper(choice, n);

		public static T1 GetChoice1<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice1Of7 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice1Of7)choice).Item : default(T1);

		public static T2 GetChoice2<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice2Of7 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice2Of7)choice).Item : default(T2);

		public static T3 GetChoice3<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice3Of7 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice3Of7)choice).Item : default(T3);

		public static T4 GetChoice4<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice4Of7 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice4Of7)choice).Item : default(T4);

		public static T5 GetChoice5<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice5Of7 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice5Of7)choice).Item : default(T5);

		public static T6 GetChoice6<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice6Of7 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice6Of7)choice).Item : default(T6);

		public static T7 GetChoice7<T1, T2, T3, T4, T5, T6, T7>(this FSharpChoice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice7Of7 ? ((FSharpChoice<T1, T2, T3, T4, T5, T6, T7>.Choice7Of7)choice).Item : default(T7);
		#endregion

		// COLLECTIONS //

		public static IReadOnlyList<T> AsReadOnly<T>(this FSharpList<T> list) => new ListProxy<T>(list);

		public static ISet<T> AsReadOnlySet<T>(this FSharpSet<T> set, bool allowExpensive = false) => new SetProxy<T>(set, allowExpensive);

		#region collection proxies

		class ListProxy<T> : IReadOnlyList<T> {

			FSharpList<T> List { get; }

			public ListProxy(FSharpList<T> list) { List = list; }

			T IReadOnlyList<T>.this[int index] => List[index];

			int IReadOnlyCollection<T>.Count => List.Length;

			IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)List).GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)List).GetEnumerator();

			public override int GetHashCode() => List.GetHashCode();

			public override bool Equals(object obj) => List.Equals(obj);

			public override string ToString() => List.ToString();
		}

		class SetProxy<T> : ISet<T> {

			FSharpSet<T> Set { get; }

			bool AllowExpensive { get; }

			public SetProxy(FSharpSet<T> set, bool allowExpensive = false) { Set = set; AllowExpensive = allowExpensive; }

			FSharpSet<T> CreateFSharpSet(IEnumerable<T> enumerable) {

				if (enumerable is FSharpSet<T>) return (FSharpSet<T>)enumerable;
				if (AllowExpensive) return new FSharpSet<T>(enumerable);
				throw new InvalidOperationException("Expensive operations have been disabled.");
			}

			ISet<T> CreateISet(IEnumerable<T> enumerable) {

				if (enumerable is ISet<T> && !(enumerable is SetProxy<T>)) return (ISet<T>)enumerable;
				if (AllowExpensive) return new HashSet<T>(enumerable);
				throw new InvalidOperationException("Expensive operations have been disabled.");
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)Set).GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)Set).GetEnumerator();

			void ICollection<T>.Add(T item) { throw new NotSupportedException("This collection is read-only."); }

			void ICollection<T>.Clear() { throw new NotSupportedException("This collection is read-only."); }

			bool ICollection<T>.Contains(T item) => Set.Contains(item);

			void ICollection<T>.CopyTo(T[] array, int arrayIndex) { throw new NotSupportedException("This collection is read-only."); }

			int ICollection<T>.Count => Set.Count;

			bool ICollection<T>.IsReadOnly => true;

			bool ICollection<T>.Remove(T item) { throw new NotSupportedException("This collection is read-only."); }

			bool ISet<T>.Add(T item) { throw new NotSupportedException("This collection is read-only."); }

			void ISet<T>.ExceptWith(IEnumerable<T> other) { throw new NotSupportedException("This collection is read-only."); }

			void ISet<T>.IntersectWith(IEnumerable<T> other) { throw new NotSupportedException("This collection is read-only."); }

			bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other) => Set.IsProperSubsetOf(CreateFSharpSet(other));

			bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other) => Set.IsProperSupersetOf(CreateFSharpSet(other));

			bool ISet<T>.IsSubsetOf(IEnumerable<T> other) => Set.IsSubsetOf(CreateFSharpSet(other));

			bool ISet<T>.IsSupersetOf(IEnumerable<T> other) => Set.IsSupersetOf(CreateFSharpSet(other));

			bool ISet<T>.Overlaps(IEnumerable<T> other) => CreateISet(other).Overlaps(Set);

			bool ISet<T>.SetEquals(IEnumerable<T> other) => CreateISet(other).SetEquals(Set);

			void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) { throw new NotSupportedException("This collection is read-only."); }

			void ISet<T>.UnionWith(IEnumerable<T> other) { throw new NotSupportedException("This collection is read-only."); }

			public override int GetHashCode() => Set.GetHashCode();

			public override bool Equals(object obj) => Set.Equals(obj);

			public override string ToString() => Set.ToString();
		}
		#endregion
	}
}
#endif