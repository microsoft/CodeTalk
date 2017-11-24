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
    [Flags]
    public enum  ParameterModifiers { None = 0, In = 1, Out = 2, Ref = 4, Params = 8, This = 16};

    public class FormalParameter
    {
        public FormalParameter(string typeName, string parameterName)
        {
            this.TypeName = typeName;
            this.ParameterName = parameterName;
        }

        public FormalParameter(string typeName, string parameterName, ParameterModifiers modifiers, FileSpan location)
            : this(typeName, parameterName)
        {
            this.Modifiers = modifiers;
            this.Location = location;
        }

        /// <summary>
        /// Type name. For C++ pointer types, this would be int*
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Parameter name. 
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Any default value set for the parameter.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Modifiers like InOutRef.
        /// </summary>
        public ParameterModifiers Modifiers { get; set; }

        /// <summary>
        /// Location of just the formal paramter. don't know 
        /// if this is required.
        /// </summary>
        public FileSpan Location { get; private set; }
     

    }
}
