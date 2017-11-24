//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.LanguageService.Entities.UDT
{
    public class Enum : UserDefinedType
    {
        public Enum(string text, FileSpan fileSpan, ISyntaxEntity parent, CodeFile currentCodeFile) : base(text, fileSpan, parent, currentCodeFile)
        {
            m_enumMembers = new List<string>();
        }

        internal void AddEnumMember(string memberName)
        {
            Debug.Assert(UdtType == TypeOfUdt.Enum); //sanity check.
            m_enumMembers.Add(memberName);
        }
        public IEnumerable<string> EnumMembers { get { return m_enumMembers; } }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.Enum;
            }
        }

        private List<string> m_enumMembers;
    }
}
