using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.Common.Tests;
using Carpool.Common.Tests.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Carpool.DAL.Tests
{
	public class DbContextUserRideTests : DbContextTestsBase
	{
		public DbContextUserRideTests(ITestOutputHelper output) : base(output)
		{
		}

		[Fact]
		public async Task GetAll_Users_SeededUserPetr()
		{
			var userFromDb = await CarpoolDbContextSUT.Users.FirstOrDefaultAsync(i => i.Id == UserSeeds.UserPetr.Id);
			DeepAssert.Equal(UserSeeds.UserPetr, userFromDb);
		}

		[Fact]
		public async Task Check_Passenger_In_UserRide()
		{
			var userRideFromDb = await CarpoolDbContextSUT.UserRides.FirstOrDefaultAsync(i => i.Id == UserRideSeeds.JakubZnojmoBreclav.Id);
			DeepAssert.Equal(UserSeeds.UserJakub.Id, userRideFromDb?.UserId);
		}

		[Fact]
		public async Task Check_UserExistance_In_UserRide()
		{
			var userRide = await CarpoolDbContextSUT.UserRides
			.Where(i => i.RideId == UserRideSeeds.JakubZnojmoBreclav.RideId)
			.Include(i => i.User).FirstAsync();
			var user = userRide.User;

			Assert.Equal(UserSeeds.UserJakub.Id, user?.Id);
		}

		[Fact]
		public async Task Update_UserRidePassenger()
		{
			var baseEntity = UserRideSeeds.JakubZnojmoBreclav;
			var entity =
				baseEntity with
				{
					User = UserSeeds.UserPassengerTom
				};
			
			CarpoolDbContextSUT.UserRides.Update(entity);
			await CarpoolDbContextSUT.SaveChangesAsync();

			await using var dbx = await DbContextFactory.CreateDbContextAsync();
			var actualEntity = await dbx.UserRides.SingleAsync(i => i.Id == entity.Id);
			Assert.Equal(entity.UserId, actualEntity.UserId);
		}

		[Fact]
		public async Task Delete_UserRide()
		{
			var baseEntity = UserRideSeeds.JakubZnojmoBreclavDelete;

			CarpoolDbContextSUT.UserRides.Remove(baseEntity);
			await CarpoolDbContextSUT.SaveChangesAsync();

			Assert.False(await CarpoolDbContextSUT.UserRides.AnyAsync(i => i.Id == baseEntity.Id));
		}

		[Fact]
		public async Task DeleteById_UserRide()
		{
			var baseEntity = UserRideSeeds.JakubZnojmoBreclavDelete;

			CarpoolDbContextSUT.Remove(
				CarpoolDbContextSUT.UserRides.Single(i => i.Id == baseEntity.Id));
			await CarpoolDbContextSUT.SaveChangesAsync();

			Assert.False(await CarpoolDbContextSUT.UserRides.AnyAsync(i => i.Id == baseEntity.Id));
		}
	}
}
