using System;
using System.Linq;
using System.Threading.Tasks;
using Carpool.BL.Facades;
using Carpool.BL.Models;
using Carpool.Common;
using Carpool.Common.Tests;
using Carpool.Common.Tests.Seeds;
using Xunit;
using Xunit.Abstractions;

namespace Carpool.BL.Tests
{
	public sealed class CarFacadeTests : CRUDFacadeTestsBase
	{
		private readonly CarFacade _carFacadeSUT;

		public CarFacadeTests(ITestOutputHelper output) : base(output)
		{
			_carFacadeSUT = new CarFacade(UnitOfWorkFactory, Mapper);
		}

		[Fact]
		public async Task Create_WithNonExistingItem_DoesNotThrow()
		{
			var listModel = Mapper.Map<CarListModel>(CarSeeds.CarOpel);
			var carDetailModel = await _carFacadeSUT.GetAsync(listModel.Id);

			Assert.Equal(listModel.Brand, carDetailModel?.Brand);
			Assert.Equal(listModel.Type, carDetailModel?.Type);
			Assert.Equal(listModel.PhotoUrl, carDetailModel?.PhotoUrl);
		}

		[Fact]
		public async Task GetAll_FromSeeded_DoesNotThrowAndContainsSeeded()
		{
			var listModel = Mapper.Map<CarListModel>(CarSeeds.CarNissan);

			var returnedModel = await _carFacadeSUT.GetAsync();

			Assert.Contains(listModel, returnedModel);
		}

		[Fact]
		public async Task Create_Save_CarDetailModel_DoesNotThrow()
		{
			var model = new CarDetailModel(
				Brand: "Porshe",
				Type: CarTypes.SportsCar,
				FirstRegistration: DateTime.Today.AddDays(-90),
				PhotoUrl: "https://i.picsum.photos/id/514/300/300.jpg?hmac=cWjIdfi9OoXTpWCC_x4On9HehQ1N0FSR6MxanIRMcfQ",
				NumOfEmptySeats: 1,
				UserId: UserSeeds.UserPetr.Id
			);
			await _carFacadeSUT.SaveAsync(model);

			// The Car exists also as a ListModel
			var modelList = (await _carFacadeSUT.GetAsync()).FirstOrDefault(i => i.Brand == model.Brand &&
																				 i.PhotoUrl == model.PhotoUrl &&
																				 i.Brand == model.Brand);
		}

		[Fact]
		public async Task Create_Save_DeepAssert_DoesNotThrow()
		{
			var model = new CarDetailModel(
				Brand: "Mazda",
				Type: CarTypes.Truck,
				FirstRegistration: DateTime.Today.AddDays(-30),
				PhotoUrl: "https://i.picsum.photos/id/197/300/300.jpg?hmac=Pk-q1CD2SxJwsAbEJzGlD4lzbPL8EHiUrPi9vjl8bRo",
				NumOfEmptySeats: 3,
				UserId: UserSeeds.UserPetr.Id
			);
			var detailModel = await _carFacadeSUT.SaveAsync(model);

			// Created model has no ID
			detailModel.Id = model.Id;
			DeepAssert.Equal(model, detailModel);
		}

		[Fact]
		public async Task Create_Change_Update_DoesNotThrows()
		{
			var model = new CarDetailModel
			(
				Brand: "Renault",
				Type: CarTypes.Van,
				FirstRegistration: DateTime.Today.AddDays(-30),
				PhotoUrl: "https://i.picsum.photos/id/146/300/300.jpg?hmac=bEnGWDUNJXc4pArmCbpIZymfU8zd8aBlfpmNpzIYF6I",
				NumOfEmptySeats: 6,
				UserId: UserSeeds.UserDaniel.Id
			);
			var detailModel = await _carFacadeSUT.SaveAsync(model);

			detailModel.Brand = "Renault2";
			await _carFacadeSUT.SaveAsync(detailModel);

			var dbDetailModel = await _carFacadeSUT.GetAsync(detailModel.Id);

			Assert.Equal("Renault2", dbDetailModel?.Brand);
			model.Id = detailModel.Id;
			model.Brand = "Renault2";
			DeepAssert.Equal(model, dbDetailModel);
		}

		[Fact]
		public async Task Delete_Created_DoesNotThrow()
		{
			var model = new CarDetailModel
			(
				Brand: "Hotweels",
				Type: CarTypes.Wagon,
				FirstRegistration: DateTime.Today.AddDays(-300),
				PhotoUrl: "https://i.picsum.photos/id/193/200/300.jpg?hmac=b5ZG1TfdndbrnQ8UJbIu-ykB2PRWv0QpHwehH0pqMgE",
				NumOfEmptySeats: 3,
				UserId: UserSeeds.UserDaniel.Id
			);
			var detailModel = await _carFacadeSUT.SaveAsync(model);

			await _carFacadeSUT.DeleteAsync(detailModel.Id);
		}

		[Fact]
		public async Task Delete_NotExisting_Throws()
		{
			_ = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			{
				await _carFacadeSUT.DeleteAsync(Guid.Parse("9864EDB7-5B50-4990-9B8A-F32876928B97"));
			});
		}

		[Fact]
		public async Task Check_RidePrahaBrno_In_CarSkodaRideList_DoesNotThrow()
		{
			var model = await _carFacadeSUT.GetAsync(CarSeeds.CarSkoda.Id);
			var ride = model?.Rides.FirstOrDefault(i => i.Id == RideSeeds.RidePrahaBrno.Id);

			Assert.Equal(RideSeeds.RidePrahaBrno.Start, ride?.Start);
			Assert.Equal(RideSeeds.RidePrahaBrno.End, ride?.End);
			Assert.Equal(RideSeeds.RidePrahaBrno.Beginning, ride?.Beginning);
		}

		[Fact]
		public async Task Get_RideListModel_From_Car_DeepAssert_Seeded_DoesNotThrow()
		{
			var model = await _carFacadeSUT.GetAsync(CarSeeds.CarCitroen.Id);
			var ride = model?.Rides.FirstOrDefault(i => i.Start == "Ostrava" && i.End == "Opava");
			var listModel = Mapper.Map<RideListModel>(RideSeeds.RideOstravaOpava);
			if(ride != null)
			{
				listModel.DriverName = ride.DriverName;
				listModel.DriverSurname = ride.DriverSurname;
				listModel.CarNumOfEmptySeats = ride.CarNumOfEmptySeats;
			}

			DeepAssert.Equal(listModel, ride);
		}

		[Fact]
		public async Task Get_Model_DeepAssert_SeededMappedModel_DoesNotThrow()
		{
			var model = await _carFacadeSUT.GetAsync(CarSeeds.CarVolvo.Id);

			DeepAssert.Equal(Mapper.Map<CarDetailModel>(CarSeeds.CarVolvo), model);
		}

		[Fact]
		public async Task UpdateCar_CannotChangeDriver_DoesNotThrow()
		{
			var carModel = new CarDetailModel
			(
				Brand: "Tesla",
				Type: CarTypes.Suv,
				FirstRegistration: DateTime.Today.AddDays(-365),
				PhotoUrl: "https://i.picsum.photos/id/670/500/400.jpg?hmac=hVqgmp6CVIky4cKd30fo-LCMkWLgqiolUAi5-ybL448",
				NumOfEmptySeats: 3,
				UserId: UserSeeds.UserPetr.Id
			);

			var modelDb = await _carFacadeSUT.SaveAsync(carModel);
			modelDb.UserId = UserSeeds.UserWithNoCars.Id;

			var modelCarDb = await _carFacadeSUT.SaveAsync(modelDb);

			Assert.NotEqual(UserSeeds.UserWithNoCars.Id, modelCarDb.UserId);
		}

		[Fact]
		public async Task Create_Get_Compare_ListAndDetailModel()
		{
			var carDetailModel = new CarDetailModel
			(
				Brand: "Mercedes",
				Type: CarTypes.Pickup,
				FirstRegistration: DateTime.Today.AddDays(-105),
				PhotoUrl: "https://i.picsum.photos/id/670/500/400.jpg?hmac=hVqgmp6CVIky4cKd30fo-LCMkWLgqiolUAi5-ybL448",
				NumOfEmptySeats: 6,
				UserId: UserSeeds.UserJakub.Id
			);

			var carDb = await _carFacadeSUT.SaveAsync(carDetailModel);
			var carListModel = (await _carFacadeSUT.GetAsync())
				.FirstOrDefault(i => i.Brand == carDetailModel.Brand && i.Type == carDetailModel.Type && i.PhotoUrl == carDetailModel.PhotoUrl);

			Assert.Equal(carDb.Id, carListModel?.Id);
		}
	}
}
