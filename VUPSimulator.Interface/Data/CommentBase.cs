using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;
using static VUPSimulator.Interface.Comment;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 评论数据表
    /// </summary>
    public class CommentBase : Line
    {
        /// <summary>
        /// 评论Hash值
        /// </summary>
        public readonly string ContentHash;
        /// <summary>
        /// 类型
        /// </summary>
        public CommentType Type;
        /// <summary>
        /// 评论内容
        /// </summary>
        public new string Comments;

        public CommentBase(ILine line, ICore core) : base(line)
        {
            Comments = line.Text;
            ContentHash = Function.GetHashCode(Comments);
            core.I18Comment.Add(ContentHash, Comments);
            this.text = ContentHash;
            Type = (CommentType)Enum.Parse(typeof(CommentType), line.info, true);
        }
        /// <summary>
        /// 绑定的游戏名称 (若为Type:Game)
        /// </summary>
        public string Game
        {
            get
            {
                if (game == null)
                    game = this[(gstr)"game"];
                return game;
            }
        }
        private string game = null;


        public Comment Create()
        {
            switch (Type)
            {
                case CommentType.Game:
                    return new Comment_Game(this) { Comments = Comments };
                //case CommentType.Video:
                //    break;
                //case CommentType.Stream:
                //    break;
                default:
                    return new Comment(this) { Comments = Comments };
            }
        }
    }
}
