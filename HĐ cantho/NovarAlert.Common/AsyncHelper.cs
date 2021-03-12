using System;
using System.Runtime.Remoting.Messaging;

namespace NovaAlert.Common
{
    public static class AsyncHelper
    {
        // Parameter and return type async method helper
        public static IAsyncResult BeginAsync<T1, TResult>(Func<T1, TResult> method, T1 parameter)
        {
            var methodCopy = method;
            if (methodCopy == null)
                throw new ArgumentNullException();

            // Start the asynchronous operation
            // No callback or state object
            return methodCopy.BeginInvoke(parameter, null, null);
        }


        // Parameter and return type async method helper - async done
        public static TResult EndAsync<T1, TResult>(IAsyncResult aResult)
        {
            Func<T1, TResult> method =
                (Func<T1, TResult>)((AsyncResult)aResult).AsyncDelegate;

            // Retrieve the result
            return method.EndInvoke(aResult);
        }

        public static void RunAsync(Action act)
        {
            var actCopy = act;
            actCopy.BeginInvoke(null, null);
        }

        public static void RunAsync<T>(Action<T> act, T param)
        {
            var actCopy = act;
            actCopy.BeginInvoke(param, null, null);
        }

        public static void RunAsync<T1, T2>(Action<T1, T2> act, T1 param1, T2 param2)
        {
            var actCopy = act;
            actCopy.BeginInvoke(param1, param2, null, null);
        }

        public static void RunAsync<T1, T2, T3>(Action<T1, T2, T3> act, T1 param1, T2 param2, T3 param3)
        {
            var actCopy = act;
            actCopy.BeginInvoke(param1, param2, param3, null, null);
        }

        public static void RunAsync<T1, T2, T3, T4>(Action<T1, T2, T3, T4> act, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            var actCopy = act;
            actCopy.BeginInvoke(param1, param2, param3, param4, null, null);
        }
    }
}
