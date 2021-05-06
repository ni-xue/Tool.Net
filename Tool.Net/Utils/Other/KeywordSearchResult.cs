using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Utils.Other
{
    /// <summary>
    /// 表示一个查找结果
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    internal struct KeywordSearchResult
    {
        private int index;
        private string keyword;
        /// <summary>
        /// 只读类型，表示空
        /// </summary>
        public static readonly KeywordSearchResult Empty = new KeywordSearchResult(-1, string.Empty);

        /// <summary>
        /// 初始化，带参数
        /// </summary>
        /// <param name="index">位置</param>
        /// <param name="keyword">关键词</param>
        public KeywordSearchResult(int index, string keyword)
        {
            this.index = index;
            this.keyword = keyword;
        }

        /// <summary>
        /// 位置
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword
        {
            get { return keyword; }
        }
    }
}
