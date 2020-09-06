using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.console.CommandPattern
{
    public class SellStock : IOrder
    {
        private Stock abcStock;

        public SellStock(Stock abcStock)
        {
            this.abcStock = abcStock;
        }

        public void Execute()
        {
            abcStock.sell();
        }
    }
}
