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

	class Table<T> : IReadOnlyTable<T> {

		readonly List<Column> _columns = new List<Column>();
		readonly List<Row> _rows = new List<Row>();
		readonly string _name;

		public string Name { get { return _name; } }

		public IReadOnlyList<IReadOnlyColumn<T>> Columns { get { return _columns; } }

		public IReadOnlyList<IReadOnlyRow<T>> Rows { get { return _rows; } }

		public Table(string name) {

			_name = name;
		}

		public void AddColumn(string name) { AddColumns(name); }

		public void AddColumns(params string[] names) { AddColumns((IReadOnlyList<string>)names); }

		public void AddColumns(IReadOnlyList<string> names) { _columns.AddRange(names.Select(n => new Column(this, n))); CheckValidity(); }

		public void AddRow(params T[] values) { AddRows(values); }

		public void AddRow(IReadOnlyList<T> values) { AddRows(values); }

		public void AddRows(T[,] values) { AddRows(Enumerable.Range(0, values.GetLength(0)).Select(i => Enumerable.Range(0, values.GetLength(1)).Select(j => values[i, j]).ToList())); }

		public void AddRows(params T[][] values) { AddRows((IReadOnlyList<T>[])values); }

		public void AddRows(params IReadOnlyList<T>[] values) { AddRows((IEnumerable<IReadOnlyList<T>>)values); }

		public void AddRows(IEnumerable<IReadOnlyList<T>> values) { _rows.AddRange(values.Select(v => new Row(this, v))); CheckValidity(); }

		void CheckValidity() {

			foreach (var row in Rows)
				if (Columns.Count != row.Values.Count)
					throw new IndexOutOfRangeException();
		}

		public override string ToString() { return string.Format("Table<{0}>[{1}x{2}] '{Name}' ({3})", typeof(T).Name, _rows.Count, _columns.Count, string.Join(", ", _columns.Select(c => c.Name))); }

		class Column : IReadOnlyColumn<T> {

			readonly Table<T> table;
			readonly string _name;

			public string Name { get { return _name; } }

			public int Index { get { return table._columns.IndexOf(this); } }

			public Column(Table<T> table, string name) {

				this.table = table;
				_name = name;
			}

			public IReadOnlyList<T> Values {
				get {
					var index = Index;
					return table.Rows.Select(r => r.Values[index]).ToList();
				}
			}

			public override string ToString() { return Name; }
		}

		class Row : IReadOnlyRow<T> {

			readonly Table<T> table;
			readonly IReadOnlyList<T> _values;

			public int Index { get { return table._rows.IndexOf(this); } }

			public IReadOnlyList<IReadOnlyColumn<T>> Columns { get { return table.Columns; } }

			public IReadOnlyList<T> Values { get { return _values; } }

			public Row(Table<T> table, params T[] values) : this(table, values.ToList()) { }

			public Row(Table<T> table, IEnumerable<T> values) : this(table, values is List<T> ? (List<T>)values : values.ToList()) { }

			Row(Table<T> table, IReadOnlyList<T> values) {

				this.table = table;
				_values = values;
			}

			public override string ToString() { return string.Join(", ", Values); }
		}
	}
}
