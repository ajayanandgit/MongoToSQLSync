namespace MongoToSQLSync
{
    /// <summary>
    /// IUnitOfWork interface
    /// </summary>
    public interface IUnitOfWork<T> where T : class
    {
        #region Properties

        Repository<T> Repository { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save();

        #endregion Methods
    }
}
