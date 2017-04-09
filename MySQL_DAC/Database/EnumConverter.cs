using System;
using System.Globalization;
using System.Windows.Data;

namespace MySQL_DAC.Database {
	public class EnumConverter: IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return ((Permissions)value).ToString();
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
