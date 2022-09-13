using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.Common.Tests;
using Carpool.Common.Tests.Factories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Carpool.DAL.Tests
{
	public class DbContextTestsBase : IAsyncLifetime
	{
		protected DbContextTestsBase(ITestOutputHelper output)
		{
			XUnitTestOutputConverter converter = new(output);
			Console.SetOut(converter);

			// DbContextFactory = new DbContextTestingInMemoryFactory(GetType().Name, seedTestingData: true);
			// DbContextFactory = new DbContextLocalDBTestingFactory(GetType().FullName!, seedTestingData: true);
			DbContextFactory = new DbContextSQLiteTestingFactory(GetType().FullName!, seedTestingData: true);

			CarpoolDbContextSUT = DbContextFactory.CreateDbContext();
		}

		protected IDbContextFactory<CarpoolDbContext> DbContextFactory { get; }
		protected CarpoolDbContext CarpoolDbContextSUT { get; }


		public async Task InitializeAsync()
		{
			await CarpoolDbContextSUT.Database.EnsureDeletedAsync();
			await CarpoolDbContextSUT.Database.EnsureCreatedAsync();
		}

		public async Task DisposeAsync()
		{
			await CarpoolDbContextSUT.Database.EnsureDeletedAsync();
			await CarpoolDbContextSUT.DisposeAsync();
		}
	}
}
