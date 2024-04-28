using LinePutScript.Localization.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 音乐文件
    /// </summary>
    public class Music
    {
        /// <summary>
        /// 音乐类型
        /// </summary>
        public enum MusicType
        {
            /// <summary>
            /// 默认
            /// </summary>
            Default,
            /// <summary>
            /// 欢快
            /// </summary>
            Cheerful,
            /// <summary>
            /// 悲伤
            /// </summary>
            Sad,
            /// <summary>
            /// 自定义: 不允许抽取 
            ///</summary>
            DIY
        }
        /// <summary>
        /// 音乐类型
        /// </summary>
        public MusicType Type;
        /// <summary>
        /// 音乐名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 音乐文件路径
        /// </summary>
        public string Path;

        private string transname = null;
        /// <summary>
        /// 翻译后的名字
        /// </summary>
        public string TransName
        {
            get
            {
                if (transname == null) transname = Name.Translate();
                return transname;
            }
        }
        public Music(FileInfo path)
        {
            Name = path.Name;
            Path = path.FullName;
            if (Enum.TryParse<MusicType>(path.Directory.Name, true, out var musictype))
            {
                Type = musictype;
            }
            else
            {
                Type = MusicType.Default;
            }
        }
        public Music()
        {

        }
    }
}
