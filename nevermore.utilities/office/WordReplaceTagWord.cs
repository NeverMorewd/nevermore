using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;

namespace nevermore.utilities.office
{
    public class WordReplaceTagWord
    {
        public static void Replace(string fileFullName)
        {

            Word.Application app = null;
            Word.Document doc = null;

            string newFile = DateTime.Now.ToString("yyyyMMddHHmmssss") + ".doc";
            string physicNewFile = "";
            try
            {
                app = new Word.Application();
                object fileName = fileFullName;
                object oMissing = System.Reflection.Missing.Value;
                doc = app.Documents.Open(ref fileName,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                //构造数据
                Dictionary<string, string> datas = new Dictionary<string, string>();
                datas.Add("{name}", "张三");
                datas.Add("{sex}", "男");
                datas.Add("{provinve}", "浙江");
                datas.Add("{address}", "浙江省杭州市");
                datas.Add("{education}", "本科");
                datas.Add("{telephone}", "12345678");
                datas.Add("{cardno}", "123456789012345678");

                object replace = Word.WdReplace.wdReplaceAll;
                foreach (var item in datas)
                {
                    app.Selection.Find.Replacement.ClearFormatting();
                    app.Selection.Find.ClearFormatting();
                    app.Selection.Find.Text = item.Key;
                    app.Selection.Find.Replacement.Text = item.Value;

                    //执行替换操作
                    app.Selection.Find.Execute(
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref replace,
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing);
                }
                doc.SaveAs(physicNewFile,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            }
            catch (Exception e)
            {

            }
            finally
            {
                if (doc != null)
                {
                    doc.Close();//关闭word文档
                }
                if (app != null)
                {
                    app.Quit();//退出word应用程序
                }
            }
        }
    }
}
