using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// L2D基础类
    /// </summary>
    public class Item_L2D_base : Item
    {
        ///// <summary>
        ///// 图片位置//图片位置使用图片数据库
        ///// </summary>
        //public string BasePath
        //{
        //    get => FindorAdd("path").Info;
        //    set => FindorAdd("path").Info = value;
        //}
        public Item_L2D_base(Line line) : base(line)
        {
            //ItemName = line.Name;
            //Name = "item";
            //info = "l2d_base";//ItemType
        }
        public Item_L2D_base(Item_L2D_base l2dbase) : base(l2dbase)
        {
            //BasePath = l2dbase.BasePath;
        }
        ///// <summary>
        ///// 基础图片位置 现在统一用 Image
        ///// </summary>
        //public string Nomal => Find("nomal").Info;
        /// <summary>
        /// 表情
        /// </summary>
        public string[] Expression => Find("expression").GetInfos();
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
        public Item_L2D CreateNew() => new Item_L2D(this);
        public Item_L2D CreateNew(int process, double imagerank, double modlerank) => new Item_L2D(this, process, imagerank, modlerank);
        public Item_L2D CreateOld(double imagerank, double modlerank, string name) => new Item_L2D(this, 100, imagerank, modlerank) { ItemName = name, Modeler = "LorisYounger" };
    }
    /// <summary>
    /// L2D完整
    /// </summary>
    public class Item_L2D : Item_L2D_base
    {
        /// <summary>
        /// 立绘等级
        /// </summary>
        public double ImageRank
        {
            get => Find("imagerank").InfoToDouble;
            set => FindorAdd("imagerank").InfoToDouble = value;
        }
        /// <summary>
        /// 建模等级
        /// </summary>
        public double ModelRank
        {
            get => Find("modelrank").InfoToDouble;
            set => FindorAdd("modelrank").InfoToDouble = value;
        }
        public string Modeler { get => this[(gstr)"modeler"]; set => this[(gstr)"modeler"] = value; }
        /// <summary>
        /// 实际综合等级
        /// </summary>
        public double TotalRank
        {//根据新鲜度计算,最低不低于50%
            get => (ImageRank + ModelRank) * (Freshness + 100) / 200;
        }
        /// <summary>
        /// 立绘生产进度 (100%)
        /// </summary>
        public int Process
        {
            get => Find("process").InfoToInt;
            set => FindorAdd("process").InfoToInt = value;
        }
        /// <summary>
        /// 新鲜度
        /// 新鲜度越低实际等级越低(根据时间恢复)
        /// </summary>
        public int Freshness
        {
            get => Find("freshness").InfoToInt;
            set
            {
                if (value > 100)
                {
                    value = 100;
                }
                else if (value < 0)//最低新鲜程度为0
                {
                    value = 0;
                }
                FindorAdd("freshness").InfoToInt = value;
            }
        }
        /// <summary>
        /// 从零开始新建一个L2d
        /// </summary>
        /// <param name="l2dbase">L2d的模型</param>
        public Item_L2D(Item_L2D_base l2dbase) : base(l2dbase)
        {
            info = "l2d";
            Freshness = 100;
            Process = 0;//50%为立绘完成 100%为模型完成
            ImageRank = 0;//等级根据进度会缓慢增加,每次增加 (min-max)/10
            ModelRank = 0;
        }
        /// <summary>
        /// 从已有进度开始新建一个L2d
        /// </summary>
        /// <param name="l2dbase">L2d的模型</param>
        /// <param name="process">生产进度,只有100才可以使用</param>
        /// <param name="imagerank">立绘分数</param>
        /// <param name="modlerank">建模分数</param>
        public Item_L2D(Item_L2D_base l2dbase, int process, double imagerank, double modlerank) : base(l2dbase)
        {
            info = "l2d";
            Freshness = 100;
            Process = process;
            ImageRank = imagerank;
            ModelRank = modlerank;
        }
        /// <summary>
        /// 从已有进度开始新建一个L2d
        /// </summary>
        public Item_L2D(Line line) : base(line)
        {

        }
        /// <summary>
        /// 表情列表 这是实际获得的表情
        /// </summary>
        public string[] ExpressionList => FindorAdd("haveexpression").GetInfos();
        /// <summary>
        /// 添加表情到表情列表
        /// </summary>
        /// <param name="newexp"></param>
        public void ExpressionADD(string newexp)
        {
            Sub sb = FindorAdd("haveexpression");
            if (sb.info == "")
            {
                sb.info = newexp;
            }
            else
            {
                sb.info += "," + newexp;
            }
        }
        /// <summary>
        /// 删除表情从表情列表
        /// </summary>
        /// <param name="rmexp"></param>
        public void ExpressionDel(string rmexp)
        {
            Sub sb = FindorAdd("haveexpression");
            if (sb.info == rmexp)
                sb.info = "";
            else
                sb.info = sb.info.GetString().Replace(rmexp + ',', "");
        }
        /// <summary>
        /// 转换成等级为星星
        /// </summary>
        /// <param name="rank">等级</param>
        /// <returns></returns>
        public static string ToRankStar(double rank)
        {
            int str = (int)(rank * 2);
            int str2 = str / 2;
            string s = "";
            for (int i = 0; i < str2; i++)
                s += "★";
            if (str % 2 == 1)
                s += "☆";
            return s;
        }
    }
}
