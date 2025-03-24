using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyDigimal.Core.Models.Creatures;

namespace MyDigimal.Core.Providers
{
    public interface ICreatureEventProvider
    {
        Task<IEnumerable<CreatureUpdateEventModel>> GetEventsByCreatureIdAsync(Guid id);
    }
}