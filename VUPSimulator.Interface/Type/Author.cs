using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 老画师作者
    /// </summary>
    public class Author
    {
        /// <summary>
        /// 技能类型
        /// </summary>
        public enum SkillType
        {
            /// <summary>
            /// Live2D立绘
            /// </summary>
            L2DPaint,
            /// <summary>
            /// Live2D建模
            /// </summary>
            L2DModel,
            /// <summary>
            /// 插图
            /// </summary>
            Illustration,
            /// <summary>
            /// 头像
            /// </summary>
            Profile,
            /// <summary>
            /// 表情包
            /// </summary>
            Expression,
        }
        /// <summary>
        /// 作者技能
        /// </summary>
        public class Skill
        {
           
            /// <summary>
            /// 技能类型
            /// </summary>
            public SkillType SkillType;
            /// <summary>
            /// 技能等级(最小值) 0-5/10
            /// </summary>
            public double SkillLevelMin;
            /// <summary>
            /// 技能等级(最大值) 0-5/10
            /// </summary>
            public double SkillLevelMax;
        }
    }
}
