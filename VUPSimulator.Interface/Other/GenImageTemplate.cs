using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;
using static VUPSimulator.Interface.EventBase;

namespace VUPSimulator.Interface.Other
{
    /// <summary>
    /// 生成图片用的模板
    /// </summary>
    /// 可能的使用例子:
    /// NiliNili视频封面
    /// 
    public class GenImageTemplate : Line
    {
        public enum GIType
        {
            Nili
        }
        public GIType Type;
        public GenImageTemplate(Line line) : base(line)
        {
            Type = (GIType)Enum.Parse(typeof(GIType), info, true);
        }
        /// <summary>
        /// 生成Nili视频的封面
        /// </summary>
        /// <returns></returns>
        public GenBase genImageNili(string text,string usrimg,string bgimg)
        {
            //Nili图片模板可能用到的参数
            Line clom = new Line(this);
            clom["GIText"].Infos["text"] = text;
            clom["GIBackGround"].Infos["bg"] = usrimg;
            return null;
        }
    }
}
