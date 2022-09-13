using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.Common;
using Carpool.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carpool.DAL.Seeds
{
	public static class CarSeeds
	{
		public static readonly CarEntity CarSkoda = new(
			Guid.Parse("6A0BABC5-0655-4B55-836A-9B48A50DACD8"),
			UserSeeds.UserPetr.Id,
			"Skoda",
			CarTypes.Sedan,
			DateTime.Now,
			"https://i.picsum.photos/id/551/300/300.jpg?hmac=LWVazeXVOTiBubPyE4TKWUtMT0BoiXmX_jZTGOo1XqM",
			4
		);
		public static readonly CarEntity CarAudi = new(
			Guid.Parse("DB6464FC-F811-4E76-9C18-992FDAF2B514"),
			UserSeeds.UserKlara.Id,
			"Audi",
			CarTypes.Cabriolet,
			DateTime.Now,
			"https://i.picsum.photos/id/1072/300/300.jpg?hmac=-cQDWArvLRB9rrmMvGpluNzMjvb_IWYgby4f62IH5Xw",
			1
		);
		public static readonly CarEntity CarFord = new(
			Guid.Parse("1A55B0E4-5054-456B-BAEB-9A9AD47C6A3F"),
			UserSeeds.UserDaniel.Id,
			"Ford",
			CarTypes.Hatchback,
			DateTime.Now,
			"https://i.picsum.photos/id/514/300/300.jpg?hmac=cWjIdfi9OoXTpWCC_x4On9HehQ1N0FSR6MxanIRMcfQ",
			3
		);

		public static void Seed(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CarEntity>().HasData(
				CarSkoda,
				CarAudi,
				CarFord);
		}
	}
}
