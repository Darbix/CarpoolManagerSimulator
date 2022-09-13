using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Carpool.BL.Facades;
using Carpool.BL.Models;
using Carpool.Common.Tests;
using Carpool.Common.Tests.Seeds;
using Xunit;
using Xunit.Abstractions;

namespace Carpool.BL.Tests
{
	public sealed class RideFacadeTests : CRUDFacadeTestsBase
	{
		private readonly RideFacade _rideFacadeSUT;

		public RideFacadeTests(ITestOutputHelper output) : base(output)
		{
			_rideFacadeSUT = new RideFacade(UnitOfWorkFactory, Mapper);
		}

		[Fact]
		public async Task Get_SeededRideCarId_Equals_ModelCarId_DoesNotThrow()
		{
			var model = await _rideFacadeSUT.GetAsync(RideSeeds.RidePrahaLiberec.Id);

			Assert.Equal(RideSeeds.RidePrahaLiberec.CarId, model?.Car?.Id);
		}

		[Fact]
		public async Task Change_RideCarModel_Save_DeepAssert_DoesNotThrow()
		{
			var changedModel = Mapper.Map<RideDetailModel>(RideSeeds.RidePrahaLiberec);
			changedModel.Car = new RideCarModel(
				Id: CarSeeds.CarOpel.Id,
				Brand: CarSeeds.CarOpel.Brand,
				Type: CarSeeds.CarOpel.Type,
				PhotoUrl: CarSeeds.CarOpel.PhotoUrl,
				NumOfEmptySeats: CarSeeds.CarOpel.NumOfEmptySeats
			);
			var savedModel = await _rideFacadeSUT.SaveAsync(changedModel);

			DeepAssert.Equal(changedModel, savedModel);
		}

		[Fact]
		public async Task Delete_NotExisting_Throws()
		{
			_ = await  Assert.ThrowsAsync<InvalidOperationException>(async () =>
			{
				await _rideFacadeSUT.DeleteAsync(Guid.Parse("661B1BBE-F5A4-4AE2-AF06-2EF871154A9A"));
			});
		}

		[Fact]
		public async Task Create_Save_Get_DeepAssert_DoesNotThrow()
		{
			var model = new RideDetailModel(
				Start: "Kolín",
				End: "Mladá Boleslav",
				Beginning: DateTime.Now.AddHours(5),
				Duration: TimeSpan.FromMinutes(45),
				DriverId: UserSeeds.UserDaniel.Id
			)
			{
				Car = new RideCarModel(
					Id: CarSeeds.CarVolvo.Id,
					Brand: CarSeeds.CarVolvo.Brand,
					Type: CarSeeds.CarVolvo.Type,
					PhotoUrl: CarSeeds.CarVolvo.PhotoUrl,
					NumOfEmptySeats: CarSeeds.CarVolvo.NumOfEmptySeats
				)
			};

			var detailModel = await _rideFacadeSUT.SaveAsync(model);
			var detailModelDb = await _rideFacadeSUT.GetAsync(detailModel.Id)?? throw new NullReferenceException();

			// 'model' does not have an ID yet
			detailModelDb.Id = model.Id;

			DeepAssert.Equal(model, detailModelDb);
		}

		[Fact]
		public async Task SaveRide_ChangeCar_DoesNotThrow()
		{
			var model = new RideDetailModel(
				Start: "Mladá Boleslav",
				End: "Kolín",
				Beginning: DateTime.Now.AddHours(5),
				Duration: TimeSpan.FromMinutes(45),
				DriverId: UserSeeds.UserDaniel.Id
			)
			{
				Car = new RideCarModel(
					Id: CarSeeds.CarVolvo.Id,
					Brand: CarSeeds.CarVolvo.Brand,
					Type: CarSeeds.CarVolvo.Type,
					PhotoUrl: CarSeeds.CarVolvo.PhotoUrl,
					NumOfEmptySeats: CarSeeds.CarVolvo.NumOfEmptySeats
				)
			};
			var detailModel = await _rideFacadeSUT.SaveAsync(model)?? throw new NullReferenceException();

			Assert.Equal(CarSeeds.CarVolvo.Id, detailModel.Car?.Id);

			detailModel.Car = new RideCarModel(
					Id: CarSeeds.CarOpel.Id,
					Brand: CarSeeds.CarOpel.Brand,
					Type: CarSeeds.CarOpel.Type,
					PhotoUrl: CarSeeds.CarOpel.PhotoUrl,
					NumOfEmptySeats: CarSeeds.CarOpel.NumOfEmptySeats
				);

			var detailModelDb = await _rideFacadeSUT.SaveAsync(detailModel);
			Assert.Equal(CarSeeds.CarOpel.Id, detailModelDb.Car?.Id);
		}

		[Fact]
		public async Task Update_ChangedSeededModelBeginning_DoesNotThrow()
		{
			var model = await _rideFacadeSUT.GetAsync(RideSeeds.RidePrahaBrno.Id)?? throw new NullReferenceException();
			model.Beginning = model.Beginning.AddDays(10);

			// Update
			await _rideFacadeSUT.SaveAsync(model);

			var modelUpdated = await _rideFacadeSUT.GetAsync(RideSeeds.RidePrahaBrno.Id);
			Assert.Equal(RideSeeds.RidePrahaBrno.Beginning.AddDays(10), modelUpdated?.Beginning);
		}

		[Fact]
		public async Task Delete_RideWithPassenger_Throws()
		{
			await _rideFacadeSUT.DeleteAsync(RideSeeds.RideOstravaOpava.Id);

			var userRide = await _rideFacadeSUT.GetAsync(UserRideSeeds.TomOstravaOpava.RideId);
			Assert.Null(userRide);
		}

		[Fact]
		public async Task Check_UserRide_Passenger_Seeded_DoesNotThrow()
		{
			var rideListModel = (await _rideFacadeSUT.GetAsync()).FirstOrDefault(i => i.Id == RideSeeds.RideZnojmoBreclav.Id)
				?? throw new NullReferenceException();
			var rideDetailModel = await _rideFacadeSUT.GetAsync(rideListModel.Id);
			var carId = rideDetailModel?.Car?.Id;

			Assert.Equal(UserRideSeeds.JakubZnojmoBreclav.Id, rideDetailModel?.Passengers?.FirstOrDefault(i => i.RideCar?.Id == carId)?.Id);
		}

		[Fact]
		public async Task Create_RideWithCar_UserAsDriver_DoesNotThrow()
		{
			var rideModel = new RideDetailModel
			(
				Start: "Olomouc",
				End: "Karlovy Vary",
				Beginning: DateTime.ParseExact("7/7/2023 10:48", "M/d/yyyy hh:mm", CultureInfo.InvariantCulture),
				Duration: TimeSpan.ParseExact("01:30", "hh\\:mm", CultureInfo.InvariantCulture),
				DriverId: UserSeeds.UserDaniel.Id
			)
			{
				Car = new RideCarModel(
					Id: CarSeeds.CarAudi.Id,
					Brand: CarSeeds.CarAudi.Brand,
					Type: CarSeeds.CarAudi.Type,
					PhotoUrl: CarSeeds.CarAudi.PhotoUrl,
					NumOfEmptySeats: CarSeeds.CarAudi.NumOfEmptySeats
				)
			};

			var model = await _rideFacadeSUT.SaveAsync(rideModel)?? throw new NullReferenceException();

			Assert.Equal(CarSeeds.CarAudi.Id, model.Car?.Id);

			if (model.Car == null)
				throw new NullReferenceException();
			model.Car.Id = CarSeeds.CarCitroen.Id;

			var modelDb = await _rideFacadeSUT.SaveAsync(model);

			Assert.NotEqual(CarSeeds.CarCitroen.Id, modelDb.Id);
		}

		[Fact]
		public async Task GetAll_FromSeeded_DoesNotThrowAndContainsSeeded()
		{
			var listModel = Mapper.Map<RideListModel>(RideSeeds.RideZnojmoBreclav);

			var returnedModel = await _rideFacadeSUT.GetAsync();

			//Assert.Contains(listModel, returnedModel);
			Assert.NotNull(returnedModel.Where(i => i.Id == listModel.Id).FirstOrDefault());
		}
	}
}
