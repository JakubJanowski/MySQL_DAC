using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MySQL_DAC.Database;

namespace MySQL_DAC.Views {
	public partial class DatabaseView: UserControl {
		private MainView mainView;
		private DataSet tableContentDataSet = new DataSet();
		private string tableName;
		private DataTable dummyDataTable;
		private DataRow importedRow;


		public DatabaseView() {
			InitializeComponent();
			tableContentDataGrid.CanUserAddRows = false;
		}

		public DatabaseView(MainView mainView): this() {
			this.mainView = mainView;
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
			refreshButton.IsEnabled = true;
			FillColumnNames();

			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.Add)) {
				addRowStackPanel.Visibility = Visibility.Visible;
				addRowButton.IsEnabled = false;
			}
			else {
				addRowStackPanel.Visibility = Visibility.Collapsed;
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.Delete))
				tableContentDataGrid.CanUserDeleteRows = true;
			else
				tableContentDataGrid.CanUserDeleteRows = false;
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.Edit))
				tableContentDataGrid.IsReadOnly = false;
			else
				tableContentDataGrid.IsReadOnly = true;
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.View)) {
				searchGrid.Visibility = Visibility.Visible;
				tablesGrid.Visibility = Visibility.Visible;
				tableContentDataGrid.Visibility = Visibility.Visible;
				addRowDataGrid.HeadersVisibility = DataGridHeadersVisibility.Row;
			}
			else {
				searchGrid.Visibility = Visibility.Collapsed;
				if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.Add)) {
					tablesGrid.Visibility = Visibility.Visible;
					tableContentDataGrid.Visibility = Visibility.Collapsed;
					addRowDataGrid.HeadersVisibility = DataGridHeadersVisibility.All;
				}
				else
					tablesGrid.Visibility = Visibility.Collapsed;
			}

			applyButton.IsEnabled = false;
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.Add) ||
				mainView.thisUserPermissions[tableName].HasFlag(Permissions.Delete) ||
				mainView.thisUserPermissions[tableName].HasFlag(Permissions.Edit))
				applyButton.IsEnabled = true;

			try {
				tableContentDataGrid.ItemsSource = tableContentDataSet.Tables[tableName].DefaultView;
			} catch (NullReferenceException ex) {
				MessageBox.Show(ex.Message);
			}

			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.Add)) {
				dummyDataTable = tableContentDataSet.Tables[tableName].Clone();
				dummyDataTable.Clear();
				addRowDataGrid.ItemsSource = dummyDataTable.DefaultView;
			}
		}

		private void setAddRowGridColumnWidth() {
			for (int i = 0; i < tableContentDataGrid.Columns.Count && i < addRowDataGrid.Columns.Count; i++)
				addRowDataGrid.Columns[i].Width = tableContentDataGrid.Columns[i].ActualWidth;
		}

		private void columnChosen(object sender, SelectionChangedEventArgs e) {
			if (columnNamesComboBox.SelectedItem == null)
				columnNamesComboBox.SelectedIndex = 0;
			FilterRows(searchBox.Text, columnNamesComboBox.SelectedItem.ToString());
		}

		private void FillColumnNames() {
			List<string> columnNamesList = new List<string>(tableContentDataSet.Tables[tableName].Columns.Count + 1);
			columnNamesList.Add("Any");
			foreach (DataColumn column in tableContentDataSet.Tables[tableName].Columns)
				columnNamesList.Add(column.ColumnName);
			columnNamesComboBox.ItemsSource = columnNamesList;
			columnNamesComboBox.SelectedIndex = 0;
		}

		private void refreshButton_Click(object sender, RoutedEventArgs e) {
			RefreshTableContents();
		}

		private void addRowDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e) {
			if (e.EditAction == DataGridEditAction.Commit) {
				Action action = delegate {
					if (((DataRowView)e.Row.Item).Row.RowState == DataRowState.Added) {
						addRowDataGrid.CanUserAddRows = false;
						importedRow = ((DataRowView)e.Row.Item).Row;
						addRowButton.IsEnabled = true;
					}
				};
				Dispatcher.BeginInvoke(action, System.Windows.Threading.DispatcherPriority.Background);
			}
		}

		private void applyButton_Click(object sender, RoutedEventArgs e) {
			if (tableContentDataSet.GetChanges() == null)
				return;

			Logger.WriteEntry($"{mainView.username} modified table {tableName} with {tableContentDataSet.GetChanges().Tables[tableName].Present()}");
			if (!DatabaseManager.SetTableContents(tableName, ref tableContentDataSet) && !mainView.thisUserPermissions[tableName].HasFlag(Permissions.View)) {
				tableContentDataSet.Tables[tableName].Clear();
			}
		}

		private void addRowButton_Click(object sender, RoutedEventArgs e) {
			try {
				tableContentDataSet.Tables[tableName].ImportRow(importedRow);
				dummyDataTable.Clear();
				addRowButton.IsEnabled = false;
				addRowDataGrid.CanUserAddRows = true;
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {
			FilterRows(searchBox.Text, columnNamesComboBox.SelectedItem.ToString());
		}

		private void FilterRows(string searchPhrase, string columnName) {
			DataView filteredView = tableContentDataSet.Tables[tableName].DefaultView;
			filteredView.RowFilter = "";
			if (!string.IsNullOrEmpty(searchPhrase)) {
				if (tableContentDataSet.Tables[tableName].Columns[0].DataType.Equals(typeof(Int32)))
					filteredView.RowFilter = $"[{tableContentDataSet.Tables[tableName].Columns[0].ColumnName}] = -3356152374523423515645680897654323452345 ";
				else if (tableContentDataSet.Tables[tableName].Columns[0].DataType.Equals(typeof(String)))
					filteredView.RowFilter = $"[{tableContentDataSet.Tables[tableName].Columns[0].ColumnName}] = 'skldASDccdnqw123##$dnkldsn423kwsf@$$@[[]iaewndwakendsjl' ";
				else if (tableContentDataSet.Tables[tableName].Columns[0].DataType.Equals(typeof(DateTime)))
					filteredView.RowFilter = $"[{tableContentDataSet.Tables[tableName].Columns[0].ColumnName}] = '{new DateTime(31243512365789051).ToString()}' ";

				if (columnName.Equals("Any")) {
					StringBuilder filterExpression = new StringBuilder();

					foreach (DataColumn column in filteredView.Table.Columns) {
						if (column.DataType.Equals(typeof(Int32))) {
							Int32 i;
							if (Int32.TryParse(searchPhrase, out i))
								filterExpression.AppendFormat($"[{column.ColumnName}] = {i} OR ");
						}
						else if (column.DataType.Equals(typeof(String)))
							filterExpression.AppendFormat($"[{column.ColumnName}] Like '%{searchPhrase}%' OR ");
						else if (column.DataType.Equals(typeof(DateTime))) {
							try {
								DateTime date = DateTime.Parse(searchPhrase);
								filterExpression.AppendFormat($"[{column.ColumnName}] = '{date}' OR ");
							} catch (Exception ex) { }
						}
					}
					if (filterExpression.Length > 3) {
						filterExpression.Remove(filterExpression.Length - 3, 3);
						filteredView.RowFilter = filterExpression.ToString();
					}
				}
				else {
					if (tableContentDataSet.Tables[tableName].Columns[columnName].DataType.Equals(typeof(Int32))) {
						Int32 i;
						if (Int32.TryParse(searchPhrase, out i))
							filteredView.RowFilter = $"[{columnName}] = {i} ";
					}
					else if (tableContentDataSet.Tables[tableName].Columns[columnName].DataType.Equals(typeof(String)))
						filteredView.RowFilter = $"[{columnName}] Like '%{searchPhrase}%' ";
					else if (tableContentDataSet.Tables[tableName].Columns[columnName].DataType.Equals(typeof(DateTime))) {
						try {
							DateTime date = DateTime.Parse(searchPhrase);
							filteredView.RowFilter = $"[{columnName}] = '{date}' ";
						} catch (Exception ex) { }
					}
				}
			}
			tableContentDataGrid.ItemsSource = filteredView;
		}

		private void tableContentDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
			if (e.PropertyType == typeof(System.DateTime))
				(e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy HH:mm:ss";
		}

		private void tableContentDataGrid_LayoutUpdated(object sender, EventArgs e) {
			setAddRowGridColumnWidth();
		}
	}
}
