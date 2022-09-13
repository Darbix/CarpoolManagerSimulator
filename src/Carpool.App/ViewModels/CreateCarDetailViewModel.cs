using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Carpool.App.Commands;
using Carpool.App.Messages;
using Carpool.App.Services;
using Carpool.BL.Facades;
using Carpool.BL.Models;

namespace Carpool.App.ViewModels
{
	public class CreateCarDetailViewModel
	{
		private CarFacade _carFacade { get; }
		private RideFacade _rideFacade { get; }
		private UserFacade _userFacade { get; }

		private readonly IMediator _mediator;

		public CreateCarDetailViewModel(
			UserFacade userFacade,
			CarFacade carFacade,
			RideFacade rideFacade,
			IMediator mediator)
		{
			_mediator = mediator; 
			_carFacade = carFacade;
			_userFacade = userFacade;
			_rideFacade = rideFacade;

			_mediator.Register<AddCarDetailMessage<UserDetailModel>>(AddCar);

			SaveCarCommand = new AsyncRelayCommand(SaveCar);

			CarDetailModel = GetEmptyCar();
		}

		/// <summary>
		/// Car detail model being added or edited
		/// </summary>
		public CarDetailModel CarDetailModel { get; set; }
		/// <summary>
		/// Owner user of the car
		/// </summary>
		public UserDetailModel UserDetailModel { get; set; } = UserDetailModel.Empty;
		/// <summary>
		/// Command to save the car
		/// </summary>
		public ICommand SaveCarCommand { get; set; }


		/// <summary>
		/// Get the default car values
		/// </summary>
		/// <returns>Empty car detail model</returns>
		public CarDetailModel GetEmptyCar()
		{
			var car = CarDetailModel.Empty;
			car.FirstRegistration = DateTime.Today;
			return car;
		}

		/// <summary>
		/// React to add or edit a car message after window open
		/// </summary>
		/// <param name="message"></param>
		private void AddCar(AddCarDetailMessage<UserDetailModel> message)
		{
			var user = message.Model;
			if (user == null)
				return;
			CarDetailModel.UserId = user.Id;
		}

		/// <summary>
		/// Save the car to the database
		/// </summary>
		/// <returns></returns>
		private async Task SaveCar()
		{
			if (CarDetailModel.Brand == null || CarDetailModel.Brand == string.Empty)
				return;

			// Check if the seats are correct according to a current passenger number
			if(CarDetailModel.Rides.Count != 0)
			{
				foreach(var ride in CarDetailModel.Rides)
				{
					var rideDetail = await _rideFacade.GetAsync(ride.Id);
					if(rideDetail != null)
					{
						if (rideDetail.Passengers.Count > CarDetailModel.NumOfEmptySeats)
						{
							MessageBox.Show("The Car is used in a ride, where is more passengers than a new seat capacity is!", "Warning");
							return;
						}
					}
				}
			}

			var model = await _carFacade.SaveAsync(CarDetailModel);

			_mediator.Send(new UpdateCarMessage<CarDetailModel> { Model = model });

			CarDetailModel = GetEmptyCar();
		}
	}
}
