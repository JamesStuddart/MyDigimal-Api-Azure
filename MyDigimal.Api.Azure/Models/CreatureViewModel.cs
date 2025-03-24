using MyDigimal.Data.Entities.Creatures;

namespace MyDigimal.Api.Azure.Models
{
    public class CreatureViewModel : CreatureEntity
    {
        internal bool IsValid => !string.IsNullOrEmpty(Name) && LogSchemaId != Guid.Empty;
    }
}