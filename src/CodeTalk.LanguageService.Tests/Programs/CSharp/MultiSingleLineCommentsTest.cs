using System;
using System.Windows;
using System.Diagnostics;
namespace testPrograms {
    public class commentsTestDummy
    {
                /// <summary>
                /// first constructor.
        /// </summary>
        /// <param name="text">The string.</param>
        public commentsTestDummy(string text)
        {
            int a = 1;
            Console.WriteLine(text);
        }
        // this is a private function.
        
        private void secondMethod()
        {
            //first comment inside function.
            //test comment.
            //another comment.
        }
    }
}