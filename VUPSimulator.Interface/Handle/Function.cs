﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using LinePutScript;
using LinePutScript.Localization.WPF;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 游戏通用方法
    /// </summary>
    public static class Function
    {
        /// <summary>
        /// HEX值转颜色
        /// </summary>
        /// <param name="HEX">HEX值</param>
        /// <returns>颜色</returns>
        public static Color HEXToColor(string HEX) => (Color)ColorConverter.ConvertFromString(HEX);
        /// <summary>
        /// 颜色转HEX值
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns>HEX值</returns>
        public static string ColorToHEX(Color color) => "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        /// <summary>
        /// 随机数生成中心
        /// </summary>
        public static Random Rnd = new Random();
        /// <summary>
        /// 返回 num1 到 num2 之间的随机数,如果num2小于num1则返回num1
        /// </summary>
        /// <param name="num1">值1</param>
        /// <param name="num2">值2</param>
        /// <returns></returns>
        public static int RndNext(int num1, int num2)
        {
            if (num1 < num2)
                return Rnd.Next(num1, num2);
            else
                return num1;
        }
        /// <summary>
        /// 返回 num1 到 num2 之间的随机数,如果num2小于num1则返回num1
        /// </summary>
        /// <param name="num1">值1</param>
        /// <param name="num2">值2</param>
        /// <returns></returns>
        public static double RndNext(double num1, double num2)
        {
            if (num1 < num2)
                return Rnd.NextDouble() * (num2 - num1) + num1;
            else
                return num1;
        }

        /// <summary>
        /// 数据计算类
        /// </summary>
        public static class Cal
        {
            public static object CalADD(object A, object B)
            {
                switch (A)
                {
                    case string s:
                        return s + (string)B;
                    case int i:
                        return i + (int)B;
                    case double d:
                        return d + (double)B;
                    case bool b:
                        return b && (bool)B;
                    default:
                        throw new ArgumentException();
                }
            }
            public static object CalSUB(object A, object B)
            {
                switch (A)
                {
                    case string s:
                        return s.Replace((string)B, "");
                    case int i:
                        return i - (int)B;
                    case double d:
                        return d - (double)B;
                    case bool b:
                        return b || (bool)B;
                    default:
                        throw new ArgumentException();
                }
            }
            public static object CalMUL(object A, object B)
            {
                switch (A)
                {
                    case string s:
                        return s.Trim(((string)B)[0]);
                    case int i:
                        return i * (int)B;
                    case double d:
                        return d * (double)B;
                    case bool b:
                        return b & (bool)B;
                    default:
                        throw new ArgumentException();
                }
            }
            public static object CalDIV(object A, object B)
            {
                switch (A)
                {
                    case string s:
                        return s.Split(((string)B)[0])[0];
                    case int i:
                        return i / (int)B;
                    case double d:
                        return d / (double)B;
                    case bool b:
                        return b | (bool)B;
                    default:
                        throw new ArgumentException();
                }
            }
            /// <summary>
            /// 前置条件是否满足 虽然说使用了EventData版,但是数据其实是通用的
            /// </summary>
            /// <param name="mw">主窗口</param>
            /// <param name="data">相关数据/运算符</param>
            /// <returns>True:可以运行</returns>
            public static bool DataEnable(IMainWindow mw, ILine data)
            {
                //判断相关参数
                bool trigger = true;
                foreach (ISub ifsub in ((Line)data).Subs.FindAll(x => x.Name.StartsWith("if")))
                {
                    if (ifsub.Name.ToLower() == "ifor")
                    {
                        if (trigger)
                            return true;
                        trigger = true;
                        continue;
                    }
                    var ifstrs = ifsub.GetInfos();
                    IComparable[] ifobj = { DataConvert(mw, ifstrs[0], data), DataConvert(mw, ifstrs[1], data) };
                    switch (ifsub.Name.ToLower())
                    {
                        case "ifequ":
                            trigger &= ifobj[0].CompareTo(ifobj[1]) == 0;
                            break;
                        case "ifneq":
                            trigger &= ifobj[0].CompareTo(ifobj[1]) != 0;
                            break;
                        case "ifbeq":
                            trigger &= ifobj[0].CompareTo(ifobj[1]) >= 0;
                            break;
                        case "ifbig":
                            trigger &= ifobj[0].CompareTo(ifobj[1]) > 0;
                            break;
                        case "iflow":
                            trigger &= ifobj[0].CompareTo(ifobj[1]) < 0;
                            break;
                        case "ifleq":
                            trigger &= ifobj[0].CompareTo(ifobj[1]) <= 0;
                            break;
                    }
                }
                //if (!trigger)
                //    return false;
                ////最后的随机数判断
                //if (!PeriodRandom)
                //    return true;
                //double per = (int)Period;
                //return per / 2 + per * Function.Rnd.NextDouble());
                return trigger;
            }

            /// <summary>
            /// 提供前置条件的判断文本,告诉玩家为啥可以或不可以使用
            /// </summary>
            /// <param name="mw">主窗口</param>
            /// <param name="data">相关数据/运算符</param>
            /// <returns>提供前置条件的判断文本</returns>
            public static string DataEnableString(IMainWindow mw, ILine data)
            {
                //判断相关参数
                bool trigger = true;
                StringBuilder sb = new StringBuilder();
                foreach (ISub ifsub in ((Line)data).Subs.FindAll(x => x.Name.StartsWith("if")))
                {
                    if (ifsub.Name.ToLower() == "ifor")
                    {
                        sb.AppendLine("通过:{0}\n或".Translate(trigger));
                        trigger = true;
                        continue;
                    }
                    var ifstrs = ifsub.GetInfos();
                    IComparable[] ifobj = { DataConvert(mw, ifstrs[0], data), DataConvert(mw, ifstrs[1], data) };
                    string[] ifobj2 = { DataConvertString(mw, ifstrs[0], data), DataConvertString(mw, ifstrs[1], data) };
                    bool b;
                    switch (ifsub.Name.ToLower())
                    {
                        case "ifequ":
                            b = ifobj[0].CompareTo(ifobj[1]) == 0;
                            trigger &= b;
                            sb.AppendLine($"{ifobj2[0]} == {ifobj2[1]} : {b}");
                            break;
                        case "ifneq":
                            b = ifobj[0].CompareTo(ifobj[1]) != 0;
                            trigger &= b;
                            sb.AppendLine($"{ifobj2[0]} != {ifobj2[1]} : {b}");
                            break;
                        case "ifbeq":
                            b = ifobj[0].CompareTo(ifobj[1]) >= 0;
                            trigger &= b;
                            sb.AppendLine($"{ifobj2[0]} >= {ifobj2[1]} : {b}");
                            break;
                        case "ifbig":
                            b = ifobj[0].CompareTo(ifobj[1]) > 0;
                            trigger &= b;
                            sb.AppendLine($"{ifobj2[0]} > {ifobj2[1]} : {b}");
                            break;
                        case "iflow":
                            b = ifobj[0].CompareTo(ifobj[1]) < 0;
                            trigger &= b;
                            sb.AppendLine($"{ifobj2[0]} < {ifobj2[1]} : {b}");
                            break;
                        case "ifleq":
                            b = ifobj[0].CompareTo(ifobj[1]) <= 0;
                            trigger &= b;
                            sb.AppendLine($"{ifobj2[0]} <= {ifobj2[1]} : {b}");
                            break;
                    }
                }
                if (sb.Length == 0)
                    return null;
                return "激活条件:\n{0}通过:{1}".Translate(sb, trigger);
            }

            /// <summary>
            /// 字符串数据转换 虽然说使用了EventData版,但是数据其实是通用的
            /// </summary>
            /// <param name="mw">主窗口</param>
            /// <param name="name">相关字符串</param>
            /// <param name="data">相关本地数据</param>
            /// <returns></returns>
            public static IComparable DataConvert(IMainWindow mw, string name, ILine data = null)
            {
                if (double.TryParse(name, out var value))
                    return value;
                switch (name.ToLower().Substring(0, 3))
                {
                    case "int":
                        return (double)mw.Save.EventData.GetInt(name);
                    case "flt":
                        return mw.Save.EventData.GetDouble(name);
                    case "str":
                        return mw.Save.EventData.GetString(name);
                    case "rdi":
                        var v = name.Split('_');
                        if (v.Length == 2) return (double)Rnd.Next(Convert.ToInt32(v[1]));
                        else if (v.Length == 3) return (double)Rnd.Next(Convert.ToInt32(v[1]), Convert.ToInt32(v[2]));
                        else
                            return (double)Rnd.Next();
                    case "rdd":
                        v = name.Split('_');
                        if (v.Length == 2) return Rnd.NextDouble() * Convert.ToDouble(v[1]);
                        else if (v.Length == 3)
                        {
                            var v1 = Convert.ToDouble(v[1]);
                            return Rnd.NextDouble() * (Convert.ToDouble(v[2]) - v1) + v1;
                        }
                        else
                            return Rnd.NextDouble();
                    //case "dat":
                    //    return mw.Save.EventData.GetDateTime(name);
                    case "bol":
                        return mw.Save.EventData.GetBool(name);
                    default:
                        switch (name.ToLower())
                        {
                            case "money"://其他指定参数
                                return mw.Save.Base.Money;
                            case "health":
                                return mw.Save.Base.Health;
                            case "strengthfood":
                                return mw.Save.Base.StrengthFood;
                            case "strengthsleep":
                                return mw.Save.Base.StrengthSleep;
                            case "strengthmin":
                                return Math.Min(mw.Save.Base.StrengthFood, mw.Save.Base.StrengthSleep);
                            case "pclip":
                                return mw.Save.Base.Pclip;
                            case "pdraw":
                                return mw.Save.Base.Pdraw;
                            case "pgame":
                                return mw.Save.Base.Pgame;
                            case "pidear":
                                return mw.Save.Base.Pidear;
                            case "pimage":
                                return mw.Save.Base.Pimage;
                            case "poperate":
                                return mw.Save.Base.Poperate;
                            case "pprogram":
                                return mw.Save.Base.Pprogram;
                            case "pspeak":
                                return mw.Save.Base.Pspeak;
                            case "psong":
                                return mw.Save.Base.Psong;
                            default:
                                //尝试解析和获取本身属性
                                if (data == null)
                                    return null;
                                string str = data[(gstr)name];
                                if (double.TryParse(str, out var d))
                                    return d;
                                return str;
                        }
                }
            }

            /// <summary>
            /// 字符串数据转换 虽然说使用了EventData版,但是数据其实是通用的
            /// </summary>
            /// <param name="mw">主窗口</param>
            /// <param name="name">相关字符串</param>
            /// <param name="data">相关本地数据</param>
            /// <returns></returns>
            public static string DataConvertString(IMainWindow mw, string name, ILine data = null)
            {
                if (double.TryParse(name, out var value))
                    return value.ToString();
                switch (name.ToLower().Substring(0, 3))
                {
                    case "int":
                        return $"{name.ToLower().Substring(3)}({mw.Save.EventData.GetInt(name)})";
                    case "flt":
                        return $"{name.ToLower().Substring(3)}({mw.Save.EventData.GetDouble(name):f2})";
                    case "str":
                        return $"{name.ToLower().Substring(3)}({mw.Save.EventData.GetString(name)})";
                    case "rdi":
                        var v = name.Split('_');
                        if (v.Length == 2) return "随机整数(0-{0})".Translate((Convert.ToInt32(v[1]) - 1));
                        else if (v.Length == 3) return "随机整数({0}-{1})".Translate(v[1], (Convert.ToInt32(v[2]) - 1));
                        else
                            return "随机整数";
                    case "rdd":
                        v = name.Split('_');
                        if (v.Length == 2) return "随机浮点数(0-{1})".Translate(v[1]);
                        else if (v.Length == 3)
                        {
                            return $"随机浮点数({v[1]}-{v[2]})";
                        }
                        else
                            return "随机浮点数(0-1)";
                    //case "dat":
                    //    return mw.Save.EventData.GetDateTime(name);
                    case "bol":
                        return $"{name.ToLower().Substring(3)}({mw.Save.EventData.GetBool(name)}";
                    default:
                        switch (name.ToLower())
                        {
                            case "money"://其他指定参数
                                return "资金".Translate() + $"({mw.Save.Base.Money:f2})";
                            case "health":
                                return "健康".Translate() + $"({mw.Save.Base.Health:f2})";
                            case "strengthfood":
                                return "饱腹".Translate() + $"({mw.Save.Base.StrengthFood:f2})";
                            case "strengthsleep":
                                return "睡眠".Translate() + $"({mw.Save.Base.StrengthSleep:f2})";
                            case "strengthmin":
                                return "体力".Translate() + $"({Math.Min(mw.Save.Base.StrengthFood, mw.Save.Base.StrengthSleep):f2})";
                            case "pclip":
                                return "剪辑".Translate() + $"({mw.Save.Base.Pclip:f2})";
                            case "pdraw":
                                return "绘画".Translate() + $"({mw.Save.Base.Pdraw:f2})";
                            case "pgame":
                                return "游戏".Translate() + $"({mw.Save.Base.Pgame:f2})";
                            case "pidear":
                                return "思维".Translate() + $"({mw.Save.Base.Pidear:f2})";
                            case "pimage":
                                return "修图".Translate() + $"({mw.Save.Base.Pimage:f2})";
                            case "poperate":
                                return "运营".Translate() + $"({mw.Save.Base.Poperate:f2})";
                            case "pprogram":
                                return "程序".Translate() + $"({mw.Save.Base.Pprogram:f2})";
                            case "pspeak":
                                return "口才".Translate() + $"({mw.Save.Base.Pspeak:f2})";
                            case "psong":
                                return "声乐".Translate() + $"({mw.Save.Base.Pspeak:f2})";
                            default:
                                //尝试解析和获取本身属性
                                if (data == null)
                                    return null;
                                string str = data[(gstr)name];
                                if (double.TryParse(str, out var d))
                                    return $"{name}({d:f2})";
                                return str;
                        }
                }
            }

            /// <summary>
            /// 执行数据计算
            /// </summary>
            /// <param name="mw">主窗口</param>
            /// <param name="data">相关数据/运算符</param>
            public static void DataCalculat(IMainWindow mw, ILine data)
            {
                foreach (ISub calsub in ((Line)data).Subs.FindAll(x => x.Name.ToLower().StartsWith("cal")))
                {
                    var calstrs = calsub.GetInfos();
                    IComparable[] calobj = { DataConvert(mw, calstrs[0]), DataConvert(mw, calstrs[1]) };
                    switch (calsub.Name.ToLower())
                    {
                        case "caladd":
                            DataSet(mw, calstrs[0], CalADD(calobj[0], calobj[1]));
                            break;
                        case "calsub":
                            DataSet(mw, calstrs[0], CalSUB(calobj[0], calobj[1]));
                            break;
                        case "calmul":
                            DataSet(mw, calstrs[0], CalMUL(calobj[0], calobj[1]));
                            break;
                        case "caldiv":
                            DataSet(mw, calstrs[0], CalDIV(calobj[0], calobj[1]));
                            break;
                        case "calset":
                            DataSet(mw, calstrs[0], calobj[1]);
                            break;
                    }
                }
            }
            /// <summary>
            /// 设置相关数据
            /// </summary>
            /// <param name="mw">主窗体</param>
            /// <param name="name">设置名称</param>
            /// <param name="value">设置值</param>
            /// <param name="data">若未找到则修改的源行</param>
            public static void DataSet(IMainWindow mw, string name, object value, ILine data = null)
            {
                switch (name.ToLower().Substring(0, 3))
                {
                    case "int":
                        mw.Save.EventData.SetInt(name, Convert.ToInt32(value));
                        break;
                    case "flt":
                        mw.Save.EventData.GetDouble(name, (double)value); break;
                    case "str":
                        mw.Save.EventData.SetString(name, (string)value); break;
                    //case "dat":
                    //    mw.Save.EventData.SetDateTime(name, (DateTime)value); break;
                    case "bol":
                        mw.Save.EventData.SetBool(name, (bool)value); break;
                    default:
                        switch (name.ToLower())
                        {
                            case "money"://其他指定参数
                                mw.Save.Base.Money = (double)value;
                                break;
                            case "health":
                                mw.Save.Base.Health = (double)value;
                                break;
                            case "strengthfood":
                                mw.Save.Base.StrengthFood = (double)value;
                                break;
                            case "strengthsleep":
                                mw.Save.Base.StrengthSleep = (double)value;
                                break;
                            case "pclip":
                                mw.Save.Base.Pclip = (double)value;
                                break;
                            case "pdraw":
                                mw.Save.Base.Pdraw = (double)value;
                                break;
                            case "pgame":
                                mw.Save.Base.Pgame = (double)value;
                                break;
                            case "pidear":
                                mw.Save.Base.Pidear = (double)value;
                                break;
                            case "pimage":
                                mw.Save.Base.Pimage = (double)value;
                                break;
                            case "poperate":
                                mw.Save.Base.Poperate = (double)value;
                                break;
                            case "pprogram":
                                mw.Save.Base.Pprogram = (double)value;
                                break;
                            case "pspeak":
                                mw.Save.Base.Pspeak = (double)value;
                                break;
                            case "psong":
                                mw.Save.Base.Psong = (double)value;
                                break;
                            default://对本身属性进行设置
                                if (data != null)
                                    data[(gstr)name] = (string)value;
                                break;
                        }
                        break;
                }
            }
        }


        public static readonly DateTime DateMaxValue = new DateTime(3000000000000000000L, DateTimeKind.Unspecified);

        /// <summary>
        /// 退回评分标准
        /// </summary>
        /// <param name="score">分数</param>
        /// <param name="color">对应分数颜色</param>
        /// <returns>
        /// 评分标准:
        /// A+ >=100%
        /// A  >=95
        /// A- >=90
        /// B+ >=85
        /// B  >=80
        /// B- >=75
        /// C+ >=70
        /// C  >=65
        /// C- >=60
        /// D  >=40
        /// E  >=20
        /// </returns>
        public static string ScoretoString(int score, out Color color)
        {
            color = Colors.Green;
            if (score >= 99)
                return "A+";
            else if (score >= 94)
                return "A";
            else if (score >= 89)
                return "A-";
            color = Colors.YellowGreen;
            if (score >= 84)
                return "B+";
            else if (score >= 79)
                return "B";
            else if (score >= 74)
                return "B-";
            color = Colors.Goldenrod;
            if (score >= 69)
                return "C+";
            else if (score >= 64)
                return "C";
            else if (score >= 59)
                return "C-";
            color = Colors.Red;
            if (score >= 40)
                return "D";
            else if (score >= 20)
                return "E";
            return "F";
        }


        //  操作指令
        //caladd#int_var,1:| 添加1至int_ver
        //calsub#int_var,1:| 添加1至int_ver
        //calmul#int_var,2:| 乘2至int_ver
        //caldiv#int_var,1:| 乘2至int_ver
        //calset#int_var,1:| 设置
        //calset#dat_var,1:| 目前支持gobj中 int,str,flt,bool(隐藏自带)
        //                      rdi 随机数int,值1,值2 rdd 随机数double,值1,值2

        //  判断指令
        //ifequ#int_var1,int_var2:| 判断是否相等
        //ifneq#int_var1,int_var2:| 判断是否不等
        //ifbeq#int_var1,int_var2:| 判断var1>=var2
        //ifbig#int_var1,int_var2:| 判断var1>var2
        //ifleq#int_var1,int_var2:| 判断var1<=var2
        //iflow#int_var1,int_var2:| 判断var1<var2
        //ifor:| 如果前面的判断均为true,则返回true,否则继续执行


        /// <summary>
        /// 单位换算缩写
        /// </summary>
        /// <param name="value">转换值</param>
        /// <returns>缩写结果</returns>
        public static string UnitConvert(long value, string tostr = "f1")
        {
            string neg = value < 0 ? "-" : "";
            value = Math.Abs(value);
            ////if (value < 1000)//因为旧版本转换不直观,更换新版
            ////    return neg + value.ToString();
            ////else if (value < 1000000)
            ////    return neg + (value / 1000.0).ToString(tostr) + 'k';
            ////else if (value < 1000000000)
            ////    return neg + (value / 1000000.0).ToString(tostr) + 'm';
            ////else if (value < 1000000000000)
            ////    return neg + (value / 1000000000.0).ToString(tostr) + 'b';
            ////else
            ////    return neg + (value / 1000000000000.0).ToString(tostr) + 't';
            if (value < 1000)
                return neg + value;
            else if (value < 100000)
                return neg + (value / 1000.0).ToString(tostr) + 'k';
            else if (value < 1000000)
                return neg + (value / 10000.0).ToString(tostr) + 'w';
            else if (value < 100000000)
                return neg + (value / 1000000.0).ToString(tostr) + "m";
            else
                return neg + (value / 100000000.0).ToString(tostr) + 'b';
        }
        /// <summary>
        /// 单位转换时间
        /// </summary>
        /// <param name="h">小时</param>
        public static string DateConvert(double h, string tostr = "f1")
        {
            string neg = h < 0 ? "-" : "";
            h = Math.Abs(h);
            if (h < 1.5)
                return neg + (h * 60).ToString(tostr) + 'm';
            else if (h < 48)
                return neg + h.ToString(tostr) + 'h';
            else if (h < 960)
                return neg + (h / 24).ToString(tostr) + 'd';
            else if (h < 3072)
                return neg + (h / 720).ToString(tostr) + 'm';
            else
                return neg + (h / 8760).ToString(tostr) + 'y';
        }
        /// <summary>
        /// 单位换算大小 单位MB
        /// </summary>
        /// <param name="speed">MB</param>
        public static string SizeConvert(int speed, string tostr = "f2")
        {
            if (speed < 5000)
                return $"{speed} MB";
            return $"{(speed / 1000.0).ToString(tostr)} GB";
        }

        /// <summary>
        /// Compares the two strings based on letter pair matches
        /// </summary>
        /// <returns>The percentage match from 0.0 to 1.0 where 1.0 is 100%</returns>
        public static double CompareStrings(string[] str1, string[] str2)
        {
            List<string> pairs1 = str1.ToList();
            List<string> pairs2 = str2.ToList();

            int intersection = 0;
            int union = pairs1.Count + pairs2.Count;

            for (int i = 0; i < pairs1.Count; i++)
            {
                for (int j = 0; j < pairs2.Count; j++)
                {
                    if (pairs1[i] == pairs2[j])
                    {
                        intersection++;
                        pairs2.RemoveAt(j);//Must remove the match to prevent "GGGG" from appearing to match "GG" with 100% success

                        break;
                    }
                }
            }
            return (2.0 * intersection) / union;
        }

        /// <summary>
        /// Generates an array containing every
        /// two consecutive letters in the input string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string[] LetterPairs(string str)
        {
            int numPairs = str.Length - 1;

            string[] pairs = new string[numPairs];

            for (int i = 0; i < numPairs; i++)
            {
                pairs[i] = str.Substring(i, 2);
            }
            return pairs;
        }

        /// <summary>
        /// 刷新时间使用的方法,方便所有窗口使用
        /// </summary>
        /// <param name="span">时间经过</param>
        /// <param name="mw">主窗口</param>
        public delegate void TimeRels(TimeSpan span, IMainWindow mw);

        /// <summary>
        /// 获取资源笔刷
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Brush ResourcesBrush(BrushType name)
        {
            return (Brush)Application.Current.Resources.MergedDictionaries.Last()[name.ToString()];
        }
        public enum BrushType
        {
            Primary,
            PrimaryTrans,
            PrimaryTrans4,
            PrimaryTransA,
            PrimaryTransE,
            PrimaryLight,
            PrimaryLighter,
            PrimaryDark,
            PrimaryDarker,
            PrimaryText,

            Secondary,
            SecondaryTrans,
            SecondaryTrans4,
            SecondaryTransA,
            SecondaryTransE,
            SecondaryLight,
            SecondaryLighter,
            SecondaryDark,
            SecondaryDarker,
            SecondaryText,

            DARKPrimary,
            DARKPrimaryTrans,
            DARKPrimaryTrans4,
            DARKPrimaryTransA,
            DARKPrimaryTransE,
            DARKPrimaryLight,
            DARKPrimaryLighter,
            DARKPrimaryDark,
            DARKPrimaryDarker,
            DARKPrimaryText,
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public enum MSGType
        {
            /// <summary>
            /// 无任何警告性质,只是给玩家看看
            /// </summary>
            none,
            /// <summary>
            /// 提醒玩家
            /// </summary>
            notify,
            /// <summary>
            /// 警告玩家
            /// </summary>
            warning,
            /// <summary>
            /// 产生错误
            /// </summary>
            error
        }

        /// <summary>
        /// 程序类型
        /// </summary>
        public enum SoftWareType : byte
        {
            //1-63 系统程序 无法自行打开 允许多开
            None = 0,
            MessageBox,
            ImageBox,
            //各种viewer
            ViewerVideo,
            ViewerFood,
            //65-127 系统程序 无法自行打开 不允许多开
            HelloWorld = 65,
            NewGame,
            SoftwareCenter,//虽然说无法自行打开,但是其实可以打开 相当于隐藏起来
            CommandLinePrompt,
            WidgetCenter,

            //129-191 标准可执行程序 可以自行打开 允许多开
            ItemList = 129,
            Tutorial,
            InternetExplorer,
            Report,

            //193-255 标准可执行程序 可以自行打开 不允许多开
            GameSetting = 193,
            MyComputer,
            Sbeam,
            PersonalInformation,
            LikeStudy,
            BetterBuy,
            NiliVideo,
            OldPainter,
            PartTimeJob,
            OBS,
            TaskManager,
            Mail,
            MusicPlayer,
            SleepHelper,
            VUPMotionCapture,
            VideoEdit,//支持不重复视频多开
            VideoEncoder,//支持双击后添加内容至VE
            PicTool,
        }

        /// <summary>
        /// 窗体大小支持
        /// </summary>
        public enum WindowsSizeChange
        {
            /// <summary>
            /// 固定大小,不可修改
            /// </summary>
            Fixed,
            /// <summary>
            /// 支持全屏
            /// </summary>
            AllowMax,
            /// <summary>
            /// 支持垂直大小修改
            /// </summary>
            AllowVertical,
            /// <summary>
            /// 支持水平大小修改
            /// </summary>
            AllowHorizontal,
            /// <summary>
            /// 支持任意大小修改
            /// </summary>
            AllowBoth
        }

        /// <summary>
        /// 启动URL
        /// </summary>
        public static void StartURL(string url)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "explorer.exe";
                startInfo.UseShellExecute = false;
                startInfo.Arguments = url;
                Process.Start(startInfo);
            }
        }

        public static StringBuilder Log = new StringBuilder();
        public static void LogAdd(string log, object fromthis)
        {
            Log.Append('[');
            Log.Append(DateTime.Now.ToShortTimeString());
            Log.Append(']');
            Log.Append('(');
            Log.Append(fromthis.GetType().ToString());
            Log.Append(") ");
            Log.AppendLine(log);
        }

        public static string LogToString()
        {
            string str = Log.ToString();
            Log.Clear();
            return str;
        }
    }

}
