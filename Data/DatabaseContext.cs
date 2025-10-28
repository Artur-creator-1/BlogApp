using Npgsql;

namespace BlogTestTask.Data
{
    public class DatabaseContext
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseContext> _logger;

        public DatabaseContext(string connectionString, ILogger<DatabaseContext> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<NpgsqlConnection> GetConnectionAsync()
        {
            try
            {
                var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to open database connection: {ex.Message}");
                throw;
            }
        }

        public async Task<int> ExecuteAsync(string sql, params NpgsqlParameter[] parameters)
        {
            try
            {
                using var connection = await GetConnectionAsync();
                using var command = new NpgsqlCommand(sql, connection);

                if (parameters.Length > 0)
                    command.Parameters.AddRange(parameters);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL execution error: {ex.Message}\nSQL: {sql}");
                throw;
            }
        }

        public async Task<object?> ExecuteScalarAsync(string sql, params NpgsqlParameter[] parameters)
        {
            try
            {
                using var connection = await GetConnectionAsync();
                using var command = new NpgsqlCommand(sql, connection);

                if (parameters.Length > 0)
                    command.Parameters.AddRange(parameters);

                return await command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Scalar query error: {ex.Message}\nSQL: {sql}");
                throw;
            }
        }

        public async Task<NpgsqlDataReader> ExecuteReaderAsync(string sql, params NpgsqlParameter[] parameters)
        {
            try
            {
                var connection = await GetConnectionAsync();
                var command = new NpgsqlCommand(sql, connection);

                if (parameters.Length > 0)
                    command.Parameters.AddRange(parameters);

                return await command.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Reader query error: {ex.Message}\nSQL: {sql}");
                throw;
            }
        }
    }
}
