using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 游戏主题
    /// </summary>
    public class Theme
    {
        public string Name;
        public string xName;
        public string Image;
        public ImageResources Images;
        public LpsDocument ThemeColor;
        public Theme(LpsDocument lps)
        {
            xName = lps.First().Name;
            Name = lps.First().Info;
            Image = lps.First().Find("image").info;

            lps.RemoveAt(0);
            ThemeColor = lps;

            Images = new ImageResources();
        }
    }
}
