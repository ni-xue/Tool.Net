using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tool.Utils.Other
{
    /// <summary>
    /// Aho-Corasick算法实现
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    internal class KeywordSearch
    {
        /// <summary>
        /// 构造节点
        /// </summary>
        /// <remarks>代码由逆血提供支持</remarks>
        private class Node
        {
            private Dictionary<char, Node> transDict;

            public Node(char c, Node parent)
            {
                this.Char = c;
                this.Parent = parent;
                this.Transitions = new List<Node>();
                this.Results = new List<string>();

                this.transDict = new Dictionary<char, Node>();
            }

            public char Char
            {
                get;
                private set;
            }

            public Node Parent
            {
                get;
                private set;
            }

            public Node Failure
            {
                get;
                set;
            }

            public List<Node> Transitions
            {
                get;
                private set;
            }

            public List<string> Results
            {
                get;
                private set;
            }

            public void AddResult(string result)
            {
                if (!Results.Contains(result))
                {
                    Results.Add(result);
                }
            }

            public void AddTransition(Node node)
            {
                this.transDict.Add(node.Char, node);
                this.Transitions = this.transDict.Values.ToList();
            }

            public Node GetTransition(char c)
            {
                Node node;
                if (this.transDict.TryGetValue(c, out node))
                {
                    return node;
                }

                return null;
            }

            public bool ContainsTransition(char c)
            {
                return GetTransition(c) != null;
            }
        }

        private Node root; // 根节点
        private string[] keywords; // 所有关键词

        /// <summary>
        /// 实例化，并赋值
        /// </summary>
        /// <param name="keywords"></param>
        public KeywordSearch(IEnumerable<string> keywords)
        {
            this.keywords = keywords.ToArray();
            this.Initialize();
        }

        /// <summary>
        /// 根据关键词来初始化所有节点
        /// </summary>
        private void Initialize()
        {
            this.root = new Node(' ', null);

            // 添加模式
            foreach (string k in this.keywords)
            {
                Node n = this.root;
                foreach (char c in k)
                {
                    Node temp = null;
                    foreach (Node tnode in n.Transitions)
                    {
                        if (tnode.Char == c)
                        {
                            temp = tnode; break;
                        }
                    }

                    if (temp == null)
                    {
                        temp = new Node(c, n);
                        n.AddTransition(temp);
                    }
                    n = temp;
                }
                n.AddResult(k);
            }

            // 第一层失败指向根节点
            List<Node> nodes = new List<Node>();
            foreach (Node node in this.root.Transitions)
            {
                // 失败指向root
                node.Failure = this.root;
                foreach (Node trans in node.Transitions)
                {
                    nodes.Add(trans);
                }
            }
            // 其它节点 BFS
            while (nodes.Count != 0)
            {
                List<Node> newNodes = new List<Node>();
                foreach (Node nd in nodes)
                {
                    Node r = nd.Parent.Failure;
                    char c = nd.Char;

                    while (r != null && !r.ContainsTransition(c))
                    {
                        r = r.Failure;
                    }

                    if (r == null)
                    {
                        // 失败指向root
                        nd.Failure = this.root;
                    }
                    else
                    {
                        nd.Failure = r.GetTransition(c);
                        foreach (string result in nd.Failure.Results)
                        {
                            nd.AddResult(result);
                        }
                    }

                    foreach (Node child in nd.Transitions)
                    {
                        newNodes.Add(child);
                    }
                }
                nodes = newNodes;
            }
            // 根节点的失败指向自己
            this.root.Failure = this.root;
        }

        /// <summary>
        /// 找出所有出现过的关键词
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<KeywordSearchResult> FindAllKeywords(string text)
        {
            List<KeywordSearchResult> list = new List<KeywordSearchResult>();

            Node current = this.root;
            for (int index = 0; index < text.Length; ++index)
            {
                Node trans;
                do
                {
                    trans = current.GetTransition(text[index]);

                    if (current == this.root)
                        break;

                    if (trans == null)
                    {
                        current = current.Failure;
                    }
                } while (trans == null);

                if (trans != null)
                {
                    current = trans;
                }

                foreach (string s in current.Results)
                {
                    list.Add(new KeywordSearchResult(index - s.Length + 1, s));
                }
            }

            return list;
        }

        /// <summary>
        /// 简单地过虑关键词
        /// </summary>
        /// <param name="text"></param>
        /// <param name="symbol">替换为？？？</param>
        /// <returns>如何替换字符为空，默认为星号</returns>
        public string FilterKeywords(string text, char symbol)
        {
            if (char.IsWhiteSpace(symbol)) { symbol = '*'; }
            StringBuilder sb = new StringBuilder();

            Node current = root;
            for (int index = 0; index < text.Length; index++)
            {
                Node trans;
                do
                {
                    trans = current.GetTransition(text[index]);

                    if (current == root)
                        break;

                    if (trans == null)
                    {
                        current = current.Failure;
                    }

                } while (trans == null);

                if (trans != null)
                {
                    current = trans;
                }

                // 处理字符
                if (current.Results.Count > 0)
                {
                    string first = current.Results[0];
                    sb.Remove(sb.Length - first.Length + 1, first.Length - 1);// 把匹配到的替换为**
                    sb.Append(new string(symbol, current.Results[0].Length));
                }
                else
                {
                    sb.Append(text[index]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 查找是否存在有违规字符
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool Contains(string text)
        {
            Node current = root;
            for (int index = 0; index < text.Length; index++)
            {
                Node trans;
                do
                {
                    trans = current.GetTransition(text[index]);

                    if (current == root)
                        break;

                    if (trans == null)
                    {
                        current = current.Failure;
                    }

                } while (trans == null);

                if (trans != null)
                {
                    current = trans;
                }

                // 处理字符
                if (current.Results.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
