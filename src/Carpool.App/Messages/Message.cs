using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.BL.Models;

namespace Carpool.App.Messages
{
	public abstract record Message<T> : IMessage
	   where T : IModel
	{
		private readonly Guid? _id;

		public Guid? Id
		{
			get => _id ?? Model?.Id;
			init => _id = value;
		}

		public Guid? TargetId { get; init; }
		public T? Model { get; init; }
	}
}
