using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.Common.Tests.Seeds;
using Carpool.DAL;
using Microsoft.EntityFrameworkCore;

namespace Carpool.Common.Tests
{
	public class CarpoolTestingDbContext : CarpoolDbContext
	{
		private readonly bool _seedTestingData;

		public CarpoolTestingDbContext(DbContextOptions contextOptions, bool seedTestingData = false)
			: base(contextOptions, seedDemoData: false)
		{
			_seedTestingData = seedTestingData;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			if (_seedTestingData)
			{
				CarSeeds.Seed(modelBuilder);
				UserSeeds.Seed(modelBuilder);
				RideSeeds.Seed(modelBuilder);
				UserRideSeeds.Seed(modelBuilder);
			}
		}
	}
}
