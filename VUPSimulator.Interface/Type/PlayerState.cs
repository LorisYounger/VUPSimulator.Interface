using LinePutScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VUPSimulator.Interface.Comment_base;
using static VUPSimulator.Interface.PlayerState;

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
            var strs = sub.GetInfos();
            Duration = Convert.ToDouble(strs[0]);
            Reason = strs[1];
        }
        /// <summary>
        /// 玩家状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="duration">持续时间</param>
        /// <param name="reason">原因</param>
        public PlayerState(StateType state, double duration,string reason)
        {
            State = state;
            Duration = duration;
            Reason = reason;
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
        /// 持续时间 (小时)
        /// </summary>
        public double Duration;
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason;
        /// <summary>
        /// 转换成存档用数据
        /// </summary>
        public Sub ToSub()
        {
            return new Sub(State.ToString(), Duration.ToString() + ',' + Reason);
        }
    }
    /// <summary>
    /// 玩家状态系统
    /// </summary>
    public class PlayerStateSystem
    {
        /// <summary>
        /// 玩家状态计算字典
        /// </summary>
        public static readonly dynamic[][] PlayerState = new dynamic[][] {
                new dynamic[]{ "精力充沛", 1.25,0.6 } ,
                new dynamic[]{ "活力", 1.5,0.7 } ,
                new dynamic[]{ "兴奋", 1.75,0.8 },
                new dynamic[]{ "感觉良好", 2.0,0.9 } ,
                new dynamic[]{ "普通", 2,1 } ,
                new dynamic[]{ "感觉不好", 2.5,1.1 } ,
                new dynamic[]{ "状态不佳", 2.5,1.1 } ,
                new dynamic[]{ "疲惫", 2.5,1.2 } ,
                new dynamic[]{ "难受", 2.75,1.2 } ,
                new dynamic[]{ "头疼", 3,1.3 } ,
                new dynamic[]{ "生病", 3,1.5 } ,
        };
        public List<PlayerState> PlayerStates = new List<PlayerState>();
        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="state">状态</param>
        public void AddState(PlayerState state)
        {
            PlayerStates.Add(state);
        }
        /// <summary>
        /// 添加玩家状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="duration">持续时间</param>
        /// <param name="reason">原因</param>
        public void AddState(StateType state, double duration, string reason)
        {
            PlayerStates.Add(new PlayerState(state, duration,reason));
        }
        /// <summary>
        /// 返回当前玩家状态
        /// </summary>
        public double OutDouble()
        {
            if (PlayerStates.Count == 0)
            {
                return 4;
            }
            else
            {
                double state = 0;
                foreach (var states in PlayerStates)
                {
                    state += (int)states.State;
                }
                state /= PlayerStates.Count;
                return state;
            }
        }
        /// <summary>
        /// 返回当前玩家状态
        /// </summary>
        public int OutInt() => (int)OutDouble();
        /// <summary>
        /// 返回当前玩家状态
        /// </summary>
        public StateType OutState() => (StateType)OutInt();
        /// <summary>
        /// 转换成存档用数据
        /// </summary>
        public Line ToLine()
        {
            Line line = new Line();
            foreach (var state in PlayerStates)
            {
                line.Add(new Sub(((int)state.State).ToString(), state.Duration.ToString("f4")));
            }
            return line;
        }
        public PlayerStateSystem(Line line)
        {
            foreach (var sub in line)
            {
                PlayerStates.Add(new PlayerState(sub));
            }
        }
        public PlayerStateSystem() { }
        /// <summary>
        /// 时间刷新
        /// </summary>
        public void TimeRels(TimeSpan span, IMainWindow mw)
        {
            foreach(var state in PlayerStates)
            {
                state.Duration -= span.TotalHours;
                if (state.Duration <= 0)
                {
                    PlayerStates.Remove(state);                 
                }
            }
        }
    }
}
