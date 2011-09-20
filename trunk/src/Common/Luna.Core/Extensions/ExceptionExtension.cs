using System;
using System.Text;

namespace Luna.Core.Extensions
{
    public static class ExceptionExtension
    {
        public static string GetExceptionMessage(this Exception ex)
        {
            var message = ex.Message;
            var stackTrace = ex.StackTrace;
            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                message = innerEx.Message;
                stackTrace = innerEx.StackTrace;
                innerEx = innerEx.InnerException;
            }
            return message + Environment.NewLine + stackTrace;
        }

#if NET4
        public static string GetExceptionMessage(this AggregateException ex)
        {
            StringBuilder builder = new StringBuilder();
            ex.InnerExceptions.ForEach(e => builder.AppendLine(e.GetExceptionMessage()));
            return builder.ToString();
        }
#endif

        public static void SafeInvoke(Action action, Action<Exception> handleEx)
        {
            SafeInvoke(action, handleEx, () => { });
        }

        public static void SafeInvoke(Action action, Action<Exception> handleEx, Action finallyAction)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                throw ex;
                //handleEx(ex);
            }
            finally
            {
                finallyAction();
            }
        }
    }
}
