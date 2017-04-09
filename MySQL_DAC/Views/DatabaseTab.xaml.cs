using System.Data;
using System.Windows.Controls;
using MySQL_DAC.Database;

namespace MySQL_DAC {
	public partial class DatabaseTab: UserControl {
		private DataSet tableContentDataSet = new DataSet();

		public DatabaseTab() {
			InitializeComponent();
		}

		private void tableChosen(object sender, SelectionChangedEventArgs e) {
			string tableName = ((ComboBox)sender).SelectedItem.ToString();
			DatabaseManager.GetTableContents(tableName, ref tableContentDataSet);
			tableContentDataGrid.ItemsSource = tableContentDataSet.Tables[tableName].DefaultView;
		}
	}
}
