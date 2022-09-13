using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.DAL;
using Microsoft.EntityFrameworkCore;

namespace Carpool.Common.Tests.Factories
{
	public class DbContextSQLiteTestingFactory : IDbContextFactory<CarpoolDbContext>
	{
		private readonly string _databaseName;
		private readonly bool _seedTestingData;

		public DbContextSQLiteTestingFactory(string databaseName, bool seedTestingData = false)
		{
			_databaseName = databaseName;
			_seedTestingData = seedTestingData;
		}
		public CarpoolDbContext CreateDbContext()
		{
			DbContextOptionsBuilder<CarpoolDbContext> builder = new();
			builder.UseSqlite($"Data Source={_databaseName};Cache=Shared");

			// contextOptionsBuilder.LogTo(System.Console.WriteLine); //Enable in case you want to see tests details, enabled may cause some inconsistencies in tests
			// builder.EnableSensitiveDataLogging();

			return new CarpoolTestingDbContext(builder.Options, _seedTestingData);
		}
	}
}
