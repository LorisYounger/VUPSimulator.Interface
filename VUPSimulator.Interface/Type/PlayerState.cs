﻿using LinePutScript;
using LinePutScript.Localization.WPF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using static VUPSimulator.Interface.Comment;
using static VUPSimulator.Interface.PlayerState;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 玩家状态类
    /// </summary>
    public class PlayerState
    {
        public PlayerState(ISub sub)
        {
            State = (StateType)Enum.Parse(typeof(StateType), sub.Name, true);
            var strs = sub.GetInfos();
            Duration = Convert.ToDouble(strs[0]);
            Reason = strs[1];
            Tag = strs.Length > 2 ? (TagType)Enum.Parse(typeof(TagType), strs[2], true) : TagType.Nomal;
        }
        /// <summary>
        /// 玩家状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="duration">持续时间</param>
        /// <param name="reason">原因</param>
        /// <param name="tag">标签</param>
        public PlayerState(StateType state, double duration, string reason, TagType tag = TagType.Nomal)
        {
            State = state;
            Duration = duration;
            Reason = reason;
            Tag = tag;
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
            /// 普通: 饱腹消耗2 饱腹消耗x1.0
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
        public string Reason { get; set; }
        /// <summary>
        /// 标签,用于召回撤销等功能
        /// </summary>
        public TagType Tag;
        /// <summary>
        /// 标签
        /// </summary>
        public enum TagType
        {
            /// <summary>
            /// 标准/默认
            /// </summary>
            Nomal,
            /// <summary>
            /// 由于食物影响
            /// </summary>
            Food,
            /// <summary>
            /// 由于药品影响
            /// </summary>
            Drag,
            /// <summary>
            /// 由于饱腹影响
            /// </summary>
            Strength,
            /// <summary>
            /// 由于运行时间
            /// </summary>
            RunTime,
            /// <summary>
            /// 由于事件影响
            /// </summary>
            Event,
        }
        /// <summary>
        /// 转换成存档用数据
        /// </summary>
        public ISub ToSub()
        {
            return new Sub($"{State}#{Duration:f4},{Sub.TextReplace(Reason)},{Tag}");
        }
        /// <summary>
        /// 当前状态描述文本
        /// </summary>
        public string StateToText { get => PlayerStateSystem.PlayerState[(int)State].Item1.Translate(); set { } }
        /// <summary>
        /// 当前状态值
        /// </summary>
        public int StateToInt { get => 4 - (int)State; set { } }
        /// <summary>
        /// 当前持续时间描述文本
        /// </summary>
        public string DurationToText { get => Function.DateConvert(Duration); set { } }
    }
    /// <summary>
    /// 玩家状态系统
    /// </summary>
    public class PlayerStateSystem
    {
        public List<PlayerStrength> StrengthsHistory = new List<PlayerStrength>();
        /// <summary>
        /// 玩家状态计算字典
        /// </summary>
        public static readonly (string, double, double, Color, Color)[] PlayerState = new (string, double, double, Color, Color)[] {
                new ( "精力充沛", 1.25,0.6 ,Color.FromRgb(66,165,254), Color.FromRgb(0, 60, 95) ) ,
                new ( "活力", 1.5,0.7, Color.FromRgb(41, 182, 246), Color.FromRgb(0, 73, 97) ) ,
                new ( "兴奋", 1.75,0.8 ,Color.FromRgb(38,198,218), Color.FromRgb(0, 77, 85)),
                new ( "感觉良好", 2.0,0.9, Color.FromRgb(77, 182, 172), Color.FromRgb(0, 77, 61) ) ,
                new ( "普通", 2,1, Color.FromRgb(139, 213, 129), Color.FromRgb(47, 153, 76) ) ,
                new ( "感觉不好", 2.5,1.1, Color.FromRgb(174, 213, 129), Color.FromRgb(62, 82, 42) ) ,
                new ( "状态不佳", 2.5,1.1, Color.FromRgb(212, 225, 87), Color.FromRgb(80, 88, 18) ) ,
                new ( "疲惫", 2.5,1.2, Color.FromRgb(251, 192, 45), Color.FromRgb(83, 72, 0) ) ,
                new ( "难受", 2.75,1.2, Color.FromRgb(255, 179, 0), Color.FromRgb(98, 66, 0) ) ,
                new ( "头疼", 3,1.3, Color.FromRgb(251, 141, 0), Color.FromRgb(98, 48, 0) ) ,
                new ( "生病", 3,1.5, Color.FromRgb(244, 81, 30), Color.FromRgb(92, 10, 0) ) ,
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
        /// 添加饱腹消耗
        /// </summary>
        /// <param name="strength">饱腹</param>
        /// <param name="duration">持续时间</param>
        /// <param name="reason">原因</param>
        /// <param name="happenedTime">发生时间</param>
        public void AddStrength(DateTime happenedTime, double strength, double duration, string reason)
        {
            var same = StrengthsHistory.FirstOrDefault(x => x.Reason == reason);
            if (same == null)
            {
                StrengthsHistory.Add(new PlayerStrength(happenedTime, strength, duration, reason));
                while (StrengthsHistory.Count >= 20)
                {
                    StrengthsHistory.RemoveAt(0);
                }
            }
            else
            {
                same.Duration += duration;
                same.Strength += strength;
                same.HappenedTime = happenedTime;
            }
            StrengthsHistory.Sort();// = StrengthsHistory.OrderBy(x => x.HappenedTime).ToList();
            //var lst = StrengthsHistory.OrderBy(x => x.HappenedTime).ToArray();
            //StrengthsHistory.Clear();
            //foreach (var item in lst)
            //{
            //    StrengthsHistory.Add(item);
            //}
        }
        /// <summary>
        /// 添加玩家状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="duration">持续时间</param>
        /// <param name="reason">原因</param>
        /// <param name="tag">标签</param>
        public void AddState(StateType state, double duration, string reason, TagType tag = TagType.Nomal)
        {
            PlayerStates.Add(new PlayerState(state, duration, reason, tag));
        }
        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="tag">状态</param>
        public void RemoveState(TagType tag)
        {
            PlayerStates.RemoveAll(x => x.Tag == tag);
            //foreach (var state in PlayerStates.Where(x => x.Tag == tag).ToList())
            //{
            //    PlayerStates.Remove(state);
            //}
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
                    state += states.StateToInt;
                }
                state /= Math.Sqrt(PlayerStates.Count);
                return Math.Min(Math.Max(4 - state, 0), 10);
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
        public ILine ToLine()
        {
            ILine line = new Line("playerstate");
            foreach (var state in PlayerStates)
            {
                line.Add(state.ToSub());
            }
            return line;
        }
        public PlayerStateSystem(ILine line)
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
        public void TimeRels(TimeSpan span)
        {
            PlayerStates.RemoveAll((state) =>
            {
                state.Duration -= span.TotalHours;
                return state.Duration <= 0;
            });
            //foreach (var state in PlayerStates)
            //{
            //    state.Duration -= span.TotalHours;
            //}
            //foreach (var state in PlayerStates.Where(x => x.Duration <= 0).ToList())
            //{
            //    PlayerStates.Remove(state);
            //}
        }
        /// <summary>
        /// 玩家饱腹消耗记录
        /// </summary>
        public class PlayerStrength : IComparable
        {
            /// <summary>
            /// 总共消耗的饱腹
            /// </summary>
            public double Strength;
            /// <summary>
            /// 原因
            /// </summary>
            public string Reason { get; set; }
            /// <summary>
            /// 持续时间 (小时)
            /// </summary>
            public double Duration;
            /// <summary>
            /// 时间(文本)
            /// </summary>
            public string StrTime { get => $"{HappenedTime:[d]-HH} ({Duration:f1}h)"; }
            /// <summary>
            /// 发生结束时间
            /// </summary>
            public DateTime HappenedTime;
            /// <summary>
            /// 花费饱腹每小时
            /// </summary>
            public double StrengthPerHour => Strength / Duration;
            /// <summary>
            /// 饱腹(文本)
            /// </summary>
            public string StrStrength { get => $"{Strength:f1} ({StrengthPerHour:f1}/h)"; }
            /// <summary>
            /// 玩家饱腹消耗记录
            /// </summary>
            /// <param name="happenedTime">发生结束时间</param>
            /// <param name="strength">总共消耗的饱腹</param>
            /// <param name="duration">持续时间 (小时)</param>
            /// <param name="reason">原因</param>
            public PlayerStrength(DateTime happenedTime, double strength, double duration, string reason)
            {
                Strength = strength;
                Duration = duration;
                Reason = reason;
                HappenedTime = happenedTime;
            }

            public int CompareTo(object obj)
            {
                return HappenedTime.CompareTo(((PlayerStrength)obj).HappenedTime);
            }
        }
    }

}
