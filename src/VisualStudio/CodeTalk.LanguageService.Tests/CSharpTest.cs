//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Microsoft.CodeTalk.LanguageService.Entities.UDT;

namespace Microsoft.CodeTalk.LanguageService.Languages.Tests
{
    /// <summary>This class contains parameterized unit tests for CSharp</summary>
    [PexClass(typeof(CSharp))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class CSharpTest
    {
        /// <summary>Test stub for Compile(String)</summary>
        [PexMethod]
        internal CodeFile CompileTest([PexAssumeUnderTest]CSharp target, string programText)
        {
            CodeFile result = target.Parse(programText, "");
            return result;
        }


        [TestMethod]
        public void BaseClassesTest()
        {
            CSharp target = new CSharp();
            target.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\BaseClasses.txt"), @".\Programs\CSharp\BaseClasses.txt");
        }

        [TestMethod]
        public void NestedClassesTest()
        {
            CSharp target = new CSharp();
            target.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\NestedClasses.txt"), @".\Programs\CSharp\NestedClasses.txt");
        }

        [TestMethod]
        public void StaticClassGenericMethodsTest()
        {
            CSharp target = new CSharp();
            CodeFile cf = target.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\StaticClassGenericMethods.txt"), @".\Programs\CSharp\StaticClassGenericMethods.txt");

            var cls = (UserDefinedType) cf.Children.First().Children.First();
            Debug.Assert(cls.Name.Equals("StaticClassGenericMethods"), "Found class name: " + cls.Name + " instead of StaticClassGenericMethods");
            Debug.Assert( (cls.StorageSpecifiers & StorageSpecifiers.Static) == StorageSpecifiers.Static, "Did not find static specifier for class StaticClassGenericMethods.");

            var methods = cls.Children.Where(child => child.Kind == SyntaxEntityKind.Function);
            Debug.Assert(methods.Count() == 3, "Found " + methods.Count() + " methods instead of 3");

            var fp = (methods.First() as FunctionDefinition).Parameters.First();
            Debug.Assert(fp.ParameterName.Equals("x"), "Parameter name is " + fp.ParameterName + " instead of x");
            Debug.Assert(fp.TypeName.Equals("string[]"), "Found type " + fp.TypeName + " instead of string[]");
            Debug.Assert((fp.Modifiers & ParameterModifiers.Params) == ParameterModifiers.Params, "Missed params specifier for parameter");


        }

        [TestMethod]
        public void ConstructorAndMethodsTest()
        {
            CSharp target = new CSharp();
            CodeFile cf = target.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\ConstructorAndMethods.txt"), @".\Programs\CSharp\ConstructorAndMethods.txt");

            var functions = cf.Children.First().Children.First().Children;
            var ctors = functions.Where(f => (f as FunctionDefinition).TypeOfFunction == FunctionTypes.Constructor);
            var dtors = functions.Where(f => (f as FunctionDefinition).TypeOfFunction == FunctionTypes.Destructor);

            Debug.Assert(ctors.Count() == 2, "Found " + ctors.Count() + " ctors instead of 2.");
            Debug.Assert(dtors.Count() == 1, "Found " + dtors.Count() + " dtors instead of 1.");
            int ctorsReturnType = ctors.Where(f => !String.IsNullOrWhiteSpace((f as FunctionDefinition).ReturnType)).Count();
            Debug.Assert(ctorsReturnType == 0); //ctors do not have a return type.
        }

        [TestMethod]
        public void OverridesAndPropertyTest()
        {
            CSharp lang = new CSharp();
            CodeFile cf = lang.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\OverridingAndProperty.txt"), @".\Programs\CSharp\OverridingAndProperty.txt");
            var methodsAndProperties = cf.Children.First().Children.First().Children;

            var prop = (MemberProperty) methodsAndProperties.Where(mp => mp.Kind == SyntaxEntityKind.Property).First();
            Debug.Assert(prop.Name.Equals("X"), "Found property with name " + prop.Name + " instead of X");
            Debug.Assert(prop.PropertyType.Equals("int"), "Found property type " + prop.PropertyType + " instead of int.");
            Assert.IsFalse(prop.IsIndexer);

            var method = (FunctionDefinition)methodsAndProperties.Where(mp => mp.Kind == SyntaxEntityKind.Function).First();
            Debug.Assert(method.IsOverride, "Method " + method.Name + " does not have override flag set.");
            Debug.Assert(method.Name.Equals("GetHashCode"), "Found method with name: " + method.Name + " instead of GetHashCode");

            var indexerProp = (MemberProperty) methodsAndProperties.Skip(2).First();
            Assert.AreEqual("", indexerProp.Name);
            Assert.IsTrue(indexerProp.IsIndexer);

            var virtualMethod = (FunctionDefinition)methodsAndProperties.Skip(3).First();
            Assert.IsTrue(virtualMethod.IsVirtual);
            Assert.AreEqual("DoSomething", virtualMethod.Name);
        }

        [TestMethod]
        public void CollectFunctionsTest()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            CSharp obj = new CSharp();
            CodeFile cf = obj.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\NestedClasses.txt"), @".\Programs\CSharp\NestedClasses.txt");
            FunctionCollector collector = new FunctionCollector();
            collector.Visit(cf);
            foreach ( var function in collector.FunctionsInFile )
            {
                Trace.WriteLine(obj.SpokenText(function));
            }
        }

        [TestMethod]
        public void CommentsTest()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            CSharp csharp = new CSharp();
            CodeFile cf = csharp.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\ClassWithComments.txt"), @".\Programs\CSharp\ClassWithComments.txt");

            var cc = new CommentCollector();
            cf.AcceptVisitor(cc);

            Assert.AreEqual(3, cc.Comments.Count());
            Assert.AreEqual(14, cc.Comments.First().Location.StartLineNumber);
            // this will be the line following the comment
            Assert.AreEqual(17, cc.Comments.First().Location.EndLineNumber);
            Assert.AreEqual(SyntaxEntityKind.Class, cc.Comments.First().Parent.Kind);
            Assert.AreEqual(cf.Children.First().Children.First().AssociatedComment, cc.Comments.First());
        }

        [TestMethod]
        public void GetContextTest()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            CSharp csharp = new CSharp();
            CodeFile cf = csharp.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\NestedClasses.txt"), @".\Programs\CSharp\NestedClasses.txt");

            var list = cf.GetContextAtLine(18).ToList();
            Assert.AreEqual(list.Count, 5);
            Assert.AreEqual(list[0].Kind, SyntaxEntityKind.Function);
            Assert.AreEqual(list[0].Name, "Foo");
            Assert.AreEqual(list[1].Kind, SyntaxEntityKind.Class);
            Assert.AreEqual(list[1].Name, "InnerClass");
            Assert.AreEqual(list[2].Kind, SyntaxEntityKind.Class);
            Assert.AreEqual(list[2].Name, "OuterClass");
            Assert.AreEqual(list[3].Kind, SyntaxEntityKind.Namespace);
            Assert.AreEqual(list[3].Name, "Microsoft.CodeTalk.LanguageService.Tests.Programs");
            Assert.AreEqual(list[4].Kind, SyntaxEntityKind.CodeFile);
        }

        [TestMethod]
        public void EnumsTest()
        {
            CSharp lang = new CSharp();
            CodeFile cf = lang.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\Enums.txt"), @".\Programs\CSharp\Enums.txt");

            var enumT = (cf.Children.First().Children.First() as EnumDefinition);
            Assert.AreEqual("Types", enumT.Name);
            Assert.AreEqual(3, enumT.EnumMembers.Count());
            Assert.IsTrue(enumT.Kind == SyntaxEntityKind.Enum);
        }

        private void processFolder (string folderName, string[] foldersToSkip)
        {
            CSharp lang = new CSharp();

            var fileEntries = System.IO.Directory.EnumerateFiles(folderName)
                                                 .Where(fn => System.IO.Path.GetExtension(fn) == ".cs");
            foreach (var fileEntry in fileEntries)
            {
                Trace.TraceInformation("Parsing file: " + fileEntry);
                lang.Parse(System.IO.File.ReadAllText(fileEntry), fileEntry);
            }
            var dirEntries = System.IO.Directory.EnumerateDirectories(folderName)
                                                .Where(fn => false == fn.Contains(".git"))
                                                .Where(fn => false == foldersToSkip.Contains(fn));
            foreach (var dirEntry in dirEntries)
            {
                processFolder(dirEntry, foldersToSkip);
            }
        }


        [TestMethod]
        public void LambdaExpressionsTest()
        {
            CSharp lang = new CSharp();
            CodeFile cf = lang.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\LambdaExpressions.txt"), @".\Programs\CSharp\LambdaExpressions.txt");

            var fns = cf.Children.First() //namespace
                        .Children.First() //class
                        .Children; //methods.
            var af1 = (FunctionDefinition) fns.First().Children.First();
            Assert.AreEqual("", af1.Name);
            Assert.IsTrue(af1.TypeOfFunction == FunctionTypes.AnonymousFunction);
            Assert.IsFalse((af1.Parent as FunctionDefinition).TypeOfFunction == FunctionTypes.AnonymousFunction);

            var af2 = (FunctionDefinition)fns.Skip(1).First() //second member fn
                                .Children.Skip(1).First(); //nested anonmyous fn in second member function
            Assert.AreEqual("", af2.Name);
            Assert.IsTrue(af2.TypeOfFunction == FunctionTypes.AnonymousFunction);
            Assert.IsFalse((af2.Parent as FunctionDefinition).TypeOfFunction == FunctionTypes.AnonymousFunction);

                var innerAf = (FunctionDefinition) af2.Children.First();
                Assert.AreEqual("", innerAf.Name);
                Assert.AreEqual(innerAf.Parameters.First().ParameterName, "lt");
                Assert.AreEqual(innerAf.Parameters.First().TypeName, "");
                Assert.IsTrue((innerAf.Parent as FunctionDefinition).TypeOfFunction == FunctionTypes.AnonymousFunction);

            var af3 = (FunctionDefinition)fns.Skip(2).First() //third member function
                                    .Children.First();
            Assert.AreEqual(2, af3.Parameters.Count());
            Assert.AreEqual("acc", af3.Parameters.First().ParameterName);
            Assert.AreEqual("val", af3.Parameters.Skip(1).First().ParameterName);
        }

        [TestMethod]
        public void DelegatesAndExternTest()
        {
            CSharp lang = new CSharp();
            CodeFile cf = lang.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\DelegatesAndExterns.txt"), @".\Programs\CSharp\DelegatesAndExterns.txt");

            var functionsAndDelegates = cf.Children.First().Children.First().Children;
            Assert.AreEqual(3, functionsAndDelegates.Count()); //top level functions.

            var del1 = (functionsAndDelegates.First() as FunctionDefinition);
            Assert.IsTrue(del1.TypeOfFunction == FunctionTypes.Delegate);
            Assert.AreEqual("GetNative", del1.Name);
            Assert.AreEqual(1, del1.Parameters.Count());
            Assert.AreEqual(AccessSpecifiers.Private, del1.AccessSpecifiers);

            var fn1 = functionsAndDelegates.Skip(1).First() as FunctionDefinition;
            Assert.AreEqual(FunctionTypes.MemberFunction, fn1.TypeOfFunction);
            var childDel1 = fn1.Children.First() as FunctionDefinition;
            Assert.AreEqual(childDel1.TypeOfFunction, FunctionTypes.AnonymousDelegate);
            Assert.AreEqual(0, childDel1.Parameters.Count());
            var childDel2 = fn1.Children.Skip(1).First() as FunctionDefinition;
            Assert.AreEqual(childDel2.TypeOfFunction, FunctionTypes.AnonymousDelegate);
            Assert.AreEqual(1, childDel2.Parameters.Count());

            var fn2 = functionsAndDelegates.Skip(2).First() as FunctionDefinition;
            Assert.AreEqual(FunctionTypes.External, fn2.TypeOfFunction);
            Assert.IsTrue(fn2.IsExtern);
            Assert.IsTrue((fn2.StorageSpecifiers & StorageSpecifiers.Extern) > 0);
            Assert.IsTrue((fn2.StorageSpecifiers & StorageSpecifiers.Static) > 0);
            Assert.AreEqual(3, fn2.Parameters.Count());
        }

        [TestMethod]
        public void GetCommentTest()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            CSharp lang = new CSharp();
            CodeFile cf = lang.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\ClassWithComments.txt"), @".\Programs\CSharp\ClassWithComments.txt");

            //10, 21, 25
            verifyComment(cf, 14, true, SyntaxEntityKind.Class, "ClassWithComments");
            verifyComment(cf, 16, true, SyntaxEntityKind.Class, "ClassWithComments");
            verifyComment(cf, 29, true, SyntaxEntityKind.Function, "Get_I");
            verifyComment(cf, 33, true, SyntaxEntityKind.Function, "Square");
            verifyComment(cf, 35, true, SyntaxEntityKind.Function, "Square");

            var cmt = cf.GetCommentAtLine(38);
            Assert.AreEqual(cmt, null);
        }

        private void verifyComment(CodeFile cf, int lineNum, bool isNonNullOwner, 
                                        SyntaxEntityKind expectedKind = SyntaxEntityKind.Comment, 
                                        string expectedName = "")
        {
            var node = cf.GetCommentAtLine(lineNum);
            Assert.AreNotEqual(node, null);
            if (isNonNullOwner)
            {
                Assert.AreEqual(node.Parent.Kind, expectedKind);
                Assert.AreEqual(node.Parent.Name, expectedName);
            }
            else
            {
                Assert.AreEqual(node.Parent, null);
            }
        }

        [TestMethod]
        public void GetContextBlockTest()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            CSharp csharp = new CSharp();
            CodeFile cf = csharp.Parse(System.IO.File.ReadAllText(@".\Programs\CSharp\Blocks.txt"), @".\Programs\CSharp\Blocks.txt");

            var list = cf.GetContextAtLine(30).ToList();
            Assert.AreEqual(list.Count, 9);
            Assert.AreEqual(list[0].Kind, SyntaxEntityKind.Else);
            Assert.AreEqual(list[1].Kind, SyntaxEntityKind.If);
            Assert.AreEqual(list[2].Kind, SyntaxEntityKind.For);
            Assert.AreEqual(list[3].Kind, SyntaxEntityKind.Try);
            Assert.AreEqual(list[4].Kind, SyntaxEntityKind.Function);
            Assert.AreEqual(list[5].Kind, SyntaxEntityKind.Class);
            Assert.AreEqual(list[6].Kind, SyntaxEntityKind.Class);
            Assert.AreEqual(list[7].Kind, SyntaxEntityKind.Namespace);
            Assert.AreEqual(list[8].Kind, SyntaxEntityKind.CodeFile);

            Assert.AreEqual(list[4].Name, "Foo");
            Assert.AreEqual(list[5].Name, "InnerClass");
            Assert.AreEqual(list[6].Name, "OuterClass");
            Assert.AreEqual(list[7].Name, "Microsoft.CodeTalk.LanguageService.Tests.Programs");
        }
    }
}
