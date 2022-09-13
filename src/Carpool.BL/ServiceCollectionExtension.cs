using Carpool.BL.Facades;
using Carpool.DAL;
using Carpool.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.EquivalencyExpression;


namespace Carpool.BL
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddBLServices(this IServiceCollection services)
		{
			services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
			services.AddSingleton<UserFacade>();
			services.AddSingleton<CarFacade>();
			services.AddSingleton<RideFacade>();

			services.AddAutoMapper((serviceProvider, cfg) =>
			{
				cfg.AddCollectionMappers();

				var dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<CarpoolDbContext>>();
				using var dbContext = dbContextFactory.CreateDbContext();
				cfg.UseEntityFrameworkCoreModel<CarpoolDbContext>(dbContext.Model);
			}, typeof(BusinessLogic).Assembly);
			return services;
		}	
	}
}
