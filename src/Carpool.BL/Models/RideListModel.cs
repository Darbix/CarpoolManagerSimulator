using AutoMapper;
using Carpool.DAL.Entities;

namespace Carpool.BL.Models
{
	public record RideListModel(
		string Start,
		string End,
		DateTime Beginning,
		TimeSpan Duration,
		string DriverName,
		string DriverSurname,
		int CarNumOfEmptySeats) : ModelBase
	{
		public string Start { get; set; } = Start;
		public string End { get; set; } = End;
		public DateTime Beginning { get; set; } = Beginning;
		public TimeSpan Duration { get; set; } = Duration;
		public string DriverName { get; set; } = DriverName;
		public string DriverSurname { get; set; } = DriverSurname;
		public int CarNumOfEmptySeats { get; set; } = CarNumOfEmptySeats;

		public class MapperProfile : Profile
		{
			public MapperProfile()
			{
				CreateMap<RideEntity, RideListModel>();
			}
		}
	}
}
