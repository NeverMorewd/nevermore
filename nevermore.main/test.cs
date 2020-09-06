using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.console
{
    public class TestSon : Test
    {
        public TestSon(string a)
        {
            m_ConnectionHandle = a;
        }
        public string ConnectionHandle { get { return m_ConnectionHandle; } }
    }
    public class Test
    {
        protected string m_ConnectionHandle;
    }
    public class TestRun
    {
        Test test = new TestSon("hahah");
        public string re;
        public TestRun()
        {
            re = test.GetType().GetProperty("ConnectionHandle").GetValue(test, null).ToString();
        }
    }
}
