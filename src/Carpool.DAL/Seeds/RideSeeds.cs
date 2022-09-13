using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carpool.DAL.Seeds
{
	public class RideSeeds
	{
		public static readonly RideEntity ridePrahaLiberec = new RideEntity(
			Id: Guid.Parse("E40A9135-4F2E-47C0-B318-2369F7CD90C1"),
			DriverId: UserSeeds.UserPetr.Id,
			CarId: CarSeeds.CarSkoda.Id,
			Start: "Praha",
			End: "Liberec",
			Beginning: DateTime.ParseExact("24/7/2022 10:15", "d/M/yyyy H:mm", CultureInfo.InvariantCulture),
			Duration: TimeSpan.FromMinutes(55)
			);

		public static readonly RideEntity rideOlomoucBrno = new RideEntity(
			Id: Guid.Parse("A7C86E31-AE08-41ED-ACE4-FAFB7607CC16"),
			DriverId: UserSeeds.UserKlara.Id,
			CarId: CarSeeds.CarAudi.Id,
			Start: "Olomouc",
			End: "Brno", 
			Beginning: DateTime.ParseExact("30/8/2022 15:30", "d/M/yyyy H:mm", CultureInfo.InvariantCulture),
			Duration: TimeSpan.FromMinutes(30)
			);
		public static readonly RideEntity rideZnojmoBreclav = new RideEntity(
			Id: Guid.Parse("6B5DFBD8-1789-4226-8C61-B4B7B9ED7FCE"),
			DriverId: UserSeeds.UserDaniel.Id,
			CarId: CarSeeds.CarFord.Id,
			Start: "Znojmo",
			End: "Břeclav",
			Beginning: DateTime.ParseExact("1/6/2022 6:00", "d/M/yyyy H:mm", CultureInfo.InvariantCulture),
			Duration: TimeSpan.FromMinutes(26)
			);

		public static void Seed(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<RideEntity>().HasData(
				ridePrahaLiberec,
				rideOlomoucBrno,
				rideZnojmoBreclav);
		}
	}
}
