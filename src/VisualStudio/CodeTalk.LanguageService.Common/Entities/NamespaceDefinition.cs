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
    public class NamespaceDefinition : AbstractSyntaxEntity
    {
        public NamespaceDefinition(string typeName, FileSpan fileLocation, ISyntaxEntity parent, CodeFile currentCodeFile) 
            : base(typeName, fileLocation, parent, currentCodeFile)
        {
        }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.Namespace;
            }
        }

        public override void AcceptVisitor(ICodeVisitor visitor)
        {
            visitor?.VisitNamespace(this);
        }

        public override string SpokenText()
        {
            return this.CurrentCodeFile.Language.SpokenText(this);
        }

        public override string DisplayText()
        {
            return base.DisplayText();
        }
    }
}
