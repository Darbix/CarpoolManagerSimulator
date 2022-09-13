using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.App.Services;
using Carpool.App.Messages;
using Carpool.App.ViewModels.Interfaces;
using Carpool.BL.Models;
using System.Windows.Navigation;
using Carpool.App.Views;
using System.Windows;

namespace Carpool.App.ViewModels
{
	public class MainViewModel: ViewModelBase
	{
		public ViewModelBase ContentViewModel { get; set; }

		public MainViewModel(
			IMediator mediator,
			IUserListViewModel userListViewModel,
			IUserDetailViewModel userDetailViewModel)
		{
			UserListViewModel = userListViewModel;
			UserDetailViewModel = userDetailViewModel;

			mediator.Register<SelectedMessage<UserDetailModel>>(OnUserSelected);
			mediator.Register<LogOutMessage>(OnLogOut);
			mediator.Register<LogInMessage<UserDetailModel>>(OnUserLogIn);

			SelectedUserDetailModel = new UserDetailModel(string.Empty, string.Empty, 0, 0);

			// Current content of the main window
			ContentViewModel = (ViewModelBase)UserListViewModel;
		}

		/// <summary>
		/// Context page view model for the all user list
		/// </summary>
		public IUserListViewModel UserListViewModel { get;}
		/// <summary>
		/// Context page view model for the logged in user
		/// </summary>
		public IUserDetailViewModel UserDetailViewModel { get; }
		/// <summary>
		/// Selected user detail model in the list of user list models
		/// </summary>
		public UserDetailModel SelectedUserDetailModel { get; set; }


		/// <summary>
		/// Change the window context after a user logs out
		/// </summary>
		/// <param name="message"></param>
		private void OnLogOut(LogOutMessage message)
		{
			ContentViewModel = (ViewModelBase)UserListViewModel;
		}
		
		/// <summary>
		/// Load a detail model of the selected user
		/// </summary>
		/// <param name="message">Received user model</param>
		private void OnUserSelected(SelectedMessage<UserDetailModel> message)
		{
			SelectUser(message.Id);
		}

		/// <summary>
		/// Change the window context after a user log in
		/// </summary>
		/// <param name="message">Received user model</param>
		private void OnUserLogIn(LogInMessage<UserDetailModel> message)
		{
			SelectUser(message.Id);

			ContentViewModel = (ViewModelBase)UserDetailViewModel;
		}

		/// <summary>
		/// Get the database detail model for the selected user list model 
		/// </summary>
		/// <param name="id"></param>
		private void SelectUser(Guid? id)
		{
			if(id is not null) 
			{
				SelectedUserDetailModel = new UserDetailModel(string.Empty, string.Empty, 0, 0) { Id = (Guid)id };
			}
		}
	}
}
