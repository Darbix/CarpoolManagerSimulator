using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Carpool.App.Commands;
using Carpool.App.Extensions;
using Carpool.App.Messages;
using Carpool.App.Services;
using Carpool.App.ViewModels.Interfaces;
using Carpool.App.Views;
using Carpool.BL.Facades;
using Carpool.BL.Models;

namespace Carpool.App.ViewModels
{

	public class UserListViewModel: ViewModelBase, IUserListViewModel
	{
		private readonly UserFacade _userFacade;
		private readonly IMediator _mediator;

		public UserListViewModel(
			UserFacade userFacade, 
			IMediator mediator, 
			CreateUserDetailViewModel createUserDetailViewModel
		) {
			_userFacade = userFacade;
			_mediator = mediator;
			CreateUserViewModel = createUserDetailViewModel;

			UserNewCommand = new RelayCommand(UserNew);
			UserSelectedCommand = new RelayCommand<UserListModel>(UserSelected);
			UserLogInCommand = new RelayCommand<UserListModel>(UserLogIn);
			UserDeleteCommand = new AsyncRelayCommand<UserListModel>(UserDeleted);
			RefreshCommand = new AsyncRelayCommand(Refresh);

			_mediator.Register<UpdateMessage<UserDetailModel>>(UserUpdated);
		}

		/// <summary>
		/// Collection of all users in the database
		/// </summary>
		public ObservableCollection<UserListModel> Users { get; } = new();
		/// <summary>
		/// Command to select a user
		/// </summary>
		public ICommand UserSelectedCommand { get;}
		/// <summary>
		/// Command to add a new user
		/// </summary>
		public ICommand UserNewCommand { get; }
		/// <summary>
		/// Command to delete a selected user
		/// </summary>
		public ICommand UserDeleteCommand { get; }
		/// <summary>
		/// Command to log in a selected user
		/// </summary>
		public ICommand UserLogInCommand { get; }
		/// <summary>
		/// Command to refresh the colection from the database data
		/// </summary>
		public ICommand RefreshCommand { get; set; }
		/// <summary>
		/// View model for adding a new user
		/// </summary>
		public CreateUserDetailViewModel CreateUserViewModel { get; }


		/// <summary>
		/// Load the list after some user was updated
		/// </summary>
		/// <param name="_"></param>
		private async void UserUpdated(UpdateMessage<UserDetailModel> _) => await LoadAsync();

		/// <summary>
		/// Load the database user objects to the collection
		/// </summary>
		/// <returns></returns>
		public async Task LoadAsync()
		{
			Users.Clear();
			var users = await _userFacade.GetAsync();
			Users.AddRange(users);
		}

		/// <summary>
		/// Refresh the state of the user list and load 
		/// </summary>
		/// <returns></returns>
		public async Task Refresh()
		{
			await LoadAsync();
		}

		/// <summary>
		/// Delete a user 
		/// </summary>
		/// <param name="userListModel"></param>
		/// <returns></returns>
		private async Task UserDeleted(UserListModel? userListModel)
		{
			if(userListModel == null) 
				return;

			await _userFacade.DeleteAsync(userListModel.Id);
			await LoadAsync();
			_mediator.Send(new DeleteMessage<UserListModel>());

		}

		/// <summary>
		/// Add a new user
		/// </summary>
		private void UserNew() {
			CreateUserDetailWindow newWindow = new CreateUserDetailWindow();
			newWindow.DataContext = CreateUserViewModel;
			newWindow.Show();
		}

		/// <summary>
		/// Send the message about the user selection changed
		/// </summary>
		/// <param name="userListModel"></param>
		private void UserSelected(UserListModel? userListModel)
		{
			if (userListModel is not null)
			{
				_mediator.Send(new SelectedMessage<UserDetailModel> { Id = userListModel.Id });
			}
		}

		/// <summary>
		/// Send the message about selected user log in
		/// </summary>
		/// <param name="userListModel"></param>
		private void UserLogIn(UserListModel? userListModel)
		{
			if (userListModel is not null)
			{
				_mediator.Send(new LogInMessage<UserDetailModel> { Id = userListModel.Id });
			}
		}
	}
	
}
