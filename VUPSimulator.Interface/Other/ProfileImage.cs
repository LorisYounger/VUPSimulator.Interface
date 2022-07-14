using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 随机生成profile图片
    /// </summary>
    public class ProfileImage
    {
        public const int len = 5;
        public List<string>[] Files = new List<string>[len];
        public Dictionary<int, BitmapImage> cache = new Dictionary<int, BitmapImage>();
        public ProfileImage()
        {
            for (int i = 0; i < len; i++)
                Files[i] = new List<string>();
        }
        public BitmapImage GetRndImage(int hash)
        {
            if (cache.ContainsKey(hash))
                return cache[hash];

            string[] imgs = new string[len];
            for (int i = 0; i < len; i++)
                imgs[i] = Files[i][Math.Abs(hash / (i + 1)) % Files.Length];

            BitmapImage bitmapImage = new BitmapImage();
            
            using (Bitmap bitMap = new Bitmap(512, 512))
            {
                using (Graphics g1 = Graphics.FromImage(bitMap))
                {
                    for (int i = len-1; i >= 0; i--)
                        using (var img = Image.FromFile(imgs[i]))
                            g1.DrawImage(img, 0, 0, 512, 512);
                }
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
            }
            cache.Add(hash, bitmapImage);
            return bitmapImage;
        }
        public BitmapImage GetRndImage(string name) => GetRndImage(name.GetHashCode());
    }
}
