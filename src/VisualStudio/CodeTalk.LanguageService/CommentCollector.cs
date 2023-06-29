//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.CodeTalk.LanguageService
{
    public class CommentCollector : DefaultCodeVisitor
    {
        public override void VisitBlock(Block block)
        {
            if (block != null && block.AssociatedComment != null)
            {
                m_commentsInFile.Add(block.AssociatedComment);
            }
            VisitChildren(block);
        }

        public override void VisitCodeFile(CodeFile codeFile)
        {
            if ( codeFile != null && codeFile.AssociatedComment != null )
            {
                m_commentsInFile.Add(codeFile.AssociatedComment);
            }
            VisitChildren(codeFile);
        }

        public override void VisitFunction(FunctionDefinition functionDefinition)
        {
            if ( functionDefinition != null && functionDefinition.AssociatedComment != null )
            {
                m_commentsInFile.Add(functionDefinition.AssociatedComment);
            }
            VisitChildren(functionDefinition);
        }

        public override void VisitNamespace(NamespaceDefinition names)
        {
            if ( names != null && names.AssociatedComment != null )
            {
                m_commentsInFile.Add(names.AssociatedComment);
            }
            VisitChildren(names);
        }

        public override void VisitUserDefinedType(UserDefinedType udt)
        {
            if ( udt != null && udt.AssociatedComment != null )
            {
                m_commentsInFile.Add(udt.AssociatedComment);
            }
            VisitChildren(udt);
        }

        public override void VisitVariable(Variable variable)
        {
            if ( variable != null && variable.AssociatedComment != null )
            {
                m_commentsInFile.Add(variable.AssociatedComment);
            }
            VisitChildren(variable);
        }

        public override void VisitProperty(MemberProperty memberProperty)
        {
            if ( memberProperty != null && memberProperty.AssociatedComment != null )
            {
                m_commentsInFile.Add(memberProperty.AssociatedComment);
            }
            VisitChildren(memberProperty);
        }

        public override void Visit(ISyntaxEntity entity)
        {
            if ( entity != null && entity.AssociatedComment != null )
            {
                m_commentsInFile.Add(entity.AssociatedComment);
            }
        }
        public IEnumerable<Comment> Comments {  get { return m_commentsInFile;  } }

        List<Comment> m_commentsInFile = new List<Comment>();
    }
}
