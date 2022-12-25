using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VUPSimulator.Interface.Comment;

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
        /// 增加 精力:饱腹
        /// </summary>
        public double StrengthFood
        {
            get => this[(gdbe)"strengthfood"];
            set => this[(gdbe)"strengthfood"] = value;
        }
        /// <summary>
        /// 增加 饱腹:睡眠
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
        /// 吃所需要花费的时间
        /// </summary>
        public double SpendTime
        {
            get => GetDouble("spendtime", 20);
            set => this[(gdbe)"spendtime"] = value;
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
            if (mw.Save.Working != null)
            {//在干别的事情,不能吃东西
                mw.winMessageBoxShow("正在干别的事情,不能吃东西", $"当前正在 {mw.Save.Working}\n吃饭要专心,不能三心二意");
                return;
            }
            if (Many == 0)
            {
                mw.Save.Items.Remove(this);
                return;
            }
            //TODO:吃过期食物
            
            mw.Save.StrengthFood += StrengthFood / 4;
            mw.Save.StrengthSleep += StrengthSleep / 4;
            double health = 0;
            //在范围内生效health
            if (Health != 0)
            {
                var rh = 1 - Math.Pow(mw.Save.Health - HealthRange, 2) / 2 / Math.Pow(HealthRangeLength, 2);
                if (rh >= 0)
                {
                    health = rh * Health;
                    mw.Save.Health += health / 4;
                }
            }
            //状态系统应用
            if (PlayerStateLength > 0)
            {
                mw.Save.PStateSystem.AddState((Interface.PlayerState.StateType)PlayerState, PlayerStateLength, $"吃 {ItemName}", Interface.PlayerState.TagType.Food);
            }
            Many -= 1;
            if (Many == 0)
                mw.Save.Items.Remove(this);
#pragma warning disable CS0612 // 类型或成员已过时
            mw.winEatingShow(this, new FoodEatTimeHandle(SpendTime, StrengthFood, StrengthSleep, health));
#pragma warning restore CS0612 // 类型或成员已过时
        }
        /// <summary>
        /// 食物使用中
        /// </summary>
        /// 这个是当食物正在吃的时候,会有个生效过程
        /// 具体效果是: 立即获得25% -> 持续时间获得50% -> 吃完后获得25%
        public class FoodEatTimeHandle
        {
            /// <summary>
            /// 吃东西所需要的时间(分钟)
            /// </summary>
            public double SpendTime;
            /// <summary>
            /// 吃完剩余时间(分钟)
            /// </summary>
            public double SpendTimeLeft;
            /// <summary>
            /// 增加 精力:饱腹
            /// </summary>
            public double StrengthFood;
            public double TstrengthFood;
            /// <summary>
            /// 增加 饱腹:睡眠
            /// </summary>
            public double StrengthSleep;
            public double TstrengthSleep;
            /// <summary>
            /// 增加 健康
            /// </summary>
            public double Health;
            public double Thealth;
            /// <summary>
            /// 新建食物食用中
            /// </summary>
            /// <param name="spendtime">吃东西所需要的时间(分钟)</param>
            /// <param name="strengthfood"> 增加 精力:饱腹</param>
            /// <param name="strengthsleep">增加 饱腹:睡眠</param>
            /// <param name="health">增加 健康</param>
            public FoodEatTimeHandle(double spendtime, double strengthfood, double strengthsleep, double health)
            {
                SpendTime = spendtime;
                SpendTimeLeft = spendtime;
                StrengthFood = strengthfood / 4;
                StrengthSleep = strengthsleep / 4;
                Health = health / 4;
                TstrengthFood = strengthfood / 2 / spendtime;
                TstrengthSleep = strengthsleep / 2 / spendtime;
                Thealth = health / 2 / spendtime;
            }
            /// <summary>
            /// 进行时间移动,吃和消化食物
            /// </summary>
            /// <returns>True:吃完了,可以关掉窗口了</returns>
            public bool TimeRels(TimeSpan span, IMainWindow mw)
            {
                if (span.TotalMinutes >= SpendTimeLeft)
                {//直接进行结算
                    mw.Save.Health += Health / 4 + Thealth * SpendTimeLeft;
                    mw.Save.StrengthFood += StrengthFood / 4 + TstrengthFood * SpendTimeLeft;
                    mw.Save.StrengthSleep += StrengthSleep / 4 + TstrengthSleep * SpendTimeLeft;
                    return true;
                }
                else
                {
                    var d = span.TotalMinutes;
                    SpendTimeLeft -= d;
                    mw.Save.Health += Thealth * d;
                    mw.Save.StrengthFood += TstrengthFood * d;
                    mw.Save.StrengthSleep += TstrengthSleep * d;
                    return false;
                }
            }
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
        /// <summary>
        /// 真食物类型(用于判断主食/饮料/零食)
        /// </summary>
        public ItemType RealFoodType
        {
            get
            {
                var sub = Find("realtype");
                if (sub != null)
                    return (ItemType)Enum.Parse(typeof(ItemType), sub.info, true);
                return Type;
            }
        }
        /// <summary>
        /// 保质期
        /// </summary>
        public double QualityGuaranteePeriod
        {
            get => GetDouble("period", 30);
            set => this[(gdbe)"period"] = value;
        }
        /// <summary>
        /// 保质期开始时间
        /// </summary>
        public DateTime QualityGuaranteeStartTime
        {
            get => GetDateTime("periodstart",DateTime.MaxValue);
            set => this[(gdat)"periodstart"] = value;
        }
        /// <summary>
        /// 保质期剩余时间
        /// </summary>
        /// <param name="now">当前时间</param>
        /// <returns></returns>
        public double QualityGuaranteeLeft(DateTime now)
        {
            return (QualityGuaranteePeriod - (now - QualityGuaranteeStartTime).TotalDays);
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
