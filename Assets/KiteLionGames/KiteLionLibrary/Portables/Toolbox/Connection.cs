


using System;
using System.Globalization;



using System.Net;
using System.Threading.Tasks;
/// <summary>
/// credit: https://stackoverflow.com/questions/2031824/what-is-the-best-way-to-check-for-internet-connectivity-using-net
/// </summary>



namespace KiteLionGames.Toolbox
{
    /// <summary>
    /// Deprecated. Does not support Unity (cuz async).
    /// 
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
            bool result = false;
            CheckForInternetConnectionAsync(() => result = true, () => result = false, timeoutMS, url);
            return result;
        }

        private static async void CheckForConnectionHelper(Action onConnect, Action onFail, int timeoutMs, string url)
        {
            try
            {
                url ??= CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => // Iran
                        "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => // China
                        "http://www.baidu.com",
                    _ =>
                        "http://www.gstatic.com/generate_204",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                //todo is this legit? this latest c# is odd!
                using HttpWebResponse response = (await request.GetResponseAsync()) as HttpWebResponse;

                onConnect?.Invoke();
            }
            catch
            {
                onFail?.Invoke();
            }
        }
    }
}

