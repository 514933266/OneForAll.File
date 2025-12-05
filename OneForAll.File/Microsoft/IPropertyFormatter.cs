using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OneForAll.File
{
    /// <summary>
    /// 接口：对象属性转换
    /// </summary>
    public interface IPropertyFormatter
    {
        /// <summary>
        /// 转换（将属性转化为）
        /// </summary>
        /// <param name="proInfo">属性</param>
        /// <param name="t">对象</param>
        /// <returns>转换结果</returns>
        string Format(PropertyInfo proInfo, object t);
    }
}
