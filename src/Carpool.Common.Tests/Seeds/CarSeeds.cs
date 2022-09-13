using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carpool.Common.Tests.Seeds
{
	public static class CarSeeds
	{
		public static readonly CarEntity CarSkoda = new(
			Id: Guid.NewGuid(),
			UserId: UserSeeds.UserPetr.Id,
			Brand: "Skoda",
			Type: CarTypes.Sedan,
			FirstRegistration: DateTime.Now,
			PhotoUrl: "https://TODO",
			NumOfEmptySeats: 4
		);

		public static readonly CarEntity CarAudi = new(
			Guid.Parse("E699575A-D8B2-4696-A364-E0259E817525"),
			UserId: UserSeeds.UserPetr.Id,
			Brand: "Audi",
			Type: CarTypes.Cabriolet,
			FirstRegistration: DateTime.Today.AddDays(-16),
			PhotoUrl: "https://i.picsum.photos/id/555/200/200.jpg?hmac=SPdHg_AxaDTFgZCoJymemxudcniLOiP2P5k6T8Eb-kc",
			NumOfEmptySeats: 4
		);

		public static readonly CarEntity CarOpel = CarAudi with { Id = Guid.Parse("58F6A073-A653-49C5-A857-5D5DC82A3A7F"), Brand = "Opel", UserId = UserSeeds.UserDaniel.Id };
		public static readonly CarEntity CarCitroen = CarAudi with { Id = Guid.Parse("40D691FE-2B4C-444C-8201-3CF515C4D050"), Brand = "Citroen", UserId = UserSeeds.UserDriver.Id };
		public static readonly CarEntity CarVolvo = CarAudi with { Id = Guid.Parse("9012C54F-FC9D-4AB9-B415-B652544C7D09"), Brand = "Volvo", UserId = UserSeeds.UserDaniel.Id };

		public static readonly CarEntity CarMazda = new(
			Guid.Parse("19367769-24F9-4126-8276-052D45643468"),
			UserId: UserSeeds.UserJan.Id,
			Brand: "Mazda",
			Type: CarTypes.Cabriolet,
			FirstRegistration: DateTime.Today.AddDays(-50),
			PhotoUrl: "https://TODO",
			NumOfEmptySeats: 1
		);

		public static readonly CarEntity CarNissan = CarMazda with { Id = Guid.Parse("64390BF1-F610-437B-8179-C13EBE979110"), Brand = "Nissan", UserId = UserSeeds.UserDriver.Id };
		public static readonly CarEntity CarNissanDelete = CarNissan with { Id = Guid.Parse("E8C6D812-A066-4B1F-8A99-67B50A2C034E") };

		public static void Seed(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CarEntity>().HasData(
				CarSkoda,
				CarAudi,
				CarOpel,
				CarCitroen,
				CarVolvo,
				CarMazda,
				CarNissan,
				CarNissanDelete);
		}
	}
}
