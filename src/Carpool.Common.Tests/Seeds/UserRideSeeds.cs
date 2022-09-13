using Carpool.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carpool.Common.Tests.Seeds
{
	public static class UserRideSeeds
	{
		public static readonly UserRideEntity EmptyUserRide = new(
			Id: default,
			UserId: default,
			RideId: default
		);

		public static readonly UserRideEntity TomOstravaOpava = new(
			Id: Guid.Parse("2E6D83AF-4552-4D4E-B775-7E5D16205C85"),
			UserId: UserSeeds.UserPassengerTom.Id,
			RideId: RideSeeds.RideOstravaOpava.Id
		);

		public static readonly UserRideEntity JakubZnojmoBreclav = new(
			Id: Guid.Parse("0E4B9A2E-45CC-42C0-B9A2-62DA93AA2737"),
			UserId: UserSeeds.UserJakub.Id,
			RideId: RideSeeds.RideZnojmoBreclav.Id
		);

		public static readonly UserRideEntity JakubZnojmoBreclavDelete = JakubZnojmoBreclav with { Id = Guid.Parse("80491944-DB6C-4EE0-90D7-C62E47792263") };

		public static void Seed(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserRideEntity>().HasData(
				TomOstravaOpava,
				JakubZnojmoBreclav,
				JakubZnojmoBreclavDelete
			);
		}
	}
}
