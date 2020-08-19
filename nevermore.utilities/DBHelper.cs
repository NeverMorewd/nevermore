using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.utilities
{
    public class DBHelper
    {
        const string m_ibmDb2Conn = @"UsingLinkedServers=true;DatabaseType=DB2;DatabaseName=DV1;DataSource=CSM-DADB202.ANR.PSAEMEA;UserID=usradm;Password=pw4usradm;Naming=System;LibraryList=ITC_TAMS";
        const string m_sqlServerConn = @"Password=Z0ndagTest;User ID=eportal;Initial Catalog=TestPorting;Data Source=beanr-eprtdev01";
        public  static string GetDataSetUsingPagedStoredProcedure(
                    string aStoredProc, string[] aParamList,
                    int aStartRecord, int aMaxRecords)
        {
            String ibmDb2ConnParam = "'" + m_ibmDb2Conn.Replace("'", "''") + "'";
            string startParam = "'" + aStartRecord.ToString() + "'";
            string maxParam = "'" + aMaxRecords.ToString() + "'";
            string aQuery = aStoredProc + " " + ibmDb2ConnParam + ", " +
                    startParam + ", " + maxParam;
            foreach (string aParam in aParamList)
            {
                aQuery += ", '" + aParam.Replace("'", "''") + "'";
            }
            SqlDataAdapter sda = new SqlDataAdapter(aQuery, m_sqlServerConn);
            DataSet ds = new DataSet();
            try
            {
                sda.Fill(ds);
            }
            catch
            {
                return "Fails";
            }
            return "Success";
        }
    }
}
