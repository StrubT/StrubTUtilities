using System.Data;
using System.Linq;

namespace StrubT {

	static class DataExtensions {

		public static IReadOnlyTable<object> AsReadOnly(this DataTable dataTable) {

			var table = new Table<object>(dataTable.TableName);
			table.AddColumns(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList());
			table.AddRows(dataTable.Rows.Cast<DataRow>().Select(r => r.ItemArray));
			return table;
		}

		public static IReadOnlyTable<T> AsReadOnly<T>(this DataTable dataTable) {

			var table = new Table<T>(dataTable.TableName);
			table.AddColumns(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList());
			table.AddRows(dataTable.Rows.Cast<DataRow>().Select(r => dataTable.Columns.Cast<DataColumn>().Select(c => r.Field<T>(c)).ToList()));
			return table;
		}

#if NETSTANDARD2_0
		public static T Field<T>(this DataRow row, DataColumn column) => (T)row[column];
#endif
	}
}
