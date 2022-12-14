using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Carpool.App.ViewModels;

namespace Carpool.App.Views
{
	public abstract class UserControlBase : UserControl
	{
		protected UserControlBase()
		{
			Loaded += OnLoaded;
		}

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is IListViewModel viewModel)
			{
				await viewModel.LoadAsync();
			}
		}
	}
}
