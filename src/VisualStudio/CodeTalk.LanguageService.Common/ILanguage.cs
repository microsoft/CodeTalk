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
    public interface ILanguage
    {
        CodeFile Compile(string programText, CompilationContext context);

        CodeFile Parse(string programText, string fileName);

        IEnumerable<CompileError> GetDiagnostics(string programText);

        string SpokenText(FunctionDefinition functionDefinition);

        string SpokenText(MemberProperty memberProperty);

        string SpokenText(FormalParameter formalParameter);

        string SpokenText(UserDefinedType udt);

        string SpokenText(NamespaceDefinition names);
    }
}
