//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using Microsoft.CodeTalk.LanguageService.Entities.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.LanguageService
{
    public class DefaultCodeVisitor : ICodeVisitor
    {
        public virtual void Visit(ISyntaxEntity entity)
        {
            if (entity != null)
            {
                entity.AcceptVisitor(this);
            }
        }

        public virtual void VisitBlock(Block block)
        {
            VisitChildren(block);
        }

        public virtual void VisitCodeFile(CodeFile codeFile)
        {
            VisitChildren(codeFile);
        }

        public virtual void VisitComment(Comment comment)
        {
            VisitChildren(comment);
        }

        public virtual void VisitFunction(FunctionDefinition functionDefinition)
        {
            VisitChildren(functionDefinition);
        }

        public virtual void VisitNamespace(NamespaceDefinition names)
        {
            VisitChildren(names);
        }

        public virtual void VisitUserDefinedType(UserDefinedType udt)
        {
            VisitChildren(udt);
        }

        public virtual void VisitClass(ClassDefinition userClass)
        {
            VisitChildren(userClass);
        }

        public virtual void VisitStruct(ClassDefinition userStruct)
        {
            VisitChildren(userStruct);
        }

        public virtual void VisitEnum(Entities.UDT.EnumDefinition userEnum)
        {
            VisitChildren(userEnum);
        }

        public virtual void VisitInterface(InterfaceDefinition userInterface)
        {
            VisitChildren(userInterface);
        }

        public virtual void VisitVariable(Variable variable)
        {
            VisitChildren(variable);
        }

        public virtual void VisitProperty(MemberProperty memberProperty)
        {
            VisitChildren(memberProperty);
        }

        public virtual void VisitTry(TryBlock tryBlock)
        {
            VisitChildren(tryBlock);
        }

        public virtual void VisitCatch(CatchBlock catchBlock)
        {
            VisitChildren(catchBlock);
        }

        public virtual void VisitIf(IfBlock ifBlock)
        {
            VisitChildren(ifBlock);
        }

        public virtual void VisitElse(ElseBlock elseBlock)
        {
            VisitChildren(elseBlock);
        }

        public virtual void VisitFor(ForBlock forBlock)
        {
            VisitChildren(forBlock);
        }

        public virtual void VisitForEach(ForEachBlock forEachBlock)
        {
            VisitChildren(forEachBlock);
        }

        public virtual void VisitWhile(WhileBlock whileBlock)
        {
            VisitChildren(whileBlock);
        }

        protected void VisitChildren(ISyntaxEntity entity)
        {
            if (entity != null && entity.Children != null)
            {
                foreach (ISyntaxEntity child in entity.Children)
                {
                    child.AcceptVisitor(this);
                }
            }
        }
    }
}
