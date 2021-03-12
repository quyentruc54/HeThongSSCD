using System;
using System.Collections.Generic;

namespace NovaAlert.Common
{
    /// <summary>
    /// Implement of Mediator pattern
    /// </summary>
    static public class Mediator
    {
        static IDictionary<string, List<Action<object>>> pl_dict = new Dictionary<string, List<Action<object>>>();

        static public void Register(string token, Action<object> callback)
        {
            if (!pl_dict.ContainsKey(token))
            {
                var callbackList = new List<Action<object>>();
                callbackList.Add(callback);
                pl_dict.Add(token, callbackList);
            }
            else
            {
                bool found = false;
                List<Action<object>> callbackList = pl_dict[token];
                foreach (var item in callbackList)
                {
                    if (item.Method.ToString() == callback.Method.ToString())
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    pl_dict[token].Add(callback);
                }
            }
        }

        static public void Unregister(string token, Action<object> callback)
        {
            if (pl_dict.ContainsKey(token))
            {
                pl_dict[token].Remove(callback);
            }
        }

        static public void NotifyColleagues(string token, object args)
        {
            if (pl_dict.ContainsKey(token))
            {
                foreach (var callback in pl_dict[token])
                {
                    callback(args);
                }
            }
        }
    }

    public static class Mediator_Message
    {
        public const string SwitchComm_Send = "SwitchComm_Send";
        public const string SwitchComm_Rcv = "SwitchComm_Rcv";
    }
}
