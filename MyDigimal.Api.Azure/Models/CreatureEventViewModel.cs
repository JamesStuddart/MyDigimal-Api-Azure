using MyDigimal.Common;

namespace MyDigimal.Api.Azure.Models
{
    public class CreatureEventViewModel
    {
        public Guid CreatureId { get; set; }
        public CreatureEventType Event { get; set; }
        public string ValueName { get; set; }
        public string OriginalValue { get; set; }
        public string NewValue { get; set; }
        public DateTime EventDate { get; set; }
    }
}