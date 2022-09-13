using AutoMapper;
using Carpool.DAL.Entities;

namespace Carpool.BL.Models
{
	public record UserDetailModel(
		string Name,
		string Surname,
		int Age,
		int PhoneNumber) : ModelBase
	{
		public string Name { get; set; } = Name;
		public string Surname { get; set; } = Surname;
		public int Age { get; set; } = Age;
		public int PhoneNumber { get; set; } = PhoneNumber;
		public string? PhotoUrl { get; set; }

		public List<CarListModel> Cars { get; init; } = new();
		public List<RideListModel> DriverRides { get; init; } = new();
		public List<UserRideDetailModel> PassengerRides { get; init; } = new();

		public class MapperProfile : Profile
		{
			public MapperProfile()
			{
				CreateMap<UserEntity, UserDetailModel>()
					.ReverseMap()
					.ForMember(entity => entity.Cars, expression => expression.Ignore())
					.ForMember(entity => entity.DriverRides, expression => expression.Ignore());
			}
		}

		public static UserDetailModel Empty => new(string.Empty, string.Empty, default, default);
	}
}
