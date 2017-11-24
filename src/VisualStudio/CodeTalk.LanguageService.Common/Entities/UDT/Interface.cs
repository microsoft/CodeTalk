//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.LanguageService.Entities.UDT
{
    public class Interface : UserDefinedType
    {
        public Interface(string text, FileSpan fileSpan, ISyntaxEntity parent, CodeFile currentCodeFile) : base(text, fileSpan, parent, currentCodeFile)
        {
            m_baseClasses = new List<string>();
        }

        internal void AddBaseClassOrInterface(string className)
        {
            m_baseClasses.Add(className);
        }

        public IEnumerable<string> BaseClasses { get { return m_baseClasses; } }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.Interface;
            }
        }

        private List<string> m_baseClasses;
    }
}
