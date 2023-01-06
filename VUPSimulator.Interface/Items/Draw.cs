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
        ///// <summary>
        ///// 图片位置//图片位置使用图片数据库
        ///// </summary>
        //public string BasePath
        //{
        //    get => FindorAdd("path").Info;
        //    set => FindorAdd("path").Info = value;
        //}
        public Draw(Line line) : base(line) { }
        public Draw(Item_L2D_base l2dbase) : base(l2dbase) { }
        /// <summary>
        /// 画师
        /// </summary>
        public string Painter => this[(gstr)"painter"];
        /// <summary>
        /// 最大星级
        /// </summary>
        public double Max => Find("max").InfoToDouble;
        /// <summary>
        /// 最小星级
        /// </summary>
        public double Min => Find("min").InfoToDouble;
        /// <summary>
        /// 随机获得10%星级
        /// </summary>
        public double RNDRANK
        {
            get
            {
                return Function.Rnd.Next((int)(Min * 10), (int)(Max * 10)) * 0.1;
            }
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
