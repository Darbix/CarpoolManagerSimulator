using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.Common;
using Carpool.Common.Tests;
using Carpool.Common.Tests.Seeds;
using Carpool.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Carpool.DAL.Tests
{
	public class DbContextCarTests : DbContextTestsBase
	{
		public DbContextCarTests(ITestOutputHelper output) : base(output)
		{
		}

		[Fact]
		public async Task GetAll_Cars_SeededCarSkoda_DoesNotThrow()
		{
			var carFromDb = await CarpoolDbContextSUT.Cars.FirstOrDefaultAsync(i => i.Id == CarSeeds.CarSkoda.Id);

			Assert.Equal(UserSeeds.UserPetr.Id, carFromDb?.UserId);
			DeepAssert.Equal(CarSeeds.CarSkoda, carFromDb);
		}

		[Fact]
		public async Task Add_New_Car()
		{
			CarEntity entity = new CarEntity()
			{
				Id = Guid.Parse("C5967EA6-B09D-471A-AE90-7490605E40CD"),
				UserId = UserSeeds.UserJakub.Id,
				Brand = "Lada",
				Type = CarTypes.Other,
				FirstRegistration = DateTime.Today.AddYears(-30),
				PhotoUrl = "https://i.picsum.photos/id/326/200/300.jpg?hmac=SKzjQ5ycCVyISiOfq2m-GqpQ5zWT_J202KPYG7z0uB4",
				NumOfEmptySeats = 3
			};

			CarpoolDbContextSUT.Cars.Add(entity);
			await CarpoolDbContextSUT.SaveChangesAsync();

			await using var dbx = await DbContextFactory.CreateDbContextAsync();
			var actualEntity = await dbx.Cars.SingleAsync(i => i.Id == entity.Id);
			DeepAssert.Equal(entity, actualEntity);
		}

		[Fact]
		public async Task Update_Car_Persisted()
		{
			var carFromDb = CarSeeds.CarOpel;
			var updatedCar =
				carFromDb with
				{
					Id = default,
					User = default,
					Brand = carFromDb.Brand + "v2",
					Type = Common.CarTypes.Cabriolet,
					FirstRegistration = default,
					PhotoUrl = carFromDb.PhotoUrl,
					NumOfEmptySeats = carFromDb.NumOfEmptySeats + 1
				};

			CarpoolDbContextSUT.Cars.Update(updatedCar);
			await CarpoolDbContextSUT.SaveChangesAsync();

			await using var dbx = await DbContextFactory.CreateDbContextAsync();
			var actualUpdatedCar = await dbx.Cars.SingleAsync(i => i.Id == updatedCar.Id);
			DeepAssert.Equal(updatedCar, actualUpdatedCar);
		}

		[Fact]
		public async Task Update_CarBrand()
		{
			var car = CarSeeds.CarMazda;

			var carUpdated = car with { Brand = "BMW" };

			CarpoolDbContextSUT.Cars.Update(carUpdated);
			await CarpoolDbContextSUT.SaveChangesAsync();

			await using var dbx = await DbContextFactory.CreateDbContextAsync();
			var actualUpdatedCar = await dbx.Cars.SingleAsync(i => i.Id == car.Id);

			Assert.Equal(car.Id, actualUpdatedCar.Id);
			Assert.NotEqual(car.Brand, actualUpdatedCar.Brand);
		}

		[Fact]
		public async Task Delete_Car()
		{
			CarEntity entityBase = await CarpoolDbContextSUT.Cars.FirstOrDefaultAsync(i => i.Id == CarSeeds.CarNissanDelete.Id)
				?? throw new NullReferenceException();
			DeepAssert.Equal(CarSeeds.CarNissanDelete, entityBase);

			CarpoolDbContextSUT.Cars.Remove(entityBase);
			await CarpoolDbContextSUT.SaveChangesAsync();

			Assert.False(await CarpoolDbContextSUT.Cars.AnyAsync(i => i.Id == entityBase.Id));
		}

		[Fact]
		public async Task DeleteById_Ingredient_WaterDeleted()
		{
			var entityBase = CarSeeds.CarNissanDelete;

			CarpoolDbContextSUT.Remove(
				CarpoolDbContextSUT.Cars.Single(i => i.Id == entityBase.Id));
			await CarpoolDbContextSUT.SaveChangesAsync();

			Assert.False(await CarpoolDbContextSUT.Cars.AnyAsync(i => i.Id == entityBase.Id));
		}
	}
}
