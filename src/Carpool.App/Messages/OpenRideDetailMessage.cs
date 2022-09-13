using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.BL.Models;

namespace Carpool.App.Messages
{
	public record OpenRideDetailMessage<T> : Message<T>
		where T : IModel
	{
	}
}
