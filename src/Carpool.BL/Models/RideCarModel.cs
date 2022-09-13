
using AutoMapper;
using Carpool.Common;
using Carpool.DAL.Entities;

namespace Carpool.BL.Models
{
	// Used for Ride-Car Mapping
	public record RideCarModel(
		Guid Id,
		string Brand,
		CarTypes Type,
		string PhotoUrl,
		int NumOfEmptySeats
	)
	{
		public Guid Id { get; set; } = Id;
		public string Brand { get; set; } = Brand;
		public CarTypes Type { get; set; } = Type;
		public string PhotoUrl { get; set; } = PhotoUrl;
		public int NumOfEmptySeats { get; set; } = NumOfEmptySeats;

		public class MapperProfile : Profile
		{
			public MapperProfile()
			{
				CreateMap<CarEntity, RideCarModel>();

			}
		}
	}
}
