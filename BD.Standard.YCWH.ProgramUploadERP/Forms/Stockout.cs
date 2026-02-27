using BD.Standard.YCWH.ProgramParse.Utils;
using BD.Standard.YCWH.ProgramUploadERP.Utils;
using Kingdee.BOS.Log;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data;

namespace BD.Standard.YCWH.ProgramUploadERP.Forms
{
    internal class Stockout
    {
        public Stockout()
        {
        }

        public void StockoutSC(string sql)
        {
            DBConnection Con = new DBConnection();
            DataSet orgIdsSet = Con.getDataSet(sql);
            if (orgIdsSet.Tables[0].Rows.Count == 0) return;
            int count = 0;
            foreach (DataRow dataH in orgIdsSet.Tables[0].Rows)
            {
                
                DataSet dSbt = Con.getDataSet(string.Format("EXEC YCWH_Stock '{0}','{1}'", dataH.ItemArray[0].ToString(), dataH.ItemArray[1].ToString()));
                foreach (DataRow item in dSbt.Tables[0].Rows)
                {
                    string formid = dataH.ItemArray[1].ToString().Equals("STK_MisDeliveryZC") ? "STK_MisDelivery" : dataH.ItemArray[1].ToString();
                    string respJson = WebApi.client.Save(formid, item.ItemArray[0].ToString());

                    //超时重新触发登录
                    if (JObject.Parse(respJson)["Result"]["ResponseStatus"]["MsgCode"].ToString().Equals("1"))
                    {
                        Kingdee.BOS.WebApi.Client.K3CloudApiClient k3CloudApiClient = WebApi.WebApiClent(ConfigurationManager.AppSettings["url"], ConfigurationManager.AppSettings["dbid"], ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["pwd"]);
                        respJson = WebApi.client.Save(formid, item.ItemArray[0].ToString());
                    }

                    int status = JObject.Parse(respJson)["Result"]["ResponseStatus"]["IsSuccess"].ToString().Equals("True") ? 1 : 0;
                    string billno = item.ItemArray[1].ToString();
                    if (billno.Contains("-ZC"))
                    {
                        billno= billno.Replace("-ZC", "");
                    }
                    Con.getDataSet($"update  YCWH_OutStock set status={status},reqJson='{item.ItemArray[0].ToString()}',respJson='{respJson}' where ID='{billno}'");
                }

            }
        }
    }
}
