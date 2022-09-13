using AutoMapper;
using Carpool.DAL.Entities;

namespace Carpool.BL.Models
{
	public record UserRideDetailModel(
		Guid RideId,
		string RideStart,
		string RideEnd,
		DateTime RideBeginning,
		TimeSpan RideDuration
		) : ModelBase
	{
		public Guid RideId { get; set; } = RideId;
		public string RideStart { get; set; } = RideStart;
		public string RideEnd { get; set; } = RideEnd;
		public DateTime RideBeginning { get; set; } = RideBeginning;
		public TimeSpan RideDuration { get; set; } = RideDuration;

		public RideCarModel? RideCar { get; set; }

		public class MapperProfile : Profile
		{
			public MapperProfile()
			{
				CreateMap<UserRideEntity, UserRideDetailModel>()
					.ReverseMap()
					.ForMember(entity => entity.Ride, expression => expression.Ignore())
					.ForMember(entity => entity.User, expression => expression.Ignore())
					.ForMember(entity => entity.UserId, expression => expression.Ignore());
			}
		}
	}
}
