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
	public class DbContextRideTests : DbContextTestsBase
	{
		public DbContextRideTests(ITestOutputHelper output) : base(output)
		{
		}

		[Fact]
		public async Task Get_SeededRidePrahaBrno_DoesNotThrow()
		{
			var rideFromDb = await CarpoolDbContextSUT.Rides.FirstOrDefaultAsync(i => i.Id == RideSeeds.RidePrahaBrno.Id);
			
			Assert.Equal(RideSeeds.RidePrahaBrno.Id, rideFromDb?.Id);
		}

		[Fact]
		public async Task GetAll_Rides_Find_Equal()
		{
			var entities = await CarpoolDbContextSUT.Rides.ToArrayAsync();

			var ride = entities.FirstOrDefault(i => i.Id == RideSeeds.RideZnojmoBreclav.Id);

			DeepAssert.Equal(RideSeeds.RideZnojmoBreclav, ride);
		}

		[Fact]
		public async Task Update_Ride_Persisted()
		{
			var rideFromDb = RideSeeds.RideOstravaOpava;
			var updatedRide =
				rideFromDb with
				{
					Id = default,
					Start = rideFromDb.Start + "Poruba",
					End = rideFromDb.End + "Kylešovice",
					Beginning = DateTime.Today.AddDays(2),
					Duration = rideFromDb.Duration - TimeSpan.FromMinutes(10),
					DriverId = UserSeeds.UserJan.Id,
					CarId = CarSeeds.CarMazda.Id
				};

			CarpoolDbContextSUT.Rides.Update(updatedRide);
			await CarpoolDbContextSUT.SaveChangesAsync();

			await using var dbx = await DbContextFactory.CreateDbContextAsync();
			var actualUpdatedRide = await dbx.Rides.SingleAsync(i => i.Id == updatedRide.Id);
			DeepAssert.Equal(updatedRide, actualUpdatedRide);
		}

		[Fact]
		public async Task Delete_Ride()
		{
			var entityBase = RideSeeds.RideZnojmoBreclavDelete;

			CarpoolDbContextSUT.Rides.Remove(entityBase);
			await CarpoolDbContextSUT.SaveChangesAsync();

			Assert.False(await CarpoolDbContextSUT.Rides.AnyAsync(i => i.Id == entityBase.Id));
		}

		[Fact]
		public async Task DeleteById_Ingredient_WaterDeleted()
		{
			var entityBase = RideSeeds.RideZnojmoBreclavDelete;

			CarpoolDbContextSUT.Remove(
				CarpoolDbContextSUT.Rides.Single(i => i.Id == entityBase.Id));
			await CarpoolDbContextSUT.SaveChangesAsync();

			Assert.False(await CarpoolDbContextSUT.Rides.AnyAsync(i => i.Id == entityBase.Id));
		}

		[Fact]
		public async Task Add_New_Ride()
		{
			RideEntity entity = new RideEntity()
			{
				Id = Guid.Parse("C5DE45D7-64A0-4E8D-AC7F-BF5CFDFB0EFC"),
				Start = "Bratislava",
				End = "Kladno",
				Beginning = DateTime.Today.AddHours(14),
				Duration = TimeSpan.FromDays(1),
				DriverId = UserSeeds.UserJakub.Id,
				CarId = CarSeeds.CarVolvo.Id
			};

			CarpoolDbContextSUT.Rides.Add(entity);
			await CarpoolDbContextSUT.SaveChangesAsync();

			await using var dbx = await DbContextFactory.CreateDbContextAsync();
			var actualEntity = await dbx.Rides.SingleAsync(i => i.Id == entity.Id);
			DeepAssert.Equal(entity, actualEntity);
		}
	}
}
