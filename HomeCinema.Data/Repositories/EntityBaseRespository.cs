using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Entities;

namespace HomeCinema.Data.Repositories
{
    public class EntityBaseRespository<T> : IEntityBaseRepository<T> where T:class, IEntityBase,new()
    {
        private HomeCinemaContext dataContext;
        #region Properties

        protected IDbFactory DbFactory { get; private set; }
        protected HomeCinemaContext DbContext => dataContext ?? (dataContext = DbFactory.Init());

        public EntityBaseRespository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }
        #endregion

        public IQueryable<T> GetAll()
        {
            return DbContext.Set<T>();
        }

        public IQueryable<T> All => GetAll();

        public IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbContext.Set<T>();
            return includeProperties.Aggregate(query, (returnItem, includeProperty) => returnItem.Include(includeProperty));
        }

        public T GetSingle(int id)
        {
            return GetAll().FirstOrDefault(x => x.ID == id);
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return DbContext.Set<T>().Where(predicate);
        }

        public void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            DbContext.Set<T>().Add(entity);
        }

        public void Edit(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }
    }
}
