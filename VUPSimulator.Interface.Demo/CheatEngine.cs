using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUPSimulator.Interface;
namespace CheatEngine
{
    public class CheatEngine : ISoftWare
    {
        public string SoftwareName => "CE修改器";
        public string SoftwareInfo => "可以修改绝大部分游戏数据,是通关利器";

        public WindowsPageHandle NewSoftWare(IMainWindow mw, string args)
        {
            return new winCheatEngine(mw);
        }
    }
}
