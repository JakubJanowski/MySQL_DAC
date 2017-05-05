using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Controls;
using MySQL_DAC.Database;

namespace MySQL_DAC.Views {
	public partial class DatabaseView: UserControl {
		private DataSet tableContentDataSet = new DataSet();
		private bool pendingEdit = false;
		private string tableName;

		public DatabaseView() {
			InitializeComponent();
		}

		private void tableChosen(object sender, SelectionChangedEventArgs e) {
			tableName = ((ComboBox)sender).SelectedItem.ToString();
			DatabaseManager.GetTableContents(tableName, ref tableContentDataSet);
			try {
				tableContentDataGrid.ItemsSource = tableContentDataSet.Tables[tableName].DefaultView;
			} catch (NullReferenceException ex) {

			}
		}

		private void tableContentDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
			if (e.EditAction == DataGridEditAction.Commit) {
				var column = e.Column as DataGridBoundColumn;
				if (column != null) {
					pendingEdit = true;
				}
				else {
					int a = 0;
				}
			}
		}

		private void tableContentDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
			if (pendingEdit) {
				pendingEdit = false;
				DatabaseManager.SetTableContents(tableName, ref tableContentDataSet);
			}
		}
	}
}
