using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.Common.Tests;
using Carpool.Common.Tests.Seeds;
using Carpool.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Carpool.DAL.Tests
{
	public class DbContextUserTests : DbContextTestsBase
	{
		public DbContextUserTests(ITestOutputHelper output) : base(output)
		{
		}

		[Fact]
		public async Task GetAll_UserRides_SeededTomOstravaOpava_DoesNotThrow()
		{
			var userFromDb = await CarpoolDbContextSUT.UserRides.FirstOrDefaultAsync(i => i.Id == UserRideSeeds.TomOstravaOpava.Id);
			DeepAssert.Equal(UserRideSeeds.TomOstravaOpava, userFromDb);
		}

		[Fact]
		public async Task AddNew_User_Persisted()
		{
			var newUser = UserSeeds.EmptyUserEntity with
			{
				Name = "Colonel",
				Surname = "Sanders",
				PassengerRides = Array.Empty<UserRideEntity>()
			};

			CarpoolDbContextSUT.Users.Add(newUser);
			await CarpoolDbContextSUT.SaveChangesAsync();

			await using var dbx = await DbContextFactory.CreateDbContextAsync();
			var actualUpdatedUser = await dbx.Users.SingleAsync(i => i.Id == newUser.Id);
			DeepAssert.Equal(newUser, actualUpdatedUser);
		}

		[Fact]
		public async Task Update_User_Persisted()
		{
			var userFromDb = UserSeeds.UserJan;
			var updatedUser =
				userFromDb with
				{
					Id = default,
					Name = userFromDb.Name + "Pokřtěn",
					Surname = "Digger",
					Age = userFromDb.Age + 1,
					PhotoUrl = default!,
					PhoneNumber = default
				};

			CarpoolDbContextSUT.Users.Update(updatedUser);
			await CarpoolDbContextSUT.SaveChangesAsync();

			await using var dbx = await DbContextFactory.CreateDbContextAsync();
			var actualUpdatedUser = await dbx.Users.SingleAsync(i => i.Id == updatedUser.Id);
			DeepAssert.Equal(updatedUser, actualUpdatedUser);
		}

		[Fact]
		public async Task Delete_UserToDelete_Deleted()
		{
			var userFromDb = UserSeeds.UserToDelete;

			CarpoolDbContextSUT.Users.Remove(userFromDb);
			await CarpoolDbContextSUT.SaveChangesAsync();

			Assert.False(await CarpoolDbContextSUT.Users.AnyAsync(i => i.Id == userFromDb.Id));
		}

		[Fact]
		public async Task Edit_RideEntityToUser()
		{
			UserEntity userFromDb = await CarpoolDbContextSUT.Users.SingleAsync(i => i.Id == UserSeeds.UserJan.Id);
			userFromDb.PassengerRides.Add
			(
				new UserRideEntity
				{
					Id = default!,
					UserId = UserSeeds.UserJan.Id,
					RideId = RideSeeds.RideEsAs.Id
				}
			);

			UserEntity updatedUserFromDb = await CarpoolDbContextSUT.Users.SingleAsync(i => i.Id == UserSeeds.UserJan.Id);
			Assert.Equal(RideSeeds.RideEsAs.Id, updatedUserFromDb.PassengerRides?.FirstOrDefault()?.RideId);
		}

		[Fact]
		public async Task Add_UserRide_To_User()
		{
			var entity = UserSeeds.EmptyUserEntity with
			{
				Name = "Karel",
				Surname = "Modra",
				PassengerRides = new List<UserRideEntity> {
					UserRideSeeds.EmptyUserRide with
					{
						RideId = RideSeeds.RidePrahaLiberec.Id
					}
				}
			};

			CarpoolDbContextSUT.Users.Add(entity);
			await CarpoolDbContextSUT.SaveChangesAsync();

			await using var dbx = await DbContextFactory.CreateDbContextAsync();
			var actualEntity = await dbx.Users
				.Include(i => i.PassengerRides)
				.SingleAsync(i => i.Id == entity.Id);

			DeepAssert.Equal(entity, actualEntity);
		}

		[Fact]
		public async Task Update_UserRide_To_User()
		{
			var entity = UserSeeds.EmptyUserEntity with
			{
				Name = "Filip",
				Surname = "Velky",
			};

			CarpoolDbContextSUT.Users.Add(entity);
			await CarpoolDbContextSUT.SaveChangesAsync();

			entity.PassengerRides.Add(UserRideSeeds.EmptyUserRide with
			{
				UserId = entity.Id,
				RideId = RideSeeds.RidePrahaLiberec.Id
			});

			CarpoolDbContextSUT.Users.Update(entity);
			await CarpoolDbContextSUT.SaveChangesAsync();

			await using var dbx = await DbContextFactory.CreateDbContextAsync();
			var actualEntity = await dbx.Users
				.Include(i => i.PassengerRides)
				.SingleAsync(i => i.Id == entity.Id);

			DeepAssert.Equal(entity, actualEntity);
		}

		[Fact]
		public async Task todo() 
		{
			var entity = await CarpoolDbContextSUT.Users
				.Include(i => i.PassengerRides)
				.ThenInclude(i => i.Ride)
				.SingleAsync(i => i.Id == UserSeeds.UserPassengerTom.Id);

			var userRide = entity.PassengerRides.FirstOrDefault(i => i.Id == UserRideSeeds.TomOstravaOpava.Id);
			
			Assert.Equal(userRide?.Id, UserRideSeeds.TomOstravaOpava.Id);
			Assert.Equal(userRide?.RideId, UserRideSeeds.TomOstravaOpava.RideId);
			Assert.Equal(userRide?.UserId, UserRideSeeds.TomOstravaOpava.UserId);
		}
	}
}
