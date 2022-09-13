using Carpool.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carpool.Common.Tests.Seeds;

public static class RideSeeds
{
	public static readonly RideEntity RidePrahaBrno = new(
		Id: Guid.Parse("841E1760-A7E7-4724-9F73-7289D5A26FB6"),
		Start: "Praha",
		End: "Brno",
		Beginning: DateTime.Today.AddDays(5),
		Duration: TimeSpan.FromHours(3),
		DriverId: UserSeeds.UserPetr.Id,
		CarId: CarSeeds.CarSkoda.Id
	);
	public static readonly RideEntity RideOstravaOpava = new(
		Id: Guid.Parse("53085B3D-122F-4CFC-95C2-92DA3013319F"),
		Start: "Ostrava",
		End: "Opava",
		Beginning: DateTime.Today.AddDays(1),
		Duration: TimeSpan.FromMinutes(30),
		DriverId: UserSeeds.UserDriver.Id,
		CarId: CarSeeds.CarCitroen.Id
	);

	public static readonly RideEntity RidePrahaLiberec = RideOstravaOpava with { Id = Guid.Parse("1EFD4D40-E554-454A-A8FD-F71CA4528317"), End = "Liberec" };
	public static readonly RideEntity RideEsAs = RideOstravaOpava with { Id = Guid.Parse("B7486F77-4CCA-49E7-B75D-3F502724BA90"), Start = "Eš", End = "Aš" };

	public static readonly RideEntity RideZnojmoBreclav = new(
		Id: Guid.Parse("B515A6FB-7380-4988-806F-ACF09C21182E"),
		Start: "Znojmo",
		End: "Breclav",
		Beginning: DateTime.Today.AddDays(2),
		Duration: TimeSpan.FromMinutes(40),
		DriverId: UserSeeds.UserDriver.Id,
		CarId: CarSeeds.CarNissan.Id
	);

	public static readonly RideEntity RideZnojmoBreclavDelete = RideOstravaOpava with { Id = Guid.Parse("5749F0C1-5AAC-4370-BC37-5889B192FAB1"), Start = "JineZnojmo" };

	public static void Seed(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<RideEntity>().HasData(
			RidePrahaBrno,
			RideOstravaOpava,
			RidePrahaLiberec,
			RideEsAs,
			RideZnojmoBreclav,
			RideZnojmoBreclavDelete);
	}
}
