using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.utilities.MEF
{
    public class MEFTest
    {
        public static void Run()
        {
            Go go = new Go();

            //获取当前执行的程序集中所有的标有特性标签的代码段
            AssemblyCatalog catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            //将所有Export特性标签存放进组件容器中（其实是一个数组里面）
            CompositionContainer container = new CompositionContainer(catalog);

            //找到所传入对象中所有拥有Import特性标签的属性，并在组件容器的数组中找到与这些属性匹配的Export特性标签所标注的类，然后进行实例化并给这些属性赋值。
            //简而言之，就是找到与Import对应的Export所标注的类，并用这个类的实例来给Import所标注的属性赋值，用于解耦。
            container.ComposeParts(go);

            if (go.test1 != null)
            {
                go.test1.show();
            }
            Console.ReadLine();
        }
    }
    //定义一个测试接口
    interface ITest
    {
        void show();
    }

    //Export出去的类型和名称都要和Import标注的属性匹配，类型可以写ITest, 也可以写Test
    [Export("wakaka", typeof(ITest))]
    class Test1 : ITest
    {
        public void show()
        {
            Console.WriteLine("wakaka");
        }
    }

    [Export("hayaya", typeof(ITest))]
    class Test2 : ITest
    {
        public void show()
        {
            Console.WriteLine("hayaya");
        }
    }

    class Go
    {
        [Import("wakaka")]
        public ITest test1 { get; set; }
        [Import("hayaya")]
        public ITest test2 { get; set; }
    }
}
