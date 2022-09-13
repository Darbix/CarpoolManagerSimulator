using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Carpool.BL.Models;

namespace Carpool.App.Converters
{
	public class IdToJoinedConverter : IMultiValueConverter
	{
		/// <summary>
		/// Convert Ride ID presence in user passenger list to word "joined" or and empty string
		/// </summary>
		/// <param name="values">First value is Guid Ride ID, Second value is UserDetailModel</param>
		/// <returns>String "joined" or and empty string</returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] != null)
			{
				Guid id;
				try
				{
					id = ((RideListModel)values[0]).Id;
				}
				catch 
				{
					return "";
				}

				if (values[1] != null)
					if (((UserDetailModel)values[1]).PassengerRides.Count > 0 &&
						((UserDetailModel)values[1]).PassengerRides.Where(i => i.RideId == id).FirstOrDefault() != null)
						return "joined";
			}
			return "";
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
