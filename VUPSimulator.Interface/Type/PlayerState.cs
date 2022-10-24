using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VUPSimulator.Interface.Comment_base;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 玩家状态类
    /// </summary>
    public class PlayerState
    {

        public PlayerState(Sub sub)
        {
            State = (StateType)Enum.Parse(typeof(StateType), sub.Name, true);
            Duration = sub.InfoToDouble;
        }
        /// <summary>
        /// 玩家状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="duration">持续时间</param>
        public PlayerState(StateType state, int duration)
        {
            State = state;
            Duration = duration;
        }
        /// <summary>
        /// 玩家状态
        /// </summary>
        public enum StateType
        {
            /// <summary>
            /// 精力充沛 消耗1.25 消耗x0.6
            /// </summary>
            VeryEnergetic,
            /// <summary>
            /// 活力 消耗1.5 消耗x0.7
            /// </summary>
            Energetic,
            /// <summary>
            /// 兴奋 消耗1.75 消耗x0.8
            /// </summary>
            Excitement,
            /// <summary>
            /// 感觉良好 消耗2 消耗x0.9
            /// </summary>
            FeelGood,
            /// <summary>
            /// 普通: 体力消耗2 体力消耗x1.0
            /// </summary>
            Nomal,
            /// <summary>
            /// 感觉不好 消耗2 消耗x1.1
            /// </summary>
            FeelBad,
            /// <summary>
            /// 状态不佳 消耗2.5 消耗x1.1
            /// </summary>
            PoorCondition,
            /// <summary>
            /// 疲惫 消耗2.5 消耗x1.2
            /// </summary>
            Tired,
            /// <summary>
            /// 难受 消耗2.75 消耗x1.2
            /// </summary>
            Uncomfortable,
            /// <summary>
            /// 头疼 消耗3 消耗x1.3
            /// </summary>
            Headache,
            /// <summary>
            /// 生病 消耗3 消耗x1.5
            /// </summary>
            Ill,
        }
        /// <summary>
        /// 状态
        /// </summary>
        public StateType State;
        /// <summary>
        /// 持续时间
        /// </summary>
        public double Duration;
    }
    public class PlayerStateSystem : Line
    {

    }
}
