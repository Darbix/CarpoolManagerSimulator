using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Carpool.App.Commands;
using Carpool.App.Extensions;
using Carpool.App.Messages;
using Carpool.App.Services;
using Carpool.App.ViewModels.Interfaces;
using Carpool.BL.Facades;
using Carpool.BL.Models;

namespace Carpool.App.ViewModels
{
	public class AllRidesListViewModel: ViewModelBase, IAllRidesListViewModel
	{
		private RideFacade _rideFacade { get; }
		private UserFacade _userFacade { get; }
		private CarFacade _carFacade { get; }

		private readonly IMediator _mediator;

		public AllRidesListViewModel(
			RideFacade rideFacade,
			UserFacade userFacade,
			CarFacade carFacade,
			IMediator mediator)
		{
			_rideFacade = rideFacade;
			_userFacade = userFacade;
			_carFacade = carFacade;
			_mediator = mediator;

			_mediator.Register<LogOutMessage>(OnUserLogOut);
			_mediator.Register<LogInMessage<UserDetailModel>>(LogIn);

			_mediator.Register<SelectedMessage<UserDetailModel>>(OnUserSelected);

			RideJoinedCommand = new RelayCommand<RideListModel>(RideJoined);
			InfoCommand = new RelayCommand<RideListModel>(InfoClicked);
			SearchCommand = new AsyncRelayCommand(Search);
			RefreshCommand = new AsyncRelayCommand(Refresh);

			User = UserDetailModel.Empty;
		}

		/// <summary>
		/// Collection of all rides available and not full for the User
		/// </summary>
		public ObservableCollection<RideListModel> Rides { get; } = new();
		/// <summary>
		/// Current User logged into a system
		/// </summary>
		public UserDetailModel User { get; set; } = UserDetailModel.Empty;
		/// <summary>
		/// Command to join a ride
		/// </summary>
		public ICommand RideJoinedCommand { get; set; }
		/// <summary>
		/// Command to show info about a selected ride
		/// </summary>
		public ICommand InfoCommand { get; set; }
		/// <summary>
		/// Command to filter the rides
		/// </summary>
		public ICommand SearchCommand { get; set; }
		/// <summary>
		/// Command to refresh the Rides list
		/// </summary>
		public ICommand RefreshCommand { get; set; }
		/// <summary>
		/// Detail model about a selected ride for detail info
		/// </summary>
		public RideDetailModel InfoRide { get; set; } = RideDetailModel.Empty;
		/// <summary>
		/// Detail model info about a selected ride Driver
		/// </summary>
		public UserDetailModel InfoDriver { get; set; } = UserDetailModel.Empty;
		/// <summary>
		/// Calculated seat capacity in the selected info ride
		/// </summary>
		public int InfoSeats { get; set; } = 0;
		/// <summary>
		/// Visibility predicate for the info panel
		/// </summary>
		public Visibility InfoVisible { get; set; } = Visibility.Hidden;
		/// <summary>
		/// Search text box content for a ride start
		/// </summary>
		public string SearchStart { get; set; } = string.Empty;
		/// <summary>
		/// Search text box content for a ride end
		/// </summary>
		public string SearchEnd { get; set; } = string.Empty;
		/// <summary>
		/// Search date box context for a ride beginning time
		/// </summary>
		public DateTime SearchBeginning { get; set; } = DateTime.Now;


		/// <summary>
		/// Load available rides from the database to the Rides collection
		/// </summary>
		/// <param name="id">Id of a selected user</param>
		/// <returns></returns>
		public async Task LoadRidesAsync(Guid id)
		{
			Rides.Clear();
			User = await _userFacade.GetAsync(id) ?? UserDetailModel.Empty;
			var rides = (await _rideFacade.GetAsync())
				.Where(i => User.DriverRides.Any(j => i.Id == j.Id) == false).ToList();

			// Show only rides, that are not full for passengers
			foreach (var ride in rides)
			{
				var rideDetail = await _rideFacade.GetAsync(ride.Id);
				if (rideDetail != null && rideDetail.Car != null)
				{
					if (rideDetail.Passengers.Count() != rideDetail.Car.NumOfEmptySeats ||
						User.PassengerRides.Any(i => i.RideId == rideDetail.Id) == true)
						Rides.Add(ride);
				}

			}
		}

		/// <summary>
		/// Search the filtered rides using specified Search Start, End and Beginning
		/// </summary>
		/// <returns></returns>
		private async Task Search() 
		{
			await LoadRidesAsync(User.Id);

			var rides = Rides.ToList();
			if(!string.IsNullOrEmpty(SearchStart))
				rides = rides.Where(i => i.Start.ToLower() == SearchStart.ToLower()).ToList();
			if (!string.IsNullOrEmpty(SearchEnd))
				rides = rides.Where(i => i.End.ToLower() == SearchEnd.ToLower()).ToList();

			rides = rides.Where(i => i.Beginning >= SearchBeginning).ToList();

			Rides.Clear();

			rides = rides.OrderBy(i => i.Beginning).ToList();
			Rides.AddRange(rides);
		}

		/// <summary>
		/// React to a selected user message update
		/// </summary>
		/// <param name="message"></param>
		private async void OnUserSelected(SelectedMessage<UserDetailModel> message)
		{
			await SelectUser(message.Id);
		}

		/// <summary>
		/// Update rides for an updated user from a message Id
		/// </summary>
		/// <param name="id">User Id</param>
		private async Task SelectUser(Guid? id)
		{
			if (id is not null)
			{
				Guid userId = (Guid)id;
				await LoadRidesAsync(userId);
			}
		}

		/// <summary>
		/// Update after a user logged in
		/// </summary>
		/// <param name="message">Logged in user model</param>
		private async void LogIn(LogInMessage<UserDetailModel> message)
		{
			if (message != null)
			{
				Guid? userId = message.Id;
				if (userId != null)
					await LoadRidesAsync((Guid)userId);
			}
		}

		/// <summary>
		/// Add a user to a ride after it was clicked
		/// </summary>
		/// <param name="rideListModel">List model of a clicked ride</param>
		private async void RideJoined(RideListModel? rideListModel)
		{
			if (rideListModel == null)
				return;

			var rideDetailModel = await _rideFacade.GetAsync(rideListModel.Id);
			if (rideDetailModel == null || rideDetailModel.Car == null)
				return;

			var carDetailModel = await _carFacade.GetAsync(rideDetailModel.Car.Id);
			if (carDetailModel == null)
				return;

			// Join carpool
			if (User.PassengerRides.Where(i => i.RideId == rideDetailModel.Id).FirstOrDefault() == null && carDetailModel.NumOfEmptySeats > 0)
			{
				// Check if there already is a colliding ride
				foreach(var r in User.PassengerRides)
				{
					// User cannot already be in a ride at the intersecting time with the new one
					if ((rideDetailModel.Beginning > r.RideBeginning) && 
						(rideDetailModel.Beginning < r.RideBeginning + r.RideDuration) ||
						(rideDetailModel.Beginning + rideDetailModel.Duration > r.RideBeginning) && 
						(rideDetailModel.Beginning + rideDetailModel.Duration < r.RideBeginning + r.RideDuration))
					{
						MessageBox.Show("You are already in a ride at this time!", "Warning");
						return;
					}
				}

				// Change
				var userRide = new UserRideDetailModel(rideListModel.Id, rideDetailModel.Start,
					rideDetailModel.End, rideDetailModel.Beginning, rideDetailModel.Duration);

				User.PassengerRides.Add(userRide);

				await _userFacade.SaveAsync(User);

				// Update (get IDs to selected model)
				var user = await _userFacade.GetAsync(User.Id);
				var dbUserRide = user?.PassengerRides.Where(i => i.RideId == rideDetailModel.Id).FirstOrDefault();
				if (dbUserRide == null)
					return;

				rideDetailModel.Passengers.Add(dbUserRide);

				InfoSeats = rideDetailModel.Car.NumOfEmptySeats - rideDetailModel.Passengers.Count();

				//Save
				await _rideFacade.SaveAsync(rideDetailModel);
			}
			// Is already in the ride? ->  Unjoin carpool
			else
			{
				var userRide = User.PassengerRides.Where(i => i.RideId == rideDetailModel.Id).FirstOrDefault();
				if (userRide == null)
					return;

				User.PassengerRides.Remove(userRide);
				rideDetailModel.Passengers.Remove(userRide);
				
				InfoSeats = rideDetailModel.Car.NumOfEmptySeats - rideDetailModel.Passengers.Count();

				await _rideFacade.SaveAsync(rideDetailModel);
				await _userFacade.SaveAsync(User);
			}

			await LoadRidesAsync(User.Id); // Converter then loads empty Ids

			var ride = await _rideFacade.GetAsync(rideDetailModel.Id);
			if(ride != null)
				InfoRide = ride;

			_mediator.Send(new UpdateMessage<UserDetailModel> { Model = User });
		}

		/// <summary>
		/// Show a ride info panel with details about a ride
		/// </summary>
		/// <param name="rideListModel">Selected ride in a list</param>
		private async void InfoClicked(RideListModel? rideListModel)
		{
			if (rideListModel == null)
				return;

			// Show info if the panel is hidden or not the last ride is selected
			if (rideListModel.Id != InfoRide.Id || InfoVisible == Visibility.Hidden)
			{
				InfoVisible = System.Windows.Visibility.Visible;
				var detail = await _rideFacade.GetAsync(rideListModel.Id);
				if (detail == null || detail.Car == null)
					return;
				InfoRide = detail;

				var user = await _userFacade.GetAsync(detail.DriverId);
				if (user == null)
					return;

				InfoDriver = user;
				InfoSeats = detail.Car.NumOfEmptySeats - detail.Passengers.Count();
			}
			else
				InfoVisible = Visibility.Hidden;
		}

		/// <summary>
		/// React to a user log out
		/// </summary>
		/// <param name="message">Logged out user</param>
		private void OnUserLogOut(LogOutMessage message)
		{
			// Hide opened car info on log out
			InfoVisible = Visibility.Hidden;
		}

		/// <summary>
		/// Refresh the ride list on a button click
		/// </summary>
		/// <returns></returns>
		private async Task Refresh()
		{
			await LoadRidesAsync(User.Id);
		}
	}
}
