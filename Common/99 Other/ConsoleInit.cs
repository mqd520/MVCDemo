using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Common
{
    /// <summary>
    /// Console Init
    /// </summary>
    public class ConsoleInit
    {
        public ConsoleInit()
        {

        }

        protected virtual void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CommonLogger.WriteLog(ELogCategory.Fatal, e: e.ExceptionObject as Exception);
            if (e.IsTerminating)
            {
                Environment.Exit(1);
            }
        }

        protected virtual void OnInit()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        protected virtual void OnWhile()
        {
            while (true)
            {
                string line = Console.ReadLine();
                if (string.Compare(line, "exit", StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(line, "quit", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    break;
                }
            }
        }

        protected virtual void OnExit()
        {
            ConsoleHelper.WriteLine(ELogCategory.Info, "Program will be exited");
        }

        /// <summary>
        /// Run
        /// </summary>
        public virtual void Run()
        {
            OnInit();

            OnWhile();

            OnExit();
        }
    }
}
