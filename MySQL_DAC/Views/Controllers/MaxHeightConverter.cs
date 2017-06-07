using System;
using System.Globalization;
using System.Windows.Data;

namespace MySQL_DAC.Views.Controllers {
	public class MaxHeightConverter: IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				throw new ArgumentException("MaxHeightConverter expects a height value", "values");

			return (double)value + 10;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
