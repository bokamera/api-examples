using System;
using ServiceStack.Text;

namespace BokaMera.API.Samples.Bootstrap
{
    public static class ObjectExtensions
    {
        public static void PrintDump(this object o, string headLine)
        {
            Console.WriteLine(headLine);
            o.PrintDump();
        }
    }
}