using System.Data;
using BD.Standard.YCWH.ProgramParse.Utils;
using BD.Standard.YCWH.ProgramUploadERP.Utils;
using Newtonsoft.Json.Linq;

namespace BD.Standard.YCWH.ProgramUploadERP
{
    internal class Supplier
    {
        public void PostSupplier(string sql)
        {
            DBConnection Con = new DBConnection();
            DataSet orgIdsSet = Con.getDataSet(sql);
            if (orgIdsSet.Tables[0].Rows.Count == 0) return;
            foreach (DataRow dataH in orgIdsSet.Tables[0].Rows)
            {
                string reqJson = BuildJson.ERPSaveJson(orgIdsSet, dataH, "FFinanceInfo");
                string respJson = WebApi.client.Save("BD_Supplier", reqJson);
                int status = JObject.Parse(respJson)["Result"]["ResponseStatus"]["IsSuccess"].ToString().Equals("True") ? 1 : 0;
                Con.getDataSet($"update  YCWH_Supplier set status={status},reqJson='{reqJson}',respJson='{respJson}' where ID='{dataH["id"]}'");

            }
        }
    }
}