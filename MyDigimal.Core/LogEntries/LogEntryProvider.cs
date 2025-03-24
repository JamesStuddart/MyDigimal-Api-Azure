using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyDigimal.Common;
using MyDigimal.Data;
using MyDigimal.Data.Entities.CreatureLogs;
using MyDigimal.Core.Models.LogEntries;

namespace MyDigimal.Core.LogEntries
{
    public class LogEntryProvider(IUnitOfWork unitOfWork) : ILogEntryProvider
    {
        public async Task<LogEntryViewModel> GetLatestLogEntryExtendedAsync(Guid creatureId, Guid userId, Guid? logSchemaEntryId = null, Guid? schemaId = null, Guid? parentLogSchemaEntryId = null)
        {
            try{
                
                var foundEntries =
                    await unitOfWork.LogEntries.GetLatestExtendedByCreatureIdAsync(creatureId, logSchemaEntryId, schemaId, parentLogSchemaEntryId);

               return MapLogsToReturnModel(userId, foundEntries, []);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                  await unitOfWork.AbortAsync();
            }

            return new LogEntryViewModel();
        }
        
        public LogEntryViewModel MapLogsToReturnModel(Guid userId, IEnumerable<LogEntryEntityExtendedEntity> foundEntries, IEnumerable<int> availableYears)
        {
            var entries = foundEntries.GroupBy(x => x.CorrelationId)
                .Select(x =>
                {
                    var parent = x.FirstOrDefault(i => i.IsParent);
                    return  new LogEntryDetailModel
                    {
                        Id = parent?.Id,
                        LogSchemaEntryId = parent?.ParentLogSchemaEntryId,
                        Title = parent?.Title,
                        Type = parent?.Type,
                        CreatureId = parent?.CreatureId,
                        Notes = parent?.Notes,
                        Value = parent?.Value,
                        Owner = parent?.Owner == userId ? parent?.Owner.ToString() : null,
                        Date = parent?.Date?.ToString("yyyy-MM-dd"),
                        ChartType = parent?.ChartType != null ? (ChartType) parent.ChartType : ChartType.HeatMap,
                        LogEntries = x.Where(i => !i.IsParent).Select(i => new LogEntryDetailModel
                        {
                            Title = i.Title,
                            LogSchemaEntryId = i.LogSchemaEntryId,
                            CreatureId = i.CreatureId,
                            Type = i.Type,
                            Value = i.Value,
                            Owner = i.Owner == userId ? i.Owner.ToString() : null
                        }).OrderBy(o => o.Title),
                        availableYears = availableYears.OrderByDescending(y => y)
                    };
                }).OrderByDescending(x => x.Date);

            return new LogEntryViewModel {Entries = entries, AvailableYears = availableYears };
        }
    }
}