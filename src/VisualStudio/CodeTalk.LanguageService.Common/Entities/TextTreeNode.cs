using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk
{
    public class TextTreeNode
    {
        public int Id;
        public string Text;
        public List<int> Children;
        public TextTreeNode(int id, string text, List<int> children)
        {
            Id = id;
            Text = text;
            Children = children;
        }
    }
    public class DrawizNode
    {
        string Text;
        public List<DrawizNode> Children;
        public DrawizNode(string text, List<DrawizNode> children)
        {
            Text = text;
            Children = children;
        }
    }
    public class createDrawizTree
    {
        public DrawizNode createDrawizTextTree(int index , List<TextTreeNode> textTreeNodes)
        {
            List<DrawizNode> children = new List<DrawizNode>();
            DrawizNode root = new DrawizNode(textTreeNodes[index].Text, children);
            foreach(int child in textTreeNodes[index].Children)
                root.Children.Add(createDrawizTextTree(child, textTreeNodes));
            return root;
        }
    }
}
