using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 所有的画基础类
    /// </summary>
    public class Draw : Item
    {
        public Draw(Line line) : base(line) { }
        /// <summary>
        /// 画师
        /// </summary>
        public string Painter
        {
            get => this[(gstr)"painter"];
            set => this[(gstr)"painter"] = value;
        }
        /// <summary>
        /// 星级
        /// </summary>
        public double Score
        {
            get => this[(gdbe)"score"];
            set => this[(gdbe)"score"] = value;
        }
        /// <summary>
        /// 画等级
        /// </summary>
        public double Rank
        {
            get => Find("rank").InfoToDouble;
            set => FindorAdd("rank").InfoToDouble = value;
        }

    }
    /// <summary>
    /// 老画师画作管理接口
    /// </summary>
    public interface IOldPainterDraw
    {

    }
}
