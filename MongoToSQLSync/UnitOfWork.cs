namespace MongoToSQLSync
{
    using System;
    using System.Data.Entity;

    /// <summary>
    /// Catalog UnitOfWork class for CBI
    /// </summary>
    public class UnitOfWork<T> : IUnitOfWork<T>, IDisposable where T : class
    {
        #region Fields

        /// <summary>
        /// the data Context object
        /// </summary>
        private readonly DbContext dataContext;
        private readonly DbSet  dbSet;

        /// <summary>
        /// variable to check whether disposed was called
        /// </summary>
        private bool disposed;

        Repository<T> _repository;

        #endregion Fields

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public UnitOfWork(string connectionString)
        {
            if (!String.IsNullOrWhiteSpace(connectionString))
            {
                dataContext = new DbContext(connectionString);
                dbSet = dataContext.Set<T>();
            }
        }

        #endregion Constructors

        #region PropertiesRepository

        public Repository<T> Repository
        {
            get
            {
                return _repository ?? (_repository = new Repository<T>(dataContext));
            }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            dataContext.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.dataContext != null && !disposed)
            {
                if (disposing)
                {
                    dataContext.Dispose();
                }
            }

            disposed = true;
        }

        #endregion Methods
    }
}
