
namespace TodoLibrary.DataAccess
{
    /// <summary>
    /// Represents an interface for SQL data access.
    /// </summary>
    public interface ISqlDataAccess
    {
        /// <summary>
        /// Loads data from the database using the specified stored procedure, parameters, and connection string name.
        /// </summary>
        /// <typeparam name="T">The type of the data to load.</typeparam>
        /// <typeparam name="U">The type of the parameters.</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters for the stored procedure.</param>
        /// <param name="connectionStringName">The name of the connection string.</param>
        /// <returns>A task representing the asynchronous operation that returns a list of loaded data.</returns>
        Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName);

        /// <summary>
        /// Saves data to the database using the specified stored procedure, parameters, and connection string name.
        /// </summary>
        /// <typeparam name="T">The type of the data to save.</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters for the stored procedure.</param>
        /// <param name="connectionStringName">The name of the connection string.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveData<T>(string storedProcedure, T parameters, string connectionStringName);

        /// <summary>
        /// Executes a delete stored procedure using the specified stored procedure, parameters, and connection string name.
        /// </summary>
        /// <typeparam name="T">The type of the parameters.</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters for the stored procedure.</param>
        /// <param name="connectionStringName">The name of the connection string.</param>
        /// <returns>A task representing the asynchronous operation that returns the number of rows affected.</returns>
        Task<int> ExecuteDeleteStoredProcedure<T>(string storedProcedure, T parameters, string connectionStringName);
    }
}