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
        /// 状态影响
        /// </summary>
        public int PlayerState
        {
            get => this[(gint)"playerstate"];
            set => this[(gint)"playerstate"] = value;
        }
        
        /// <summary>
        /// 使用该物品
        /// </summary>
        public void Use(IMainWindow mw)
        {
            mw.Save.StrengthFood += StrengthFood;
            mw.Save.StrengthSleep += StrengthSleep;
            mw.Save.Health += Health;

            Many -= 1;
            if (Many == 0)
                mw.Save.Items.Remove(this);
        }
        public override string Image
        {
            get
            {
                var line = Find("image");
                if (line == null)
                {
                    return "food_" + ItemName;
                }
                return line.Info;
            }
        }
        /// <summary>
        /// 物品描述
        /// </summary>
        public override string Description
        {
            get
            {
                string heleff = "无";
                var h = Health;
                if (h != 0)
                {
                    if (h > 0)
                    {//正面影响
                        if (h <= 0.02)
                            heleff = "几乎无良性";
                        else if (h <= 0.1)
                            heleff = "略微良性";
                        else if (h <= 0.2)
                            heleff = $"轻微良性(+{h:2})";
                        else if (h <= 0.5)
                            heleff = $"中等良性(+{h:2})";
                        else if (h <= 1)
                            heleff = $"大幅度良性(+{h:2})";
                        else if (h <= 5)
                            heleff = $"非常良性(+{h:2})";
                        else
                            heleff = $"(+{h:2})";
                    }
                    else
                    {//负面
                        if (h >= 0.02)
                            heleff = "几乎无负面";
                        else if (h >= 0.1)
                            heleff = "略微负面";
                        else if (h >= 0.2)
                            heleff = $"轻微负面({h:2})";
                        else if (h >= 0.5)
                            heleff = $"中等负面({h:2})";
                        else if (h >= 1)
                            heleff = $"显著负面({h:2})";
                        else if (h >= 5)
                            heleff = $"严重负面({h:2})";
                        else
                            heleff = $"({h:2})";
                    }
                }
                return GetString("desc", ItemDisplayName) + $"\n卡路里: {StrengthFood * 10:f1}大卡\n咖啡因: {StrengthSleep:f1}mg\n健康影响: {heleff}";
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

        public override double SortValue => StrengthFood;
        public override string[] Categories => new string[] { "食物", "零食" };

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
        public override string[] Categories => new string[] { "食物", "主食" };
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
        public override string[] Categories => new string[] { "食物", "饮料" };
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
        public override string[] Categories => new string[] { "食物", "功能性" };
    }
    /// <summary>
    /// 药品
    /// </summary>
    public class Food_Drug : Food
    {
        public Food_Drug(Line line) : base(line)
        {

        }
        /// <summary>
        /// 健康影响范围
        /// </summary>
        public double HealthRange
        {
            get => this[(gdbe)"range"];
            set => this[(gdbe)"range"] = value;
        }
        public override double SortValue => Health;
        public override string[] Categories => new string[] { "食物", "药品" };
    }
}
