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

namespace Microsoft.CodeTalk.LanguageService
{
    public enum TypeOfUdt { Class, Struct, Interface, Enum };

    public abstract class UserDefinedType : AbstractAddressableEntity
    {
        public UserDefinedType(string text, FileSpan fileSpan, ISyntaxEntity parent, CodeFile currentCodeFile)
            : base(text, fileSpan, parent, currentCodeFile)
        {
        }

        public TypeOfUdt UdtType { get; internal set; }

        public override SyntaxEntityKind Kind
        {
            get
            {
                switch (UdtType)
                {
                    default: throw new NotSupportedException(String.Format(CultureInfo.InvariantCulture, "UDTType {0} is not supported.", this.UdtType));
                }
            }
        }

        public override void AcceptVisitor(ICodeVisitor visitor)
        {
            visitor?.VisitUserDefinedType(this);
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
