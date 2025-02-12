

namespace ChatFPT.Application.Repositories
{
    using global::ChatFPT.Application.Interface;
    using global::ChatFPT.Insfracstructure.Base;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace ChatFPT.Infrastructure.Repositories
    {
        public class UnitOfWork(ChatBoxDBContext dbContext) : IUnitOfWork
        {
            private bool disposed = false;
            private readonly ChatBoxDBContext _dbContext = dbContext;

            public IGenericRepository<T> GetRepository<T>() where T : class
            {
                return new GenericRepository<T>(_dbContext);
            }
            public void Save()
            {
                _dbContext.SaveChanges();
            }
            public async Task SaveAsync()
            {
                await _dbContext.SaveChangesAsync();
            }
            protected virtual void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        _dbContext.Dispose();
                    }
                }
                disposed = true;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void BeginTransaction()
            {
                _dbContext.Database.BeginTransaction();
            }

            public void CommitTransaction()
            {
                _dbContext.Database.CommitTransaction();
            }

            public void RollBack()
            {
                _dbContext.Database.RollbackTransaction();
            }
        }
    }
}
    
