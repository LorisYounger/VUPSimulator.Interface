using LinePutScript;
using LinePutScript.Converter;
using Panuon.WPF;
using Panuon.WPF.UI.Configurations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VUPSimulator.Interface.Item_Salability;

namespace VUPSimulator.Interface
{
    public class UIData
    {
        public UIData()
        {
            // 初始化
            BetterBuy = new BetterBuyData();
            DesktopUI = new DesktopUIData();
            MusicPlayer = new MusicPlayerData();
            Nili = new NiliData();
            OBS = new ObsData();
            Sbeam = new SbeamData();
            OldPainter = new OldPainterData();
        }
        public UIData(LPS data)
        {
            BetterBuy = LPSConvert.DeserializeObject<BetterBuyData>(data["BetterBuyData"]);
            DesktopUI = new DesktopUIData(data);
            MusicPlayer = LPSConvert.DeserializeObject<MusicPlayerData>(data["MusicPlayerData"]);
            Nili = LPSConvert.DeserializeObject<NiliData>(data["NiliData"]);
            OBS = LPSConvert.DeserializeObject<ObsData>(data["OBSData"]);
            Sbeam = LPSConvert.DeserializeObject<SbeamData>(data["SbeamData"]);
            OldPainter = LPSConvert.DeserializeObject<OldPainterData>(data["OldPainterData"]);
        }
        public List<ILine> ToList()
        {
            List<ILine> data = new List<ILine>
            {
                LPSConvert.SerializeObject(BetterBuy,"BetterBuyData"),
                LPSConvert.SerializeObject(MusicPlayer,"MusicPlayerData"),
                LPSConvert.SerializeObject(Nili,"NiliData"),
                LPSConvert.SerializeObject(OBS,"OBSData"),
                LPSConvert.SerializeObject(Sbeam,"SbeamData"),
                LPSConvert.SerializeObject(OldPainter,"OldPainterData"),
            };
            DesktopUI.ToList(data);
            return data;
        }

        /// <summary>
        /// 更好买数据
        /// </summary>
        public BetterBuyData BetterBuy;
        /// <summary>
        /// 更好买数据
        /// </summary>
        public class BetterBuyData
        {
            /// <summary>
            /// 上次折扣时间
            /// </summary>
            [Line] public DateTime LastDiscont = DateTime.MinValue;

            /// <summary>
            /// 折扣数据
            /// </summary>
            [Line] public Dictionary<string, int> Discont = new Dictionary<string, int>();

            /// <summary>
            /// 定时购买的商品
            /// </summary>
            [Line] public ObservableCollection<ScheduleBuyItem> ScheduleBuyItems = new ObservableCollection<ScheduleBuyItem>();

            /// <summary>
            /// 购买历史
            /// </summary>
            [Line] public ObservableCollection<HistoryBuyItem> BuyHistory = new ObservableCollection<HistoryBuyItem>();

            /// <summary>
            /// 定时购买
            /// </summary>
            public class ScheduleBuyItem : NotifyPropertyChangedBase
            {
                public ScheduleBuyItem()
                {

                }
                public ScheduleBuyItem(BetterBuyItem betterBuyItem)
                {
                    this.betterBuyItem = betterBuyItem;
                    SalabilityItemName = betterBuyItem.Name;
                }
                /// <summary>
                /// 商品
                /// </summary>
                public Item_Salability.BetterBuyItem BetterBuyItem(IMainWindow mw)
                {
                    if (betterBuyItem == null)
                    {
                        Item_Salability v = mw.Core.Items_Salability.FirstOrDefault(x => x.ItemName == SalabilityItemName);
                        if (v != null)
                            betterBuyItem = new Item_Salability.BetterBuyItem(v, mw.Save.UIData.BetterBuy, mw);
                    }
                    return betterBuyItem;
                }
                private Item_Salability.BetterBuyItem betterBuyItem;
                /// <summary>
                /// 更好买物品
                /// </summary>
                [Line] public string SalabilityItemName { get; set; }

                /// <summary>
                /// 数量
                /// </summary>
                [Line] public int Quantity { get; set; }

                /// <summary>
                /// 频次(天)
                /// </summary>
                public enum BuyFrequency
                {
                    None = 0,
                    EveryDay = 1,
                    EveryWeek = 7,
                    EveryMonth = 30,
                }
                /// <summary>
                /// 购买频率
                /// </summary>
                [Line] public BuyFrequency Frequency { get; set; }

                /// <summary>
                /// 下次购买时间 (当NextBuyTime>Frequency则会自动购买)
                /// </summary>
                [Line] public int NextBuyTime { get; set; }

                public int Frequency_id
                {
                    get
                    {
                        switch (Frequency)
                        {
                            default:
                            case BuyFrequency.None:
                                return 0;
                            case BuyFrequency.EveryDay:
                                return 1;
                            case BuyFrequency.EveryWeek:
                                return 2;
                            case BuyFrequency.EveryMonth:
                                return 3;
                        }
                    }
                    set
                    {
                        switch (value)
                        {
                            default:
                            case 0:
                                Frequency = BuyFrequency.None;
                                break;
                            case 1:
                                Frequency = BuyFrequency.EveryDay;
                                break;
                            case 2:
                                Frequency = BuyFrequency.EveryWeek;
                                break;
                            case 3:
                                Frequency = BuyFrequency.EveryMonth;
                                break;
                        }
                    }
                }
            }
            /// <summary>
            /// 购买历史
            /// </summary>
            public class HistoryBuyItem
               : NotifyPropertyChangedBase
            {
                private Item_Salability.BetterBuyItem _betterBuyItem;

                /// <summary>
                /// 商品
                /// </summary>
                public Item_Salability.BetterBuyItem BetterBuyItem(IMainWindow mw)
                {
                    if (betterBuyItem == null)
                    {
                        Item_Salability v = mw.Core.Items_Salability.FirstOrDefault(x => x.ItemName == SalabilityItemName);
                        if (v != null)
                            _betterBuyItem = new Item_Salability.BetterBuyItem(v, mw.Save.UIData.BetterBuy, mw);
                    }
                    return betterBuyItem;
                }
                public Item_Salability.BetterBuyItem betterBuyItem
                {
                    get => _betterBuyItem;
                    set
                    {
                        _betterBuyItem = value;
                        SalabilityItemName = value.Name;
                    }
                }
                /// <summary>
                /// 更好买物品
                /// </summary>
                [Line] public string SalabilityItemName { get; set; }
                /// <summary>
                /// 数量
                /// </summary>
                [Line] public int Quantity { get; set; }

                /// <summary>
                /// 金额
                /// </summary>
                [Line] public double Amount { get; set; }

                /// <summary>
                /// 购买日期
                /// </summary>
                [Line] public DateTime BuyTime { get; set; }
            }
        }

        /// <summary>
        /// 桌面UI数据
        /// </summary>
        public DesktopUIData DesktopUI { get; set; }

        /// <summary>
        /// 桌面UI数据
        /// </summary>
        public class DesktopUIData
        {
            public DesktopUIData()
            {
                OftenUsed = new Line("UI_OftenUsed");
                Shortcut = new List<ILine>();
            }
            public DesktopUIData(LPS data)
            {
                OftenUsed = data["UI_OftenUsed"];
                Shortcut = data.FindAllLine("UI_Shortcut").ToList();
            }
            public void ToList(List<ILine> list)
            {
                list.Add(OftenUsed);
                list.AddRange(Shortcut);
            }
            /// <summary>
            /// 经常使用
            /// </summary>
            public ILine OftenUsed;
            /// <summary>
            /// ShortCut按钮
            /// </summary>
            public List<ILine> Shortcut;
        }


        /// <summary>
        /// 音乐播放器数据
        /// </summary>
        public MusicPlayerData MusicPlayer { get; set; }

        /// <summary>
        /// 音乐播放器数据
        /// </summary>
        public class MusicPlayerData
        {
            public enum LoopType
            {
                /// <summary>
                /// 无循环
                /// </summary>
                None,
                /// <summary>
                /// 单曲循环
                /// </summary>
                Single,
                /// <summary>
                /// 列表循环
                /// </summary>
                List
            }

            /// <summary>
            /// 是否循环播放
            /// </summary>
            [Line] public LoopType Loop = LoopType.None;
            /// <summary>
            /// 当前播放音乐名字
            /// </summary>
            [Line] public string playBGMName = "暂无播放音乐";
            /// <summary>
            /// 是否播放
            /// </summary>
            [Line] public bool IsPlaying = true;
        }
        /// <summary>
        /// Nili数据
        /// </summary>
        public NiliData Nili { get; set; }
        /// <summary>
        /// Nili数据
        /// </summary>
        public class NiliData
        {
            /// <summary>
            /// 上次自动生成Nili视频的时间
            /// </summary>
            [Line] public int LastTime = -30;
            /// <summary>
            /// 设置: 是否加入收入
            /// </summary>
            [Line] public bool IsJoinProfit = false;
        }
        /// <summary>
        /// OBS数据
        /// </summary>
        public ObsData OBS { get; set; }
        /// <summary>
        /// OBS数据
        /// </summary>
        public class ObsData
        {
            /// <summary>
            /// 设置: 码率
            /// </summary>
            [Line] public int BitRate = 6000;
            /// <summary>
            /// 设置: 是否使用CPU
            /// </summary>
            [Line] public bool UseCPU = false;
            /// <summary>
            /// 设置: 分辨率质量
            /// </summary>
            [Line] public Video.VideoResolution Resolution = Video.VideoResolution.r1920x1080;

            /// <summary>
            /// 设置: VUPX轴
            /// </summary>
            [Line] public double VUPX = 20;
            /// <summary>
            /// 设置: VUPY轴
            /// </summary>
            [Line] public double VUPY = 0;
            /// <summary>
            /// 设置: VUP高度
            /// </summary>
            [Line] public double VUPH = 150;
        }

        /// <summary>
        /// Sbeam数据
        /// </summary>
        public SbeamData Sbeam { get; set; }
        /// <summary>
        /// Sbeam数据
        /// </summary>
        public class SbeamData
        {
            /// <summary>
            /// 愿望单
            /// </summary>
            [Line] public List<string> WishList = new List<string>();
        }
        /// <summary>
        /// 老画师数据
        /// </summary>
        public OldPainterData OldPainter { get; set; }
        /// <summary>
        /// 老画师数据
        /// </summary>
        public class OldPainterData
        {

        }
    }

}
