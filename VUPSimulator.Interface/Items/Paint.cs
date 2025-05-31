using LinePutScript;
using LinePutScript.Localization.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 由画师绘制的物品,例如L2D等
    /// </summary>
    public class Item_Paint : Item
    {
        public Item_Paint(ILine line) : base(line)
        {

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
        /// 预计花费时长
        /// </summary>
        public double Spendtime => Find("spendtime").InfoToDouble;
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
        /// 实际综合等级 (星级)
        /// </summary>
        public virtual double TotalRank
        {
            get => Find("totalrank").InfoToDouble;
            set => FindorAdd("totalrank").Info = value.ToString();
        }
        /// <summary>
        /// 物品介绍 (包括属性等)
        /// </summary>
        public virtual string PaintIntroduce => "作品名称: {0}\n画师: {1}".Translate(ItemDisplayName, PainterReal);
    }
}
