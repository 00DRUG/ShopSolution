using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Common
{
    public class CursorResult<ProductDto>
    {
        public IEnumerable<ProductDto> Items { get; set; } = new List<ProductDto>();

        public int? NextCursor { get; set; }

        public bool HasNextPage => NextCursor.HasValue;
    }
}
