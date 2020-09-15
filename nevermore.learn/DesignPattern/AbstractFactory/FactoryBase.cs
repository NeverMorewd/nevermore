using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.learn.DesignPattern.AbstractFactory
{
    public abstract class FactoryBase
    {
        public abstract ITestInterface CreateFactoryOne();
        public abstract ITestInterface CreateFactoryTwo();
    }
}
