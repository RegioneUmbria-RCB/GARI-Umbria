using System;
using System.Collections.Generic;

namespace AgronicaCoreAPI.models.entities
{
    public class PaginatedResult<T>
    {
        public List<T> items { get; set; }
        public int totalCount { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int totalPages => pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;
    }
}
