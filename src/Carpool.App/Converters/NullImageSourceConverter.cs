using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Carpool.App.Converters
{
	public class NullImageSourceConverter : IValueConverter
	{
		/// <summary>
		/// Convert string or null value to blank ImageSource to avoid Binding errors
		/// </summary>
		/// <param name="value">String value or NULL</param>
		/// <returns>Proper ImageSource or the Unset one</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return DependencyProperty.UnsetValue;
			else if (value is string)
			{
				string? path = value as string;
				if(path != null && path != string.Empty)
				{
					var img = new ImageSourceConverter().ConvertFromString(path);
					if (img != null)
						return img;
				}
				else
				{
					return DependencyProperty.UnsetValue;
				}
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
