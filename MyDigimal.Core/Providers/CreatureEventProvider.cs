using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyDigimal.Common;
using MyDigimal.Data;
using MyDigimal.Data.Entities.Creatures;
using MyDigimal.Core.Models.Creatures;

namespace MyDigimal.Core.Providers
{
    public class CreatureEventProvider : ICreatureEventProvider
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatureEventProvider(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CreatureUpdateEventModel>> GetEventsByCreatureIdAsync(Guid id)
        {
            var history = await _unitOfWork.CreatureEvents.GetByCreatureIdAsync(id);
            _unitOfWork.AbortAsync();
            
            return await Process(history);
        }

        private async Task<IEnumerable<CreatureUpdateEventModel>> Process(IEnumerable<CreatureEventEntity> events)
        {
            
            var returnEvents = events.Select(async x =>
            {            
                var creator = await _unitOfWork.Users.GetByIdAsync(x.CreatedBy);

                var eventType = (CreatureEventType) x.Event;
                var eventName = "Updated Details";
                var eventDescription = string.Empty;
                
                switch (eventType)
                {
                    case CreatureEventType.Generic:
                        eventName = "Updated Details";
                        eventDescription = string.Empty;
                        break;
                    case CreatureEventType.Created:
                        eventName = "Updated Details";
                        eventDescription = string.Empty;
                        break;
                    case CreatureEventType.OwnerChange:
                        eventName = "Owner Change";
                        eventDescription = string.Empty;
                        break;
                    case CreatureEventType.StatusChange:
                        eventName = "Updated Details";
                        eventDescription = string.Empty;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                return new CreatureUpdateEventModel
                {
                    CreatureId = x.CreatureId,
                    Name = eventName,
                    Description = eventDescription,
                    Value = x.NewValue,
                    EventDate = x.EventDate,
                    CreatedBy = creator.Name
                };
            });

            return await Task.WhenAll(returnEvents.ToArray());
        }
        
    }
}