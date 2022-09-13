using Carpool.Common;

namespace Carpool.DAL.Entities
{
	public record CarEntity(
		Guid Id,
		Guid UserId,
		string Brand,
		CarTypes Type,
		DateTime FirstRegistration,
		string PhotoUrl,
		int NumOfEmptySeats) : IEntity
	{
	// Parameter-less constructor for Automapper
	#nullable disable
		public CarEntity() : this(default, default, default, default, default, default, default) { }
	#nullable enable

		public UserEntity? User { get; init; }
		public ICollection<RideEntity> Rides { get; init; } = new List<RideEntity>();
	}
}
