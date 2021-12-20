using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models.Import
{
    public class VMImportBikeRiders
    {
        //private static DataLogic dl = new DataLogic();
        private const string sExcelFilePath = @"C:\Users\eirik.AMS\Dropbox\CyclingTopTenGame\ExcelData\cq20160515.xls";
        private const string sExcelSheetName = "Races";
        private static string sSqlConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerConnectionString"].ConnectionString;

        public static void exportToDatabase()
        {
            //BikeRaces = new List<BikeRace>();
            //create our connection string
            string sexcelconnectionstring = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + sExcelFilePath +
            ";Extended Properties='Excel 8.0;HDR=YES;';";//HDR=YES ignores first row(it is a header)

            OleDbConnection con = new OleDbConnection(sexcelconnectionstring);
            OleDbCommand oconn = new OleDbCommand("Select * From [" + sExcelSheetName + "$]", con);
            con.Open();

            OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
            DataTable dtExcelRaces = new DataTable();
            dtExcelRaces.Columns.AddRange(new DataColumn[6] {
                new DataColumn("StartDate", typeof(string)),
                new DataColumn("EndDate", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Description", typeof(string)),
                new DataColumn("Category", typeof(string)),
                new DataColumn("Country", typeof(string)) });
            sda.Fill(dtExcelRaces);

            DataTable dtBikeRacesToSql = new DataTable();
            dtBikeRacesToSql.Columns.Add("BikeRaceId");
            dtBikeRacesToSql.Columns.Add("Name");
            dtBikeRacesToSql.Columns.Add("StartDate");
            dtBikeRacesToSql.Columns.Add("EndDate");
            dtBikeRacesToSql.Columns.Add("Country");
            dtBikeRacesToSql.Columns.Add("BikeRaceCategoryId");

            foreach (System.Data.DataRow drExcelRace in dtExcelRaces.Rows)
            {
                try
                {
                    DataRow drToSql = dtBikeRacesToSql.NewRow();
                    drToSql["Name"] = drExcelRace["Name"].ToString();
                    drToSql["StartDate"] = drExcelRace["StartDate"].ToString();
                    drToSql["EndDate"] = drExcelRace["EndDate"].ToString();
                    drToSql["Country"] = drExcelRace["Country"].ToString();
                    //drToSql["BikeRaceCategoryId"] = getBikeRaceCategoryId(drExcelRace["Category"].ToString());
                    dtBikeRacesToSql.Rows.Add(drToSql);
                }
                catch (Exception)
                {
                    continue;
                }

            }
            //saveBikeRaces(dtBikeRacesToSql);//save to db after import
        }

        //private static int getBikeRaceCategoryId(string sExcelCategory)
        //{
        //    int iBikeRaceCategoryId;
        //    switch (sExcelCategory)
        //    {
        //        case "1.UWT":
        //            iBikeRaceCategoryId = (int)BikeRaceCategory.BikeRaceCategoryIdEnum.OneDayWT;
        //            break;
        //        case "2.UWT":
        //            iBikeRaceCategoryId = (int)BikeRaceCategory.BikeRaceCategoryIdEnum.StageRaceWT;
        //            break;
        //        case "1.HC":
        //            iBikeRaceCategoryId = (int)BikeRaceCategory.BikeRaceCategoryIdEnum.OneDayHC;
        //            break;
        //        case "2.HC":
        //            iBikeRaceCategoryId = (int)BikeRaceCategory.BikeRaceCategoryIdEnum.StageRaceHC;
        //            break;
        //        default:
        //            iBikeRaceCategoryId = (int)BikeRaceCategory.BikeRaceCategoryIdEnum.OneDayWT;
        //            break;
        //    }
        //    return iBikeRaceCategoryId;
        //}

        ////bulkinsert bikeraces
        //private static void saveBikeRaces(DataTable dtBikeRaces)
        //{
        //    dl.DeleteAllBikeRaces();//delete all before inserting new

        //    dl.AddBikeRacesInBulk(dtBikeRaces, sSqlConnStr);
        //}
    }
}