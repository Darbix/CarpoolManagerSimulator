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
using Carpool.BL.Facades;
using Carpool.BL.Models;
using Carpool.DAL.Seeds;

namespace Carpool.App.ViewModels
{
	public class CreateRideDetailViewModel
	{
		private readonly IMediator _mediator;
		private readonly RideFacade _rideFacade;
		private readonly CarFacade _carFacade;
		private readonly UserFacade _userFacade;

		public CreateRideDetailViewModel(
			RideFacade rideFacade,
			CarFacade carFacade,
			IMediator mediator,
			UserFacade userFacade)
		{
			_mediator = mediator;
			_mediator.Register<AddRideDetailMessage<UserDetailModel>>(AddRideDetail);
			_mediator.Register<EditRideDetailMessage<RideDetailModel>>(EditRideDetail);


			RideModel = getDefaultRide();
			RideSaveCommand = new AsyncRelayCommand<Window?>(RideSavedAsync);

			_rideFacade = rideFacade;
			_carFacade = carFacade;
			_userFacade = userFacade;
		}

		/// <summary>
		/// Ride detail model being added or edited
		/// </summary>
		public RideDetailModel RideModel { get; set; } = RideDetailModel.Empty;
		/// <summary>
		/// Driver user of the ride
		/// </summary>
		public UserDetailModel UserDetailModel { get; set; } = UserDetailModel.Empty;
		/// <summary>
		/// Available car list to add to the ride
		/// </summary>
		public ObservableCollection<CarListModel> Cars { get; } = new();
		/// <summary>
		/// Selected car from the offered list
		/// </summary>
		public CarListModel? SelectedCarListModel { get; set; }
		/// <summary>
		/// Value to show the TimeSpan duration as a DateTime value
		/// </summary>
		public DateTime TempEnd { get; set; } = DateTime.Today;
		/// <summary>
		/// Command to save the ride
		/// </summary>
		public ICommand RideSaveCommand { get; }


		/// <summary>
		/// React to a message after an add-ride window loaded
		/// </summary>
		/// <param name="message">Ride driver user detail model</param>
		private async void AddRideDetail(AddRideDetailMessage<UserDetailModel> message)
		{
			RideModel = RideDetailModel.Empty;
			RideModel.Beginning = DateTime.Now;
			TempEnd = DateTime.Today;

			if (message.Id == null)
				return;
			Guid id = (Guid)message.Id;
			var user = await _userFacade.GetAsync(id);
			if (user != null)
				UserDetailModel = user;

			await LoadCarsAsync();
		}

		/// <summary>
		/// React to a message after an edit-ride window loaded
		/// </summary>
		/// <param name="message">Ride driver user detail model</param>
		private async void EditRideDetail(EditRideDetailMessage<RideDetailModel> message)
		{
			var id = message?.Model?.DriverId;
			if (message == null || message.Model == null || id == null)
				return;

			TempEnd = DateTime.Today + message.Model.Duration;

			RideModel = message.Model;

			var user = await _userFacade.GetAsync((Guid)id);
			if (user != null)
				UserDetailModel = user;

			await LoadCarsAsync();
		}

		/// <summary>
		/// Load available cars from the database
		/// </summary>
		/// <returns></returns>
		private async Task LoadCarsAsync()
		{
			Cars.Clear();
			var cars = (await _carFacade.GetAsync()).Where(i => UserDetailModel.Cars.Contains(i));
			Cars.AddRange(cars);
		}

		/// <summary>
		/// Get the default empty default ride model
		/// </summary>
		/// <returns>Default ride detail model</returns>
		private RideDetailModel getDefaultRide()
		{
			TempEnd = DateTime.Today;
			return new RideDetailModel(string.Empty, string.Empty, DateTime.Now, TimeSpan.FromSeconds(0), Guid.Empty);
		}

		/// <summary>
		/// Save the ride filled with new values into the database
		/// </summary>
		/// <param name="window">This window to close</param>
		/// <returns></returns>
		private async Task RideSavedAsync(Window? window)
		{
			Guid carListId = Guid.Empty;
			if (string.IsNullOrEmpty(RideModel.Start) || string.IsNullOrEmpty(RideModel.End))
				return;
			if (SelectedCarListModel == null && RideModel.Car != null)
				carListId = RideModel.Car.Id;
			else if(SelectedCarListModel != null)
				carListId = SelectedCarListModel.Id;

			RideModel.DriverId = UserDetailModel.Id;
			RideModel.Duration = TempEnd.TimeOfDay;

			// Default car is the last one (in ride edit mode)
			CarDetailModel? carDetailModel = await _carFacade.GetAsync(carListId);
			
			carDetailModel = await _carFacade.GetAsync(carListId);
			if (carDetailModel == null)
				return;


			//if the ride is edited, not created
			if (RideModel.Id != Guid.Empty)
			{
				var oldRideModel = await _rideFacade.GetAsync(RideModel.Id);
				if (oldRideModel != null && oldRideModel.Car != null)
				{
					// If a car is changed to one with less seats
					if (oldRideModel.Passengers.Count > carDetailModel.NumOfEmptySeats)
					{
						MessageBox.Show("Car " + carDetailModel.Brand + " has a lower seat capacity than a number of passengers is. Passengers cannot fit in!", "Warning");
						RideModel = oldRideModel;
						_mediator.Send(new UpdateRideMessage<RideDetailModel> { Model = RideModel });
						return;
					}

					// Check if there are some passenger conflicts in their other rides
					if(oldRideModel.Passengers.Count > 0)
					{
						var userRideList = await _userFacade.GetAsync();
						foreach (var u in userRideList)
						{
							var userDetail = await _userFacade.GetAsync(u.Id);
							if (userDetail != null)
							{
								// If a user is a passenger in this ride
								if (oldRideModel.Passengers.Intersect(userDetail.PassengerRides).Any())
								{
									foreach (var r in userDetail.PassengerRides)
									{
										// If the passenger ride is not this and a time intersects
										if(oldRideModel.Id != r.RideId)
										{
											// User cannot already be in a ride at the intersecting time
											if ((RideModel.Beginning > r.RideBeginning) &&
												(RideModel.Beginning < r.RideBeginning + r.RideDuration) ||
												(RideModel.Beginning + RideModel.Duration > r.RideBeginning) &&
												(RideModel.Beginning + RideModel.Duration < r.RideBeginning + r.RideDuration))
											{
												MessageBox.Show("The ride contains passengers who already are in another ride at this time. The time cannot be changed!", "Warning");
												RideModel = oldRideModel;
												_mediator.Send(new UpdateRideMessage<RideDetailModel> { Model = RideModel });
												return;
											}
										}
									}
								}
							}
						}
					}
				}
			}

			RideModel.Car = new RideCarModel(
				carDetailModel.Id, carDetailModel.Brand, Common.CarTypes.Coupe,
				carDetailModel.PhotoUrl, carDetailModel.NumOfEmptySeats);

			var model = await _rideFacade.SaveAsync(RideModel);

			_mediator.Send(new UpdateRideMessage<RideDetailModel> { Model = model });
			RideModel = getDefaultRide();
			window?.Close();
		}
	}
}
