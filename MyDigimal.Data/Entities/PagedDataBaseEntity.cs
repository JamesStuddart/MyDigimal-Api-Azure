using System.Collections.Generic;

namespace MyDigimal.Data.Entities
{
    public class PagedDataBaseEntity<TModel> where TModel : class
    {
        public IEnumerable<TModel> Data { get; set; }
        
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}