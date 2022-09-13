using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Carpool.BL;
using Carpool.App.Settings;
using Carpool.DAL;
using Microsoft.Extensions.Options;
using Carpool.DAL.Factories;
using Carpool.App.ViewModels;
using Carpool.App.Services.MessageDialog;
using Carpool.App.Services;
using Carpool.App.ViewModels.Interfaces;
using Carpool.App.Extensions;
using System.Windows.Navigation;

namespace Carpool.App
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly IHost _host;
		public App()
		{
			_host = Host.CreateDefaultBuilder()
					.ConfigureAppConfiguration(ConfigureAppConfiguration)
					.ConfigureServices((context, services) => { ConfigureServices(context.Configuration, services); })
					.Build();

		}
		private static void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
		{
			builder.AddJsonFile(@"AppSettings.json", false, false);
		}

		private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddBLServices();

			services.Configure<DALSettings>(configuration.GetSection("Carpool:DAL"));

			services.AddSingleton<IDbContextFactory<CarpoolDbContext>>(provider =>
			{
				var dalSettings = provider.GetRequiredService<IOptions<DALSettings>>().Value;
				return new SqlServerDbContextFactory(dalSettings.ConnectionString!, dalSettings.SkipMigrationAndSeedDemoData);
			});

			services.AddSingleton<MainWindow>();

			services.AddSingleton<IMessageDialogService, MessageDialogService>();
			services.AddSingleton<IMediator, Mediator>();

			services.AddSingleton<MainViewModel>();
			services.AddSingleton<CreateUserDetailViewModel>();
			services.AddSingleton<CreateRideDetailViewModel>();
			services.AddSingleton<CreateCarDetailViewModel>();
			services.AddSingleton<RideDetailViewModel>();

			services.AddSingleton<IUserListViewModel, UserListViewModel>();
			services.AddSingleton<IUserDetailViewModel, UserDetailViewModel>();
			services.AddSingleton<IRideListViewModel, RideListViewModel>();
			services.AddSingleton<IAllRidesListViewModel, AllRidesListViewModel>();
			services.AddSingleton<ICarListViewModel, CarListViewModel>();
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			await _host.StartAsync();

			var dbContextFactory = _host.Services.GetRequiredService<IDbContextFactory<CarpoolDbContext>>();

			var dalSettings = _host.Services.GetRequiredService<IOptions<DALSettings>>().Value;

			await using (var dbx = await dbContextFactory.CreateDbContextAsync())
			{
				if (dalSettings.SkipMigrationAndSeedDemoData)
				{
					await dbx.Database.EnsureDeletedAsync();
					await dbx.Database.EnsureCreatedAsync();
				}
				else
				{
					await dbx.Database.MigrateAsync();
				}
			}

			var mainWindow = _host.Services.GetRequiredService<MainWindow>();
			mainWindow.Show();

			base.OnStartup(e);
		}

		protected override async void OnExit(ExitEventArgs e)
		{
			using (_host)
			{
				await _host.StopAsync(TimeSpan.FromSeconds(5));
			}

			base.OnExit(e);
		}
	}
}
