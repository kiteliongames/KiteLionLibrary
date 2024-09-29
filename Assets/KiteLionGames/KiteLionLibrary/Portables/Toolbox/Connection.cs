#region FileHeader

// test
// Project: Assembly-CSharp
// File:    Connection.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.55
// 
// Copyright (c) 2024 SomeGameDevs, LLC. All rights reserved.
// 
// This source code is the property of SomeGameDevs, LLC and may not be
// copied, distributed, modified, or used in any way without prior written
// permission from SomeGameDevs, LLC.
// 
// Description:
// [Provide a brief description of what this file/class does.]
// 
// Previous Header (if any):
// 
// License:
// This code is provided "as is," without warranty of any kind, express or
// implied, including but not limited to the warranties of merchantability,
// fitness for a particular purpose, and noninfringement. In no event shall
// the authors or copyright holders be liable for any claim, damages, or
// other liability, whether in an action of contract, tort, or otherwise,
// arising from, out of, or in connection with the software or the use or
// other dealings in the software.

#endregion


using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

/// <summary>
/// credit: https://stackoverflow.com/questions/2031824/what-is-the-best-way-to-check-for-internet-connectivity-using-net
/// </summary>



namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox
{
    /// <summary>
    ///     Deprecated. Does not support Unity (cuz async).
    /// </summary>
    [Obsolete("No alternative in unity!")]
    public class Connection
    {
        public static void CheckForInternetConnectionAsync(Action onConnect, Action onFail, int timeoutMs = 10000, string url = null)
        {
            Task.Run(() => CheckForConnectionHelper(onConnect, onFail, timeoutMs, url));
        }

        public static bool GetInternetConnectionAsync(int timeoutMS = 10000, string url = null)
        {
            var result = false;
            CheckForInternetConnectionAsync(() => result = true, () => result = false, timeoutMS, url);
            return result;
        }

        private static async void CheckForConnectionHelper(Action onConnect, Action onFail, int timeoutMs, string url)
        {
            try
            {
                url ??= CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") =>// Iran
                        "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") =>// China
                        "http://www.baidu.com",
                    _ =>
                        "http://www.gstatic.com/generate_204",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                //todo is this legit? this latest c# is odd!
                using var response = await request.GetResponseAsync() as HttpWebResponse;

                onConnect?.Invoke();
            }
            catch
            {
                onFail?.Invoke();
            }
        }
    }
}
