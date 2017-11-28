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
    public abstract class AbstractAddressableEntity : AbstractSyntaxEntity
    {

        internal AbstractAddressableEntity(string name, FileSpan loc, ISyntaxEntity parent, CodeFile currentCodeFile)
            : base(name, loc, parent, currentCodeFile)
        {
            init();
        }

        private void init()
        {
            TemplateTypeParameters = new List<string>();
            Attributes = new List<string>();
            AccessSpecifiers = AccessSpecifiers.None;
            StorageSpecifiers = StorageSpecifiers.None;
        }

        public bool IsStatic
        {
            get { return (StorageSpecifiers & StorageSpecifiers.Static) > 0; }
        }

        public bool IsAbstract { get; internal set; }

        public AccessSpecifiers AccessSpecifiers { get; internal set; }

        public StorageSpecifiers StorageSpecifiers { get; internal set; }

        public IEnumerable<string> TemplateTypeParameters { get; internal set; }

        public IEnumerable<string> Attributes { get; internal set; }

        public void AddTypeParameter(string typeParameterName)
        {
            ((List<string>)TemplateTypeParameters).Add(typeParameterName);
        }

        public void AddAttribute(string attributeName)
        {
            ((List<string>)Attributes).Add(attributeName);
        }
    }
}
