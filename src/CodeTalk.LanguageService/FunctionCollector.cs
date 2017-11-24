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
    public class FunctionCollector : DefaultCodeVisitor
    {
        public FunctionCollector()
        {
            m_functionsToCollect = FunctionTypes.MemberFunction | FunctionTypes.Constructor | FunctionTypes.Destructor | FunctionTypes.External | FunctionTypes.GlobalFunction | FunctionTypes.Operator | FunctionTypes.ConversionOperator;
        }

        public FunctionCollector(FunctionTypes type)
        {
            m_functionsToCollect = type;
        }

        List<FunctionDefinition> m_functions = new List<FunctionDefinition>();
        public override void VisitFunction(FunctionDefinition functionDefinition)
        {
            if ( functionDefinition == null )
            {
                throw new InvalidOperationException("Function passed should not be null.");
            }
            if ((functionDefinition.TypeOfFunction & m_functionsToCollect) != 0)
            {
                m_functions.Add(functionDefinition);
            }
            base.VisitFunction(functionDefinition);
        }

        private FunctionTypes m_functionsToCollect = FunctionTypes.None;

        public IEnumerable<FunctionDefinition> FunctionsInFile { get { return m_functions; } }
    }
}
