using BD.Standard.FOM.Program.Utils;
using Kingdee.BOS.WebApi.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;

namespace BD.Standard.YCWH.ProgramUploadERP.Utils
{
    public class WebApi
    {
        private static readonly string Logpath = @"WEBAPI\" + DateTime.Now.ToString("yyyyMM");
        public static K3CloudApiClient client { get; set; }

        static Logger logger = new Logger(ConfigurationManager.AppSettings["log"] + Logpath, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

        /// <summary>
        /// 星空WebApi登录
        /// </summary>
        /// <param name="url">站点地址</param>
        /// <param name="accountid">账套id</param>
        /// <param name="username">用户名</param>
        /// <param name="password">用户密码</param>
        /// <returns>
        /// 返回K3CloudApiClient数据，返回值!=null成功;==null失败
        /// </returns>
        public static K3CloudApiClient WebApiClent(string url, string accountid, string username, string password)
        {
            logger.WriteLog("触发登录！" );
            client = new K3CloudApiClient(url);
            var loginResult = client.ValidateLogin(accountid, username, password, 2052);
            var result = JObject.Parse(loginResult)["LoginResultType"].Value<int>();
            if (result == 1)
            {
                logger.WriteLog("登录返回成功！");
                return client;
            }
            logger.WriteLog("登录返回失败！");
            return null;
        }

    }
}
