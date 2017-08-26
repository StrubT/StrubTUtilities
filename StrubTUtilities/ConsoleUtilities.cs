using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace StrubT {

	public static class ConsoleUtilities {

		#region kernel32

		// SOURCE: http://stackoverflow.com/a/24040827/1325979

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
		public static void AttachConsoleWindow() => AttachConsole(ParentProcessID);

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

		public static void SetIcon(Icon icon) => SetConsoleIcon(icon.Handle);
		#endregion

		#region Print[List|Table]

		public static void PrintList(string title, params string[] items) => PrintList(title, items.AsEnumerable());

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
					if (v is IFormattable formattable && formats != null && formats.Count >= i && !string.IsNullOrEmpty(formats[i]))
						return (object)formattable.ToString(formats[i], null);
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
				var maxScale = 0;

				for (var r = 0; r < table.Rows.Count; r++) {

					switch (table.Rows[r].Values[c]) {
						case sbyte sb: var sql = new SqlDecimal(sb); goto PrintTable_SqlDecimal;
						case byte ub: sql = new SqlDecimal(ub); goto PrintTable_SqlDecimal;
						case short ss: sql = new SqlDecimal(ss); goto PrintTable_SqlDecimal;
						case ushort us: sql = new SqlDecimal(us); goto PrintTable_SqlDecimal;
						case int si: sql = new SqlDecimal(si); goto PrintTable_SqlDecimal;
						case uint ui: sql = new SqlDecimal(ui); goto PrintTable_SqlDecimal;
						case long sl: sql = new SqlDecimal(sl); goto PrintTable_SqlDecimal;
						case ulong ul: sql = new SqlDecimal(Convert.ToDecimal(ul)); goto PrintTable_SqlDecimal;
						case decimal m:
							sql = new SqlDecimal(m);
PrintTable_SqlDecimal:
							maxScale = Math.Max(maxScale, sql.Scale);
							break;

						case float f:
						case double d:
							maxScale = Math.Max(maxScale, 3); //constant floating-point precision
							break;

						default:
							isNumeric = false;
							break;
					}
				}

				return (IsNumeric: isNumeric, MaxScale: maxScale);
			}).ToList();

			var values = table.Rows.Select(r => r.Values.Select((v, i) => {
				if (columnInfo[i].IsNumeric && v != null)
					return Convert.ToDecimal(v).ToString(string.Format("#,##0.{0}", new string('0', columnInfo[i].MaxScale)));
				return v?.ToString();
			}).ToList()).ToList();

			var columnWidths = Enumerable.Range(0, table.Columns.Count).Select(i => Math.Max(table.Columns[i].Name.Length, values.Max(r => r[i] != null ? r[i].Length : 0))).ToList();

			Console.WriteLine("*** {0} ***", table.Name);
			Console.WriteLine();

			Console.WriteLine(string.Join(" | ", table.Columns.Select((c, i) => string.Format(columnInfo[i].IsNumeric ? "{1}{0}" : "{0}{1}", c.Name, new string(' ', columnWidths[i] - c.Name.Length)))));
			Console.WriteLine(new string('-', columnWidths.Sum() + (columnWidths.Count - 1) * 3));

			foreach (var row in values)
				Console.WriteLine(string.Join(" | ", row.Select((v, i) => string.Format($"{{0,{(columnInfo[i].IsNumeric ? columnWidths[i] : -columnWidths[i])}}}", v))));

			Console.WriteLine();
			Console.WriteLine();
		}
		#endregion
	}
}
