using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.DTOs
{
    public record StockUpdateMessage(int ProductId, int NewQuantity);
}
