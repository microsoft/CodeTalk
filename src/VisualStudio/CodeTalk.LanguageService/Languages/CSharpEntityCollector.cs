//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Globalization;
using Microsoft.CodeTalk.LanguageService.Entities.UDT;

namespace Microsoft.CodeTalk.LanguageService
{
    internal class CSharpEntityCollector : CSharpSyntaxWalker
    {
        internal CSharpEntityCollector(SyntaxTree tree, SyntaxWalkerDepth depth = SyntaxWalkerDepth.Trivia) 
            : base(depth)
        {
            m_currentTree = tree;
        }

        public CodeFile Root {  get { return m_currentCodeFile; } }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            if ( node == null )
            {
                throw new InvalidOperationException("Compilation Unit node is null!");
            }

            m_currentCodeFile = new CodeFile(node.Language, new FileSpan( m_currentTree.GetLineSpan(node.FullSpan)), new CSharp());
            m_currentParent = m_currentCodeFile;
            m_currentCodeFile.CurrentCodeFile = m_currentCodeFile;
            base.VisitCompilationUnit(node);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            if (node == null)
            {
                throw new InvalidOperationException("Namespace Declaration node is null!");
            }
            var namespaceObj = new NamespaceDefinition(node.Name.GetText().ToString().Trim(),
                                                new FileSpan(m_currentTree.GetLineSpan(node.Span)), 
                                                m_currentParent,
                                                m_currentCodeFile);
            namespaceObj.AssociatedComment = CSharpEntityCreationHelper.GetComment(namespaceObj, node, m_currentTree, m_currentCodeFile);

            ISyntaxEntity oldParent = setCurrentParent(namespaceObj);

            base.VisitNamespaceDeclaration(node);
            m_currentParent = oldParent;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var classObj = CSharpEntityCreationHelper.CreateClass(node, m_currentParent, m_currentCodeFile, m_currentTree);

            ISyntaxEntity oldParent = setCurrentParent(classObj);
            base.VisitClassDeclaration(node);
            m_currentParent = oldParent;
        }
        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            var classObj = CSharpEntityCreationHelper.CreateStruct(node, m_currentParent, m_currentCodeFile, m_currentTree);

            ISyntaxEntity oldParent = setCurrentParent(classObj);
            base.VisitStructDeclaration(node);
            m_currentParent = oldParent;
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            var intfObj = CSharpEntityCreationHelper.CreateInterface(node, m_currentParent, m_currentCodeFile, m_currentTree);

            ISyntaxEntity oldParent = setCurrentParent(intfObj);
            base.VisitInterfaceDeclaration(node);
            m_currentParent = oldParent;
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            var enumObj = CSharpEntityCreationHelper.CreateEnum(node, m_currentParent, m_currentCodeFile, m_currentTree);

            ISyntaxEntity oldParent = setCurrentParent(enumObj);
            base.VisitEnumDeclaration(node);
            m_currentParent = oldParent;
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            if (node == null)
            {
                throw new InvalidOperationException("Enum Member Declaration node is null!");
            }
            (m_currentParent as Entities.UDT.EnumDefinition).AddEnumMember(node.Identifier.Text);
            base.VisitEnumMemberDeclaration(node);
        }
        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            base.VisitFieldDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var methodObj = CSharpEntityCreationHelper.CreateMethod(node, m_currentParent, m_currentCodeFile, m_currentTree);

            ISyntaxEntity oldParent = setCurrentParent(methodObj);
            base.VisitMethodDeclaration(node);
            m_currentParent = oldParent;
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            var methodObj = CSharpEntityCreationHelper.CreateMethod(node, m_currentParent, m_currentCodeFile, m_currentTree);

            ISyntaxEntity oldParent = setCurrentParent(methodObj);
            base.VisitConstructorDeclaration(node);
            m_currentParent = oldParent;
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            var methodObj = CSharpEntityCreationHelper.CreateMethod(node, m_currentParent, m_currentCodeFile, m_currentTree);

            ISyntaxEntity oldParent = setCurrentParent(methodObj);
            base.VisitDestructorDeclaration(node);
            m_currentParent = oldParent;
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            var methodObj = CSharpEntityCreationHelper.CreateMethod(node, m_currentParent, m_currentCodeFile, m_currentTree);
            Debug.Assert(methodObj.TypeOfFunction == FunctionTypes.AnonymousFunction);

            ISyntaxEntity oldParent = setCurrentParent(methodObj);
            base.VisitSimpleLambdaExpression(node);
            m_currentParent = oldParent;

        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            var methodObj = CSharpEntityCreationHelper.CreateMethod(node, m_currentParent, m_currentCodeFile, m_currentTree);
            Debug.Assert(methodObj.TypeOfFunction == FunctionTypes.AnonymousFunction);

            ISyntaxEntity oldParent = setCurrentParent(methodObj);
            base.VisitParenthesizedLambdaExpression(node);
            m_currentParent = oldParent;
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            var methodObj = CSharpEntityCreationHelper.CreateMethod(node, m_currentParent, m_currentCodeFile, m_currentTree);
            Debug.Assert(methodObj.TypeOfFunction == FunctionTypes.AnonymousDelegate ||
                            methodObj.TypeOfFunction == FunctionTypes.Delegate);

            ISyntaxEntity oldParent = setCurrentParent(methodObj);
            base.VisitDelegateDeclaration(node);
            m_currentParent = oldParent;
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            var methodObj = CSharpEntityCreationHelper.CreateMethod(node, m_currentParent, m_currentCodeFile, m_currentTree);
            Debug.Assert(methodObj.TypeOfFunction == FunctionTypes.AnonymousDelegate);

            ISyntaxEntity oldParent = setCurrentParent(methodObj);
            base.VisitAnonymousMethodExpression(node);
            m_currentParent = oldParent;
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            var methodObj = CSharpEntityCreationHelper.CreateMethod(node, m_currentParent, m_currentCodeFile, m_currentTree);
            Debug.Assert(methodObj.TypeOfFunction == FunctionTypes.Operator);

            ISyntaxEntity oldParent = setCurrentParent(methodObj);
            base.VisitOperatorDeclaration(node);
            m_currentParent = oldParent;
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            var methodObj = CSharpEntityCreationHelper.CreateMethod(node, m_currentParent, m_currentCodeFile, m_currentTree);
            Debug.Assert(methodObj.TypeOfFunction == FunctionTypes.ConversionOperator);

            ISyntaxEntity oldParent = setCurrentParent(methodObj);
            base.VisitConversionOperatorDeclaration(node);
            m_currentParent = oldParent;
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            var prop = CSharpEntityCreationHelper.CreateProperty(node, m_currentParent, m_currentCodeFile, m_currentTree);
            Debug.Assert(prop.IsIndexer);

            ISyntaxEntity oldParent = setCurrentParent(prop);
            base.VisitIndexerDeclaration(node);
            m_currentParent = oldParent;
        }


        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var propertyObj = CSharpEntityCreationHelper.CreateProperty(node, m_currentParent, m_currentCodeFile, m_currentTree);

            ISyntaxEntity oldParent = setCurrentParent(propertyObj);
            base.VisitPropertyDeclaration(node);
            m_currentParent = oldParent;
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            FormalParameter fp = CSharpEntityCreationHelper.CreateFormalParameter(node);
            
            if (m_currentParent.GetType() == typeof(FunctionDefinition))
            {
                (m_currentParent as FunctionDefinition).AddFormalParameter(fp);
            }
            else if ( m_currentParent.GetType() == typeof(MemberProperty))
            {
                (m_currentParent as MemberProperty).SoleParameter = fp;
            }
            else
            {
                String msg = String.Format(CultureInfo.InvariantCulture, "Cannot process parameter if parent is: {0} [{1}]", m_currentParent.Kind, m_currentParent.Name);
                throw new InvalidOperationException(msg);
            }
            base.VisitParameter(node);
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            //Jumping the gun here. Instead of going down further, we simply do the 
            //processing here and add the base classes.
            if (node == null)
            {
                throw new InvalidOperationException("Simple Base Type node is null!");
            }
            ((m_currentParent as InheritableUserDefinedType)).AddBaseClassOrInterface(node.ChildNodes().First().GetText().ToString());
            base.VisitSimpleBaseType(node);
        }

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
            if (node == null)
            {
                throw new InvalidOperationException("Type Parameter node is null!");
            }
            ((m_currentParent as AbstractAddressableEntity)).AddTypeParameter(node.Identifier.Text);
            base.VisitTypeParameter(node);
        }

		public override void VisitTryStatement(TryStatementSyntax node)
		{
			var tryObj = CSharpEntityCreationHelper.CreateTryBlock(node, m_currentParent, m_currentCodeFile, m_currentTree);
			
			ISyntaxEntity oldParent = setCurrentParent(tryObj);
			base.VisitTryStatement(node);
			m_currentParent = oldParent;
		}

		public override void VisitCatchClause(CatchClauseSyntax node)
		{
			var catchObj = CSharpEntityCreationHelper.CreateCatchBlock(node, m_currentParent, m_currentCodeFile, m_currentTree);

			ISyntaxEntity oldParent = setCurrentParent(catchObj);
			base.VisitCatchClause(node);
			m_currentParent = oldParent;
		}

		public override void VisitIfStatement(IfStatementSyntax node)
		{
			var ifObj = CSharpEntityCreationHelper.CreateIfBlock(node, m_currentParent, m_currentCodeFile, m_currentTree);

			ISyntaxEntity oldParent = setCurrentParent(ifObj);
			base.VisitIfStatement(node);
			m_currentParent = oldParent;
		}

		public override void VisitElseClause(ElseClauseSyntax node)
		{
			var elseObj = CSharpEntityCreationHelper.CreateElseBlock(node, m_currentParent, m_currentCodeFile, m_currentTree);

			ISyntaxEntity oldParent = setCurrentParent(elseObj);
			base.VisitElseClause(node);
			m_currentParent = oldParent;
		}

		public override void VisitForStatement(ForStatementSyntax node)
		{
			var forObj = CSharpEntityCreationHelper.CreateForBlock(node, m_currentParent, m_currentCodeFile, m_currentTree);

			ISyntaxEntity oldParent = setCurrentParent(forObj);
			base.VisitForStatement(node);
			m_currentParent = oldParent;
		}

		public override void VisitForEachStatement(ForEachStatementSyntax node)
		{
			var foreachObj = CSharpEntityCreationHelper.CreateForEachBlock(node, m_currentParent, m_currentCodeFile, m_currentTree);

			ISyntaxEntity oldParent = setCurrentParent(foreachObj);
			base.VisitForEachStatement(node);
			m_currentParent = oldParent;
		}

		public override void VisitWhileStatement(WhileStatementSyntax node)
		{
			var whileObj = CSharpEntityCreationHelper.CreateWhileBlock(node, m_currentParent, m_currentCodeFile, m_currentTree);

			ISyntaxEntity oldParent = setCurrentParent(whileObj);
			base.VisitWhileStatement(node);
			m_currentParent = oldParent;
		}

		private ISyntaxEntity setCurrentParent(ISyntaxEntity obj)
        {
            m_currentParent.AddChild(obj);
            (obj as AbstractSyntaxEntity).Parent = m_currentParent;

            ISyntaxEntity savedParent = m_currentParent;
            m_currentParent = obj;

            return savedParent;
        }

        private CodeFile m_currentCodeFile = null;
        private ISyntaxEntity m_currentParent = null;
        private SyntaxTree m_currentTree;
    }
}
