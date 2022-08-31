using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;

namespace VUPSimulator.Interface
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
            Nili,
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
            clom["GIText"].Infos["t"] = text;
            clom["GIImage"].Infos["p"] = usrimg;
            clom["GIBackGround"].Infos["bg"] = bgimg;
            return new GenBase(clom);
        }
        /// <summary>
        /// 生成Nili视频的封面
        /// </summary>
        /// <returns></returns>
        public GenBase genImageNili(Sub gi)
        {
            //Nili图片模板可能用到的参数
            Line clom = new Line(this);
            clom["GIText"].Infos["t"] = gi.Infos["git"];
            clom["GIImage"].Infos["p"] = gi.Infos["gii"];
            clom["GIBackGround"].Infos["bg"] = gi.Infos["gibg"];
            return new GenBase(clom);
        }
    }

    /// <summary>
    /// 自动生成图片基本类, 可以生成图片控件,方便多次调用
    /// </summary>
    public class GenBase : Line
    {
        public GenBase(Line line) : base(line) { }
        public GenImage Create(IMainWindow mw)
        {
            return new GenImage(mw, this);
        }
    }
}
