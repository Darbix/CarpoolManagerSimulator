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
	public class CarListViewModel: ViewModelBase, ICarListViewModel
	{
		private CarFacade _carFacade { get; }
		private UserFacade _userFacade { get; }

		private readonly IMediator _mediator;

		public CarListViewModel(
			CarFacade carFacade,
			UserFacade userFacade,
			IMediator mediator,
			CreateCarDetailViewModel createCarDetailViewModel)
		{
			_carFacade = carFacade;
			_userFacade = userFacade;
			_mediator = mediator;
			CreateCarDetailViewModel = createCarDetailViewModel;

			_mediator.Register<SelectedMessage<UserDetailModel>>(OnUserSelected);
			_mediator.Register<UpdateMessage<UserDetailModel>>(OnUserUpdated);
			_mediator.Register<UpdateCarMessage<CarDetailModel>>(OnCarUpdated);
			_mediator.Register<LogOutMessage>(OnUserLogOut);
			_mediator.Register<LogInMessage<UserDetailModel>>(LogIn);

			User = UserDetailModel.Empty;

			AddCarCommand = new RelayCommand(AddCar);
			EditCarCommand = new RelayCommand(EditCar);
			CarSelectedCommand = new AsyncRelayCommand<CarListModel>(CarSelected);
			DeleteCarCommand = new AsyncRelayCommand<CarListModel>(DeleteCar);
		}

		/// <summary>
		/// Collection of user cars
		/// </summary>
		public ObservableCollection<CarListModel> Cars { get; } = new();
		/// <summary>
		/// User logged into a system
		/// </summary>
		public UserDetailModel User { get; set; }
		/// <summary>
		/// Command for adding a car
		/// </summary>
		public ICommand AddCarCommand { get; }
		/// <summary>
		/// Command for editing a car
		/// </summary>
		public ICommand EditCarCommand { get; }
		/// <summary>
		/// Command to show info about a selected car 
		/// </summary>
		public ICommand CarSelectedCommand { get; }
		/// <summary>
		/// Command dor deletion of a car
		/// </summary>
		public ICommand DeleteCarCommand { get; }
		/// <summary>
		/// Create/Add car view model of the window
		/// </summary>
		public CreateCarDetailViewModel CreateCarDetailViewModel { get; set; }
		/// <summary>
		/// Detail model of a selected car for the info panel
		/// </summary>
		public CarDetailModel SelectedCarDetailModel { get; set; } = CarDetailModel.Empty;
		/// <summary>
		/// Window created after a car add/edit click
		/// </summary>
		private CreateCarDetailWindow? CarDetailWindow { get; set; }
		/// <summary>
		/// Visibility predicate of the car info panel
		/// </summary>
		public System.Windows.Visibility InfoVisible { get; set; } = System.Windows.Visibility.Hidden;


		/// <summary>
		/// Load cars into a collection
		/// </summary>
		/// <param name="id">User Id of the owner</param>
		/// <returns></returns>
		public async Task LoadCarsAsync(Guid id)
		{
			Cars.Clear();
			User = await _userFacade.GetAsync(id) ?? UserDetailModel.Empty;
			var cars = (await _carFacade.GetAsync()).Where(i => User.Cars.Contains(i));
			Cars.AddRange(cars);
		}

		/// <summary>
		/// React to user selected message
		/// </summary>
		/// <param name="message"></param>
		private async void OnUserSelected(SelectedMessage<UserDetailModel> message)
		{
			await SelectUser(message.Id);
		}
		
		/// <summary>
		/// Select a user to load the cars
		/// </summary>
		/// <param name="id">User Id</param>
		private async Task SelectUser(Guid? id)
		{
			if (id is not null)
			{
				Guid userId = (Guid)id;
				await LoadCarsAsync(userId);
			}
		}

		/// <summary>
		/// React to user log in to a system to update
		/// </summary>
		/// <param name="message">User detail model</param>
		private async void LogIn(LogInMessage<UserDetailModel> message)
		{
			if (message != null)
			{
				Guid? userId = message.Id;
				if (userId != null)
					await LoadCarsAsync((Guid)userId);
			}
		}

		/// <summary>
		/// React to selected car to show the info about it
		/// </summary>
		/// <param name="carListModel">Selected car from the list</param>
		/// <returns></returns>
		private async Task CarSelected(CarListModel? carListModel)
		{
			if (carListModel == null)
			{
				SelectedCarDetailModel = CarDetailModel.Empty;
				InfoVisible = System.Windows.Visibility.Hidden;
				return;
			}

			var detailModel = await _carFacade.GetAsync(carListModel.Id);
			if (detailModel == null)
				return;

			SelectedCarDetailModel = detailModel;
			InfoVisible = System.Windows.Visibility.Visible;
		}

		/// <summary>
		/// Open a window for adding a car
		/// </summary>
		private void AddCar()
		{
			CarDetailWindow = new CreateCarDetailWindow();

			CreateCarDetailViewModel.CarDetailModel = CreateCarDetailViewModel.GetEmptyCar();
			CarDetailWindow.DataContext = CreateCarDetailViewModel;

			_mediator.Send(new AddCarDetailMessage<UserDetailModel>() { Model = User });
			CarDetailWindow.Show();
		}

		/// <summary>
		/// Open a window for editing an existing car
		/// </summary>
		private void EditCar()
		{
			if(SelectedCarDetailModel.Id != Guid.Empty)
			{
				CarDetailWindow = new CreateCarDetailWindow();

				_mediator.Send(new AddCarDetailMessage<UserDetailModel>() { Model = User });

				CreateCarDetailViewModel.CarDetailModel = SelectedCarDetailModel;
				CarDetailWindow.DataContext = CreateCarDetailViewModel;
				CarDetailWindow.Show();
			}
		}

		/// <summary>
		/// React to user data update
		/// </summary>
		/// <param name="message">User model</param>
		private async void OnUserUpdated(UpdateMessage<UserDetailModel> message)
		{
			var user = message.Model;
			if (user == null)
				return;

			User = user;
			await LoadCarsAsync(User.Id);
		}

		/// <summary>
		/// Reload cars after some of them was updated/added/deleted
		/// </summary>
		/// <param name="message">Car detail model</param>
		private async void OnCarUpdated(UpdateCarMessage<CarDetailModel> message)
		{
			var car = message.Model;
			if (car == null)
				return;
			await LoadCarsAsync(User.Id);
			CarDetailWindow?.Close();

			// Car detail info hide/show
			if (car.Id != Guid.Empty)
				InfoVisible = System.Windows.Visibility.Visible;

			var carDetail = await _carFacade.GetAsync(car.Id);
			if(carDetail != null)
				SelectedCarDetailModel = carDetail;

			_mediator.Send(new UpdateRideMessage<RideDetailModel>() { Model = null });
		}

		/// <summary>
		/// Delete a selected car
		/// </summary>
		/// <param name="carListModel">Car to delete</param>
		/// <returns></returns>
		private async Task DeleteCar(CarListModel? carListModel)
		{
			if(carListModel != null)
			{
				await _carFacade.DeleteAsync(carListModel.Id);
				await LoadCarsAsync(User.Id);
				_mediator.Send(new SelectedMessage<UserDetailModel>() { Model = User });
			}
		}

		/// <summary>
		/// Update when the user logs in
		/// </summary>
		/// <param name="message"></param>
		private void OnUserLogOut(LogOutMessage message)
		{
			InfoVisible = System.Windows.Visibility.Hidden;
		}
	}
}
