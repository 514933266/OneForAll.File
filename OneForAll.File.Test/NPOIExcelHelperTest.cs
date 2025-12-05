using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneForAll.Core;
using OneForAll.Core.Utility;
using OneForAll.File.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace OneForAll.File.Test
{
    [TestClass]
    public class NPOIExcelHelperTest
    {
        private readonly string _sourceFilePath = @"C:\Users\xhp\Desktop\成交总表.xlsx";

        [TestMethod]
        public void Import()
        {
            var stream = FileHelper.Read(_sourceFilePath);
            var dts = NPOIExcelHelper.Import(stream);
            var obj = ReflectionHelper.ToList<WmsGoodsImport>(dts.First(), out List<ValidateTableResult> errors);
        }

        [TestMethod]
        public void Export()
        {
            var stream = FileHelper.Read(_sourceFilePath);
            var dts = NPOIExcelHelper.Import(stream, FileTypeEnum.Xlsx, true);
            //NPOIExcelHelper.Export(dts, FileType.xlsx, @"G:\测试.xlsx", new int[] { 3, 0, 2 }, true, false);

            var dts2 = new List<WmsGoodsImport>();
            for (var i = 0; i < 10000; i++)
            {
                dts2.Add(new WmsGoodsImport()
                {
                    TypeName = "饮料",
                    Number = i,
                    Name = "雪碧",
                    Url = "http://www.baidu.com",
                    BuyTime = DateTime.Now
                });
            }
            //var table = dts2.ToTable();
            NPOIExcelHelper.EntityExport(dts2, FileTypeEnum.Xlsx, @"G:\测试.xlsx", new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, true, false);
        }
    }

    /// <summary>
    /// 物品导入对象
    /// </summary>
    [Display(Name = "物品表")]
    public class WmsGoodsImport
    {
        [Display(Name = "类型名称")]
        public string TypeName { get; set; } = "";

        public int Number { get; set; }

        [Display(Name = "名称")]
        public string Name { get; set; }
        public string Specification { get; set; }

        public string Unit { get; set; }

        public string Description { get; set; }

        [Display(Name = "链接")]
        public string Url { get; set; }

        [Display(Name = "购买日期")]
        [DisplayFormat(DataFormatString = "yyyy年MM月dd日")]
        public DateTime? BuyTime { get; set; }

        [Display(Name = "创建日期")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
