using System;
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
	public sealed class UserFacadeTests : CRUDFacadeTestsBase
	{
		private readonly UserFacade _userFacadeSUT;

		public UserFacadeTests(ITestOutputHelper output) : base(output)
		{
			_userFacadeSUT = new UserFacade(UnitOfWorkFactory, Mapper);
		}

		[Fact]
		public async Task Create_WithNonExistingItem_DoesNotThrow()
		{
			var model = new UserDetailModel
			(
				Name: "Jakub",
				Surname: "Zelený",
				Age: 34,
				PhoneNumber: 736141842
			);

			var _ = await _userFacadeSUT.SaveAsync(model);
		}

		[Fact]
		public async Task GetAll_Single_SeededUserPetr_DoesNotThrow()
		{
			var users = await _userFacadeSUT.GetAsync();
			var user = users.Single(i => i.Id == UserSeeds.UserPetr.Id);

			DeepAssert.Equal(Mapper.Map<UserListModel>(UserSeeds.UserPetr), user);
		}

		[Fact]
		public async Task GetById_SeededUserWithNoCars_DoesNotThrow()
		{
			var user = await _userFacadeSUT.GetAsync(UserSeeds.UserWithNoCars.Id);

			DeepAssert.Equal(Mapper.Map<UserDetailModel>(UserSeeds.UserWithNoCars), user);
		}

		[Fact]
		public async Task Create_WithoutCar_DoesNotThrowAndEqualsCreated()
		{
			var model = new UserDetailModel
			(
				Name: "John",
				Surname: "Johnny",
				Age: 28,
				PhoneNumber: 123777666
			);

			var returnedModel = await _userFacadeSUT.SaveAsync(model);

			// Check model with no ID with a Db model, which already has some ID
			returnedModel.Id = model.Id;
			DeepAssert.Equal(model, returnedModel);
		}

		[Fact]
		public async Task Delete_NotExisting_Throws()
		{
			_ = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			{
				await _userFacadeSUT.DeleteAsync(Guid.Parse("B4241A24-08AE-4A92-9FF7-CB58B6944FFC"));
			});
		}

		[Fact]
		public async Task Get_SeededUserPetr_DriverRidePrahaBrno_DoesNotThrow()
		{
			var user = await _userFacadeSUT.GetAsync(UserSeeds.UserPetr.Id);
			Assert.Equal(RideSeeds.RidePrahaBrno.Start, user?.DriverRides.FirstOrDefault()?.Start);
			Assert.Equal(RideSeeds.RidePrahaBrno.Beginning, user?.DriverRides.FirstOrDefault()?.Beginning);
		}

		[Fact]
		public async Task Update_FromSeededUserForUpdate_DoesNotThrow()
		{
			var detailModel = Mapper.Map<UserDetailModel>(UserSeeds.UserForUpdate);
			detailModel.Name = "Lucie";

			await _userFacadeSUT.SaveAsync(detailModel);
		}

		[Fact]
		public async Task CheckUpdate_FromSeededUserForUpdate_DoesNotThrow()
		{
			var detailModel = Mapper.Map<UserDetailModel>(UserSeeds.UserForUpdate);
			detailModel.PhotoUrl = "https://i.picsum.photos/id/646/500/500.jpg?hmac=lpvU_3fux-MX0w5BU4hty3uf3v0K9_Xw97HYvWC-DJc";

			await _userFacadeSUT.SaveAsync(detailModel);

			var returnedModel = await _userFacadeSUT.GetAsync(detailModel.Id);
			DeepAssert.Equal(detailModel, returnedModel);
		}

		[Fact]
		public async Task Delete_SeededUserToDelete_DoesNotThrow()
		{
			var detailModel = Mapper.Map<UserDetailModel>(UserSeeds.UserToDelete);

			await _userFacadeSUT.DeleteAsync(detailModel);
		}

		[Fact]
		public async Task DeletedUserCheck_SeededUserToDelete_DoesNotThrow()
		{
			var detailModel = Mapper.Map<UserDetailModel>(UserSeeds.UserToDelete2);
			await _userFacadeSUT.DeleteAsync(detailModel);

			var returnedModel = await _userFacadeSUT.GetAsync(UserSeeds.UserToDelete2.Id);
			Assert.Equal(default, returnedModel);
		}

		[Fact]
		public async Task UserAsPassenger_SeededUserRide_DoesNotThrow()
		{
			var userModel = await _userFacadeSUT.GetAsync(UserSeeds.UserPassengerTom.Id);

			Assert.Equal(UserRideSeeds.TomOstravaOpava.Id, userModel?.PassengerRides?.FirstOrDefault(i => i.RideStart == "Ostrava")?.Id);
		}

		[Fact]
		public async Task Check_UserRide_Passenger_Seeded_DoesNotThrow()
		{
			var userListModel = (await _userFacadeSUT.GetAsync()).FirstOrDefault(i => i.Id == UserSeeds.UserJakub.Id)
				?? throw new NullReferenceException();
			var userDetailModel = await _userFacadeSUT.GetAsync(userListModel.Id);

			Assert.Equal(UserRideSeeds.JakubZnojmoBreclav.Id, userDetailModel?.PassengerRides?.FirstOrDefault(i => i.RideStart == "Znojmo")?.Id);
		}

		[Fact]
		public async Task Create_Passenger_InUserRideSeededModel_DoesNotThrow()
		{
			// Create a passenger
			var userModel = new UserDetailModel
			(
				Name: "Pavel",
				Surname: "Pravý",
				Age: 58,
				PhoneNumber: 779456333
			);

			await _userFacadeSUT.SaveAsync(userModel);
			// Get his ID
			var list = await _userFacadeSUT.GetAsync();
			var userListModel = list.FirstOrDefault(i => i.Name == userModel.Name && i.Surname == userModel.Surname)?? throw new NullReferenceException();
			var userListModelId = userListModel.Id;
			userModel = await _userFacadeSUT.GetAsync(userListModelId)?? throw new NullReferenceException();

			userModel.PassengerRides.Add
			(
				new UserRideDetailModel(
					RideId: RideSeeds.RideOstravaOpava.Id,
					RideStart: RideSeeds.RideOstravaOpava.Start,
					RideEnd: RideSeeds.RideOstravaOpava.End,
					RideBeginning: RideSeeds.RideOstravaOpava.Beginning,
					RideDuration: RideSeeds.RideOstravaOpava.Duration
				)
			);
			await _userFacadeSUT.SaveAsync(userModel);
			var userDetailModel = await _userFacadeSUT.GetAsync(userListModelId);

			Assert.Equal(RideSeeds.RideOstravaOpava.CarId, userDetailModel?.PassengerRides[0].RideCar?.Id);
			Assert.Equal(RideSeeds.RideOstravaOpava.Start, userDetailModel?.PassengerRides[0].RideStart);
			Assert.Equal(RideSeeds.RideOstravaOpava.Beginning, userDetailModel?.PassengerRides[0].RideBeginning);
		}

		[Fact]
		public async Task GetAll_FromSeeded_DoesNotThrowAndContainsSeeded()
		{
			var listModel = Mapper.Map<UserListModel>(UserSeeds.UserJakub);

			var returnedModel = await _userFacadeSUT.GetAsync();

			Assert.Contains(listModel, returnedModel);
		}
	}
}
