using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyDigimal.Data.Entities.CreatureLogs;
using MyDigimal.Core.Models.LogEntries;

namespace MyDigimal.Core.LogEntries
{
    public interface ILogEntryProvider
    {
        Task<LogEntryViewModel> GetLatestLogEntryExtendedAsync(Guid creatureId, Guid userId,
            Guid? logSchemaEntryId = null, Guid? schemaId = null, Guid? parentLogSchemaEntryId = null);

        LogEntryViewModel MapLogsToReturnModel(Guid userId, IEnumerable<LogEntryEntityExtendedEntity> foundEntries,
            IEnumerable<int> availableYears);
    }
}