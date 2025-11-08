# OneForAll.File 文件类库
### 1. 文件校验
#### Doc、Image、Zip等常用文件的hex校验和扩展名校验，所有类继承基础接口IValidateFileType
```C#
// 图片校验
ValidateImageType: IValidateImageType
// 文档校验
ValidateDocType: IValidateDocType
// 压缩文件校验
ValidateZipType: IValidateZipType
```
#### 示例
1. 实例调用方式
```C#
new ValidateZipType.Validate(string fileName, Stream file)
```
2. 依赖注入接口调用方式
```C#
IValidateZipType.Validate(string fileName, Stream file)
```
### 2. 文件操作
#### 示例
1. FileHelper：基础操作
```C#
// 创建空白文件
FileHelper.Create(string filePath)
// 写入
FileHelper.Write(string filePath, Stream stream)
// 读取
FileHelper.ReadStream(string filePath, Stream stream)
// 移动
FileHelper.Move(string source, string target)
// 复制
FileHelper.Copy(string source, string target, bool deleteSource = false, bool overWrite = true)
// 获取文件信息
FileHelper.GetList(string path)
```
2. TextHelper：文本文件操作
```C#
// 创建空白文件
TextHelper.Create(string fileName, bool recover)
// 写入
TextHelper.Write(string fileName, string content)
// 读取
TextHelper.Read(string path, Encoding encoding = null)
```
3. DirectoryHelper：目录操作
```C#
// 创建
DirectoryHelper.Create(string path)
// 移动
DirectoryHelper.Move(string sourceDir, string targetDir)
// 移动文件
DirectoryHelper.MoveFiles(string directorySource, string directoryTarget, SearchOption option)
// 复制
DirectoryHelper.Copy(string source, string target)
```
4. Uploader：文件上传器
```C#
await new Uploader().WriteAsync(Stream fileStream, string path, string fileName, bool autoName = false, int maxSize = 0)
```
5. NPOIExcelHelper：基于NPOI的Excel文件操作
```C#
// 导出
NPOIExcelHelper.Export(List<DataTable> dts, FileType type, string filePath, int[] noWriteColumns = null, bool isWriteColumnHeader = false)
// 导入
NPOIExcelHelper.Import<T>(string filePath)
NPOIExcelHelper.Import(Stream stream, FileType type=FileType.xlsx, bool isFirstTitle = false)
```
