namespace MongoToSQLSync
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// generic IRepository interface
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntity>
        where TEntity : class
    {
        #region Methods

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id of the object to delete.</param>
        void Delete(object id);

        /// <summary>
        /// Deletes the specified entity to delete.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        void Delete(TEntity entityToDelete);

        /// <summary>
        /// Gets the specified filter.
        /// </summary>
        /// <param name="filter">The filter condition.</param>
        /// <param name="orderBy">The order by condition.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="enableTracking">if set to <c>true</c> [enable tracking].</param>
        /// <returns>
        /// the TEntity object from th db
        /// </returns>
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", bool enableTracking = true);

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id of the object to get.</param>
        /// <returns>the objec of type TEntity </returns>
        TEntity GetById(object id);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Updates the specified entity to update.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        void Update(TEntity entityToUpdate);

        #endregion Methods
    }
}