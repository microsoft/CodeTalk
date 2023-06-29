﻿//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.CodeTalk.LanguageService.Entities.UDT
{
    public class EnumDefinition : InheritableUserDefinedType
    {
        public EnumDefinition(string text, FileSpan fileSpan, ISyntaxEntity parent, CodeFile currentCodeFile) : base(text, fileSpan, parent, currentCodeFile)
        {
            m_enumMembers = new List<string>();
        }

        internal void AddEnumMember(string memberName)
        {
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
