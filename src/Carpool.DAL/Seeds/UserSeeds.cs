using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carpool.DAL.Seeds
{
	public static class UserSeeds
	{
		public static readonly UserEntity UserPetr = new UserEntity(
			Guid.Parse("88A47675-517A-4C9B-8EEA-55C5DFAF13BA"),
			"Petr",
			"Novák",
			28,
			"https://i.picsum.photos/id/193/200/200.jpg?hmac=JHo5tWHSRWvVbL3HX6rwDNdkvYPFojLtXkEGEUCgz6A",
			123777666
		);
		public static readonly UserEntity UserKlara = new UserEntity(
			Guid.Parse("1233D727-D211-4247-B1FD-219781A4B0A9"),
			"Klára",
			"Nováková",
			19,
			"https://i.picsum.photos/id/65/200/300.jpg?hmac=o9HaDBPcrCPi8zfB6MoTe6MNNVPsEN4orpzsHhCPlbU",
			778456196
		);
		public static readonly UserEntity UserDaniel = new UserEntity(
			Guid.Parse("EBF19B10-14D6-40FC-8372-733B72B75172"),
			"Daniel",
			"Velký",
			36,
			"https://i.picsum.photos/id/129/300/300.jpg?hmac=Xrvqa1wJZVwRt7oDthbt4Je4Lq-kcMqQFm3HIJ7C4qM",
			746123665
		);

		public static void Seed(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserEntity>().HasData(
				UserPetr,
				UserKlara,
				UserDaniel);
		}
	}
}
