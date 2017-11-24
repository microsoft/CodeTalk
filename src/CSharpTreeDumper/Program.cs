using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeTalk.LanguageService;
using Microsoft.CodeAnalysis.CSharp;

namespace DumperProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            if ( args.Length == 0 )
            {
                Console.WriteLine("Usage: <program> <csharp-file>");
            }
            else
            {
                CSharpTreeDumper dumper = new CSharpTreeDumper(Microsoft.CodeAnalysis.SyntaxWalkerDepth.Trivia);
                CSharpSyntaxTree tree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(System.IO.File.ReadAllText(args[0]));
                dumper.Visit(tree.GetRoot());
            }
        }
    }
}
