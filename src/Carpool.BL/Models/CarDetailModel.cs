using AutoMapper;
using Carpool.Common;
using Carpool.DAL.Entities;

namespace Carpool.BL.Models
{
	public record CarDetailModel(
		string Brand,
		CarTypes Type,
		DateTime FirstRegistration,
		string PhotoUrl,
		int NumOfEmptySeats,
		Guid UserId) : ModelBase
	{
		public string Brand { get; set; } = Brand;
		public CarTypes Type { get; set; } = Type;
		public DateTime FirstRegistration { get; set; } = FirstRegistration;
		public string PhotoUrl { get; set; } = PhotoUrl;
		public int NumOfEmptySeats { get; set; } = NumOfEmptySeats;
		public List<RideListModel> Rides { get; init; } = new(); 

		public Guid UserId { get; set; } = UserId; // Driver

		public class MapperProfile : Profile
		{
			public MapperProfile()
			{
				CreateMap<CarEntity, CarDetailModel>()
					.ReverseMap()
					.ForMember(entity => entity.UserId, expression => expression.Ignore())
					.ForMember(entity => entity.Rides, expression => expression.Ignore())
					.ForMember(entity => entity.User, expression => expression.Ignore());
			}
		}

		public static CarDetailModel Empty => new(string.Empty, default, default, string.Empty, default, default);
	}
}
