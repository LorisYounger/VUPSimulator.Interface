using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 食品基本类
    /// </summary>
    public abstract class Food : Item_Salability
    {
        public Food(Line line) : base(line)
        {

        }

        /// <summary>
        /// 增加 精力:体力
        /// </summary>
        public double StrengthFood
        {
            get => this[(gdbe)"strengthfood"];
            set => this[(gdbe)"strengthfood"] = value;
        }
        /// <summary>
        /// 增加 体力:睡眠
        /// </summary>
        public double StrengthSleep
        {
            get => this[(gdbe)"strengthsleep"];
            set => this[(gdbe)"strengthsleep"] = value;
        }
        /// <summary>
        /// 使用该物品
        /// </summary>
        public void Use(IMainWindow mw)
        {
            mw.Save.StrengthFood += StrengthFood;
            mw.Save.StrengthSleep += StrengthSleep;
            Many -= 1;
            if (Many == 0)
                mw.Save.Items.Remove(this);
        }
    }
}
