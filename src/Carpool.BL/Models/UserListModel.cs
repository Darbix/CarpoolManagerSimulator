using AutoMapper;
using Carpool.DAL.Entities;

namespace Carpool.BL.Models
{
	public record UserListModel(
		string Name,
		string Surname) : ModelBase
	{
		public string Name { get; set; } = Name;
		public string Surname { get; set; } = Surname;
		public string? PhotoUrl { get; set; }

		public class MapperProfile : Profile
		{
			public MapperProfile()
			{
				CreateMap<UserEntity, UserListModel>();
			}
		}
	}
}
