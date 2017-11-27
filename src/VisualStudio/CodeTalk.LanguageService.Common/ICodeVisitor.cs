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
    public interface ICodeVisitor
    {
        void Visit(ISyntaxEntity entity);

        void VisitCodeFile(CodeFile codeFile);

        void VisitNamespace(NamespaceDefinition names);

        void VisitUserDefinedType(UserDefinedType udt);

        void VisitClass(ClassDefinition userClass);

        void VisitStruct(ClassDefinition userStruct);

        void VisitEnum(Entities.UDT.EnumDefinition userEnum);

        void VisitInterface(InterfaceDefinition userInterface);

        void VisitFunction(FunctionDefinition functionDefinition);

        void VisitProperty(MemberProperty memberProperty);

        void VisitVariable(Variable variable);

        void VisitComment(Comment comment);

        void VisitBlock(Block block);

		void VisitTry(TryBlock tryBlock);

		void VisitCatch(CatchBlock catchBlock);

		void VisitIf(IfBlock ifBlock);

		void VisitElse(ElseBlock elseBlock);

		void VisitFor(ForBlock forBlock);

		void VisitForEach(ForEachBlock forEachBlock);

		void VisitWhile(WhileBlock whileBlock);

	}
}
