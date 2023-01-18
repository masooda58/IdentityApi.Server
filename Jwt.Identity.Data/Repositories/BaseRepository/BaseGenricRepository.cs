using Jwt.Identity.Data.Context;
using Jwt.Identity.Domain.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Jwt.Identity.Data.Repositories.BaseRepository
{
    public abstract class BaseGenricRepository<TEntity> : IBaseGenricRepository<TEntity> where TEntity : class
    {

        internal IdentityContext context;
        internal DbSet<TEntity> dbSet;


        protected BaseGenricRepository(IdentityContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {

            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                         (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }

        }


        public abstract Task<TEntity> GetByAsync(object id);
        public abstract Task<TEntity> GetByIdNotraking(object id);

        public virtual async Task<TEntity> GetById(object id)
        {
            return await dbSet.FindAsync(id);
        }




        public virtual async Task InsertAsync(TEntity entity)
        {

            await dbSet.AddAsync(entity);


        }

        public virtual async Task DeleteAsync(object id)
        {

            TEntity entityToDelete = await GetByAsync(id);
            await DeleteAsync(entityToDelete);




        }

        public virtual async Task DeleteAsync(TEntity entityToDelete)
        {

            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
           

        }

        public virtual void Update(TEntity entityToUpdate)
        {
            bool isDetached = context.Entry(entityToUpdate).State == EntityState.Detached;
            if (isDetached)
                dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
             
        }
    }

}
