//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.LanguageService
{
	public abstract class AbstractSyntaxEntity : ISyntaxEntity
	{
		/// <summary>
		/// We need this because we frequently encounter situations where
		/// we have to create a class/function object before we find out 
		/// its name.
		/// </summary>
		protected AbstractSyntaxEntity()
		{
			m_children = new List<ISyntaxEntity>();
			m_errors = new List<CompileError>();
		}
		private AbstractSyntaxEntity(string name, FileSpan location)
			: this()
		{
			//Anonymous functions are AbstractSyntaxEntities but don't have names. So cannot
			//block empty/null name here. 
			Debug.Assert(location != null, "Location of entity must be specified.");

			this.Name = name;
			this.Location = location;
		}

		private AbstractSyntaxEntity(string name, FileSpan location, ISyntaxEntity parent)
			: this(name, location)
		{
			this.Parent = parent; //OK if the parent is null.
		}

		protected AbstractSyntaxEntity(string name, FileSpan location, ISyntaxEntity parent, CodeFile currentCodeFile)
			: this(name, location, parent)
		{
			this.CurrentCodeFile = currentCodeFile;
		}
		public FileSpan Location
		{
			get;
			internal protected set;
		}

		public string Name
		{
			get;
			internal protected set;
		}

		public ISyntaxEntity Parent
		{
			get;
			internal protected set;
		}

		public Comment AssociatedComment
		{
			get;
			internal protected set;
		}

		public CodeFile CurrentCodeFile
		{
			get;
			internal protected set;
		}

		public abstract SyntaxEntityKind Kind { get; }

		public abstract void AcceptVisitor(ICodeVisitor visitor);

		public virtual IEnumerable<ISyntaxEntity> Children
		{
			get { return m_children; }
		}

		public virtual void AddChild(ISyntaxEntity child)
		{
			m_children.Add(child);
			m_children.Sort(delegate (ISyntaxEntity c1, ISyntaxEntity c2)
			{
				return c1.Location.StartLineNumber.CompareTo(c2.Location.StartLineNumber);
			});
		}

		public virtual IEnumerable<CompileError> ErrorsAtEntity
		{
			get { return m_errors; }
		}

		public virtual void AddError(CompileError err)
		{
			m_errors.Add(err);
		}

		public virtual string SpokenText()
		{
			return this.Kind + " " + this.Name + " at line " + this.Location.StartLineNumber;
		}

		public virtual string DisplayText()
		{
			return this.Name + " at line " + this.Location.StartLineNumber;
		}

		private List<ISyntaxEntity> m_children;
		private List<CompileError> m_errors;
	}
}
