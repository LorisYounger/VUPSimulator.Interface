using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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
        public Item_L2D_base(ILine line) : base(line)
        {
            //ItemIdentifier = line.Identy;
            //Identy = "item";
            //info = "l2d_base";//ItemType
        }
        public Item_L2D_base(Item_L2D_base l2dbase) : base(l2dbase)
        {
            //BasePath = l2dbase.BasePath;
        }

        /// <summary>
        /// 表情列表
        /// </summary>
        public List<(int expid, ExpressionType exp)> ExpressionList
        {
            get
            {
                if (expressionList != null) return expressionList;
                expressionList = new List<(int expid, ExpressionType exp)>();
                int expid = 0;
                while (Find($"exp{expid}") != null)
                {
                    var exp = (expid, (ExpressionType)Find($"exp{expid}").InfoToInt);
                    if (exp.Item2 != ExpressionType.none)
                    {
                        expressionList.Add(exp);
                    }
                    expid++;
                }
                return expressionList;
            }
        }
        /// <summary>
        /// 表情列表
        /// </summary>
        List<(int expid, ExpressionType exp)> expressionList;


        /// <summary>
        /// 表情类型
        /// </summary>
        public enum ExpressionType
        {
            /// <summary>  
            /// 占位符
            /// </summary>  
            none,
            /// <summary>  
            /// A普通
            /// </summary>  
            A_Nomal,
            /// <summary>  
            /// B开心
            /// </summary>  
            B_Happy,
            /// <summary>  
            /// C愤怒
            /// </summary>  
            C_Angry,
            /// <summary>  
            /// D悲伤
            /// </summary>  
            D_Sad,
            /// <summary>  
            /// E害羞
            /// </summary>  
            E_Shy,
            /// <summary>  
            /// F惊讶
            /// </summary>  
            F_Surprise,
            /// <summary>  
            /// G感动
            /// </summary>  
            G_Touch,
            /// <summary>  
            /// H害怕
            /// </summary>  
            H_Afraid,
            /// <summary>  
            /// I嘲笑
            /// </summary>  
            I_Derision,
            /// <summary>  
            /// J脸红
            /// </summary>  
            J_Redden,
            /// <summary>  
            /// K无语
            /// </summary>  
            K_Speechless,
            L_Other,
            M_Other,
            N_Other,
            O_Other,
        }

        /// <summary>
        /// 画师 (用于定位) OldPainterAuthor (ID)
        /// </summary>
        public string Painter => this[(gstr)"painter"];
        /// <summary>
        /// 真画师 (部分作者会被塞到其他集团名下)
        /// </summary>
        public string PainterReal
        {
            get
            {
                if (Find("painterreal") == null)
                {
                    return Painter;
                }
                else
                {
                    return Find("painterreal").Info;
                }
            }
        }
        /// <summary>
        /// 最大星级
        /// </summary>
        public double Max => Find("max").InfoToDouble;
        /// <summary>
        /// 最小星级
        /// </summary>
        public double Min => Find("min").InfoToDouble;
        /// <summary>
        /// 基础价格
        /// </summary>
        public double PriceBase => Find("pricebase").InfoToDouble;
        /// <summary>
        /// 表情价格
        /// </summary>
        public double PriceExp => Find("priceexp")?.InfoToDouble ?? PriceBase / 5;
        /// <summary>
        /// 预计花费时长
        /// </summary>
        public double Spendtime => Find("spendtime").InfoToDouble;
        ///// <summary>
        ///// 随机获得10%星级, 根据多给的钱进行判断
        ///// 同时消耗多给的钱和等
        ///// </summary>
        //public double RNDRANK(double PriceBetter)
        //{
        //    return Function.Rnd.Next((int)(Min * 100), (int)(Max * 100)) / 100;
        //}
        public Item_L2D CreateNew() => new Item_L2D(this);
        public Item_L2D CreateOld(ILine line) => new Item_L2D(this, line[(gdbe)"star"], line[(gdbe)"build"], line["haveexpression"].info)
        { ItemDisplayName = "初始L2D", Modeler = "LorisYounger" };
        public override bool AllowMultiple => false;
        /// <summary>
        /// L2D默认图片
        /// </summary>
        public override string Image
        {
            get
            {
                //L2D 使用 ExpressionList
                //if (ExpressionList.Count > 0)
                //{
                //    return 
                //}
                //else
                //    return base.Image;
                return $"l2d_{ItemIdentifier}_0";
            }
        }
        public ImageSource ImageSourse(IMainWindow mw, int expid) => mw.Core.ImageSources.FindImage($"l2d_{ItemIdentifier}_{expid}", "item_l2d");
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
            get => (ImageRank + ModelRank) * (Freshness + 100 + Math.Pow(ExpressionHave.Count, 1.5) * 10) / 200;
        }
        /// <summary>
        /// 立绘生产进度 (100%) 50%为立绘完成 100%为模型完成
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
            ExpressionADD(0);
        }
        /// <summary>
        /// 从已有进度开始新建一个L2d
        /// </summary>
        /// <param name="l2dbase">L2d的模型</param>
        /// <param name="imagerank">立绘分数</param>
        /// <param name="modlerank">建模分数</param>
        public Item_L2D(Item_L2D_base l2dbase, double imagerank, double modlerank, string expressionHave) : base(l2dbase)
        {
            info = "l2d";
            Freshness = 100;
            Process = 100;
            ImageRank = imagerank;
            ModelRank = modlerank;
            if (string.IsNullOrWhiteSpace(expressionHave))
                expressionHave = "0";
            FindorAdd("haveexpression").info = expressionHave;
        }
        /// <summary>
        /// 从已有进度开始新建一个L2d
        /// </summary>
        public Item_L2D(ILine line) : base(line)
        {

        }
        /// <summary>
        /// 表情列表 这是实际获得的表情
        /// </summary>
        public List<int> ExpressionHave
        {
            get => FindorAdd("haveexpression").GetInfos().Select(int.Parse).ToList();
            set => FindorAdd("haveexpression").info = string.Join(",", value);
        }
        /// <summary>
        /// 添加表情到表情列表 (自动去重)
        /// </summary>
        public void ExpressionADD(int newexp)
        {
            var exp = ExpressionHave;
            if (exp.Contains(newexp))
                return;
            exp.Add(newexp);
            ExpressionHave = exp;
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
