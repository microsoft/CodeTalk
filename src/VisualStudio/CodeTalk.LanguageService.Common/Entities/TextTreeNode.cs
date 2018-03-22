using Microsoft.CodeTalk.LanguageService;
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
    public class DrawizNode : AbstractSyntaxEntity
    {
        string Text;
        public List<DrawizNode> Children;

        public override SyntaxEntityKind Kind => SyntaxEntityKind.ImageDescription;

        public DrawizNode()
        {
            this.Text = "Empty";
            Children = new List<DrawizNode>();
            this.Location = new FileSpan(0, 0, 0, 0);
        }

        public DrawizNode(string text, List<DrawizNode> children)
        {
            Text = text;
            Children = children;
            this.Location = new FileSpan(0, 0, 0, 0);
        }

        public static DrawizNode createDrawizTextTree(int index, List<TextTreeNode> textTreeNodes)
        {
            List<DrawizNode> children = new List<DrawizNode>();
            DrawizNode root = new DrawizNode(textTreeNodes[index].Text, children);
            foreach (int child in textTreeNodes[index].Children)
            {
                root.AddChild(createDrawizTextTree(child, textTreeNodes));
                //root.Children.Add(createDrawizTextTree(child, textTreeNodes));
            }
            return root;
        }

        public override void AcceptVisitor(ICodeVisitor visitor)
        {
        }

        public override string SpokenText() => this.Text;

        public override string DisplayText() => this.Text;

    }
}
