using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.LanguageService.Entities.UDT
{
    public abstract class InheritableUserDefinedType : UserDefinedType
    {
        public InheritableUserDefinedType(string text, FileSpan fileSpan, ISyntaxEntity parent, CodeFile currentCodeFile) : base(text, fileSpan, parent, currentCodeFile)
        {
            m_baseClasses = new List<string>();
        }

        internal void AddBaseClassOrInterface(string className)
        {
            m_baseClasses.Add(className);
        }

        public IEnumerable<string> BaseClasses { get { return m_baseClasses; } }

        private List<string> m_baseClasses;

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
