using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Carpool.App.Commands;
using Carpool.App.Messages;
using Carpool.App.Services;
using Carpool.BL.Facades;
using Carpool.BL.Models;

namespace Carpool.App.ViewModels
{
	public class CreateUserDetailViewModel
	{
		private readonly IMediator _mediator;
		private readonly UserFacade _userFacade;

		public CreateUserDetailViewModel(UserFacade userFacade, IMediator mediator)
		{
			_mediator = mediator;

			UserModel = getDefaultUser();

			UserSaveCommand = new AsyncRelayCommand<Window?>(UserSavedAsync);

			_userFacade = userFacade;
		}

		/// <summary>
		/// User model being created or edited
		/// </summary>
		public UserDetailModel UserModel { get; set; }
		/// <summary>
		/// Command to save the new user model to the database
		/// </summary>
		public ICommand UserSaveCommand { get; }


		/// <summary>
		/// Get the default empty user detail model for the window load
		/// </summary>
		/// <returns>Blank User detail model</returns>
		private UserDetailModel getDefaultUser()
		{
			return new UserDetailModel(string.Empty, string.Empty, 0, 0);
		}

		/// <summary>
		/// Save/Update the user to the database
		/// </summary>
		/// <param name="window">This window to close</param>
		/// <returns></returns>
		private async Task UserSavedAsync(Window? window)
		{
			if(!string.IsNullOrEmpty(UserModel.Name))
			{
				var model = await _userFacade.SaveAsync(UserModel);

				_mediator.Send(new UpdateMessage<UserDetailModel> { Model = model });
				UserModel = getDefaultUser();
				window?.Close();
			}
		}
	}
}
