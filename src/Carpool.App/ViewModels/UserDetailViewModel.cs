using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Carpool.App.Commands;
using Carpool.App.Messages;
using Carpool.App.Services;
using Carpool.App.ViewModels.Interfaces;
using Carpool.App.Views;
using Carpool.BL.Facades;
using Carpool.BL.Models;

namespace Carpool.App.ViewModels
{
	public class UserDetailViewModel : ViewModelBase, IUserDetailViewModel
	{
		private readonly UserFacade _userFacade;
		private readonly IMediator _mediator;
		private UserDetailModel _model = UserDetailModel.Empty;

		public UserDetailViewModel(
			UserFacade userFacade,
			IMediator mediator,
			IRideListViewModel rideListViewModel,
			ICarListViewModel carListViewModel,
			IAllRidesListViewModel allRidesListViewModel,
			CreateUserDetailViewModel createUserDetailViewModel)
		{
			_userFacade = userFacade;
			_mediator = mediator;
			RideListViewModel = rideListViewModel;
			AllRidesListViewModel = allRidesListViewModel;
			CarListViewModel = carListViewModel;
			CreateUserDetailViewModel = createUserDetailViewModel;
			SelectedUserDetailModel = _model;

			LogOutCommand = new RelayCommand(LogOut);
			EditUserCommand = new RelayCommand(EditUser);

			_mediator.Register<SelectedMessage<UserDetailModel>>(OnUserSelected);
			_mediator.Register<UpdateMessage<UserDetailModel>>(UserUpdated);
		}

		/// <summary>
		/// Command for user log out handling
		/// </summary>
		public ICommand LogOutCommand { get; }
		/// <summary>
		/// Command for editing the user 
		/// </summary>
		public ICommand EditUserCommand { get; set; }
		/// <summary>
		/// View model for a UserControl - list of own rides
		/// </summary>
		public IRideListViewModel RideListViewModel { get; set; }
		/// <summary>
		/// View model for a UserControl - list of all available rides
		/// </summary>
		public IAllRidesListViewModel AllRidesListViewModel { get; set; }
		/// <summary>
		/// View model for a UserControl - list of all own cars
		/// </summary> 
		public ICarListViewModel CarListViewModel { get; set; }
		/// <summary>
		/// Logged in user detail model
		/// </summary>
		public UserDetailModel SelectedUserDetailModel { get; set; }
		/// <summary>
		/// View model for a user data editing window
		/// </summary>
		public CreateUserDetailViewModel CreateUserDetailViewModel { get; set; }


		/// <summary>
		/// Send a log out message
		/// </summary>
		void LogOut()
		{
		 	_mediator.Send(new LogOutMessage());
		}

		/// <summary>
		/// Update when a user info was changed
		/// </summary>
		/// <param name="message">Message with a changed user model</param>
		private async void UserUpdated(UpdateMessage<UserDetailModel> message)
		{
			if (message != null && message.Id != null && message.Model != null)
			{
				Guid id = (Guid)message.Id;
				SelectedUserDetailModel = message.Model;
				await LoadAsync(id);
			}
		}

		/// <summary>
		/// Select a user as a selected detail model
		/// </summary>
		/// <param name="message">Message with selected user list model</param>
		private void OnUserSelected(SelectedMessage<UserDetailModel> message)
		{
			SelectUser(message.Id);
		}

		/// <summary>
		/// Get a user detail model from the database using id
		/// </summary>
		/// <param name="id">User id</param>
		private void SelectUser(Guid? id)
		{
			if (id is not null)
			{
				Guid userId = (Guid)id;
				_ = LoadAsync(userId);
			}
		}

		/// <summary>
		/// User detail model as an empty or filled object
		/// </summary>
		public UserDetailModel? Model
		{
			get => _model;
			set
			{
				_model = value?? UserDetailModel.Empty;
				SelectedUserDetailModel = _model;
			}
		}

		/// <summary>
		/// Load a user from the database using ID
		/// </summary>
		/// <param name="id">Id of a user to be logged in</param>
		/// <returns></returns>
		public async Task LoadAsync(Guid id) => Model = await _userFacade.GetAsync(id) ?? UserDetailModel.Empty;

		/// <summary>
		/// Open a window for a user editing
		/// </summary>
		private void EditUser()
		{
			CreateUserDetailWindow newWindow = new CreateUserDetailWindow();
			CreateUserDetailViewModel.UserModel = SelectedUserDetailModel;
			newWindow.DataContext = CreateUserDetailViewModel;
			newWindow.Show();
		}
	}
}
