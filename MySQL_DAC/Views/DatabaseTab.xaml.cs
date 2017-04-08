using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MySQL_DAC {
	/// <summary>
	/// Interaction logic for DatabaseTab.xaml
	/// </summary>

	//public ComboBox TableNamesComboBox {
	//	get { return tableNamesComboBox; }
	//	set { tableNamesComboBox = value  }
	//}

	public partial class DatabaseTab: UserControl {
		private DataSet tableContentDataSet = new DataSet();

		public DatabaseTab() {
			InitializeComponent();
		}

		private void tableChosen(object sender, SelectionChangedEventArgs e) {
			string tableName = ((ComboBox)sender).SelectedItem.ToString();
			DatabaseManager.GetTableContents(tableName, ref tableContentDataSet);
			//tableContentDataGrid.DataContext = tableContentDataSet; 
			tableContentDataGrid.ItemsSource = tableContentDataSet.Tables[tableName].DefaultView;
		}
	}
}
