using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Carpool.App.Commands;
using Carpool.App.Extensions;
using Carpool.App.Messages;
using Carpool.App.Services;
using Carpool.App.Views;
using Carpool.BL.Facades;
using Carpool.BL.Models;

namespace Carpool.App.ViewModels
{
	public class RideDetailViewModel: INotifyPropertyChanged
	{
		private readonly IMediator _mediator;
		private readonly UserFacade _userFacade;
		private readonly RideFacade _rideFacade;
		private readonly CarFacade _carFacade;

		public RideDetailViewModel(
			IMediator mediator,
			UserFacade userFacade,
			RideFacade rideFacade,
			CarFacade carFacade,
			CreateRideDetailViewModel createRideDetailViewModel)
		{
			_mediator = mediator;
			_rideFacade = rideFacade;
			_userFacade = userFacade;
			_carFacade = carFacade;

			CreateRideDetailViewModel = createRideDetailViewModel;

			_mediator.Register<OpenRideDetailMessage<RideDetailModel>>(OpenRideDetail);
			_mediator.Register<UpdateRideMessage<RideDetailModel>>(UpdateRideDetailAsync);

			EditRideCommand = new RelayCommand(EditRide);
			RemovePassengerCommand = new AsyncRelayCommand<UserListModel>(RemovePassenger);
			AddPassengerCommand = new AsyncRelayCommand<UserListModel>(AddPassenger);
			RefreshCommand = new AsyncRelayCommand(Refresh);
		}

		/// <summary>
		/// Selected ride detail model
		/// </summary>
		public RideDetailModel SelectedRideModel { get; set; } = RideDetailModel.Empty;
		/// <summary>
		/// Calculated seat free capacity in the ride
		/// </summary>
		public int SeatCapacity { get; set; } = 0;


		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string? name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		private RideCarModel _rideCarModel = new RideCarModel(default, string.Empty, Common.CarTypes.None, string.Empty, default);
		/// <summary>
		/// Car model used in the ride
		/// </summary>
		public RideCarModel RideCarModel 
		{
			get => _rideCarModel;
			set
			{
				_rideCarModel = value;
				OnPropertyChanged(nameof(RideCarModel));
			}
		}
		/// <summary>
		/// Collection of ride passengers
		/// </summary>
		public ObservableCollection<UserListModel> Passengers { get; } = new();
		/// <summary>
		/// Collection of available user to add into the ride
		/// </summary>
		public ObservableCollection<UserListModel> Users { get; } = new();
		/// <summary>
		/// Command to edit the ride
		/// </summary>
		public ICommand EditRideCommand { get; }
		/// <summary>
		/// Command to remove a passenger from the ride
		/// </summary>
		public ICommand RemovePassengerCommand { get; }
		/// <summary>
		/// Command to add a passenger to the ride
		/// </summary>
		public ICommand AddPassengerCommand { get; }
		/// <summary>
		/// Command to refresh the passengers and available users
		/// </summary>
		public ICommand RefreshCommand { get; }
		/// <summary>
		/// View model of the window to edit the ride
		/// </summary>
		public CreateRideDetailViewModel CreateRideDetailViewModel { get; set; }


		/// <summary>
		/// Load data after this window is loaded
		/// </summary>
		/// <param name="message"></param>
		private async void OpenRideDetail(OpenRideDetailMessage<RideDetailModel> message)
		{
			if(message != null && message.Model != null)
			{
				SelectedRideModel = message.Model;
				if(SelectedRideModel.Car != null)
					RideCarModel = SelectedRideModel.Car;
				await LoadAsync();
			}
		}

		/// <summary>
		/// Load passengers and users into lists
		/// </summary>
		/// <returns></returns>
		private async Task LoadAsync()
		{
			Passengers.Clear();
			Users.Clear();

			var userList = await _userFacade.GetAsync();
			List<UserListModel> passengers = new();
			List<UserListModel> users = new();

			foreach (var u in userList)
			{
				var userDetail = await _userFacade.GetAsync(u.Id);

				if (userDetail != null) 
				{
					// If a user is a passenger in this ride
					if (SelectedRideModel.Passengers.Intersect(userDetail.PassengerRides).Any())
						passengers.Add(u);
					// Else it can be added as a passenger in a comboBox
					else if (userDetail.Id != SelectedRideModel.DriverId)
						users.Add(u);
				}
			}

			Passengers.AddRange(passengers);
			Users.AddRange(users);

			if (SelectedRideModel.Car != null)
			{
				RideCarModel = SelectedRideModel.Car;
				SeatCapacity = SelectedRideModel.Car.NumOfEmptySeats - SelectedRideModel.Passengers.Count;
			}
		}

		/// <summary>
		/// Refresh both lists of passengers and users
		/// </summary>
		/// <returns></returns>
		private async Task Refresh()
		{
			if(SelectedRideModel != null)
			{
				SelectedRideModel = await _rideFacade.GetAsync(SelectedRideModel.Id)?? RideDetailModel.Empty;
				await LoadAsync();
			}
		}

		/// <summary>
		/// Update ride info after it was edited/changed
		/// </summary>
		/// <param name="message">Received changed ride model</param>
		private void UpdateRideDetailAsync(UpdateRideMessage<RideDetailModel>? message)
		{
			if(message != null && message.Model != null)
				SelectedRideModel = message.Model;
			if (SelectedRideModel.Car != null)
			{
				RideCarModel = SelectedRideModel.Car;
				SeatCapacity = SelectedRideModel.Car.NumOfEmptySeats - SelectedRideModel.Passengers.Count;
			}
		}

		/// <summary>
		/// Open a window to edit the ride
		/// </summary>
		private void EditRide()
		{
			CreateRideDetailWindow newWindow = new CreateRideDetailWindow();
			_mediator.Send(new EditRideDetailMessage<RideDetailModel>() { Model = SelectedRideModel });
			newWindow.DataContext = CreateRideDetailViewModel;
			newWindow.Show();
		}

		/// <summary>
		/// Add a user to a ride
		/// </summary>
		/// <param name="userListModel">Selected passenger list model</param>
		/// <returns></returns>
		public async Task AddPassenger(UserListModel? userListModel)
		{
			if(userListModel != null && SelectedRideModel.Car != null)
			{
				// Cannot add seat-capacity-overflowing passengers
				if (SelectedRideModel.Passengers.Count + 1 > SelectedRideModel.Car.NumOfEmptySeats)
				{
					MessageBox.Show("Cannot add a passenger exceeding a seat capacity!", "Warning");
					return;
				}

				var userRide = new UserRideDetailModel(SelectedRideModel.Id, SelectedRideModel.Start,
					SelectedRideModel.End, SelectedRideModel.Beginning, SelectedRideModel.Duration);

				var passenger = await _userFacade.GetAsync(userListModel.Id);
				if (passenger != null)
				{
					// Check if there already is a colliding ride in passenger rides
					foreach (var r in passenger.PassengerRides)
					{
						// User cannot be already in a ride at the intersecting time
						if ((SelectedRideModel.Beginning > r.RideBeginning) &&
							(SelectedRideModel.Beginning < r.RideBeginning + r.RideDuration) ||
							(SelectedRideModel.Beginning + SelectedRideModel.Duration > r.RideBeginning) &&
							(SelectedRideModel.Beginning + SelectedRideModel.Duration < r.RideBeginning + r.RideDuration))
						{
							MessageBox.Show("User already is in a ride at this time and cannot be added!", "Warning");
							return;
						}
					}

					var carDetailModel = await _carFacade.GetAsync(SelectedRideModel.Car.Id);

					if (carDetailModel != null && carDetailModel.NumOfEmptySeats > 0)
					{
						passenger.PassengerRides.Add(userRide);

						await _userFacade.SaveAsync(passenger);

						var user = await _userFacade.GetAsync(passenger.Id);
						var dbUserRide = user?.PassengerRides.Where(i => i.RideId == SelectedRideModel.Id).FirstOrDefault();
						if (dbUserRide == null)
							return;

						SelectedRideModel.Passengers.Add(dbUserRide);

						SelectedRideModel = await _rideFacade.SaveAsync(SelectedRideModel);


						await LoadAsync();

						_mediator.Send(new UpdateRideMessage<RideDetailModel>() { Model = SelectedRideModel });
					}
				}
			}
		}

		/// <summary>
		/// Remove a passenger from a ride
		/// </summary>
		/// <param name="userListModel">Selected passenger list model</param>
		/// <returns></returns>
		private async Task RemovePassenger(UserListModel? userListModel)
		{

			if (SelectedRideModel == null || userListModel == null || SelectedRideModel.Car == null)
				return;

			var passenger = await _userFacade.GetAsync(userListModel.Id);

			if (passenger != null)
			{
				var userRide = passenger.PassengerRides.Where(i => i.RideId == SelectedRideModel.Id).FirstOrDefault();

				var carDetailModel = await _carFacade.GetAsync(SelectedRideModel.Car.Id);

				if (userRide == null || carDetailModel == null)
					return;

				passenger.PassengerRides.Remove(userRide);
				SelectedRideModel.Passengers.Remove(userRide);

				SelectedRideModel =  await _rideFacade.SaveAsync(SelectedRideModel);
				await _userFacade.SaveAsync(passenger);

				await LoadAsync();

				_mediator.Send(new UpdateRideMessage<RideDetailModel>() { Model = SelectedRideModel });
			}
		}
	}
}
