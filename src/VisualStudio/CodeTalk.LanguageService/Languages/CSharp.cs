//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using Microsoft.CodeTalk.LanguageService.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Resources;
using Microsoft.CodeTalk.LanguageService.Properties;
using System.Globalization;

namespace Microsoft.CodeTalk.LanguageService
{
    internal class CSharp : ILanguage
    {
		ResourceManager rm = new ResourceManager(typeof(Resources));

		public CodeFile Compile(string programText, CompilationContext context)
        {
            return null;
        }

        public IEnumerable<CompileError> GetDiagnostics(string programText)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            var diagnostics = tree.GetDiagnostics();
            var errorList = new List<CompileError>();
            foreach (var diagnostic in diagnostics)
            {
                var span = diagnostic.Location.GetMappedLineSpan();
                var errorLocation = new FileSpan(span);
                var error = new CompileError(diagnostic.GetMessage(), errorLocation);
                errorList.Add(error);
            }

            return errorList;
        }

        public string SpokenText(FunctionDefinition functionDefinition)
        {
            if ( functionDefinition == null )
            {
                return String.Empty;
            }
            string functionName = functionDefinition.Name;
            switch ( functionDefinition.TypeOfFunction )
            {
                case FunctionTypes.AnonymousDelegate:
                    functionName = FunctionTypes.AnonymousDelegate.ToString();
                    break;
                case FunctionTypes.AnonymousFunction:
                    functionName = FunctionTypes.AnonymousFunction.ToString();
                    break;
                case FunctionTypes.Delegate:
                    functionName = FunctionTypes.Delegate.ToString() + " " + functionDefinition.Name;
                    break;
                case FunctionTypes.Constructor:
                    functionName = FunctionTypes.Constructor.ToString() + " " + functionDefinition.Name;
                    break;
                case FunctionTypes.Destructor:
                    functionName = FunctionTypes.Destructor.ToString() + " " + functionDefinition.Name;
                    break;
                case FunctionTypes.External:
                    functionName = FunctionTypes.External.ToString() + $" {rm.GetString("Function", CultureInfo.CurrentCulture)} " + functionDefinition.Name;
                    break;
                case FunctionTypes.Operator:
                    functionName = FunctionTypes.Operator.ToString() + " " + functionDefinition.Name;
                    break;
                default:
                    functionName = $"{rm.GetString("Function", CultureInfo.CurrentCulture)} " + functionDefinition.Name;

					break;
            }
            return functionName + $" {rm.GetString("AtLine", CultureInfo.CurrentCulture)} " + functionDefinition.Location.StartLineNumber;

		}

        public string SpokenText(MemberProperty memberProperty)
        {
            if (memberProperty == null)
            {
                return String.Empty;
            }
            if ( memberProperty.IsIndexer )
            {
                return Constants.IndexerPropertyName + $" {rm.GetString("AtLine", CultureInfo.CurrentCulture)} " + memberProperty.Location.StartLineNumber;
            }
            else
            {
                return memberProperty.Name + $" {rm.GetString("AtLine", CultureInfo.CurrentCulture)} " + memberProperty.Location.StartLineNumber;
            }
        }

        //private string typeParametersAsString(IEnumerable<string> typeParameters)
        //{
        //    string typeParamsAsStr = String.Join(",", typeParameters);
        //    return "<" + typeParamsAsStr + ">";
        //}

        //private string formalParametersAsString(IEnumerable<FormalParameter> formalParameters)
        //{
        //    string formalParametersAsStr = String.Join(",", formalParameters.Select(fp => SpokenText(fp)));
        //    return "(" + formalParametersAsStr + ")";

        //}

        public string SpokenText(FormalParameter formalParameter)
        {
            if ( formalParameter == null)
            {
                return String.Empty;
            }
            string formalParameterAsStr = formalParameter.Modifiers + " " + formalParameter.TypeName + " " + formalParameter.ParameterName;
            return formalParameterAsStr + $" {rm.GetString("AtLine", CultureInfo.CurrentCulture)} " + formalParameter.Location.StartLineNumber;
        }

        public string SpokenText(UserDefinedType udt)
        {
            if (udt == null)
            {
                return String.Empty;
            }

            string whichType = udt.Kind.ToString();

            // Modified by prvai : Removing access specifiers and storage specifiers.
            return /*udt.AccessSpecifiers.ToString() + " " + udt.StorageSpecifiers + " " +*/ whichType + " " + udt.Name + " at line " + udt.Location.StartLineNumber;
        }

        public string SpokenText(NamespaceDefinition names)
        {
            if ( names == null)
            {
                return String.Empty;
            }
            return names.Kind + " " + names.Name + $" {rm.GetString("AtLine", CultureInfo.CurrentCulture)} " + names.Location.StartLineNumber;
        }

        public CodeFile Parse(string programText, string fileName)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            SyntaxNode root = tree.GetRoot();
            CSharpEntityCollector collector = new CSharpEntityCollector(tree);
            collector.Visit(root);

            collector.Root.Language = this;
            collector.Root.Name = String.IsNullOrWhiteSpace(fileName) ? "CSharp" : fileName;

            return collector.Root;
        }

    }
}
