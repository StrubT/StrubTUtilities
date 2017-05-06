using System;
using System.Collections.Generic;
using System.Linq;

namespace StrubT {

	public interface IReadOnlyTable<out T> {

		string Name { get; }

		IReadOnlyList<IReadOnlyColumn<T>> Columns { get; }

		IReadOnlyList<IReadOnlyRow<T>> Rows { get; }
	}

	public interface IReadOnlyColumn<out T> {

		int Index { get; }

		string Name { get; }

		IReadOnlyList<T> Values { get; }
	}

	public interface IReadOnlyRow<out T> {

		int Index { get; }

		IReadOnlyList<IReadOnlyColumn<T>> Columns { get; }

		IReadOnlyList<T> Values { get; }
	}

	public class Table<T> : IReadOnlyTable<T> {

		readonly List<Column> _columns = new List<Column>();
		readonly List<Row> _rows = new List<Row>();

		public string Name { get; }

		public IReadOnlyList<IReadOnlyColumn<T>> Columns => _columns;

		public IReadOnlyList<IReadOnlyRow<T>> Rows => _rows;

		public Table(string name) {

			Name = name;
		}

		public void AddColumn(string name) => AddColumns(name);

		public void AddColumns(params string[] names) => AddColumns((IReadOnlyList<string>)names);

		public void AddColumns(IReadOnlyList<string> names) { _columns.AddRange(names.Select(n => new Column(this, n))); CheckValidity(); }

		public void AddRow(params T[] values) => AddRows(values);

		public void AddRow(IReadOnlyList<T> values) => AddRows(values);

		public void AddRows(T[,] values) => AddRows(Enumerable.Range(0, values.GetLength(0)).Select(i => Enumerable.Range(0, values.GetLength(1)).Select(j => values[i, j]).ToList()));

		public void AddRows(params T[][] values) => AddRows((IReadOnlyList<T>[])values);

		public void AddRows(params IReadOnlyList<T>[] values) => AddRows((IEnumerable<IReadOnlyList<T>>)values);

		public void AddRows(IEnumerable<IReadOnlyList<T>> values) { _rows.AddRange(values.Select(v => new Row(this, v))); CheckValidity(); }

		void CheckValidity() {

			foreach (var row in Rows)
				if (Columns.Count != row.Values.Count)
					throw new IndexOutOfRangeException();
		}

		public override string ToString() => $"{nameof(Table<T>)}<{typeof(T).Name}>[{_rows.Count}x{_columns.Count}] '{Name}' ({string.Join(", ", _columns.Select(c => c.Name))})";

		class Column : IReadOnlyColumn<T> {

			Table<T> Table { get; }

			public string Name { get; }

			public int Index => Table._columns.IndexOf(this);

			public Column(Table<T> table, string name) {

				Table = table;
				Name = name;
			}

			public IReadOnlyList<T> Values {
				get {
					var index = Index;
					return Table.Rows.Select(r => r.Values[index]).ToList();
				}
			}

			public override string ToString() => Name;
		}

		class Row : IReadOnlyRow<T> {

			Table<T> Table { get; }

			public int Index => Table._rows.IndexOf(this);

			public IReadOnlyList<IReadOnlyColumn<T>> Columns => Table.Columns;

			public IReadOnlyList<T> Values { get; }

			public Row(Table<T> table, params T[] values) : this(table, values.ToList()) { }

			public Row(Table<T> table, IEnumerable<T> values) : this(table, values is List<T> ? (List<T>)values : values.ToList()) { }

			Row(Table<T> table, IReadOnlyList<T> values) {

				Table = table;
				Values = values;
			}

			public override string ToString() => string.Join(", ", Values);
		}
	}
}
