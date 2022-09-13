namespace Carpool.DAL.Entities
{
	public record RideEntity(
		Guid Id,
		Guid DriverId,
		Guid CarId,
		string Start,
		string End,
		DateTime Beginning,
		TimeSpan Duration) : IEntity
	{
	// Parameter-less constructor for Automapper
	#nullable disable
		public RideEntity() : this(default, default, default, default, default, default, default) { }
	#nullable enable
		public UserEntity? Driver { get; init; }
		public CarEntity? Car { get; init; }
		public ICollection<UserRideEntity> Passengers { get; init; } = new List<UserRideEntity>();
	}
}
