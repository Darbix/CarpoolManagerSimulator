using AutoMapper;
using Carpool.Common;
using Carpool.DAL.Entities;

namespace Carpool.BL.Models
{
	public record CarListModel(
		string Brand,
		CarTypes Type,
		int NumOfEmptySeats) : ModelBase
	{
		public string Brand { get; set; } = Brand;
		public CarTypes Type { get; set; } = Type;
		public int NumOfEmptySeats { get; set; } = NumOfEmptySeats;
		public string? PhotoUrl { get; set; }

		public class MapperProfile : Profile
		{
			public MapperProfile()
			{
				CreateMap<CarEntity, CarListModel>();
			}
		}
	}
}
