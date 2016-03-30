using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlDependencySample
{
    class Program
    {
        static void Main(string[] args)
        {
            //UnhandledExceptionイベントハンドラを追加
            Thread.GetDomain().UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

            var sample = new DependencySample();
            Environment.Exit(0);
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorExit(e.ExceptionObject as Exception);
        }

        private static void ErrorExit(Exception ex)
        {
            Console.WriteLine("Error!!");
            Environment.Exit(1);
        }
    }
}
