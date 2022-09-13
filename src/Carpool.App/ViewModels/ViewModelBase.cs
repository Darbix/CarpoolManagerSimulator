using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Carpool.App.ViewModels.Interfaces;

namespace Carpool.App.ViewModels
{
	public abstract class ViewModelBase : IViewModel, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		protected ViewModelBase()
		{
			if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
			{
				// ReSharper disable once VirtualMemberCallInConstructor
				LoadInDesignMode();
			}
		}

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public virtual void LoadInDesignMode() { }
	}
}
