using System;
using System.Threading.Tasks;
using MyDigimal.Core.Models.LogSchema;

namespace MyDigimal.Core.Schemas
{
    public interface ILogSchemaFactory
    {
        public Task<LogSchemaModel> BuildSchema(Guid id, Guid userId, bool includePublic = true, Guid? creatureId = null);
    }
}