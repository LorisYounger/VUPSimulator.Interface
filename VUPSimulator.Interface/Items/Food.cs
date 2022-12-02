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
        /// 健康影响
        /// </summary>
        public double Health
        {
            get => this[(gdbe)"health"];
            set => this[(gdbe)"health"] = value;
        }
        /// <summary>
        /// 健康影响范围
        /// </summary>
        public double HealthRange
        {
            get => this[(gdbe)"healthrange"];
            set => this[(gdbe)"healthrange"] = value;
        }
        /// <summary>
        /// 健康影响范围长度
        /// </summary>
        public double HealthRangeLength
        {
            get => GetDouble("healthrangelength", 5);
            set => this[(gdbe)"healthrangelength"] = value;
        }
        /// <summary>
        /// 状态影响
        /// </summary>
        public int PlayerState
        {
            get => this[(gint)"playerstate"];
            set => this[(gint)"playerstate"] = value;
        }
        /// <summary>
        /// 状态持续时间
        /// </summary>
        public double PlayerStateLength
        {
            get => this[(gdbe)"playerstatelength"];
            set => this[(gdbe)"playerstatelength"] = value;
        }
        /// <summary>
        /// 使用该物品
        /// </summary>
        public void Use(IMainWindow mw)
        {
            if (Many == 0)
            {
                mw.Save.Items.Remove(this);
                return;
            }
            mw.Save.StrengthFood += StrengthFood;
            mw.Save.StrengthSleep += StrengthSleep;
            //在范围内生效health
            if (Health != 0)
            {
                var rh = 1 - Math.Pow(mw.Save.Health - HealthRange, 2) / 2 / Math.Pow(HealthRangeLength, 2);
                if (rh >= 0)
                    mw.Save.Health += rh * Health;
            }
            //状态系统应用
            if (PlayerStateLength > 0)
            {
                mw.Save.PStateSystem.AddState((Interface.PlayerState.StateType)PlayerState, PlayerStateLength, $"吃 {ItemName}", Interface.PlayerState.TagType.Food);
            }
            Many -= 1;
            if (Many == 0)
                mw.Save.Items.Remove(this);

        }
        /// <summary>
        /// 物品描述
        /// </summary>
        public override string Description
        {
            get
            {
                string ret = GetString("desc", ItemDisplayName);
                var v = StrengthFood;
                if (v != 0)
                    ret += $"\n卡路里: {StrengthFood * 10:f1}大卡";
                v = StrengthSleep;
                if (v != 0)
                    ret += $"\n咖啡因: {StrengthSleep:f1}mg";
                var h = Health;
                if (h != 0)
                {
                    string heleff;
                    if (h > 0)
                    {//正面影响
                        if (h <= 0.02)
                            heleff = "几乎无良性";
                        else if (h <= 0.1)
                            heleff = "略微良性";
                        else if (h <= 0.2)
                            heleff = $"轻微良性(+{h:f2})";
                        else if (h <= 0.5)
                            heleff = $"中等良性(+{h:f2})";
                        else if (h <= 1)
                            heleff = $"大幅度良性(+{h:f2})";
                        else if (h <= 5)
                            heleff = $"非常良性(+{h:f2})";
                        else
                            heleff = $"(+{h:f2})";
                    }
                    else
                    {//负面
                        if (h >= -0.02)
                            heleff = "几乎无负面";
                        else if (h >= -0.1)
                            heleff = "略微负面";
                        else if (h >= -0.2)
                            heleff = $"轻微负面({h:f2})";
                        else if (h >= -0.5)
                            heleff = $"中等负面({h:f2})";
                        else if (h >= -1)
                            heleff = $"显著负面({h:f2})";
                        else if (h >= -5)
                            heleff = $"严重负面({h:f2})";
                        else
                            heleff = $"({h:f2})";
                    }
                    ret += $"\n健康影响: {heleff}";

                    var hr = HealthRange;
                    if (hr != 0)
                    {
                        var hrl = HealthRangeLength;
                        ret += $" [有效范围{hr - hrl}-{hr + hrl}]";
                    }
                }
                if (PlayerStateLength > 0)
                {
                    ret += $"\n心情作用: {PlayerStateSystem.PlayerState[PlayerState][0]}[{4 - PlayerState}] ({Function.DateConvert(PlayerStateLength)})";
                }
                return ret;
            }
        }
    }
    /// <summary>
    /// 零食
    /// </summary>
    public class Food_NoHealth : Food
    {
        public Food_NoHealth(Line line) : base(line)
        {

        }

        public override double SortValue => 4 - PlayerState;
        public override string[] Categories => new string[] { "食品", "零食" };

    }
    /// <summary>
    /// 主食
    /// </summary>
    public class Food_Health : Food
    {
        public Food_Health(Line line) : base(line)
        {

        }

        public override double SortValue => StrengthFood;
        public override string[] Categories => new string[] { "食品", "主食" };
    }
    /// <summary>
    /// 饮料
    /// </summary>
    public class Food_Drink : Food
    {
        public Food_Drink(Line line) : base(line)
        {

        }
        public override double SortValue => StrengthFood;
        public override string[] Categories => new string[] { "食品", "饮料" };
    }
    /// <summary>
    /// 功能性
    /// </summary>
    public class Food_Functional : Food
    {
        public Food_Functional(Line line) : base(line)
        {

        }
        public override double SortValue => StrengthSleep;
        public override string[] Categories => new string[] { "食品", "功能性" };
    }
    /// <summary>
    /// 药品
    /// </summary>
    public class Food_Drug : Food
    {
        public Food_Drug(Line line) : base(line)
        {

        }

        public override double SortValue => Health;
        public override string[] Categories => new string[] { "食品", "药品" };
    }
}
