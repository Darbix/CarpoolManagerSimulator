using AutoMapper;
using Carpool.DAL.Entities;

namespace Carpool.BL.Models
{
	public record RideDetailModel(
		string Start,
		string End,
		DateTime Beginning,
		TimeSpan Duration,
		Guid DriverId
		) : ModelBase
	{
		public string Start { get; set; } = Start;
		public string End { get; set; } = End;
		public DateTime Beginning { get; set; } = Beginning;
		public TimeSpan Duration { get; set; } = Duration;
		public Guid DriverId { get; set; } = DriverId;
		public List<UserRideDetailModel> Passengers { get; init; } = new();

		public RideCarModel? Car { get; set; }

		public class MapperProfile : Profile
		{
			public MapperProfile()
			{
				CreateMap<RideEntity, RideDetailModel>()
					.ReverseMap()
					.ForMember(entity => entity.DriverId, expression => expression.Ignore())
					.ForMember(entity => entity.Driver, expression => expression.Ignore())
					// Car ID can be changed, but not car values
					.ForMember(entity => entity.Car, expression => expression.Ignore());
			}
		}
		public static RideDetailModel Empty => new(string.Empty, string.Empty, default, default, default);

	}
}
