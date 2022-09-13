using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpool.DAL.Entities;

namespace Carpool.DAL.UnitOfWork
{
	public interface IUnitOfWork : IAsyncDisposable
	{
		IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
		Task CommitAsync();
	}
}
