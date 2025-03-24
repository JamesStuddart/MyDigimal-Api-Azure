using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace MyDigimal.Data
{
    public class NpgSqlDbContext(IOptions<NpgSqlConnectionConfig> connectionConfig) : IDbContext
    {
        private IDbConnection _connection = new NpgsqlConnection(connectionConfig.Value.Connection);
        private IDbTransaction _transaction;
        private bool _disposed;
        private bool _initialized;

        private async Task BeginAsync()
        {
            if (!_initialized)
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _transaction = _connection.BeginTransaction();
                _initialized = true;
            }
        }

        public async Task SaveChangesAsync()
        {
            await BeginAsync();

            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
            }
        }

        public async Task AbortChangesAsync()
        {
            await BeginAsync();

            try
            {
                _transaction.Rollback();
            }
            catch
            {
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
            }
        }

        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, object param = null)
        {
            await BeginAsync();
            return await _connection.QueryAsync<TEntity>(query, param);
        }

        public async Task ExecuteAsync<TEntity>(string query, TEntity entity) where TEntity : class
        {
            await BeginAsync();
            await _connection.ExecuteAsync(query, entity);
        }

        public async Task<object> ExecuteScalarAsync<TEntity>(string query, TEntity entity) where TEntity : class
        {
            await BeginAsync();
            return await _connection.ExecuteScalarAsync(query, entity);
        }

        public async Task<TEntity> QuerySingleOrDefaultAsync<TEntity>(string query, object param)
        {
            await BeginAsync();
            return await _connection.QuerySingleOrDefaultAsync(query, param);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }

                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }

                _disposed = true;
            }
        }

        ~NpgSqlDbContext()
        {
            Dispose(false);
        }
    }
}