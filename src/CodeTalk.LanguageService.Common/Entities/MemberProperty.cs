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
    public class MemberProperty : AbstractAddressableEntity
    {
        internal MemberProperty(string name, FileSpan loc, ISyntaxEntity parent, CodeFile currentCodeFile) 
            : base(name, loc, parent, currentCodeFile)
        {
        }

        public string PropertyType { get; internal set; }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.Property;
            }
        }

        /// <summary>
        /// Whether the property is a this[] property. 
        /// </summary>
        public bool IsIndexer { get; set; }

        /// <summary>
        /// Name and type of the indexer parameter.
        /// </summary>
        public FormalParameter SoleParameter { get; set; }

        public override void AcceptVisitor(ICodeVisitor visitor)
        {
            visitor?.VisitProperty(this);
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
