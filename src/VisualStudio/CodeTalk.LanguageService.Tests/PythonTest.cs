//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using Microsoft.CodeTalk.LanguageService;
using System;
using Microsoft.CodeTalk.LanguageService.Languages;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Microsoft.CodeTalk.LanguageService.Languages.Tests
{
    /// <summary>This class contains parameterized unit tests for Python</summary>
    [TestClass]
    [PexClass(typeof(Python))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class PythonTest
    {

        [TestMethod]
        public void ClassesTest()
        {
            var targuet = new Python();
            var cf = targuet.Parse(System.IO.File.ReadAllText(@".\Programs\PythonTests\pythonClasses.py"), @".\Programs\PythonTests\pythonClasses.py");
            var c1 = (UserDefinedType)cf.Children.first();
                        Debug.Assert(c1.Kind == SyntaxEntityKind.Class, "Error in parsing classes");
                    }

        [TestMethod]
        public void FunctionsTest()
        {
            var targuet = new Python();
            var cf = targuet.Parse(System.IO.File.ReadAllText(@".\Programs\PythonTests\pythonClasses.py"), @".\Programs\PythonTests\pythonClasses.py");
            var c1 = cf.Children.first (); 
                        Debug.Assert(cf.Children.first().count() == 4 ,"error parsing functions.");

                    }

        [TestMethod]
        public void forTest()
        {
            var targuet = new Python();
            var cf = targuet.Parse(System.IO.File.ReadAllText(@".\Programs\PythonTests\pythonClasses.py"), @".\Programs\PythonTests\pythonClasses.py");
        }

        }
}
