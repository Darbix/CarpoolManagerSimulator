using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Carpool.DAL.Factories
{
	internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CarpoolDbContext>
	{
		public CarpoolDbContext CreateDbContext(string[] args)
		{
			DbContextOptionsBuilder<CarpoolDbContext> builder = new();
			builder.UseSqlServer(
				@"Data Source=(LocalDB)\MSSQLLocalDB;
                Initial Catalog = Carpool;
                MultipleActiveResultSets = True;
                Integrated Security = True; ");

			return new CarpoolDbContext(builder.Options, false); //TODO !!
		}
	}
}
