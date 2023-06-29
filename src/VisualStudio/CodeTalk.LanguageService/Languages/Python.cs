//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Scripting;

using Microsoft.Scripting.Hosting;
using IronPython.Compiler.Ast;
using IronPython.Compiler;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Runtime;
using System.Globalization;
using System.Resources;
using Microsoft.CodeTalk.LanguageService.Properties;
//using Microsoft.PythonTools.Parsing;

namespace Microsoft.CodeTalk.LanguageService
{
    internal class Python : ILanguage
    {
		ResourceManager rm = new ResourceManager(typeof(Resources));

        public CodeFile Compile(String programText, CompilationContext context)
        { 
            throw new NotImplementedException();
        }
        public string SpokenText(FunctionDefinition functionDefinition)
        {
            if ( functionDefinition == null)
            {
                return String.Empty;
            }
            return functionDefinition.Kind + " " + functionDefinition.Name + $" {rm.GetString("AtLine", CultureInfo.CurrentCulture)} " + functionDefinition.Location.StartLineNumber;
        }

        public string SpokenText(MemberProperty memberProperty)
        {
            if ( memberProperty == null)
            {
                return String.Empty;
            }
            return String.Empty; //no properties in Python.
        }
        //private string typeParametersAsString(IEnumerable<string> typeParameters)
        //{
        //    string typeParamsAsStr = String.Join(",", typeParameters);
        //    return "(" + typeParamsAsStr + ")";
        //}
        public string SpokenText(FormalParameter formalParameter)
        {
            if ( formalParameter == null)
            {
                return String.Empty;
            }
            return formalParameter.ParameterName + $" {rm.GetString("AtLine", CultureInfo.CurrentCulture)} " + formalParameter.Location.StartLineNumber;
        }
        public CodeFile Parse(string programText, string fileName)
        {
            ScriptEngine py = null;
            SourceUnit src = null;
            LanguageContext pythonLanguageContext = null;
            CompilerContext cc = null;
            IronPython.PythonOptions pyOptions;
            IronPython.Compiler.Parser pyParser  = null;
            PythonAst ast = null;
            try
            {
                py = IronPython.Hosting.Python.CreateEngine();
                src = HostingHelpers.GetSourceUnit(py.CreateScriptSourceFromString(programText));
                pythonLanguageContext = HostingHelpers.GetLanguageContext(py);
                cc = new CompilerContext(src, pythonLanguageContext.GetCompilerOptions(), ErrorSink.Default);
                pyOptions = pythonLanguageContext.Options as IronPython.PythonOptions;
                pyParser = Parser.CreateParser(cc, pyOptions);
                ast = pyParser.ParseFile(true);
            }
            finally
            {
                pyParser?.Dispose();
            }

            PythonEntityCollector collector = new PythonEntityCollector();
            ast.Walk(collector);


            var cf = collector.getCodeFile();
            cf.Name = String.IsNullOrWhiteSpace(fileName) ? $"{rm.GetString("PythonString", CultureInfo.CurrentCulture)}" : fileName;

            return cf;
        }

        public IEnumerable<CompileError> GetDiagnostics(string programText)
        {
            ScriptEngine py = null;
            SourceUnit src = null;
            LanguageContext pythonLanguageContext = null;
            CompilerContext cc = null;
            IronPython.PythonOptions pyOptions;
            IronPython.Compiler.Parser pyParser = null;
            IEnumerable<CompileError> errorList = Enumerable.Empty<CompileError>();
            try
            {
                py = IronPython.Hosting.Python.CreateEngine();
                src = HostingHelpers.GetSourceUnit(py.CreateScriptSourceFromString(programText));
                pythonLanguageContext = HostingHelpers.GetLanguageContext(py);
                cc = new CompilerContext(src, pythonLanguageContext.GetCompilerOptions(), ErrorSink.Default);
                pyOptions = pythonLanguageContext.Options as IronPython.PythonOptions;
                pyParser = Parser.CreateParser(cc, pyOptions);
                pyParser.ParseFile(true);
            }
            catch (Microsoft.Scripting.SyntaxErrorException e)
            {
                CompileError syntaxError = new CompileError(e.Message, new FileSpan(e.RawSpan.Start.Line, e.RawSpan.Start.Column, e.RawSpan.End.Line, e.RawSpan.End.Column));
                errorList = errorList.Concat(new[] { syntaxError });
            }
            finally
            {
                pyParser?.Dispose();
            }

            return errorList;
        }
        
        public string SpokenText(UserDefinedType udt)
        {
            if ( udt == null)
            {
                return String.Empty;
            }
            return udt.Name + $" {rm.GetString("AtLine", CultureInfo.CurrentCulture)} " + udt.Location.StartLineNumber;
        }

        public string SpokenText(NamespaceDefinition names)
        {
            return string.Empty;
        }
    }
}
