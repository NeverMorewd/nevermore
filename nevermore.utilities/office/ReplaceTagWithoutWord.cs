using Aspose.Words;
using Microsoft.Office.Interop.Word;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.utilities.office
{
    public class ReplaceTagWithoutWord
    {
        public  ReplaceTagWithoutWord()
        {
            string templatePath = @"E:\project\client-windows\CloudRoom\CloudRoom\bin\Debug\WpsRecord\模板2.docx";

            Aspose.Words.Document doc = new Aspose.Words.Document(templatePath);
            try
            {
                Hashtable tables = new Hashtable();
                tables.Add("{Tag1}", "【" +"撒大大萨达萨达萨达是"+ "】");
                tables.Add("{Tag2}", "撒大大萨达萨达萨达是");
                tables.Add("{Tag3}", "撒大大萨达萨达萨达是");
                //tables.Add("法定代表人", "1");
                //tables.Add("基金投资者", "1");
                //tables.Add("基金投资者住所", "1");
                //tables.Add("投资者身份证号", "1");
                //tables.Add("投资者性别", "1");
                //tables.Add("投资者联系方式", "1");
                //tables.Add("投资者年龄", "1");


                tables.Add("基金合同编号", "【" + "1" + "】");
                tables.Add("基金合同编号2", "1");
                tables.Add("基金合同名称", "《" + "1" + "私募基金合同》");
                tables.Add("金额大写", "1");//不带单位只读数字

                //tables.Add("金额小写", (String.Format("{0:N}", Utils.StrToDecimal(loanAmount, 0) * 10000)));
                //tables.Add("金额小写不乘一万", (String.Format("{0:N}", Utils.StrToDecimal(loanAmount, 0))));
                //tables.Add("付款日期", beginTime);
                //tables.Add("付款日期2", beginTime2);
                //tables.Add("收益开始日期", beginTime);
                //tables.Add("封闭开始日期", beginTime);
                //tables.Add("封闭结束日期", endTime);
                //tables.Add("封闭月数", totalMonth);
                //tables.Add("开户行", fundActBankName);
                //tables.Add("银行卡号", fundActBankNum);
                //tables.Add("基金账户名称", fundActName);
                //tables.Add("通知日期", beginTime);
                //tables.Add("基金名称", fundName + "私募投资");
                //tables.Add("委托人银行卡号", agentBankNum);
                //tables.Add("委托人开户行", agentBankName);
                //tables.Add("委托人账户名称", agentName);

                GetHTFile(doc, tables);
                string downname = @"E:\project\client-windows\CloudRoom\CloudRoom\bin\Debug\WpsRecord\模板2withValue.docx";
                doc.Save(downname);
            }
            catch (Exception ex)
            {

            }
        }
        public static void GetHTFile(Aspose.Words.Document doc, Hashtable table)
        {
            BookmarkCollection bookmarks = doc.Range.Bookmarks;
            foreach (Aspose.Words.Bookmark mark in bookmarks)
            {
                if (table.ContainsKey(mark.Name))
                {
                    mark.Text = table[mark.Name].ToString();
                }
            }
        }
    }
}
