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
	public class RideListViewModel: ViewModelBase, IRideListViewModel
	{
		private RideFacade _rideFacade { get; }
		private UserFacade _userFacade { get; }
		private CarFacade _carFacade { get; }

		private readonly IMediator _mediator;

		public RideListViewModel(
			RideFacade rideFacade,
			UserFacade userFacade,
			CarFacade carFacade,
			IMediator mediator,
			CreateRideDetailViewModel createRideDetailViewModel, 
			RideDetailViewModel rideDetailViewModel)
		{
			_rideFacade = rideFacade;
			_userFacade = userFacade;
			_carFacade = carFacade;
			_mediator = mediator;
			CreateRideDetailViewModel = createRideDetailViewModel;
			RideDetailViewModel = rideDetailViewModel;

			_mediator.Register<SelectedMessage<UserDetailModel>>(OnUserSelected);
			_mediator.Register<UpdateRideMessage<RideDetailModel>>(OnRideUpdated);
			_mediator.Register<LogInMessage<UserDetailModel>>(LogIn);

			User = UserDetailModel.Empty;

			AddNewRideCommand = new RelayCommand(AddNewRide);
			DeleteRideCommand = new AsyncRelayCommand(DeleteRide);
			OwnRideClickedCommand = new RelayCommand<RideListModel>(OwnRideClicked);
		}

		/// <summary>
		/// Collection of logged-in user own rides
		/// </summary>
		public ObservableCollection<RideListModel> Rides { get; } = new();
		/// <summary>
		/// Driver in the rides in the collection
		/// </summary>
		public UserDetailModel User { get; set; } = UserDetailModel.Empty;
		/// <summary>
		/// View model for the ride creation window
		/// </summary>
		public CreateRideDetailViewModel CreateRideDetailViewModel { get; set; }
		/// <summary>
		/// View model for the ride detail info window
		/// </summary>
		public RideDetailViewModel RideDetailViewModel { get; set; }
		/// <summary>
		/// Selected ride list model in the collection
		/// </summary>
		public RideListModel? SelectedRide { get; set; }
		/// <summary>
		/// Command for new ride addition
		/// </summary>
		public ICommand AddNewRideCommand { get; }
		/// <summary>
		/// Command for selected ride deletion
		/// </summary>
		public ICommand DeleteRideCommand { get; }
		/// <summary>
		/// Command for double-clicked ride window open
		/// </summary>
		public ICommand OwnRideClickedCommand { get; set; }
		/// <summary>
		/// Window for the info about the selected ride
		/// </summary>
		private RideDetailWindow? RideDetailWindow { get; set; }


		/// <summary>
		/// Load rides into a collection
		/// </summary>
		/// <param name="id">Drive user id</param>
		/// <returns></returns>
		public async Task LoadRidesAsync(Guid id)
		{
			Rides.Clear();
			User = await _userFacade.GetAsync(id)?? UserDetailModel.Empty;
			var rides = (await _rideFacade.GetAsync()).Where(i => User.DriverRides.Contains(i));
			Rides.AddRange(rides);
		}

		/// <summary>
		/// Update current user
		/// </summary>
		/// <param name="message"></param>
		private async void OnUserSelected(SelectedMessage<UserDetailModel> message)
		{
			await SelectUser(message.Id);
		}

		/// <summary>
		/// Load data after User update
		/// </summary>
		/// <param name="id"></param>
		private async Task SelectUser(Guid? id)
		{
			if (id is not null)
			{
				Guid userId = (Guid)id;
				await LoadRidesAsync(userId);
			}
		}

		/// <summary>
		/// Update the ride list when a ride is updated
		/// </summary>
		/// <param name="message"></param>
		private async void OnRideUpdated(UpdateRideMessage<RideDetailModel> message)
		{
			await LoadRidesAsync(User.Id);
		}

		/// <summary>
		/// Update after a user logged in
		/// </summary>
		/// <param name="message">Current user detail model message</param>
		private async void LogIn(LogInMessage<UserDetailModel> message)
		{
			if (message != null)
			{
				Guid? userId = message.Id;
				if(userId != null)
					await LoadRidesAsync((Guid)userId);
			}
		}

		/// <summary>
		/// Add a new ride using a ride creating window
		/// </summary>
		private void AddNewRide()
		{
			var newWindow = new CreateRideDetailWindow();
			_mediator.Send(new AddRideDetailMessage<UserDetailModel>() { Model = User });
			newWindow.DataContext = CreateRideDetailViewModel;
			newWindow.Show();
		}

		/// <summary>
		/// Delete the selected ride
		/// </summary>
		/// <returns></returns>
		private async Task DeleteRide()
		{
			if (SelectedRide != null)
			{
				await _rideFacade.DeleteAsync(SelectedRide.Id);
				await LoadRidesAsync(User.Id);
			}
		}

		/// <summary>
		/// Show detail info on ride double click
		/// </summary>
		/// <param name="rideListModel">Message with a clicked ride</param>
		private async void OwnRideClicked(RideListModel? rideListModel)
		{
			if (rideListModel == null)
				return;

			var rideDetailModel = await _rideFacade.GetAsync(rideListModel.Id);
			if (rideDetailModel == null)
				return;

			RideDetailWindow = new RideDetailWindow();

			RideDetailWindow.DataContext = RideDetailViewModel;
			_mediator.Send(new OpenRideDetailMessage<RideDetailModel>() { Model = rideDetailModel });

			RideDetailWindow.Show();
		}
	}
}
