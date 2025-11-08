using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneForAll.Core;
using OneForAll.File.Enums;

namespace OneForAll.File.Test
{
    [TestClass]
    public class FileHelperTest
    {
        private readonly string _sourceFilePath = @"C:\Users\xuhaopeng\Desktop\logo.png";
        private readonly string _targetFilePath = @"C:\Users\xuhaopeng\Desktop\test1.txt";

        [TestMethod]
        public void Write()
        {
            //FileHelper.Create(_sourceFilePath);
        }
        
        [TestMethod]
        public void WriteStream()
        {
            var data = FileHelper.Read(_sourceFilePath);
            FileHelper.Write(_targetFilePath, data);
        }

        [TestMethod]
        public void WriteByte()
        {
            var bytes = FileHelper.ReadByte(_sourceFilePath);
            FileHelper.Write(_targetFilePath, bytes);
        }

        [TestMethod]
        public void ReadStream()
        {
            var stream = FileHelper.Read(_sourceFilePath);
        }

        [TestMethod]
        public void ReadByte()
        {
            var bytes = FileHelper.ReadByte(_sourceFilePath);
        }

        [TestMethod]
        public void ReadByte2()
        {
            var bytes = FileHelper.ReadByte(_sourceFilePath, 4);
        }

        [TestMethod]
        public void Move()
        {
            FileHelper.Move(_sourceFilePath, _targetFilePath);
        }

        [TestMethod]
        public void MoveByCreateTime()
        {
            FileHelper.MoveByCreateTime(_sourceFilePath, _targetFilePath, DatePart.Day, 1);
        }

        [TestMethod]
        public void Copy()
        {
            FileHelper.Copy(_sourceFilePath, _targetFilePath, false, true);
        }

        [TestMethod]
        public void CheckIsWritable()
        {
            var writable = FileHelper.CheckIsWritable(_sourceFilePath);
        }

        [TestMethod]
        public void CheckIsExists()
        {
            var exists = FileHelper.CheckIsExists(_sourceFilePath);
        }

        [TestMethod]
        public void ValidateFileType()
        {
            var textStream = FileHelper.Read(_sourceFilePath);
            var result = FileHelper.ValidateFileType<ZipTypeEnum>(textStream, 4);
        }

        [TestMethod]
        public void ValidateFileName()
        {
            var result = FileHelper.ValidateFileName<DocTypeEnum>(_sourceFilePath);
        }
    }
}
