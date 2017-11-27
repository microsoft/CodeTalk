//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using Microsoft.CodeTalk.LanguageService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.LanguageService
{
    [Flags]
    public enum FunctionTypes {  None = 0, Constructor = 1, Destructor = 2, MemberFunction = 4, External = 8, GlobalFunction = 16, AnonymousFunction = 32, Delegate = 64, AnonymousDelegate = 128, Operator = 256, ConversionOperator = 512 };

    /// <summary>
    /// 
    /// </summary>
    public class FunctionDefinition : AbstractAddressableEntity
    {
        /// <summary>
        /// <summary>
        /// Constructor for Function type.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="location">The FileSpan location of the function</param>
        /// <param name="parent">the parent of the current CodeFile</param>
        /// <param name="currentCodeFile">The current CodeFile</param>
        public FunctionDefinition(string name, FileSpan location, ISyntaxEntity parent, CodeFile currentCodeFile)
            : base(name, location, parent, currentCodeFile)
        {
            Parameters = new List<FormalParameter>();
            IsOverride = false;
            ReturnType = String.Empty;
        }
        public void AddFormalParameter(FormalParameter formalParameter)
        {
            if (formalParameter != null)
            {
                ((List<FormalParameter>)this.Parameters).Add(formalParameter);
            }
        }

        public string ReturnType { get; internal set; }

        public bool IsOverride { get; internal set; }

        public bool IsVirtual { get; internal set; }

        public bool IsAsync { get; internal set; }

        public bool IsExtern { get; internal set; }

        public FunctionTypes TypeOfFunction{ get; internal set; }

        public IEnumerable<FormalParameter> Parameters { get; internal set; }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.Function;
            }
        }

        public override void AcceptVisitor(ICodeVisitor visitor)
        {
            visitor?.VisitFunction(this);
        }

        public override string SpokenText()
        {
            return this.CurrentCodeFile.Language.SpokenText(this);
        }

        public override string DisplayText()
        {
            return base.DisplayText();
        }

        internal void AddBlock(Block b)
        {
            base.AddChild(b);
        }

        internal void AddLocalVariable(Variable v)
        {
            base.AddChild(v);
        }       
    }
}
