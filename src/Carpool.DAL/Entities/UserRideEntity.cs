
namespace Carpool.DAL.Entities
{
	public record UserRideEntity(
		Guid Id,
		Guid UserId,
		Guid RideId) : IEntity
	{
	// Parameter-less constructor for Automapper
	#nullable disable
		public UserRideEntity() : this(default, default, default) { }
	#nullable enable
		public UserEntity? User { get; init; }
		public RideEntity? Ride { get; init; }

	}
}
