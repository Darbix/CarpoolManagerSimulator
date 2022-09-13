namespace Carpool.DAL.Entities
{
	public record UserEntity(
		Guid Id,
		string Name,
		string Surname,
		int Age,
		string? PhotoUrl,
		int PhoneNumber) : IEntity
	{
		public ICollection<CarEntity> Cars { get; init; } = new List<CarEntity>();
		public ICollection<RideEntity> DriverRides { get; init; } = new List<RideEntity>();
		public ICollection<UserRideEntity> PassengerRides { get; init; } = new List<UserRideEntity>();

	}

}
