using Unity;

namespace nevermore.learn.IOC
{
    public interface ISqlHelper
    {
        string SqlConnection();
    }

    public interface IOtherHelper
    {
        string GetSqlConnection();
    }
    public class MssqlHelper : ISqlHelper
    {
        public string SqlConnection()
        {
            return "this mssql.";
        }
    }
    public class MysqlHelper : ISqlHelper
    {
        public string SqlConnection()
        {
            return "this mysql.";
        }
    }

    public class MyOtherHelper : IOtherHelper
    {
        ISqlHelper sql;
        public MyOtherHelper(ISqlHelper sql)
        {
            this.sql = sql;
        }
        public string GetSqlConnection()
        {
            return this.sql.SqlConnection();
        }
    }

    public class RunLearn
    {
        public static void Run()
        {
            IUnityContainer mycontainer = new UnityContainer();


            //已有对象实例的配置容器注册
            // MysqlHelper d = new MysqlHelper();
            //mycontainer.RegisterInstance<ISqlHelper>(d);

            //类型的配置容器注册
            mycontainer.RegisterType<ISqlHelper, MysqlHelper>();

            //配置文件注册
            //UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            //section.Configure(mycontainer);
            //mycontainer.LoadConfiguration(); 

            //调用依赖
            ISqlHelper mysql = mycontainer.Resolve<ISqlHelper>();

            //构造函数注入
            mycontainer.RegisterType<IOtherHelper, MyOtherHelper>();
            IOtherHelper other = mycontainer.Resolve<IOtherHelper>();
        }
        
    }
}