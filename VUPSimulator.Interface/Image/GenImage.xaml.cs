using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using LinePutScript;

namespace VUPSimulator.Interface
{

    /// <summary>
    /// 自动生成的图片,可用于Nili等各种场合
    /// </summary>
    public partial class GenImage : UserControl
    {
        /// <summary>
        /// 图片名字,方便调用
        /// </summary>
        public new string Name;
        IMainWindow mw;
        List<GIBase> GIBases = new List<GIBase>();
        /// <summary>
        /// 生成封面
        /// </summary>
        /// <param name="data">数据</param>        
        public GenImage(IMainWindow mw, Line data)
        {
            InitializeComponent();
            Name = data.Info;
            this.mw = mw;
            foreach (Sub sub in data)
                GIBases.Add(GIBase.Create(sub));
            Rels();
        }
        /// <summary>
        /// 生成封面
        /// 生成顺序 -背景-用户图像-文本
        /// </summary>
        /// <param name="mw">主窗体</param>
        /// <param name="giBases">所有控件</param>
        public GenImage(IMainWindow mw, string name, params GIBase[] giBases)
        {
            InitializeComponent();
            this.mw = mw;
            Name = name;
            GIBases.AddRange(giBases);
            Rels();
        }
        public void Rels()
        {
            InGrid.Children.Clear();
            foreach (var gi in GIBases)
                InGrid.Children.Add(gi.GenUI(mw));
        }
        public Line ToLine()
        {
            Line Data = new Line("gimg", Name);
            foreach (var gi in GIBases)
                Data.Add(gi.ToSub());
            return Data;
        }
        /// <summary>
        /// 图层基本类
        /// </summary>
        public abstract class GIBase
        {
            public double Opacity = 1;
            public readonly string Type;
            public GIBase(string type, double opacity)
            {
                Opacity = opacity;
                Type = type;
            }
            public GIBase(Sub sub)
            {
                Opacity = sub.Infos.GetDouble("o", 1);
            }
            public abstract UIElement GenUI(IMainWindow mw);
            public virtual Sub ToSub() => new Sub(Type, "o=" + Opacity);

            protected private static ImageSource getImage(IMainWindow mw, string image)
            {
                if (image.StartsWith(":"))
                {
                    if (System.IO.File.Exists(image))
                    {
                        return new BitmapImage(new Uri(image));
                    }
                    else
                    {
                        return new BitmapImage(new Uri("pack://application:,,,/Res/Image/system/error.png"));
                    }
                }
                else
                {
                    return mw.Core.ImageSources.FindImage(image);
                }
            }

            /// <summary>
            /// 创建控件
            /// </summary>
            /// <param name="sub">数据</param>
            /// <returns></returns>
            public static GIBase Create(Sub sub)
            {
                switch (sub.Name)
                {
                    case "GIBackGround":
                        return new GIBackGround(sub);
                    case "GIText":
                        return new GIText(sub);
                    case "GIImage":
                        return new GIImage(sub);
                    default:
                        return null;
                }
            }

        }
        /// <summary>
        /// 带位置的控件基本类
        /// </summary>
        public abstract class GIPlaceBase : GIBase
        {
            public GIPlaceBase(string type, double x, double y, double rotate = 0, double opacity = 1) : base(type, opacity)
            {
                PointX = x;
                PointY = y;
                Rotate = rotate;
            }
            public GIPlaceBase(Sub sub) : base(sub)
            {
                PointX = sub.Infos.GetDouble("x", 0);
                PointY = sub.Infos.GetDouble("y", 0);
                Rotate = sub.Infos.GetDouble("r", 0);
            }
            /// <summary>
            /// 位置X
            /// </summary>
            public double PointX;
            /// <summary>
            /// 位置Y
            /// </summary>
            public double PointY;
            /// <summary>
            /// 旋转角度
            /// </summary>
            public double Rotate;

            public override Sub ToSub()
            {
                var sub = base.ToSub();
                sub.Infos[(gdbe)"x"] = PointX;
                sub.Infos[(gdbe)"x"] = PointY;
                return sub;
            }
        }
        /// <summary>
        /// 生成背景,全覆盖
        /// </summary>
        public class GIBackGround : GIBase
        {
            public string BackGround;
            public GIBackGround(string backgroundpath, double opacity = 1) : base("GIBackGround", opacity)
            {
                BackGround = backgroundpath;
            }
            public GIBackGround(Sub sub) : base(sub)
            {
                BackGround = sub.Infos.GetString("bg", "");
            }
            public override UIElement GenUI(IMainWindow mw)
            {
                return new Image() { Source = getImage(mw, BackGround), Opacity = Opacity, Stretch = Stretch.UniformToFill };
            }
            public override Sub ToSub()
            {
                var sub = base.ToSub();
                sub.Infos[(gstr)"bg"] = BackGround;
                return sub;
            }
        }
        /// <summary>
        /// 生成文本
        /// </summary>
        public class GIText : GIPlaceBase
        {
            public int TextSize;
            public Color TextColor;
            public string Text;
            public GIText(double x, double y, int textSize, string textcolor, string text, double rotate = 0, double opacity = 1) : base("GIText", x, y, rotate, opacity)
            {
                TextColor = Function.HEXToColor(textcolor);
                TextSize = textSize;
                Text = text;
            }
            public GIText(Sub sub) : base(sub)
            {
                TextSize = sub.Infos.GetInt("s", 12);
                TextColor = Function.HEXToColor(sub.Infos.GetString("c", "#000000"));
                Text = sub.Infos.GetString("t", "");
            }
            public override UIElement GenUI(IMainWindow mw)
            {
                return new Label()
                {
                    Margin = new Thickness(PointX, PointY, 0, 0),
                    FontSize = TextSize,
                    Foreground = new SolidColorBrush(TextColor),
                    Background = null,
                    Padding = new Thickness(0),
                    Content = Text,
                    FontWeight = FontWeights.Bold,
                    Opacity = Opacity,
                    LayoutTransform = new RotateTransform(Rotate),
                    RenderTransformOrigin = new Point(0.5, 0.5),
                };
            }

            public override Sub ToSub()
            {
                var sub = base.ToSub();
                sub.Infos[(gint)"s"] = TextSize;
                sub.Infos[(gstr)"c"] = Function.ColorToHEX(TextColor);
                sub.Infos[(gstr)"t"] = Text;
                return sub;
            }
        }
        /// <summary>
        /// 生成图片
        /// </summary>
        public class GIImage : GIPlaceBase
        {
            public string ImagePath;
            public double Size;
            public GIImage(double x, double y, string imagepath, double size, double rotate = 0, double opacity = 1) : base("GIImage", x, y, rotate, opacity)
            {
                ImagePath = imagepath;
                Size = size;
            }
            public GIImage(Sub sub) : base(sub)
            {
                ImagePath = sub.Infos.GetString("p", "");
                Size = sub.Infos.GetDouble("s", 50);
            }
            public override UIElement GenUI(IMainWindow mw)
            {
                return new Image()
                {
                    Source = getImage(mw, ImagePath),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(PointX, PointY, 0, 0),
                    Opacity = Opacity,
                    Width = Size,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    LayoutTransform = new RotateTransform(Rotate),
                };
            }
            public override Sub ToSub()
            {
                var sub = base.ToSub();
                sub.Infos[(gstr)"p"] = ImagePath;
                sub.Infos[(gdbe)"s"] = Size;
                return sub;
            }
        }
    }
}
