using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyDigimal.Common;
using MyDigimal.Core.Models.LogEntries;
using MyDigimal.Data;
using MyDigimal.Core.LogEntries;
using MyDigimal.Core.Models.LogSchema;
using Newtonsoft.Json;

namespace MyDigimal.Core.Schemas
{
    public class LogSchemaFactory(IUnitOfWork unitOfWork, ILogEntryProvider logEntryProvider) : ILogSchemaFactory
    {
        public async Task<LogSchemaModel> BuildSchema(Guid id, Guid userId, bool includePublic = true, Guid? creatureId = null)
        {
            var schema = await unitOfWork.LogSchemas.GetByIdAsync(id, userId, includePublic);

            if (schema == null)
            {
                throw new Exception("Unable to locate schema");
            }
            
            var parentEntries = (await unitOfWork.LogSchemaEntries.GetBySchemaIdAsync(id)).ToList();

            var parentId = parentEntries.Where(x=> x.ParentId == null).Select(x => x.Id);
            
            var childEntries = (await unitOfWork.LogSchemaEntries.GetByParentEntryIdsAsync(parentId)).ToList();


            if (creatureId.HasValue)
            {
                foreach (var parentEntry in parentEntries.Where(x => x.ParentId == null && x.RepeatLastEntry))
                {
                    var logEntryView = await logEntryProvider.GetLatestLogEntryExtendedAsync(creatureId.Value, userId, null, parentEntry.SchemaId, parentEntry.Id);

                    if (logEntryView == null || !logEntryView.Entries.Any()) continue;
                    
                    var latestEntry = logEntryView.Entries.FirstOrDefault();

                    if (latestEntry == null) continue;
                    
                    parentEntry.DefaultValue = latestEntry.Value;

                    foreach (var childEntry in childEntries.Where(x =>
                        x.ParentId == parentEntry.Id && x.RepeatLastEntry))
                    {
                        var latestChildEntry =
                            latestEntry.LogEntries.FirstOrDefault(c => c.Title == childEntry.Title);
                        
                        if(latestChildEntry == null) continue;
                        
                        childEntry.DefaultValue = latestChildEntry.Value;
                    }
                }
            }

            await unitOfWork.AbortAsync();
            return new LogSchemaModel
            {
                Id = schema.Id,
                Title = schema.Title,
                IsPublic = schema.IsPublic,
                Author = schema.Author,
                Created = schema.Created,
                Modified = schema.Modified,
                RecommendedSpecies = schema.RecommendedSpecies,
                RecommendedCommonNames = schema.RecommendedCommonNames,
                Entries = parentEntries.Where(x=> x.ParentId == null).Select(x => new LogSchemaEntriesModel
                {
                    Id = x.Id,
                    Icon = x.Icon,
                    SchemaId = x.SchemaId,
                    ParentId = x.ParentId,
                    Title = x.Title,
                    Type = x.Type,
                    Index = x.Index,
                    QuickAction = x.QuickAction,
                    ChartType = (ChartType)x.ChartType,
                    Values = x.Values != null ? JsonConvert.DeserializeObject<IEnumerable<string>>(x.Values) : null,
                    DefaultValue = x.DefaultValue,
                    Required = x.Required,
                    RepeatLastEntry = x.RepeatLastEntry,
                    ChildEntries = childEntries.Any(c => c.ParentId == x.Id) ? childEntries.Where(c => c.ParentId == x.Id).Select(c => new LogSchemaEntriesModel
                        {
                            Id = c.Id,
                            Icon = c.Icon,
                            SchemaId = c.SchemaId,
                            ParentId = c.ParentId,
                            Title = c.Title,
                            Type = c.Type,
                            Index = c.Index,
                            DefaultValue = c.DefaultValue,
                            Required = c.Required,
                            RepeatLastEntry = c.RepeatLastEntry,
                            Values = c.Values != null ? JsonConvert.DeserializeObject<IEnumerable<string>>(c.Values) : null
                        }).OrderBy(c => c.Index) : null
                }).OrderBy(x=>x.Index)
            };
        }
    }
}