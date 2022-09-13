using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Carpool.App.ViewModels;

namespace Carpool.App.Views
{
	/// <summary>
	/// Interaction logic for CreateUserDetailWindow.xaml
	/// </summary>
	public partial class CreateUserDetailWindow : Window
	{
		public CreateUserDetailWindow()
		{
			InitializeComponent();
		}

		private void Age_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var textBox = (sender as TextBox);
			if (textBox == null)
				return;
			var fullText = e.Text;

			int val;
			e.Handled = !int.TryParse(fullText, out val);
		}

		private void PhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var textBox = (sender as TextBox);
			if (textBox == null)
				return;
			var fullText = e.Text;

			int val;
			e.Handled = !int.TryParse(fullText, out val);
		}
	}
}
