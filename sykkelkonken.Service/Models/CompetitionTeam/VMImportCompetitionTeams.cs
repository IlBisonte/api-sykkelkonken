using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models.CompetitionTeam
{
    public class VMImportCompetitionTeams
    {
        private const string sExcelFilePath = @"G:\Sykkelkonken\Sykkel 2019.xlsx";
        private const string sExcelSheetName = "sykkel";
        //private static string sSqlConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerConnectionString"].ConnectionString;

        public static void readExcelCompTeams()
        {
            IList<VMCompetitionTeam> competitionTeams = new List<VMCompetitionTeam>();
            //BikeRiders = new List<BikeRider>();
            //create our connection string
            string sexcelconnectionstring = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + sExcelFilePath +
            ";Extended Properties='Excel 8.0;IMEX=1;HDR=YES;';";//HDR=YES ignores first row(it is a header)

            OleDbConnection con = new OleDbConnection(sexcelconnectionstring);
            OleDbCommand oconn = new OleDbCommand("Select * From [" + sExcelSheetName + "$]", con);
            con.Open();

            OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
            System.Data.DataTable dtExcelRiders = new System.Data.DataTable();
            //dtExcelRiders.Columns.AddRange(new DataColumn[9] { new DataColumn("Rank", typeof(string)),
            //    new DataColumn("Prev", typeof(string)),
            //    new DataColumn("RiderId", typeof(string)),
            //    new DataColumn("UCICode", typeof(string)),
            //    new DataColumn("Name", typeof(string)),
            //    new DataColumn("Team", typeof(string)),
            //    new DataColumn("Nationality", typeof(string)),
            //    new DataColumn("Age", typeof(string)),
            //    new DataColumn("CQ",typeof(string)) });
            sda.Fill(dtExcelRiders);
            if (dtExcelRiders != null)
            {
                List<int> columnIdxToIgnore = new List<int>();
                int iColumnIdx = 0;
                foreach (DataColumn column in dtExcelRiders.Columns)
                {
                    if (!column.Caption.Equals("Name") && !column.Caption.Equals("CQ") && !column.Caption.Equals("Name1") && !column.Caption.Equals("CQ1"))
                    {
                        competitionTeams.Add(new VMCompetitionTeam()
                        {
                            CompetitionTeamId = iColumnIdx + 1,
                            TeamName = column.Caption
                        });
                    }
                    else
                    {
                        columnIdxToIgnore.Add(iColumnIdx);
                    }
                    iColumnIdx++;
                }

                int iRowIdx = 1;
                foreach (DataRow row in dtExcelRiders.Rows)
                {
                    if (row["Name"].ToString().Equals("Sum"))
                    {
                        break;
                    }
                    VMBikeRider bikeRider = new VMBikeRider();
                    bikeRider.BikeRiderId = iRowIdx;
                    bikeRider.BikeRiderName = row["Name"].ToString();
                    int cqPoints = 0;
                    int.TryParse(row["CQ"].ToString(), out cqPoints);
                    bikeRider.CQPoints = cqPoints;
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        string item = row.ItemArray[i].ToString();
                        if (item.Length > 0 && !columnIdxToIgnore.Contains(i))
                        {
                            VMCompetitionTeam competitionTeam = competitionTeams.Where(ct => ct.CompetitionTeamId == i + 1).First();
                            if (competitionTeam != null)
                            {
                                competitionTeam.BikeRiders.Add(bikeRider);
                            }
                        }
                    }

                    iRowIdx++;
                }

                if (competitionTeams != null)
                {

                }
            }

            /*IList<Model.BikeTeam> modelBikeTeams = new List<Model.BikeTeam>();
            dl.FindAllRecords(ref modelBikeTeams);


            DataTable dtBikeRidersToSql = new DataTable();
            dtBikeRidersToSql.Columns.Add("BikeRiderId");
            dtBikeRidersToSql.Columns.Add("CompetitionName");
            dtBikeRidersToSql.Columns.Add("FullName");
            dtBikeRidersToSql.Columns.Add("BikeTeamId");
            dtBikeRidersToSql.Columns.Add("Nationality");

            foreach (System.Data.DataRow drExcelRider in dtExcelRiders.Rows)
            {
                try
                {
                    string sBikeTeamRiderFromExcel = drExcelRider["Team"].ToString();
                    Model.BikeTeam bikeTeamRider = modelBikeTeams.Where(bt => bt.Code.Trim().Equals(sBikeTeamRiderFromExcel.Trim())).FirstOrDefault();
                    int? iBikeTeamId = null;
                    if (bikeTeamRider != null)
                    {
                        iBikeTeamId = bikeTeamRider.BikeTeamId;
                    }

                    string sNationality = drExcelRider["Nationality"].ToString();
                    string sRiderFullName = drExcelRider["Name"].ToString();
                    string sCompetitionName = getRiderCompetitionName(sRiderFullName, sNationality);


                    DataRow drToSql = dtBikeRidersToSql.NewRow();
                    drToSql["CompetitionName"] = sCompetitionName;
                    drToSql["FullName"] = sRiderFullName;
                    drToSql["BikeTeamId"] = iBikeTeamId;
                    drToSql["Nationality"] = sNationality;
                    dtBikeRidersToSql.Rows.Add(drToSql);
                    //BikeRider bikeRider = new BikeRider()
                    //{
                    //    CompetitionName = sCompetitionName,
                    //    FullName = sRiderFullName,
                    //    BikeTeamId = sBikeTeamId,
                    //    BikeTeamCode = sBikeTeamCode,
                    //    Nationality = sNationality,
                    //};
                    //BikeRiders.Add(bikeRider);
                }
                catch (Exception ex)
                {
                    continue;
                }

            }*/
            //BikeRiders = BikeRiders.OrderBy(bt => bt.BikeTeamCode).ThenBy(bt => bt.CompetitionName).ToList();
            //saveBikeRiders(dtBikeRidersToSql);//save to db after import
        }

        private static string getRiderCompetitionName(string sRiderFullName, string sNationality)
        {
            string sCompetitionName = "";
            int iFirstName = (from ch in sRiderFullName.ToArray()
                              where Char.IsLower(ch)
                              select sRiderFullName.IndexOf(ch)).FirstOrDefault();
            iFirstName -= 1;
            string sFirstNameFirstLetter = sRiderFullName.Substring(iFirstName, 1);
            string sLastName = sRiderFullName.Substring(0, iFirstName).Trim();
            sCompetitionName = string.Format("{0} {1}", sFirstNameFirstLetter, getRiderCompetitionLastName(sLastName, sNationality));
            return sCompetitionName;
        }

        private static string getRiderCompetitionLastName(string sFullLastName, string sNationality)
        {
            switch (sNationality)
            {
                case "ESP":
                    sFullLastName = getSpanishCompetitionLastName(sFullLastName);
                    break;
                case "COL":
                    sFullLastName = getSpanishCompetitionLastName(sFullLastName);
                    break;
                case "VEN":
                    sFullLastName = getSpanishCompetitionLastName(sFullLastName);
                    break;
                default:
                    break;
            }
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sFullLastName.ToLower());
        }

        private static string getSpanishCompetitionLastName(string sFullLastName)
        {
            bool bSpecialCase = sFullLastName.Substring(0, 6).ToLower().IndexOf("de la ") > -1 || sFullLastName.Substring(0, 3).ToLower().IndexOf("de ") > -1;
            if (bSpecialCase)
            {
                if (sFullLastName.Substring(0, 6).ToLower().IndexOf("de la ") > -1)
                {
                    sFullLastName = string.Format("{0}{1}", sFullLastName.Substring(0, 6), sFullLastName.Substring(6).Split(' ').First());
                }
                else if (sFullLastName.Substring(0, 3).ToLower().IndexOf("de ") > -1)
                {
                    sFullLastName = string.Format("{0}{1}", sFullLastName.Substring(0, 3), sFullLastName.Substring(3).Split(' ').First());
                }

                return sFullLastName;
            }
            else
            {
                return sFullLastName.Split(' ').First();
            }

        }

        //bulkinsert teams
        private static void saveTeams(DataTable dtCompTeams)
        {
            //dl.DeleteAllBikeRiders();//delete all before inserting new

            //dl.AddBikeRidersInBulk(dtBikeRiders, sSqlConnStr);
        }
    }
}