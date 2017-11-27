//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Globalization;
using Microsoft.CodeTalk.LanguageService.Entities.UDT;

namespace Microsoft.CodeTalk.LanguageService
{
    internal static class CSharpEntityCreationHelper
    {
        internal static UserDefinedType CreateClass(BaseTypeDeclarationSyntax node, ISyntaxEntity parent, CodeFile currentCodeFile, SyntaxTree tree)
        {
            UserDefinedType typeObj = new ClassDefinition(node.Identifier.Text, new FileSpan(tree.GetLineSpan(node.Span)), parent, currentCodeFile);

            processModifiers(typeObj, node.Modifiers);
            typeObj.AccessSpecifiers = typeObj.AccessSpecifiers == AccessSpecifiers.None ? AccessSpecifiers.Internal : typeObj.AccessSpecifiers;
            typeObj.AssociatedComment = GetComment(typeObj, node, tree, currentCodeFile);

            return typeObj;
        }

        internal static UserDefinedType CreateStruct(BaseTypeDeclarationSyntax node, ISyntaxEntity parent, CodeFile currentCodeFile, SyntaxTree tree)
        {
            UserDefinedType typeObj = new StructDefinition(node.Identifier.Text, new FileSpan(tree.GetLineSpan(node.Span)), parent, currentCodeFile);

            processModifiers(typeObj, node.Modifiers);
            typeObj.AccessSpecifiers = typeObj.AccessSpecifiers == AccessSpecifiers.None ? AccessSpecifiers.Internal : typeObj.AccessSpecifiers;
            typeObj.AssociatedComment = GetComment(typeObj, node, tree, currentCodeFile);

            return typeObj;
        }

        internal static UserDefinedType CreateEnum(BaseTypeDeclarationSyntax node, ISyntaxEntity parent, CodeFile currentCodeFile, SyntaxTree tree)
        {
            UserDefinedType typeObj = new Entities.UDT.EnumDefinition(node.Identifier.Text, new FileSpan(tree.GetLineSpan(node.Span)), parent, currentCodeFile);

            processModifiers(typeObj, node.Modifiers);
            typeObj.AccessSpecifiers = typeObj.AccessSpecifiers == AccessSpecifiers.None ? AccessSpecifiers.Internal : typeObj.AccessSpecifiers;
            typeObj.AssociatedComment = GetComment(typeObj, node, tree, currentCodeFile);

            return typeObj;
        }

        internal static UserDefinedType CreateInterface(BaseTypeDeclarationSyntax node, ISyntaxEntity parent, CodeFile currentCodeFile, SyntaxTree tree)
        {
            UserDefinedType typeObj = new InterfaceDefinition(node.Identifier.Text, new FileSpan(tree.GetLineSpan(node.Span)), parent, currentCodeFile);

            processModifiers(typeObj, node.Modifiers);
            typeObj.AccessSpecifiers = typeObj.AccessSpecifiers == AccessSpecifiers.None ? AccessSpecifiers.Internal : typeObj.AccessSpecifiers;
            typeObj.AssociatedComment = GetComment(typeObj, node, tree, currentCodeFile);

            return typeObj;
        }

        #region PROCESS FUNCTIONS
        internal static FunctionDefinition CreateMethod(MethodDeclarationSyntax node, ISyntaxEntity parent, CodeFile currentCodeFile, SyntaxTree tree)
        {
            FunctionDefinition func = createFunctionObject(node.Identifier.Text, tree.GetLineSpan(node.Span), node.Modifiers, parent, currentCodeFile);
            func.ReturnType = node.ReturnType.ToString();
            func.AssociatedComment = GetComment(func, node, tree, currentCodeFile);

            return func;

        }
        internal static FunctionDefinition CreateMethod(ConstructorDeclarationSyntax node, ISyntaxEntity parent, CodeFile currentCodeFile, SyntaxTree tree)
        {
            FunctionDefinition func = createFunctionObject(node.Identifier.Text, tree.GetLineSpan(node.Span), node.Modifiers, parent, currentCodeFile);
            func.TypeOfFunction = FunctionTypes.Constructor;
            func.AssociatedComment = GetComment(func, node, tree, currentCodeFile);

            return func;
        }
        internal static FunctionDefinition CreateMethod(DestructorDeclarationSyntax node, ISyntaxEntity parent, CodeFile currentCodeFile, SyntaxTree tree)
        {
            FunctionDefinition func = createFunctionObject(node.Identifier.Text, tree.GetLineSpan(node.Span), node.Modifiers, parent, currentCodeFile);
            func.TypeOfFunction = FunctionTypes.Destructor;
            func.AssociatedComment = GetComment(func, node, tree, currentCodeFile);

            return func;
        }

        internal static FunctionDefinition CreateMethod(LambdaExpressionSyntax node, ISyntaxEntity parent, CodeFile currentCodeFile, SyntaxTree tree)
        {
            FunctionDefinition func = createFunctionObject("", tree.GetLineSpan(node.Span), new SyntaxTokenList(), parent, currentCodeFile);
            func.TypeOfFunction = FunctionTypes.AnonymousFunction;
            func.AssociatedComment = GetComment(func, node, tree, currentCodeFile);

            return func;
        }

        internal static FunctionDefinition CreateMethod(DelegateDeclarationSyntax node, ISyntaxEntity parent, CodeFile currentCodeFile, SyntaxTree tree)
        {
            FunctionDefinition func = createFunctionObject(node.Identifier != null ? node.Identifier.Text : String.Empty, tree.GetLineSpan(node.Span), node.Modifiers, parent, currentCodeFile);
            func.TypeOfFunction = String.IsNullOrWhiteSpace(func.Name) ? FunctionTypes.AnonymousDelegate : FunctionTypes.Delegate;
            func.AssociatedComment = GetComment(func, node, tree, currentCodeFile);

            return func;
        }

        internal static FunctionDefinition CreateMethod(AnonymousMethodExpressionSyntax node, ISyntaxEntity parent, CodeFile codeFile, SyntaxTree tree)
        {
            FunctionDefinition func = createFunctionObject(String.Empty, tree.GetLineSpan(node.Span), new SyntaxTokenList(), parent, codeFile);
            func.TypeOfFunction = FunctionTypes.AnonymousDelegate;
            func.AssociatedComment = GetComment(func, node, tree, codeFile);

            return func;
        }

        internal static FunctionDefinition CreateMethod(OperatorDeclarationSyntax node, ISyntaxEntity parent, CodeFile codeFile, SyntaxTree tree)
        {
            FunctionDefinition func = createFunctionObject(node.OperatorToken.Text, tree.GetLineSpan(node.Span), node.Modifiers, parent, codeFile);
            func.TypeOfFunction = FunctionTypes.Operator;
            func.AssociatedComment = GetComment(func, node, tree, codeFile);

            return func;
        }

        internal static FunctionDefinition CreateMethod(ConversionOperatorDeclarationSyntax node, ISyntaxEntity parent, CodeFile codefile, SyntaxTree tree)
        {
            FunctionDefinition func = createFunctionObject(node.Type.ToString(), tree.GetLineSpan(node.Span), node.Modifiers, parent, codefile);
            func.TypeOfFunction = FunctionTypes.ConversionOperator;
            func.AssociatedComment = GetComment(func, node, tree, codefile);

            return func;
        }


        private static FunctionDefinition createFunctionObject(string nodeName, FileLinePositionSpan span, SyntaxTokenList modifiers, ISyntaxEntity parent, CodeFile currentCodeFile)
        {
            FunctionDefinition func = new FunctionDefinition(nodeName, new FileSpan(span), parent, currentCodeFile);

            if (modifiers != null) //modifiers are null for anonymous functions.
            {
                processModifiers(func, modifiers);
            }

            foreach (var modifier in modifiers)
            {
                if (modifier.Kind() == SyntaxKind.OverrideKeyword) { func.IsOverride = true; }
                if (modifier.Kind() == SyntaxKind.AsyncKeyword) { func.IsAsync = true; }
                if (modifier.Kind() == SyntaxKind.ExternKeyword) { func.IsExtern = true; func.StorageSpecifiers |= StorageSpecifiers.Extern; }
                if (modifier.Kind() == SyntaxKind.VirtualKeyword) { func.IsVirtual = true; }
            }

            if (parent.Kind == SyntaxEntityKind.Class || parent.Kind == SyntaxEntityKind.Interface || parent.Kind == SyntaxEntityKind.Interface)
            {
                if (func.IsExtern)
                {
                    func.TypeOfFunction = FunctionTypes.External; // Defined externally.
                }
                else
                {
                    func.TypeOfFunction = FunctionTypes.MemberFunction;
                }
            }
            else if (parent.Kind == SyntaxEntityKind.Function)
            {
                func.TypeOfFunction = FunctionTypes.AnonymousFunction;
            }
            else
            {
                func.TypeOfFunction = FunctionTypes.GlobalFunction;
            }

            func.AccessSpecifiers = func.AccessSpecifiers == AccessSpecifiers.None ? AccessSpecifiers.Private : func.AccessSpecifiers;
            return func;
        }

        private static void processModifiers(AbstractAddressableEntity classMethodOrVariable, SyntaxTokenList modifiers)
        {
            classMethodOrVariable.StorageSpecifiers = StorageSpecifiers.Instance; //default. 
            classMethodOrVariable.AccessSpecifiers = AccessSpecifiers.None; //can do this becase we know it's C#

            foreach (var modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.AbstractKeyword:
                        classMethodOrVariable.IsAbstract = true;
                        break;
                    case SyntaxKind.StaticKeyword:
                        classMethodOrVariable.StorageSpecifiers |= StorageSpecifiers.Static;
                        break;
                    case SyntaxKind.PublicKeyword:
                        classMethodOrVariable.AccessSpecifiers = AccessSpecifiers.Public;
                        break;
                    case SyntaxKind.PrivateKeyword:
                        classMethodOrVariable.AccessSpecifiers = AccessSpecifiers.Private;
                        break;
                    case SyntaxKind.InternalKeyword:
                        classMethodOrVariable.AccessSpecifiers = processInternalAndProtected(classMethodOrVariable.AccessSpecifiers, AccessSpecifiers.Internal);
                        break;
                    case SyntaxKind.ProtectedKeyword:
                        classMethodOrVariable.AccessSpecifiers = processInternalAndProtected(classMethodOrVariable.AccessSpecifiers, AccessSpecifiers.Protected);
                        break;
                    case SyntaxKind.OverrideKeyword: //We cannot handle this for both classes and methods, so we let methods handle it separately.
                        break;
                    case SyntaxKind.AsyncKeyword:
                        break;
                    case SyntaxKind.PartialKeyword:
                        break;
                    case SyntaxKind.AliasKeyword:
                        break;
                    case SyntaxKind.AssemblyKeyword:
                        break;
                    case SyntaxKind.BaseKeyword:
                        break;
                    case SyntaxKind.SealedKeyword:
                        break;
                    case SyntaxKind.DefaultKeyword:
                        break;
                    case SyntaxKind.ExplicitKeyword:
                        break;
                    case SyntaxKind.ExternKeyword: 
                        break;
                    case SyntaxKind.VirtualKeyword: 
                        break;
                    case SyntaxKind.UnsafeKeyword:
                        break;
                    case SyntaxKind.NewKeyword:
                        break;
                    default:
                        throw new NotSupportedException(String.Format(CultureInfo.InvariantCulture, "Keyword {0} is not supported", modifier.Text));
                }
            }
        }
        #endregion


        #region PROCESS PROPERTIES
        internal static MemberProperty CreateProperty(PropertyDeclarationSyntax node, ISyntaxEntity parent, CodeFile codeFile, SyntaxTree tree)
        {
            MemberProperty p = new MemberProperty(node.Identifier.Text, new FileSpan(tree.GetLineSpan(node.Span)), parent, codeFile);
            p.PropertyType = node.Type.ToString();
            p.AssociatedComment = GetComment(p, node, tree, codeFile);

            return p;
        }

        internal static MemberProperty CreateProperty(IndexerDeclarationSyntax node, ISyntaxEntity parent, CodeFile codeFile, SyntaxTree tree)
        {
            MemberProperty p = new MemberProperty("", new FileSpan(tree.GetLineSpan(node.Span)), parent, codeFile);
            p.PropertyType = node.Type.ToString();
            p.IsIndexer = true;
            p.AssociatedComment = GetComment(p, node, tree, codeFile);

            return p;
        }
        #endregion

        #region PROCESS BLOCKS

        internal static TryBlock CreateTryBlock(TryStatementSyntax node, ISyntaxEntity parent, CodeFile codeFile, SyntaxTree tree)
        {
            var tryBlock = new TryBlock("", new FileSpan(tree.GetLineSpan(node.Span)), parent, codeFile);
            return tryBlock;
        }

        internal static CatchBlock CreateCatchBlock(CatchClauseSyntax node, ISyntaxEntity parent, CodeFile codeFile, SyntaxTree tree)
        {
            return new CatchBlock("", new FileSpan(tree.GetLineSpan(node.Span)), parent, codeFile)
            {
                CatchType = node.Declaration.Type.ToString(),
                CatchFilterClause = (null == node.Filter) ? string.Empty : node.Filter.FilterExpression.ToString()
            };
        }

        internal static IfBlock CreateIfBlock(IfStatementSyntax node, ISyntaxEntity parent, CodeFile codeFile, SyntaxTree tree)
        {
            return new IfBlock("", new FileSpan(tree.GetLineSpan(node.Span)), parent, codeFile);
        }

        internal static ElseBlock CreateElseBlock(ElseClauseSyntax node, ISyntaxEntity parent, CodeFile codeFile, SyntaxTree tree)
        {
            return new ElseBlock("", new FileSpan(tree.GetLineSpan(node.Span)), parent, codeFile);
        }

        internal static ForBlock CreateForBlock(ForStatementSyntax node, ISyntaxEntity parent, CodeFile codeFile, SyntaxTree tree)
        {
            return new ForBlock("", new FileSpan(tree.GetLineSpan(node.Span)), parent, codeFile);
        }

        internal static ForEachBlock CreateForEachBlock(ForEachStatementSyntax node, ISyntaxEntity parent, CodeFile codeFile, SyntaxTree tree)
        {
            return new ForEachBlock("", new FileSpan(tree.GetLineSpan(node.Span)), parent, codeFile);
        }

        internal static WhileBlock CreateWhileBlock(WhileStatementSyntax node, ISyntaxEntity parent, CodeFile codeFile, SyntaxTree tree)
        {
            return new WhileBlock("", new FileSpan(tree.GetLineSpan(node.Span)), parent, codeFile);
        }
        #endregion

        internal static FormalParameter CreateFormalParameter(ParameterSyntax node)
        {
            FormalParameter fp = new FormalParameter(node.Type != null ? node.Type.ToFullString().Trim() : String.Empty, node.Identifier.Text);
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.InKeyword:
                        fp.Modifiers |= ParameterModifiers.In;
                        break;
                    case SyntaxKind.OutKeyword:
                        fp.Modifiers |= ParameterModifiers.Out;
                        break;
                    case SyntaxKind.RefKeyword:
                        fp.Modifiers |= ParameterModifiers.Ref;
                        break;
                    case SyntaxKind.ParamsKeyword:
                        fp.Modifiers |= ParameterModifiers.Params;
                        break;
                    case SyntaxKind.ThisKeyword:
                        fp.Modifiers |= ParameterModifiers.This;
                        break;
                    default:
                        throw new NotSupportedException(String.Format(CultureInfo.InvariantCulture, "Unable to process keyword {0} in parameter declaration {1}", modifier, node.ToString()));
                }
            }
            if (node.Default != null)
            {
                fp.DefaultValue = node.Default.Value.ToString();
            }

            return fp;

        }

        private static AccessSpecifiers processInternalAndProtected(AccessSpecifiers current, AccessSpecifiers newAccessSpecifier)
        {
            if (current == AccessSpecifiers.None)
            {
                return AccessSpecifiers.Internal;
            }
            else if (current == AccessSpecifiers.Protected)
            {
                return current | newAccessSpecifier;
            }
            else if (current == AccessSpecifiers.Internal)
            {
                return current | newAccessSpecifier;
            }
            else
            {
                string msg = String.Format(CultureInfo.InvariantCulture, "Cannot specify {0} and {1} together.", current, newAccessSpecifier);
                throw new InvalidProgramException(msg);
            }
        }

        internal static Comment GetComment(ISyntaxEntity owner, CSharpSyntaxNode node, SyntaxTree tree, CodeFile currentCodeFile)
        {
            var trivias = node.GetLeadingTrivia().Where(t => t.Kind() == SyntaxKind.MultiLineDocumentationCommentTrivia ||
                                                            t.Kind() == SyntaxKind.MultiLineCommentTrivia ||
                                                            t.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia ||
                                                            t.Kind() == SyntaxKind.SingleLineCommentTrivia);

            if (trivias.Any())
            {
                string commentText = String.Empty;
                FileSpan overallSpan = null;

                foreach (var trivia in trivias)
                {
                    if (trivia != null && trivia.Token.Value != null)
                    {
                        overallSpan = processSpan(overallSpan, tree.GetLineSpan(trivia.Span));

                        if (trivia.Kind() == SyntaxKind.MultiLineDocumentationCommentTrivia ||
                             trivia.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia)
                        {
                            var xml = trivia.GetStructure();
                            commentText += String.Join(Environment.NewLine, xml.GetText().Lines);
                        }
                        else
                        {
                            commentText += trivia.ToFullString();
                        }

                    }
                }

                var comment = new Comment(commentText, overallSpan, owner, currentCodeFile);
                comment.Parent = owner;
                
                currentCodeFile.AddComment(comment);
                return comment;
            }
            else
            {
                return null;
            }
        }

        private static FileSpan processSpan(FileSpan current, FileLinePositionSpan toCombine)
        {
            if (current == null)
            {
                return new FileSpan(toCombine);
            }
            else
            {
                return FileSpan.Combine(current, new FileSpan(toCombine));
            }
        }



    }
}
