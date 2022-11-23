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
    public abstract class CommentBase : Line
    {
        public readonly string ContentHash;
        public CommentType Type;

        public CommentBase(Line line) : base(line)
        {
            ContentHash = Function.GetHashCode(line.text);
            Type = (CommentType)Enum.Parse(typeof(CommentType), line.info, true);
        }

        //public Comment Create(LpsDocument CommentData, Line data)
        //{
        //    switch ((CommentType)Enum.Parse(typeof(CommentType), info, true))
        //    {
        //        case CommentType.Game:
        //            return new Comment_Game(this, data);
        //        //case CommentType.Video:
        //        //    break;
        //        //case CommentType.Stream:
        //        //    break;
        //        default:
        //            return new Comment_base(this, data);
        //    }
        //}
    }
}
