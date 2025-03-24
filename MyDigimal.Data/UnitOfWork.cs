using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyDigimal.Common.Cryptography;
using MyDigimal.Data.Repositories.Implementations;
using MyDigimal.Data.Repositories.Implementations.CreatureLogs;
using MyDigimal.Data.Repositories.Implementations.Creatures;
using MyDigimal.Data.Repositories.Implementations.System;

namespace MyDigimal.Data
{
    public class UnitOfWork(IDbContext context, IEncryptor encryptor, ILogger<UnitOfWork> logger)
        : IUnitOfWork, IAsyncDisposable
    {
        private readonly IDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IEncryptor _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
        private readonly Dictionary<Type, object> _repositories = new();

        private TRepository GetRepository<TRepository>(Func<TRepository> factory) where TRepository : class
        {
            if (_repositories.TryGetValue(typeof(TRepository), out var repo))
            {
                return (TRepository)repo;
            }

            var newRepo = factory();
            _repositories[typeof(TRepository)] = newRepo;
            return newRepo;
        }

        public LogEntriesRepository LogEntries => GetRepository(() => new LogEntriesRepository(_context, _encryptor));
        public LogSchemasRepository LogSchemas => GetRepository(() => new LogSchemasRepository(_context, _encryptor));

        public LogSchemaEntriesRepository LogSchemaEntries =>
            GetRepository(() => new LogSchemaEntriesRepository(_context, _encryptor));

        public LogSchemaSuggestedSpeciesRepository LogSchemaSuggestedSpecies =>
            GetRepository(() => new LogSchemaSuggestedSpeciesRepository(_context, _encryptor));

        public CreaturesRepository Creatures => GetRepository(() => new CreaturesRepository(_context, _encryptor));

        public CreatureNotesRepository CreatureNotes =>
            GetRepository(() => new CreatureNotesRepository(_context, _encryptor));

        public CreatureImagesRepository CreatureImages =>
            GetRepository(() => new CreatureImagesRepository(_context, _encryptor));

        public CreatureGroupRepository CreatureGroups =>
            GetRepository(() => new CreatureGroupRepository(_context, _encryptor));

        public CreatureEventsRepository CreatureEvents =>
            GetRepository(() => new CreatureEventsRepository(_context, _encryptor));

        public UsersRepository Users => GetRepository(() => new UsersRepository(_context, _encryptor));

        public UserAuthPlatformRepository UserAuthPlatforms =>
            GetRepository(() => new UserAuthPlatformRepository(_context, _encryptor));

        public NewsRepository News => GetRepository(() => new NewsRepository(_context, _encryptor));

        public NotificationsRepository Notifications =>
            GetRepository(() => new NotificationsRepository(_context, _encryptor));

        public ReportingLogEntryRepository ReportingLogEntries =>
            GetRepository(() => new ReportingLogEntryRepository(_context));

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Commit failed.");
                throw;
            }
        }

        public async Task AbortAsync()
        {
            try
            {
                await _context.AbortChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Rollback failed.");
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_context is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                _context?.Dispose();
            }
        }
    }
}