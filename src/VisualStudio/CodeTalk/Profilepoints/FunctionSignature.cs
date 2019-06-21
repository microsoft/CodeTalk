using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeTalk.LanguageService;

namespace Microsoft.CodeTalk.Profilepoints
{
    class FunctionSignature
    {
        ISyntaxEntity function;
        public FunctionSignature(ISyntaxEntity function)
        {
            this.function = function;
        }

        public override int GetHashCode()
        {
            return function.GetHashCode();
        }
    }
}
