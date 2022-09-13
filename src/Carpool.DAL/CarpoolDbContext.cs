using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.Common;
using Carpool.DAL.Entities;
using Carpool.DAL.Seeds;
using Microsoft.EntityFrameworkCore;

namespace Carpool.DAL
{
	public class CarpoolDbContext : DbContext
	{
		private readonly bool _seedDemoData;
		public CarpoolDbContext(DbContextOptions contextOptions, bool seedDemoData = false) : base(contextOptions)
		{
			_seedDemoData = seedDemoData;
		}

		public DbSet<CarEntity> Cars => Set<CarEntity>();
		public DbSet<UserEntity> Users => Set<UserEntity>();
		public DbSet<RideEntity> Rides => Set<RideEntity>();
		public DbSet<UserRideEntity> UserRides => Set<UserRideEntity>();


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<CarEntity>()
				.HasOne(i => i.User)
				.WithMany(i => i.Cars)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<RideEntity>()
				.HasOne(i => i.Driver)
				.WithMany(i => i.DriverRides)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<UserRideEntity>()
				.HasOne(i => i.User)
				.WithMany(i => i.PassengerRides)
				.OnDelete(DeleteBehavior.Restrict);

			// DAL.Seeds Test data for the database creation
			if (_seedDemoData)
			{
				CarSeeds.Seed(modelBuilder);
				UserSeeds.Seed(modelBuilder);
				RideSeeds.Seed(modelBuilder);
			}

		}
	}

}
