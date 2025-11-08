using OneForAll.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneForAll.File.Test
{
    [TestClass]
    public class DirectoryHelperTest
    {
        [TestMethod]
        public void Copy()
        {
            string source = @"C:\Users\xuhaopeng\Desktop\test";
            string dirTarget = @"C:\Users\xuhaopeng\Desktop\test1";
            DirectoryHelper.Copy(source, dirTarget);
        }
    }
}
