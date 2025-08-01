﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using LinePutScript;
using LinePutScript.Localization.WPF;
using Panuon.WPF;
using static VUPSimulator.Interface.UIData;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 物品类
    /// </summary>
    public class Item : Line
    {
        /// <summary>
        /// 物品类型列表,如果需要自己DIY物品可以直接使用int 之后会添加相关接口TODO
        /// </summary>
        public enum ItemType
        {
            none,//无
            l2d_base,//L2D基础类
            l2d,//L2D立绘
            cpu,//CPU部件
            gpu,//GPU部件
            memory,//内存
            motherboard,//主板
            camera,//摄像头
            microphone,//麦克风
            game_base,//游戏基本类
            game,//游戏类
            videoraw,//未剪辑的视频,虽然说不是item类,但是反正都可以拿来用
            videoedit,//未剪辑的视频,虽然说不是item类,但是反正都可以拿来用
            videofin,//未剪辑的视频,虽然说不是item类,但是反正都可以拿来用
            food_all,//所有食物
            food_nohealth,//零食
            food_health,//主食
            food_drink,//饮料
            food_functional,//功能性
            food_drug,//药品
        }
        public ItemType Type
        {
            get => (ItemType)Enum.Parse(typeof(ItemType), info);
            set => info = value.ToString();
        }
        public Item(ILine line) : base(line)
        {

        }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemIdentifier
        {
            get => Find("name").Info;
            set => FindorAdd("name").Info = value;
        }
        /// <summary>
        /// 物品显示名称
        /// </summary>
        public string ItemDisplayName
        {
            get => GetString("displayname", ItemIdentifier);
            set => SetString("displayname", value);
        }
        /// <summary>
        /// 物品数量
        /// </summary>
        public int Many
        {
            get
            {
                ISub sub = Find("many");
                if (sub == null)
                    return 1;
                return sub.InfoToInt;
            }
            set => FindorAdd("many").InfoToInt = value;
        }
        /// <summary>
        /// 允许多个物品数量堆叠
        /// </summary>
        public virtual bool AllowMultiple => true;

        /// <summary>
        /// 物品图片
        /// </summary>
        public virtual string Image
        {
            get
            {
                var line = Find("image");
                if (line == null)
                {
                    return $"item_{info}_{ItemIdentifier}";
                }
                return line.Info;
            }
        }
        /// <summary>
        /// 物品图片
        /// </summary>
        public virtual ImageSource ImageSourse(IMainWindow mw) => mw.Core.ImageSources.FindImage(Image, "item_" + info);

        /// <summary>
        /// 根据物品类型自动生成相应Item (给mod作者准备的)
        /// </summary>
        public static Dictionary<string, Func<ILine, Item>> NewItemFunction = new Dictionary<string, Func<ILine, Item>>();
        /// <summary>
        /// 根据物品类型自动生成相应Item
        /// </summary>
        /// <param name="line">物品</param>
        /// <returns></returns>
        public static Item New(ILine line)
        {
            Item item = null;
            switch (line.GetString().ToLower())
            {
                case "l2d_base":
                    item = new Item_L2D_base(line);
                    break;
                case "l2d":
                    item = new Item_L2D(line);
                    break;
                case "cpu":
                    item = new Item_CPU(line);
                    break;
                case "gpu":
                    item = new Item_GPU(line);
                    break;
                case "memory":
                    item = new Item_Memory(line);
                    break;
                case "motherboard":
                    item = new Item_MotherBoard(line);
                    break;
                case "camera":
                    item = new Item_Camera(line);
                    break;
                case "microphone":
                    item = new Item_Microphone(line);
                    break;
                case "food_nohealth":
                    item = new Food_NoHealth(line);
                    break;
                case "food_health":
                    item = new Food_Health(line);
                    break;
                case "food_drink":
                    item = new Food_Drink(line);
                    break;
                case "food_functional":
                    item = new Food_Functional(line);
                    break;
                case "food_drug":
                    item = new Food_Drug(line);
                    break;
                default:
                    if (NewItemFunction.TryGetValue(line.GetString().ToLower(), out var function))
                    {
                        item = function(line);
                    }
                    item ??= new Item(line);
                    break;
            }
            return item;
        }
    }
    /// <summary>
    /// 可出售的物品
    /// </summary>
    public abstract class Item_Salability : Item
    {
        public Item_Salability(ILine line) : base(line)
        {

        }

        /// <summary>
        /// 物品单价
        /// </summary>
        public double Price
        {
            get => GetDouble("price", 0);
            set => SetDouble("price", value);
        }
        /// <summary>
        /// 物品描述
        /// </summary>
        public virtual string Description
        {
            get => GetString("desc", ItemDisplayName);
            set => SetString("desc", value);
        }
        /// <summary>
        /// 物品分类
        /// </summary>
        public virtual string[] Categories => Find("categories").GetInfos();

        /// <summary>
        /// 物品排序值
        /// </summary>
        public abstract double SortValue { get; }

        /// <summary>
        /// 更好卖物品
        /// </summary>
        public class BetterBuyItem : NotifyPropertyChangedBase
        {
            /// <summary>
            /// 可出售的物品
            /// </summary>
            public Item_Salability SalaItem;
            private BetterBuyData data;
            private Dispatcher d;
            public BetterBuyItem(Item_Salability salaitem, BetterBuyData data, IMainWindow mw)
            {
                SalaItem = salaitem;
                this.data = data;
                d = mw.Dispatcher;
                d.Invoke(() => { ImageShot = salaitem.ImageSourse(mw); });
                UpdateDiscount();
            }
            public BetterBuyItem(Dispatcher dispatcher, Item_Salability salaitem, BetterBuyData data, ImageSource sourse, int quantity, int discount)
            {
                SalaItem = salaitem;
                this.data = data;
                d = dispatcher;
                d.Invoke(() => { ImageShot = sourse; }, DispatcherPriority.ApplicationIdle);
                UpdateDiscount();
                _quantity = quantity;
                _discount = discount;
            }
            /// <summary>
            /// 物品图像
            /// </summary>
            public ImageSource ImageShot { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name => SalaItem.ItemIdentifier;
            /// <summary>
            /// 显示名称
            /// </summary>
            public string DisplayName => SalaItem.ItemDisplayName;
            /// <summary>
            /// 物品描述
            /// </summary>
            public string Description => SalaItem.Description;
            /// <summary>
            /// 物品分类
            /// </summary>
            public string[] Categories => SalaItem.Categories;
            /// <summary>
            /// 物品价格
            /// </summary>
            public double Price => SalaItem.Price;
            /// <summary>
            /// 商品实际价格
            /// </summary>
            public double RealPrice => SalaItem.Price * Discount / 100.0;
            /// <summary>
            /// 选择的物品个数
            /// </summary>
            public int Quantity { get => _quantity; set => Set(ref _quantity, value); }
            private int _quantity = 1;
            /// <summary>
            /// 商品折扣 (100%)
            /// </summary>
            public int Discount { get => _discount; set => Set(ref _discount, value); }
            private int _discount = 100;
            /// <summary>
            /// 更新商品折扣
            /// </summary>
            public void UpdateDiscount()
            {
                if (data.Discont.TryGetValue(SalaItem.ItemIdentifier, out int dis))
                {
                    Discount = dis;
                    return;
                }
                int mindis = data.Discont.TryGetValue("discount_all", out dis) ? dis : 100;
                foreach (var item in Categories)
                {
                    if (data.Discont.TryGetValue(item, out dis))
                    {
                        mindis = Math.Min(dis, mindis);
                    }
                }
                Discount = mindis;
            }
            /// <summary>
            /// 物品排序值
            /// </summary>
            public double SortValue => SalaItem.SortValue;
            /// <summary>
            /// 克隆自身
            /// </summary>
            public BetterBuyItem Clone()
            {
                return new BetterBuyItem(d, SalaItem, data, ImageShot, Quantity, Discount);
            }
        }


    }


}
