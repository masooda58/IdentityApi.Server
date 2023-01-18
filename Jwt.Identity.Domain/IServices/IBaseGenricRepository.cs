using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Jwt.Identity.Framework.Response;

namespace Jwt.Identity.Domain.IServices
{
    public interface IBaseGenricRepository<TEntity> where TEntity : class
    {
        public IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
        public  Task<TEntity> GetByAsync(object id);
        public Task<TEntity> GetById(object id);
     
        public Task InsertAsync(TEntity entity);
        public Task DeleteAsync(object id);

        public Task DeleteAsync(TEntity entityToDelete);
        public  void Update(TEntity entityToUpdate);
    }
}