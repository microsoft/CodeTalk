//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using IronPython.Compiler.Ast;


namespace Microsoft.CodeTalk.LanguageService
{
    class PythonEntityCollector : PythonWalker
    {
        private CodeFile m_currentCodeFile;
        private ISyntaxEntity m_currentParent = null;
        private Stack<ISyntaxEntity> m_savedParents;

        public PythonEntityCollector()
        {
            m_savedParents = new Stack<ISyntaxEntity>();
        }

        public override bool Walk(PythonAst ast)
        {
            if ( ast == null)
            {
                return false;
            }

            m_currentCodeFile = new CodeFile("Python", new FileSpan(ast.Start.Line, ast.Start.Column, ast.End.Line, ast.End.Column), new Python());
            m_currentParent = m_currentCodeFile;
            return true;

        }

        /// <summary>
        /// This function overrides the behavior when the current node being walked is a ClassDefinition
        /// </summary>
        /// <param name="node"> the ClassDefinition node</param>
        /// <returns> boolean value. not important for this implementation. it is handeled by a call to the base Walk() method.</returns>
        public override bool Walk(IronPython.Compiler.Ast.ClassDefinition node)
        {
            UserDefinedType udt = PythonEntityCreationHelper.createClassUDT(node, m_currentCodeFile, m_currentParent);
            m_currentParent.AddChild(udt);
            udt.Parent = m_currentParent;
            m_savedParents.Push(m_currentParent);
            m_currentParent = udt;
                                    
            return true;
                    }

        public override void PostWalk(IronPython.Compiler.Ast.ClassDefinition node)
        {
            Trace.TraceInformation("in method postWalk(ClassDefinition node)");
            m_currentParent = m_savedParents.Pop();
            Trace.TraceInformation("restored m_current parent to " + m_currentParent.Name);
            base.PostWalk(node);
        }

        /// <summary>
        /// This function overrides the behavior when the node being walked is a FunctionDefinition.
        /// </summary>
        /// <param name="node"> the python ast node of type FunctionDefinition. </param>
        /// <returns> boolean value. not important for current implementation. it is taken care of by a call to the base class's walk method.</returns>
        public override bool Walk(IronPython.Compiler.Ast.FunctionDefinition node)
        {
            if ( node == null )
            {
                return false;
            }
            FunctionDefinition fn = PythonEntityCreationHelper.createFunction(node, m_currentCodeFile, m_currentParent);
            node.Parameters.ToList().ForEach(param => fn.AddFormalParameter(new FormalParameter("", param.Name)));
            m_currentParent.AddChild(fn);
            fn.Parent = m_currentParent;
            m_savedParents.Push(m_currentParent);
            m_currentParent = fn;

            return true;

        }

        public override void PostWalk(IronPython.Compiler.Ast.FunctionDefinition node)
        {
            if ( node == null )
            {
                return;
            }
            Trace.TraceInformation("in postWalk(FunctionDefinition node) for node " + node.Name);
            m_currentParent = m_savedParents.Pop();
            Trace.TraceInformation("restored m_currentParent to " + m_currentParent.Name);
            base.PostWalk(node);
        }

        public override bool Walk(IronPython.Compiler.Ast.ForStatement node)
        {
            if (node == null)
            {
                return false;
            }
            var forBlock = PythonEntityCreationHelper.createForBlock(node, m_currentCodeFile, m_currentParent);
            m_currentParent.AddChild(forBlock);
            forBlock.Parent = m_currentParent;
            m_savedParents.Push(m_currentParent);
            m_currentParent = forBlock;
            return true;
        }

        public override void PostWalk(IronPython.Compiler.Ast.ForStatement node)
        {
if(node == null)
            {
                return;
            }
            m_currentParent = m_savedParents.Pop();
            base.PostWalk(node);
        }

        public override bool Walk(IronPython.Compiler.Ast.ComprehensionFor node)
        {
            if (node == null)
            {
                return false;
            }
            var forBlock = PythonEntityCreationHelper.createComprehensionForBlock(node, m_currentCodeFile, m_currentParent);
            m_currentParent.AddChild(forBlock);
            forBlock.Parent = m_currentParent;
            m_savedParents.Push(m_currentParent);
            m_currentParent = forBlock;
            return true;
        }

        public override void PostWalk(IronPython.Compiler.Ast.ComprehensionFor node)
        {
            if (node == null)
            {
                return;
            }
            m_currentParent = m_savedParents.Pop();
            base.PostWalk(node);
        }

                public override bool Walk(IronPython.Compiler.Ast.WhileStatement node)
        {
            if(node == null)
            {
                return false;
            }
            var whileBlock = PythonEntityCreationHelper.createWhileBlock(node, m_currentCodeFile, m_currentParent);
            m_currentParent.AddChild(whileBlock);
            whileBlock.Parent = m_currentParent;
            m_savedParents.Push(m_currentParent);
            m_currentParent = whileBlock;
            return true;

        }

        public override void PostWalk(IronPython.Compiler.Ast.WhileStatement node)
        {
            if(node == null)
            {
                return;
            }
            m_currentParent = m_savedParents.Pop();
            base.PostWalk(node);
        }

        public override bool Walk(IronPython.Compiler.Ast.IfStatement node)
        {
            if (node == null)
            {
                return false;
            }
            var ifBlock = PythonEntityCreationHelper.createIfBlock(node, m_currentCodeFile, m_currentParent);
            m_currentParent.AddChild(ifBlock);
            ifBlock.Parent = m_currentParent;
            m_savedParents.Push(m_currentParent);
            m_currentParent = ifBlock;
            return true;
        }

        public override void PostWalk(IronPython.Compiler.Ast.IfStatement node)
        {
            if(node == null)
            {
                return;
            }
            m_currentParent = m_savedParents.Pop();
            base.PostWalk(node);
        }

        public override bool Walk(IronPython.Compiler.Ast.ComprehensionIf node)
        {
            if (node == null)
            {
                return false;
            }
            var ifBlock = PythonEntityCreationHelper.createComprehensionIfBlock(node, m_currentCodeFile, m_currentParent);
            m_currentParent.AddChild(ifBlock);
            ifBlock.Parent = m_currentParent;
            m_savedParents.Push(m_currentParent);
            m_currentParent = ifBlock;
            return true;
        }
        
        public override void PostWalk(IronPython.Compiler.Ast.ComprehensionIf node)
        {
            if (node == null)
            {
                return;
            }
            m_currentParent = m_savedParents.Pop();
            base.PostWalk(node);
        }

        public override bool Walk(IronPython.Compiler.Ast.TryStatement node)
        {
            if (node == null)
            {
                return false;
            }
            var tryBlock = PythonEntityCreationHelper.createTryBlock(node, m_currentCodeFile, m_currentParent);
            m_currentParent.AddChild(tryBlock);
            tryBlock.Parent = m_currentParent;
            m_savedParents.Push(m_currentParent);
            m_currentParent = tryBlock;
            return true;
        }

        public override void PostWalk(IronPython.Compiler.Ast.TryStatement node)
        {
            if (node == null)
            {
                return;
            }
            m_currentParent = m_savedParents.Pop();
            base.PostWalk(node);
        }

        
        ///<summary>
        ///This method returns the populated CodeFile.
        /// </summary>
        /// <returns>the populated CodeFile</returns>
        public CodeFile getCodeFile()
        {
            return m_currentCodeFile;
        }

    }
}
