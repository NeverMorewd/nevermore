using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.learn.DesignPattern.AbstractFactory
{
    public class FactoryB : FactoryBase
    {
        public override ITestInterface CreateFactoryOne()
        {
            throw new NotImplementedException();
        }

        public override ITestInterface CreateFactoryTwo()
        {
            throw new NotImplementedException();
        }
    }
}
