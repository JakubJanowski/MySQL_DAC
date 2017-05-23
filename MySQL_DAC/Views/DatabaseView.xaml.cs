using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MySQL_DAC.Database;

namespace MySQL_DAC.Views {
	public partial class DatabaseView: UserControl {
		private MainView mainView;
		private DataSet tableContentDataSet = new DataSet();
		private bool pendingEdit = false;
		private string tableName;

		public DatabaseView() {
			InitializeComponent();
			tableContentDataGrid.CanUserAddRows = false;
		}

		public DatabaseView(MainView mainView) {
			InitializeComponent();
			this.mainView = mainView;
			tableContentDataGrid.CanUserAddRows = false;
		}
		internal void SetMainView(MainView mainView) {
			this.mainView = mainView;
		}

		private void RefreshTableContents() {
			if (tableNamesComboBox.SelectedItem != null) {
				tableName = tableNamesComboBox.SelectedItem.ToString();
				if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.View))
					DatabaseManager.GetTableContents(tableName, ref tableContentDataSet);
				else
					DatabaseManager.GetTableContents(tableName, ref tableContentDataSet, false);
			}
		}

		private void tableChosen(object sender, SelectionChangedEventArgs e) {
			RefreshTableContents();

			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.Add))
				addRowDataGrid.Visibility = Visibility.Visible;
			else
				addRowDataGrid.Visibility = Visibility.Collapsed;
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.Delete))
				tableContentDataGrid.CanUserDeleteRows = true;
			else
				tableContentDataGrid.CanUserDeleteRows = false;
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.Edit))
				tableContentDataGrid.IsReadOnly = false;
			else
				tableContentDataGrid.IsReadOnly = true;

			try {
				tableContentDataGrid.ItemsSource = tableContentDataSet.Tables[tableName].DefaultView;
				} catch (NullReferenceException ex) {
				MessageBox.Show(ex.Message);
			}

			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.Add)) {
				DataTable dummyDataView = tableContentDataSet.Tables[tableName].Clone();
				dummyDataView.Clear();
				addRowDataGrid.ItemsSource = dummyDataView.DefaultView;
			}
		}

		private object Clone(object input) {
			object cloned;
			string inputVisualAsString = System.Windows.Markup.XamlWriter.Save(input);
			if (inputVisualAsString == null)
				return null;
			
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(inputVisualAsString.Length)) {
				using (System.IO.StreamWriter sw = new System.IO.StreamWriter(stream)) {
					sw.Write(inputVisualAsString);
					sw.Flush();
					stream.Seek(0, System.IO.SeekOrigin.Begin);
					
					cloned = System.Windows.Markup.XamlReader.Load(stream) as object;
				}
			}

			return cloned;
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

		private void refreshButton_Click(object sender, RoutedEventArgs e) {
			RefreshTableContents();
		}
	}
}
