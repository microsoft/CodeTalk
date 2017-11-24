//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.LanguageService
{
    public class CodeFile : AbstractSyntaxEntity
    {
        internal CodeFile(string name, FileSpan location, ILanguage language) 
            : base(name, location, null, null)
        {
            base.CurrentCodeFile = this;
            Language = language;
        }

        public ILanguage Language
        {
            get; internal set;
        }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.CodeFile;
            }
        }

        public override void AcceptVisitor(ICodeVisitor visitor)
        {
            visitor?.VisitCodeFile(this);
        }

        public override string SpokenText()
        {
            return base.SpokenText();
        }

        public override string DisplayText()
        {
            return base.DisplayText();
        }

        public void AddComment(Comment comment)
        {
            m_comments.Add(comment);
        }

        public IEnumerable<ISyntaxEntity> GetContextAtLine(int lineNumber)
        {
            // modifed by prvai : fixed at source
            //lineNumber = lineNumber; //Silly VS starts counting from zero.
            List<ISyntaxEntity> entities = new List<ISyntaxEntity>();
            collect(this, lineNumber, entities);
            //Add the CodeFile to the context as it supersedes namespace.
            entities.Add(this as ISyntaxEntity);

            return entities;
        }

        public Comment GetCommentAtLine(int lineNumber)
        {
            // modifed by prvai : fixed at source
            //lineNumber = lineNumber; //Silly VS starts counting from zero.
            foreach ( var comment in m_comments )
            {
                if ( comment.Location.Encompasses(lineNumber) )
                {
                    return comment;
                }
            }
            return null;
        }

        #region SHORTCUTS_TO_GET_ENTITIES
        public IEnumerable<Comment> Comments {  get { return m_comments; } }
        #endregion

        private void collect(ISyntaxEntity node, int lineNumber, List<ISyntaxEntity> entities)
        {
            foreach ( var child in node.Children )
            {
                if (child.Location.Encompasses(lineNumber) )
                {
                    collect(child, lineNumber, entities);
                    entities.Add(child);
                }
            }
        }

        private List<Comment> m_comments = new List<Comment>();
    }

    //internal class CommentComparer : IEqualityComparer<Comment>
    //{
    //    private LocationComparer m_locComparer = new LocationComparer();
    //    public bool Equals(Comment x, Comment y)
    //    {
    //        return m_locComparer.Equals(x.Location, y.Location);
    //    }

    //    public int GetHashCode(Comment obj)
    //    {
    //        return m_locComparer.GetHashCode(obj.Location);
    //    }
    //}
}
