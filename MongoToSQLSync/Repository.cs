namespace MongoToSQLSync
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// the repository class
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        #region Fields

        /// <summary>
        /// Catalog context variable
        /// </summary>
        private readonly DbContext dbContext;

        /// <summary>
        /// dbset variable
        /// </summary>
        private readonly DbSet<TEntity> dbSet;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public Repository(DbContext context)
        {
            dbContext = context;
            dbSet = context.Set<TEntity>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id of the object to delete.</param>
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// Deletes the specified entity to delete.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }

            dbSet.Remove(entityToDelete);
        }


        /// <summary>
        /// Gets the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="orderBy">The order by expression.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="enableTracking">if set to <c>true</c> [enable tracking].</param>
        /// <returns>
        /// an IEnumerable of TEntity objects
        /// </returns>
        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool enableTracking = true)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (!enableTracking)
                query = query.AsNoTracking();

            foreach (var includeProperty in includeProperties.Split(
                new[] { ',' },
                StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return orderBy != null ? orderBy(query).ToList() : query.ToList();
        }

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id of the object to get.</param>
        /// <returns>the TEntity object corresponding to the id</returns>
        public virtual TEntity GetById(object id)
        {
            return dbSet.Find(id);
        }

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id to get.</param>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>the TEntity object corresponding to the id</returns>
        public virtual TEntity GetById(object id, Expression<Func<TEntity, ICollection<TEntity>>> navigationProperty, Expression<Func<TEntity, bool>> predicate)
        {
            var entity = dbSet.Find(id);
            dbContext.Entry(entity)
                     .Collection(navigationProperty)
                     .Query()
                     .Where(predicate)
                     .Load();

            return entity;
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Updates the specified entity to update.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            dbContext.Entry(entityToUpdate).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public virtual List<TResult> ExexuteStoreProc<TResult>(string sql, params object[] args)
        {
            return this.dbContext.Database.SqlQuery<TResult>(sql, args).ToList();
        }

        public virtual void ExexuteCommand(string sql, params object[] args)
        {
            ((IObjectContextAdapter)dbContext).ObjectContext.ExecuteStoreCommand(sql, args);
        }

        #endregion Methods

    }
}