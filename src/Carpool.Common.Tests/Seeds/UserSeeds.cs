using Carpool.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carpool.Common.Tests.Seeds
{
	public static class UserSeeds
	{
		public static readonly UserEntity EmptyUserEntity = new(
			Id: default,
			Name: default!,
			Surname: default!,
			Age: default,
			PhotoUrl: default,
			PhoneNumber: default
		);

		public static readonly UserEntity UserPetr = new(
			Id: Guid.Parse("88A47675-517A-4C9B-8EEA-55C5DFAF13BA"),
			Name: "Petr",
			Surname: "Novak",
			Age: 28,
			PhotoUrl: "TODO",
			PhoneNumber: 123777666
		);

		public static readonly UserEntity UserJan = new(
			Id: Guid.Parse("A3FFCCFF-86F3-4C53-959E-60C02C5E510A"),
			Name: "Jan",
			Surname: "Hrabec",
			Age: 44,
			PhotoUrl: "TODO",
			PhoneNumber: 777666555
		);

		public static readonly UserEntity UserForUpdate = new(
			Id: Guid.Parse("B9FD2C50-8E52-4A36-B0BA-8B2035D91958"),
			Name: "Sonia",
			Surname: "Radová",
			Age: 31,
			PhotoUrl: "https://i.picsum.photos/id/778/300/300.jpg?hmac=cAXTk4j4OzP0xNemMLcxJuDe1ukB8h5fio44tt2D2hQ",
			PhoneNumber: 774123545
		);

		public static readonly UserEntity UserToDelete = new(
			Id: Guid.Parse("5BC491E1-1A3D-447D-BC94-6E9CE343A24D"), 
			Name: "Tim",
			Surname: "Mit",
			Age: 41,
			PhotoUrl: "https://i.picsum.photos/id/778/300/300.jpg?hmac=cAXTk4j4OzP0xNemMLcxJuDe1ukB8h5fio44tt2D2hQ",
			PhoneNumber: 774447545
			);


		public static readonly UserEntity UserWithNoCars = UserPetr with { Id = Guid.Parse("7A33F274-A559-4146-AE13-07AC40F8CBF0"), Name = "Filip" };
		public static readonly UserEntity UserToDelete2 = UserPetr with { Id = Guid.Parse("3A9FB60D-2BCE-4B9B-9A7E-FDD68E4CD46B"), Name = "Ondra" };
		public static readonly UserEntity UserDaniel = UserPetr with { Id = Guid.Parse("0C57CCD6-2CF6-4299-86C5-96B1C2746C6F"), Name = "Daniel" };
		public static readonly UserEntity UserDriver = UserPetr with { Id = Guid.Parse("C8090A65-A224-4651-831B-B505622517C7"), Name = "Erik", Surname = "Dobrý" };
		public static readonly UserEntity UserPassengerTom = UserPetr with { Id = Guid.Parse("E9AF2A3B-40B3-4E06-B506-92ACBD82139C"), Name = "Tom", Surname = "Nedobrý" };
		public static readonly UserEntity UserJakub = UserPetr with { Id = Guid.Parse("109A2197-B49F-4233-9A38-BB71237D97AA"), Name = "Jakub", Surname = "Bell" };

		public static void Seed(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserEntity>().HasData(
				UserPetr,
				UserWithNoCars,
				UserForUpdate,
				UserToDelete,
				UserToDelete2,
				UserDaniel,
				UserDriver,
				UserPassengerTom,
				UserJan,
				UserJakub);
		}
	}
}
