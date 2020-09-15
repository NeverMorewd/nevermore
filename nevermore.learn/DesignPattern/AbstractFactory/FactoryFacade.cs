using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.learn.DesignPattern.AbstractFactory
{
    public class FactoryFacade
    {
        public static FactoryBase CreateFactory(string aparam)
        {
            if (aparam.Contains("A"))
            {
                return new FactoryA();
            }
            else
            {
                return new FactoryB();
            }
        }
    }
}
