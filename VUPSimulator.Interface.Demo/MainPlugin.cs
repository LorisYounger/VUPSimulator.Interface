using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUPSimulator.Interface;

namespace CheatEngine
{
    public class MainPlugin : VUPSimulator.Interface.MainPlugin
    {

        public MainPlugin(IMainWindow mw) : base(mw) { }

        /// <summary>
        /// 初始化游戏数据库
        /// </summary>
        /// <returns></returns>
        public override async Task Load()
        {
            //添加窗体到游戏三方菜单栏
            MW.Core.SoftWares.Add(new CheatEngine());
        }

        //因为不需要对游戏数据干啥,所以这些类保持为空即可
        //public override async void EndGame()
        //{
        //}
        //public override async void StartGame()
        //{          
        //}
        //public override async void Save()
        //{
        //}

    }
}
