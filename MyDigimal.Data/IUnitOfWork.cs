using System.Threading.Tasks;
using MyDigimal.Data.Repositories.Implementations;
using MyDigimal.Data.Repositories.Implementations.CreatureLogs;
using MyDigimal.Data.Repositories.Implementations.Creatures;
using MyDigimal.Data.Repositories.Implementations.System;

namespace MyDigimal.Data
{
    public interface IUnitOfWork
    {
        public LogEntriesRepository LogEntries { get; }
        public LogSchemasRepository LogSchemas { get; }
        public LogSchemaEntriesRepository LogSchemaEntries { get; }
        public LogSchemaSuggestedSpeciesRepository LogSchemaSuggestedSpecies { get; }
        public CreaturesRepository Creatures { get; }
        public CreatureImagesRepository CreatureImages { get; }
        public CreatureNotesRepository CreatureNotes { get; }
        public CreatureGroupRepository CreatureGroups { get; }
        public CreatureEventsRepository CreatureEvents { get; }
        public UsersRepository Users { get; }
        public UserAuthPlatformRepository UserAuthPlatforms { get; }
        public UserExternalAuthRepository UserExternalAuth { get; }
        public NewsRepository News { get; }
        public NotificationsRepository Notifications { get; }
        public ReportingLogEntryRepository ReportingLogEntries { get; }
        

        Task CommitAsync();
        Task AbortAsync();
    }
}