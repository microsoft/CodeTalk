using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.LanguageService.Entities.Drawiz
{
    public class DrawizEntity : ISyntaxEntity
    {
        public DrawizEntity(string text, string fileName)
        {
            m_description = text;
            m_fileName = fileName;
        }
        public string Name =>  m_description;

        public FileSpan Location => FileSpan.Default;

        public ISyntaxEntity Parent => throw new NotImplementedException();

        public Comment AssociatedComment => throw new NotImplementedException();

        public SyntaxEntityKind Kind =>  SyntaxEntityKind.ImageDescription;

        public CodeFile CurrentCodeFile => null;

        public IEnumerable<ISyntaxEntity> Children => throw new NotImplementedException();

        public IEnumerable<CompileError> ErrorsAtEntity => throw new NotImplementedException();

        public void AcceptVisitor(ICodeVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public void AddChild(ISyntaxEntity child)
        {
            throw new NotImplementedException();
        }

        public string DisplayText()
        {
            return m_description;
        }

        public string SpokenText()
        {
            return m_description;
        }

        private string m_description, m_fileName;
    }
}
