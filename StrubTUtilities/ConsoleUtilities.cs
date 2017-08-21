using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace StrubT {

	public static class ConsoleUtilities {

		#region kernel32

		const int ParentProcessID = -1;

		const int HideWindowCommand = 0;
		const int ShowWindowCommand = 5;

		const uint CloseMenuItemID = 0xF060;
		const uint MenuItemCommand = 1;

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		static extern bool AttachConsole(int dwProcessId);

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern bool SetWindowText(IntPtr hwnd, string lpString);

		[DllImport("user32.dll")]
		static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("user32.dll")]
		static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleIcon(IntPtr hIcon);

		/// <remarks>
		/// redirect console output to parent process;
		/// must be before any calls to Console.WriteLine()
		/// </remarks>
		public static void AttachConsoleWindow() {

			AttachConsole(ParentProcessID);
		}

		public static void ShowConsoleWindow() {

			var handle = GetConsoleWindow();
			if (handle == IntPtr.Zero)
				AllocConsole();
			else
				ShowWindow(handle, ShowWindowCommand);
		}

		public static void HideConsoleWindow() {

			var handle = GetConsoleWindow();
			ShowWindow(handle, HideWindowCommand);
		}

		public static void SetWindowText(string text) {

			var handle = GetConsoleWindow();
			SetWindowText(handle, text);
		}

		public static void DisableCloseButton() {

			var handle = GetConsoleWindow();
			var hmenu = GetSystemMenu(handle, false);
			EnableMenuItem(hmenu, CloseMenuItemID, MenuItemCommand);
		}

		public static void SetConsoleIcon(Icon icon) {

			SetConsoleIcon(icon.Handle);
		}
		#endregion

		#region Print[List|Table]

		public static void PrintList(string title, params string[] items) { PrintList(title, items.AsEnumerable()); }

		public static void PrintList(string title, IEnumerable<string> items) {

			Console.WriteLine("*** {0} ***", title);
			Console.WriteLine();

			foreach (var item in items)
				Console.WriteLine(item);

			Console.WriteLine();
			Console.WriteLine();
		}

		public static void PrintTable<T>(string title, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<T>> values) {

			var table = new Table<T>(title);
			table.AddColumns(headers);
			table.AddRows(values);
			PrintTable(table);
		}

		public static void PrintTable<T>(string title, IReadOnlyList<string> headers, IReadOnlyList<string> formats, IEnumerable<IReadOnlyList<T>> values) {

			var cells = values.Select(l => {
				var row = l.Select((v, i) => {
					if (v is IFormattable && formats != null && formats.Count >= i && !string.IsNullOrEmpty(formats[i]))
						return (object)((IFormattable)v).ToString(formats[i], null);
					return v;
				}).ToList();

				return row;
			});

			var table = new Table<object>(title);
			table.AddColumns(headers);
			table.AddRows(cells);
			PrintTable(table);
		}

		public static void PrintTable<T>(IReadOnlyTable<T> table) {

			var columnInfo = Enumerable.Range(0, table.Columns.Count).Select(c => {
				var isNumeric = true;
				//var maxPrecision = 0;
				var maxScale = 0;

				for (var r = 0; r < table.Rows.Count; r++) {
					var v = table.Rows[r].Values[c];

					sbyte? sb; byte? ub;
					short? ss; ushort? us;
					int? si; uint? ui;
					long? sl; ulong? ul;
					decimal? m; SqlDecimal sql;
					if ((sb = v as sbyte?).HasValue) { sql = new SqlDecimal(sb.Value); goto PrintTable_SqlDecimal; }
					if ((ub = v as byte?).HasValue) { sql = new SqlDecimal(ub.Value); goto PrintTable_SqlDecimal; }
					if ((ss = v as short?).HasValue) { sql = new SqlDecimal(ss.Value); goto PrintTable_SqlDecimal; }
					if ((us = v as ushort?).HasValue) { sql = new SqlDecimal(us.Value); goto PrintTable_SqlDecimal; }
					if ((si = v as int?).HasValue) { sql = new SqlDecimal(si.Value); goto PrintTable_SqlDecimal; }
					if ((ui = v as uint?).HasValue) { sql = new SqlDecimal(ui.Value); goto PrintTable_SqlDecimal; }
					if ((sl = v as long?).HasValue) { sql = new SqlDecimal(sl.Value); goto PrintTable_SqlDecimal; }
					if ((ul = v as ulong?).HasValue) { sql = new SqlDecimal(Convert.ToDecimal(ul.Value)); goto PrintTable_SqlDecimal; }
					if ((m = v as decimal?).HasValue) { sql = new SqlDecimal(m.Value); goto PrintTable_SqlDecimal; }
					goto PrintTable_NotSqlDecimal;

PrintTable_SqlDecimal:
//maxPrecision = Math.Max(maxPrecision, sql.Precision - sql.Scale);
					maxScale = Math.Max(maxScale, sql.Scale);
					continue;

PrintTable_NotSqlDecimal:
					if (v is float || v is double) {
						//var s = v.ToString();
						//var i = s.IndexOf(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
						//maxPrecision = Math.Max(maxPrecision, i < 0 ? s.Length : s.Length - i);
						maxScale = Math.Max(maxScale, 3); //constant floating-point precision
					} else
						isNumeric = false;
				}

				return new { IsNumeric = isNumeric, /*MaxPrecision = maxPrecision,*/ MaxScale = maxScale };
			}).ToList();

			var values = table.Rows.Select(r => r.Values.Select((v, i) => {
				if (columnInfo[i].IsNumeric && v != null)
					return Convert.ToDecimal(v).ToString(string.Format("#,##0.{0}", new string('0', columnInfo[i].MaxScale)));
				return v != null ? v.ToString() : null;
			}).ToList()).ToList();

			var columnWidths = Enumerable.Range(0, table.Columns.Count).Select(i => Math.Max(table.Columns[i].Name.Length, values.Max(r => r[i] != null ? r[i].Length : 0))).ToList();

			Console.WriteLine("*** {0} ***", table.Name);
			Console.WriteLine();

			Console.WriteLine(string.Join(" | ", table.Columns.Select((c, i) => string.Format(columnInfo[i].IsNumeric ? "{1}{0}" : "{0}{1}", c.Name, new string(' ', columnWidths[i] - c.Name.Length)))));
			Console.WriteLine(new string('-', columnWidths.Sum() + (columnWidths.Count - 1) * 3));

			foreach (var row in values)
				Console.WriteLine(string.Join(" | ", row.Select((v, i) => string.Format(string.Format("{{0,{0}}}", columnInfo[i].IsNumeric ? columnWidths[i] : -columnWidths[i]), v))));

			Console.WriteLine();
			Console.WriteLine();
		}
		#endregion
	}
}
