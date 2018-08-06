using SimpleMonitor.Business.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonitor.Business.Repository
{
    public class GenericRepository<TEntity> where TEntity : BaseEntity  
    {
        internal SimpleMonitorContext context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(SimpleMonitorContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {

            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }

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

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            entity.InsertAuditValues();
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = (TEntity)dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if ((context.Entry(entityToDelete).State == EntityState.Detached))
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            //if (entityToUpdate.GetType() == typeof(BaseEntity))
            //    ((BaseEntity)entityToUpdate).UpdateAuditValues();

            entityToUpdate.UpdateAuditValues();

            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public int GetMaxID(string columnName, string entity)
        {
            return context.Database.SqlQuery<int>(string.Format("SELECT MAX({0}) FROM [dbo].{1}", columnName, entity)).FirstOrDefault();
        }

        public int GetMaxID(string columnName, string entity, string condition)
        {
            return context.Database.SqlQuery<int>(string.Format("SELECT ISNULL(MAX({0}),0) FROM [dbo].{1} WHERE {2}", columnName, entity, condition)).FirstOrDefault();
        }
    }
}
