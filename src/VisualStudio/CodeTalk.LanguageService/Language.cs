//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.CodeTalk.LanguageService
{
    public enum SupportedLanguage { CSharp, Python };

    public class Language
    {

		public Language(string fileName)
        {

			string ext = System.IO.Path.GetExtension(fileName);
            if ( s_extensionLanguageMap.ContainsKey(ext))
            {
                m_language = s_langImplnMap[s_extensionLanguageMap[ext]];
            }
            else
            {
                var msg = String.Format(CultureInfo.InvariantCulture, "Extension '{0}' is not supported. Supported extensions are: {1}",
                                                        ext,
                                                        String.Join(",", s_extensionLanguageMap.Keys));
                throw new NotSupportedException(msg);
            }
        }

        public Language(SupportedLanguage language)
        {
            if (s_langImplnMap.ContainsKey(language))
            {
                m_language = s_langImplnMap[language];
            }
            else
            {
                throw new ArgumentOutOfRangeException("Language " + language + " has not been implemented yet.");
            }
        }

        public CodeFile Parse(string programText, string fileName)
        {
			try
			{
				return m_language.Parse(programText, fileName);
			}
			catch(Exception e)
			{
				throw new CodeTalkLanguageServiceException(e);
			}
        }

        public IEnumerable<CompileError> GetDiagnostics(string programText)
        {
            return m_language.GetDiagnostics(programText);
        }

        public string SpokenText(FunctionDefinition functionDefinition)
        {
            return m_language.SpokenText(functionDefinition);
        }

        public string SpokenText(FormalParameter formalParameter)
        {
            return m_language.SpokenText(formalParameter);
        }

        static Language()
        {
            s_extensionLanguageMap.Add(".cs", SupportedLanguage.CSharp);
            s_extensionLanguageMap.Add(".py", SupportedLanguage.Python);

            s_langImplnMap.Add(SupportedLanguage.CSharp, new CSharp());
            s_langImplnMap.Add(SupportedLanguage.Python, new Python());
        }

        private static Dictionary<string, SupportedLanguage> s_extensionLanguageMap = new Dictionary<string, SupportedLanguage>();
        private static Dictionary<SupportedLanguage, ILanguage> s_langImplnMap = new Dictionary<SupportedLanguage, ILanguage>();
        private ILanguage m_language;
    }
}
