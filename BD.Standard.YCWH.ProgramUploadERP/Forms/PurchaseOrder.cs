using BD.Standard.YCWH.ProgramParse.Utils;
using BD.Standard.YCWH.ProgramUploadERP.Utils;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System;
using System.Data;
using BD.Standard.FOM.Program.Utils;

namespace BD.Standard.YCWH.ProgramUploadERP
{
    internal class PurchaseOrder
    {
        public PurchaseOrder()
        {
        }

        public void PostPurchaseOrder(string sql)
        {
            DBConnection Con = new DBConnection();
            DataSet orgIdsSet = Con.getDataSet(sql);
            if (orgIdsSet.Tables[0].Rows.Count == 0) return;
            foreach (DataRow dataH in orgIdsSet.Tables[0].Rows)
            {
                string reqJson = BuildJson.ERPSaveJson(orgIdsSet, dataH, "FPOOrderEntry");
                string respJson = WebApi.client.Save("PUR_PurchaseOrder", reqJson);

                int status = JObject.Parse(respJson)["Result"]["ResponseStatus"]["IsSuccess"].ToString().Equals("True") ? 1 : 0;
                Con.getDataSet($"update  YCWH_PurchaseOrder set status={status},reqJson='{reqJson}',respJson='{respJson}' where ID='{dataH["id"]}'");
                if (status==0)
                {
                       string Logpath = @"PurchaseOrder\" + DateTime.Now.ToString("yyyyMM");
                        Logger logger = new Logger(ConfigurationManager.AppSettings["log"] + Logpath, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                       logger.WriteLog("采购订单上传失败:"+ reqJson);
                }
            }
        }
    }
}