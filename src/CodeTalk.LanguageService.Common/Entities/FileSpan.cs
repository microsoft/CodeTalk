//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System;

namespace Microsoft.CodeTalk.LanguageService
{
    public class FileSpan
    {
        public FileSpan(FileLinePositionSpan span)
        {
            StartLineNumber = span.StartLinePosition.Line + 1;
            StartColumn = span.StartLinePosition.Character;
            EndLineNumber = span.EndLinePosition.Line + 1;
            EndColumn = span.EndLinePosition.Character;

        }

        /// <summary>
        /// <summary>
        /// Constructor for FileSpan.
        /// </summary>
        /// <param name="startLineNumber">The line number at which the code corresponding to the node starts</param>
        /// <param name="startColumn">The column at which the code corresponding to the current node starts</param>
        /// <param name="endLineNumber">The line number at which the code corresponding to the current node ends.</param>
        /// <param name="endColumn">The column at which the code corresponding to the current node ends.</param>
        public FileSpan(int startLineNumber, int startColumn, int endLineNumber, int endColumn)
        {
            StartLineNumber = startLineNumber;
            StartColumn = startColumn;
            EndLineNumber = endLineNumber;
            EndColumn = endColumn;
        }

        public int StartLineNumber
        {
            get;
            internal set;
        }

        public int StartColumn
        {
            get;
            internal set;
        }

        public int EndLineNumber
        {
            get;
            internal set;
        }

        public int EndColumn
        {
            get;
            internal set;
        }

        public bool Encompasses(int lineNumber)
        {
            return lineNumber >= StartLineNumber && lineNumber <= EndLineNumber;
        }

        public static FileSpan Combine(FileSpan fileSpan1, FileSpan fileSpan2)
        {
            if ( fileSpan1 == null )
            {
                return fileSpan2;
            }
            if ( fileSpan2 == null )
            {
                return fileSpan1;
            }
            //Starting line of the new span is the lesser of the two line numbers
            //Starting column is the column of the 'earlier' span 
            int startLine = fileSpan1.StartLineNumber <= fileSpan2.StartLineNumber ? fileSpan1.StartLineNumber : fileSpan2.StartLineNumber;
            int startCol = fileSpan1.StartLineNumber <= fileSpan2.StartLineNumber ? fileSpan1.StartColumn : fileSpan2.StartColumn;
            //Ending line of the new span is the greater of the two line numbers
            //Ending column is the column of the 'later' span
            int endLine = fileSpan1.EndLineNumber >= fileSpan2.EndLineNumber ? fileSpan1.EndLineNumber : fileSpan2.EndLineNumber;
            int endCol = fileSpan1.EndLineNumber >= fileSpan2.EndLineNumber ? fileSpan1.EndColumn : fileSpan2.EndColumn;

            return new LanguageService.FileSpan(startLine, startCol, endLine, endCol);
        }
        public int LengthOfSpan { get; }

    }

    internal class LocationComparer : IEqualityComparer<FileSpan>
    {
        public bool Equals(FileSpan fileSpan1, FileSpan fileSpan2)
        {
            if ( fileSpan1 == null || fileSpan2 == null )
            {
                throw new InvalidOperationException("Cannot pass null parameters to Equals method.");
            }
            return (fileSpan1.StartLineNumber == fileSpan2.StartLineNumber) && (fileSpan1.EndLineNumber == fileSpan2.EndLineNumber)
                    && (fileSpan1.StartColumn == fileSpan2.StartColumn) && (fileSpan1.EndColumn == fileSpan2.EndColumn);
        }

        public int GetHashCode(FileSpan fileSpan)
        {
            if ( fileSpan == null )
            {
                throw new InvalidOperationException("Cannot pass null parameter.");
            }
            var x = fileSpan.StartColumn << 16 | fileSpan.StartLineNumber;
            var y = x << 16 | fileSpan.EndLineNumber;
            return y << 16 | fileSpan.EndColumn;
        }
    }
}