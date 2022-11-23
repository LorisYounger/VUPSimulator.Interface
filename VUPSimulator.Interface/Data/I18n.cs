using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;
namespace VUPSimulator.Interface.Data
{
    /// <summary>
    /// 多语言
    /// </summary>
    public class I18n
    {
        public I18n()
        {

        }
        /// <summary>
        /// 查询该ID的翻译
        /// </summary>
        /// <param name="i18nid">用于定义翻译的id</param>
        /// <returns>查找翻译,若无翻译则原路返回</returns>
        public string this[string i18nid]
        {
            get
            {
                if (I18N.TryGetValue(i18nid, out string value))
                {
                    return value;
                }
                else
                {
                    return i18nid;
                }
            }
            set => I18N[i18nid] = value;
        }
        public Dictionary<string, string> I18N = new Dictionary<string, string>();
        /// <summary>
        /// 语言名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 添加翻译对
        /// </summary>       
        public void Add(string i18nid, string value)
        {
            I18N[i18nid] = value;
        }
        /// <summary>
        /// 额外源
        /// </summary>
        /// <returns></returns>
        public I18n ExternalSource;
        /// <summary>
        /// 查找翻译
        /// </summary>
        /// <param name="i18nid">用于定义翻译的id</param>
        /// <returns></returns>
        public string Find(string i18nid)
        {
            if (I18N.TryGetValue(i18nid, out string value))
            {
                return value;
            }
            else
            {
                if (ExternalSource != null)
                {
                    return ExternalSource.Find(i18nid);
                }
                else
                {
                    return i18nid;
                }
            }
        }
    }

}
