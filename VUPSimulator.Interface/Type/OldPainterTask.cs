using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 老画师绘画任务
    /// </summary>
    public class OldPainterTask : Line
    {
        public OldPainterTask(ILine line) : base(line) { }
        public OldPainterTask() { }
        /// <summary>
        /// 画师任务
        /// </summary>
        public Author.Type Type
        {
            get => (Author.Type)this[(gint)"type"];
            set => this[(gint)("type")] = (int)value;
        }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author
        {
            get => this[(gstr)"author"];
            set => this[(gstr)"author"] = value;
        }
        /// <summary>
        /// 进度
        /// </summary>
        public double Process
        {
            get => this[(gdbe)"process"];
            set => this[(gdbe)"process"] = value;
        }
        /// <summary>
        /// 应征作者参数
        /// </summary>
        public class AuthorInTask : Sub
        {
            public AuthorInTask(ISub sub)
            {
                Set(sub);
            }
            public AuthorInTask(string author, double price)
            {
                Name = "authorintask";
                Infos.SetString("a", author);
                Infos.SetDouble("p", price);
            }
            /// <summary>
            /// 应征价格
            /// </summary>
            public double Price => Infos.GetDouble("p");
            /// <summary>
            /// 应征作者名称
            /// </summary>
            public string Author => Infos.GetString("a");
        }
        /// <summary>
        /// 所有应征作者
        /// </summary>
        public List<AuthorInTask> Authors
        {
            get
            {
                var list = new List<AuthorInTask>();
                foreach (var item in FindAll("authorintask"))
                {
                    list.Add(new AuthorInTask(item));
                }
                return list;
            }
        }
        /// <summary>
        /// 添加应征作者
        /// </summary>
        /// <param name="ait"></param>
        public void AddAuthorInTask(AuthorInTask ait) => Add(ait);
        /// <summary>
        /// 是否完毕(关闭企划/退款/完成)
        /// </summary>
        public bool IsFinish
        {
            get => GetBool("finish");
            set => SetBool("finish", value);
        }
        /// <summary>
        /// 设置当前项目为完毕
        /// </summary>
        public void ToFinishTask()
        {
            IsFinish = true;
            RemoveAll("authorintask");
        }
    }
}
