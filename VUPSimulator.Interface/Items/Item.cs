using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using LinePutScript;
using Panuon.WPF;

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
        public Item(Line line) : base(line)
        {

        }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemName
        {
            get => Find("name").Info;
            set => FindorAdd("name").Info = value;
        }
        public string ItemDisplayName
        {
            get => GetString("displayname", ItemName);
            set => SetString("displayname", value);
        }
        /// <summary>
        /// 物品数量
        /// </summary>
        public int Many
        {
            get
            {
                Sub sub = Find("many");
                if (sub == null)
                    return 1;
                return sub.InfoToInt;
            }
            set => FindorAdd("many").InfoToInt = value;
        }

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
                    return $"item_{info}_{ItemName}";
                }
                return line.Info;
            }
        }
        /// <summary>
        /// 物品图片
        /// </summary>
        public ImageSource ImageSourse(IMainWindow mw) => mw.Core.ImageSources.FindImage(Image, "item_" + info);
        /// <summary>
        /// 根据物品类型自动生成相应Item
        /// </summary>
        /// <param name="line">物品</param>
        /// <returns></returns>
        public static Item New(Line line)
        {
            Item item;
            switch (line.info.GetString().ToLower())
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
                    item = new Item(line);
                    break;
            }
            return item;
        }
        /// <summary>
        /// 根据物品类型自动生成相应可出售Item
        /// </summary>
        /// <param name="line">可出售物品</param>
        /// <returns></returns>
        public static Item_Salability NewSalability(Line line)
        {
            Item_Salability item;
            switch (line.info.GetString().ToLower())
            {
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
                default:
                    throw new Exception("MOD生成错误:不可出售的物品");
            }
            return item;
        }
    }
    /// <summary>
    /// 可出售的物品
    /// </summary>
    public abstract class Item_Salability : Item
    {
        public Item_Salability(Line line) : base(line)
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
            private Line data;
            private Dispatcher d;
            public BetterBuyItem(Item_Salability salaitem, Line data, IMainWindow mw)
            {
                SalaItem = salaitem;
                this.data = data;
                d = mw.Dispatcher;
                d.Invoke(() => { ImageShot = salaitem.ImageSourse(mw); });
                UpdateDiscount();
            }
            public BetterBuyItem(Dispatcher dispatcher, Item_Salability salaitem, Line data, ImageSource sourse, int quantity)
            {
                SalaItem = salaitem;
                this.data = data;
                d = dispatcher;
                d.Invoke(() => { ImageShot = sourse; });
                UpdateDiscount();
                _quantity = quantity;
            }
            /// <summary>
            /// 物品图像
            /// </summary>
            public ImageSource ImageShot { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name => SalaItem.ItemName;
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
                var dis = data.Find("discount_" + SalaItem.ItemName);
                if (dis != null)
                {
                    Discount = dis.InfoToInt;
                    return;
                }
                int mindis = 100;
                foreach (var item in Categories)
                {
                    dis = data.Find("discount_" + item);
                    if (dis != null)
                    {
                        mindis = Math.Min(dis.InfoToInt, mindis);
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
                return new BetterBuyItem(d, SalaItem, data, ImageShot, Quantity);
            }
        }
    }


}
