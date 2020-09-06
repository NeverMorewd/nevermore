using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.utilities.office
{
    public class insertPicToDoc
    {
        public void insert(object aFileName, string aPicName,string aBookMark)
        {
            Microsoft.Office.Interop.Word.Application worldApp = new Microsoft.Office.Interop.Word.Application();

            Microsoft.Office.Interop.Word.Document doc = null;
            object oMissing = System.Reflection.Missing.Value;
            doc = worldApp.Documents.Open(ref aFileName,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            try
            {
                //定义该插入图片是否为外部链接
                object linkToFile = false;
                //定义插入图片是否随word文档一起保存
                object saveWithDocument = true;

                //图片
                string replacePic = aPicName;
                if (doc.Bookmarks.Exists(aBookMark) == true)
                {
                    object bookMark = aBookMark;
                    //查找书签
                    doc.Bookmarks.get_Item(ref bookMark).Select();
                    //设置图片位置
                    worldApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    //在书签的位置添加图片
                    InlineShape inlineShape = worldApp.Selection.InlineShapes.AddPicture(replacePic, ref linkToFile, ref saveWithDocument, ref oMissing);
                    //设置图片大小
                    inlineShape.Width = 100;
                    inlineShape.Height = 100;
                    inlineShape.Select();
                    inlineShape.ConvertToShape().IncrementLeft(-60.0f);


                    //将图片设置浮动在文字上方
                    inlineShape.ConvertToShape().WrapFormat.Type = Microsoft.Office.Interop.Word.WdWrapType.wdWrapFront;



                }
            }
            catch
            {
                doc.Saved = false;
                //word文档中不存在该书签，关闭文档
                doc.Close(ref oMissing, ref oMissing, ref oMissing);
            }
        }
    }
}
