using sykkelkonken.Data;
using sykkelkonken.Service.Filters;
using sykkelkonken.Service.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace sykkelkonken.Service.Controllers
{
    public class ImportController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public ImportController()
        {
            _unitOfWork = new UnitOfWork();
        }


        //[JwtAuthentication]
        //[AcceptVerbs("GET", "POST")]
        //public HttpResponseMessage AddNewBikeRaces(int year)
        //{

        //}

        [JwtAuthentication]
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage ImportBikeRaces(int year)
        {
            HttpResponseMessage result = null;

            TransactionScope scope = null;
            var httpRequest = HttpContext.Current.Request;

            try
            {
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {

                        var postedFile = httpRequest.Files[file];
                        var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);

                        using (var excel = new OfficeOpenXml.ExcelPackage(postedFile.InputStream))
                        {
                            var tbl = new DataTable();
                            //var ws = excel.Workbook.Worksheets["Year Ranking"];
                            var ws = excel.Workbook.Worksheets.First();
                            var hasHeader = true;  // adjust accordingly

                            // add DataColumns to DataTable
                            foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                            {
                                tbl.Columns.Add(hasHeader ? firstRowCell.Text
                                    : String.Format("Column {0}", firstRowCell.Start.Column));
                            }
                            tbl.Columns.Add("BikeRaceCategoryId");

                            DateTime dtTimeStamp = DateTime.Now;
                            // add DataRows to DataTable
                            int startRow = hasHeader ? 2 : 1;
                            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                            {
                                var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                                DataRow row = tbl.NewRow();
                                foreach (var cell in wsRow)
                                {
                                    row[cell.Start.Column - 1] = cell.Text;
                                }
                                if (row["Class"].ToString().Contains("1.UWT"))
                                {
                                    if (IsMonument(row["Name"].ToString()))
                                    {
                                        row["BikeRaceCategoryId"] = (int)sykkelkonken.Data.BikeRaceCategory.BikeRaceCategoryIdEnum.Monument;
                                    }
                                    else
                                    {
                                        row["BikeRaceCategoryId"] = (int)sykkelkonken.Data.BikeRaceCategory.BikeRaceCategoryIdEnum.OneDay;
                                    }
                                }
                                else
                                {
                                    if (row["Name"].ToString().Contains("Tour de France"))
                                    {
                                        row["BikeRaceCategoryId"] = (int)sykkelkonken.Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance;
                                    }
                                    else if (row["Name"].ToString().Contains("Giro d'Italia") || row["Name"].ToString().Contains("La Vuelta ciclista a España"))
                                    {
                                        row["BikeRaceCategoryId"] = (int)sykkelkonken.Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta;
                                    }
                                    else
                                    {
                                        row["BikeRaceCategoryId"] = (int)sykkelkonken.Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace;
                                    }
                                }
                                tbl.Rows.Add(row);
                            }
                            var msg = String.Format("DataTable successfully created from excel-file. Colum-count:{0} Row-count:{1}",
                                                    tbl.Columns.Count, tbl.Rows.Count);

                            string sSqlConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Context"].ConnectionString;

                            //using (scope = new TransactionScope(TransactionScopeOption.Required))
                            {
                                //this._unitOfWork.BikeRaces.DeleteAllBikeRaces();//truncate to delete and reseed
                                //this._unitOfWork.BikeRaces.AddBikeRacesInBulk(tbl, sSqlConnStr);
                                foreach (DataRow br in tbl.Rows)
                                {

                                    var bikeRace = _unitOfWork.BikeRaces.GetBikeRace(br["Name"].ToString());
                                    if (bikeRace != null)
                                    {
                                        //add new BikeRaceDetail the selected year
                                        BikeRaceDetail bikeRaceDetail = new BikeRaceDetail();
                                        bikeRaceDetail.BikeRaceId = bikeRace.BikeRaceId;
                                        bikeRaceDetail.Year = year;
                                        bikeRaceDetail.Name = bikeRace.Name;
                                        bikeRaceDetail.CountryName = br["Country"].ToString();
                                        if (br["Date From"] != null)
                                        {
                                            DateTime dtFrom = new DateTime();
                                            if (DateTime.TryParse(br["Date From"].ToString(), out dtFrom))
                                            {
                                                bikeRaceDetail.StartDate = Convert.ToDateTime(br["Date From"]);
                                            }
                                        }
                                        if (br["Date To"] != null)
                                        {
                                            DateTime dtTo = new DateTime();
                                            if (DateTime.TryParse(br["Date To"].ToString(), out dtTo))
                                            {
                                                bikeRaceDetail.FinishDate = Convert.ToDateTime(br["Date To"]);
                                            }
                                        }

                                        bikeRaceDetail.BikeRaceCategoryId = Convert.ToInt32(br["BikeRaceCategoryId"]);
                                        if (bikeRaceDetail.BikeRaceCategoryId == (int)sykkelkonken.Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace)
                                        {
                                            //add stages to stagerace
                                            int noOfStages = (int)(bikeRaceDetail.FinishDate - bikeRaceDetail.StartDate).Value.TotalDays + 1;
                                            bikeRaceDetail.NoOfStages = noOfStages;
                                        }
                                        else if (bikeRaceDetail.BikeRaceCategoryId == (int)sykkelkonken.Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance ||
                                           bikeRaceDetail.BikeRaceCategoryId == (int)sykkelkonken.Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta)
                                        {
                                            //add stages to GT
                                            bikeRaceDetail.NoOfStages = 21;
                                        }
                                        _unitOfWork.BikeRaces.AddBikeRaceDetail(bikeRaceDetail);
                                    }
                                    else
                                    {
                                        //add new BikeRace and new BikeRaceDetail the selected year
                                        BikeRace newBikeRace = new BikeRace();
                                        newBikeRace.Name = br["Name"].ToString();
                                        newBikeRace.CountryName = br["Country"].ToString();
                                        int bikeRaceId = _unitOfWork.BikeRaces.AddBikeRaceSave(newBikeRace);

                                        BikeRaceDetail bikeRaceDetail = new BikeRaceDetail();
                                        bikeRaceDetail.BikeRaceId = bikeRaceId;
                                        bikeRaceDetail.Year = year;
                                        bikeRaceDetail.Name = br["Name"].ToString();
                                        bikeRaceDetail.CountryName = br["Country"].ToString();
                                        if (br["Date From"] != null)
                                        {
                                            DateTime dtFrom = new DateTime();
                                            if (DateTime.TryParse(br["Date From"].ToString(), out dtFrom))
                                            {
                                                bikeRaceDetail.StartDate = Convert.ToDateTime(br["Date From"]);
                                            }
                                        }
                                        if (br["Date To"] != null)
                                        {
                                            DateTime dtTo = new DateTime();
                                            if (DateTime.TryParse(br["Date To"].ToString(), out dtTo))
                                            {
                                                bikeRaceDetail.FinishDate = Convert.ToDateTime(br["Date To"]);
                                            }
                                        }
                                        bikeRaceDetail.BikeRaceCategoryId = Convert.ToInt32(br["BikeRaceCategoryId"]);
                                        _unitOfWork.BikeRaces.AddBikeRaceDetail(bikeRaceDetail);
                                    }
                                }

                                _unitOfWork.Complete();
                                //scope.Complete();
                            }
                        }
                    }

                    result = Request.CreateResponse(HttpStatusCode.Created);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                string sFeilmelding = "";
                do
                {
                    sFeilmelding += string.Format("- {0}", ex.Message);
                    ex = ex.InnerException;
                } while (ex != null);
                result = Request.CreateResponse(HttpStatusCode.BadRequest, sFeilmelding);
            }
            finally
            {
                if (scope != null)
                {
                    scope.Dispose();
                }
            }

            return result;
        }

        private bool IsMonument(string sBikeRaceName)
        {
            bool bIsMonument = false;

            if (sBikeRaceName.Contains("Milano-Sanremo"))
            {
                bIsMonument = true;
            }
            else if (sBikeRaceName.Contains("Ronde van Vlaanderen"))
            {
                bIsMonument = true;
            }
            else if (sBikeRaceName.Contains("Paris-Roubaix"))
            {
                bIsMonument = true;
            }
            else if (sBikeRaceName.Contains("Liège-Bastogne-Liège"))
            {
                bIsMonument = true;
            }
            else if (sBikeRaceName.Contains("Il Lombardia"))
            {
                bIsMonument = true;
            }

            return bIsMonument;
        }

        [JwtAuthentication]
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage ImportBikeRiders(int year)
        {
            HttpResponseMessage result = null;

            TransactionScope scope = null;
            var httpRequest = HttpContext.Current.Request;

            try
            {
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {

                        var postedFile = httpRequest.Files[file];
                        var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);

                        using (var excel = new OfficeOpenXml.ExcelPackage(postedFile.InputStream))
                        {
                            var tbl = new DataTable();
                            //var ws = excel.Workbook.Worksheets["Year Ranking"];
                            var ws = excel.Workbook.Worksheets.First();
                            var hasHeader = true;  // adjust accordingly

                            // add DataColumns to DataTable
                            foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                            {
                                tbl.Columns.Add(hasHeader ? firstRowCell.Text
                                    : String.Format("Column {0}", firstRowCell.Start.Column));
                            }

                            DateTime dtTimeStamp = DateTime.Now;
                            // add DataRows to DataTable
                            int startRow = hasHeader ? 2 : 1;
                            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                            {
                                var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                                DataRow row = tbl.NewRow();
                                foreach (var cell in wsRow)
                                {
                                    row[cell.Start.Column - 1] = cell.Text;
                                }
                                tbl.Rows.Add(row);
                            }

                            foreach (DataRow br in tbl.Rows)
                            {
                                if (br["Name"].ToString() == "")
                                {
                                    continue;
                                }
                                //if same name, then check nationality. if both the same then ???
                                var bikeRider = _unitOfWork.BikeRiders.GetBikeRiderByNameNationality(br["Name"].ToString(), br["Nationality"].ToString());
                                if (bikeRider != null)
                                {
                                    if (bikeRider.BirthDate == null)
                                    {
                                        var birthdate = br["UCICode"].ToString();
                                        birthdate = birthdate.Substring(3);
                                        var birthdateYear = birthdate.Substring(0, 4);
                                        int iBDYear = year;
                                        Int32.TryParse(birthdateYear, out iBDYear);
                                        var birthdateMonth = birthdate.Substring(4, 2);
                                        int iBDMonth = 1;
                                        Int32.TryParse(birthdateMonth, out iBDMonth);
                                        var birthdateDay = birthdate.Substring(6, 2);
                                        int iBDDay = 1;
                                        Int32.TryParse(birthdateDay, out iBDDay);
                                        if (iBDMonth == 0 || iBDDay == 0)
                                        {
                                            iBDMonth = 1;
                                            iBDDay = 1;
                                        }
                                        bikeRider.BirthDate = new DateTime(iBDYear, iBDMonth, iBDDay);
                                    }
                                    //add new BikeRiderDetail the selected year
                                    //check to see if bikerider has detail the selected year and then update cqpoints
                                    IList<BikeRiderDetail> lstBikeRiderDetails = _unitOfWork.BikeRiders.GetBikeRiderDetails(brd => brd.BikeRiderId == bikeRider.BikeRiderId && brd.Year == year);
                                    if (lstBikeRiderDetails.Count > 0)
                                    {
                                        var brd = lstBikeRiderDetails.FirstOrDefault();
                                        if (brd != null)
                                        {
                                            if (brd.CQPoints < 35)
                                            {
                                                string s = "test";
                                                if (s.Length > 0)
                                                {

                                                }
                                            }
                                            if (br["CQ"] != null)
                                            {
                                                int cqPoints = 0;
                                                if (Int32.TryParse(br["CQ"].ToString(), out cqPoints))
                                                {
                                                    brd.CQPoints = cqPoints;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        BikeRiderDetail bikeRiderDetail = new BikeRiderDetail();
                                        bikeRiderDetail.BikeRiderId = bikeRider.BikeRiderId;
                                        bikeRiderDetail.Year = year;
                                        bikeRiderDetail.BikeTeamCode = br["Team"].ToString();


                                        if (br["CQ"] != null)
                                        {
                                            int cqPoints = 0;
                                            if (Int32.TryParse(br["CQ"].ToString(), out cqPoints))
                                            {
                                                bikeRiderDetail.CQPoints = cqPoints;
                                            }
                                        }
                                        _unitOfWork.BikeRiders.AddBikeRiderDetail(bikeRiderDetail);
                                    }
                                }
                                else
                                {
                                    //add new BikeRace and new BikeRaceDetail the selected year
                                    BikeRider newBikeRider = new BikeRider();
                                    newBikeRider.BikeRiderName = br["Name"].ToString();
                                    newBikeRider.Nationality = br["Nationality"].ToString();
                                    var birthdate = br["UCICode"].ToString();
                                    birthdate = birthdate.Substring(3);
                                    var birthdateYear = birthdate.Substring(0, 4);
                                    int iBDYear = year;
                                    Int32.TryParse(birthdateYear, out iBDYear);
                                    var birthdateMonth = birthdate.Substring(4, 2);
                                    int iBDMonth = 1;
                                    Int32.TryParse(birthdateMonth, out iBDMonth);
                                    var birthdateDay = birthdate.Substring(6, 2);
                                    int iBDDay = 1;
                                    Int32.TryParse(birthdateDay, out iBDDay);
                                    if (iBDMonth == 0 || iBDDay == 0)
                                    {
                                        iBDMonth = 1;
                                        iBDDay = 1;
                                    }
                                    newBikeRider.BirthDate = new DateTime(iBDYear, iBDMonth, iBDDay);
                                    int bikeRiderId = _unitOfWork.BikeRiders.AddBikeRiderSave(newBikeRider);

                                    //add new BikeRiderDetail the selected year
                                    BikeRiderDetail bikeRiderDetail = new BikeRiderDetail();
                                    bikeRiderDetail.BikeRiderId = bikeRiderId;
                                    bikeRiderDetail.Year = year;
                                    bikeRiderDetail.BikeTeamCode = br["Team"].ToString();
                                    if (br["CQ"] != null)
                                    {
                                        int cqPoints = 0;
                                        if (Int32.TryParse(br["CQ"].ToString(), out cqPoints))
                                        {
                                            bikeRiderDetail.CQPoints = cqPoints;
                                        }
                                    }
                                    _unitOfWork.BikeRiders.AddBikeRiderDetail(bikeRiderDetail);
                                }
                            }
                            _unitOfWork.Complete();
                        }
                    }

                    result = Request.CreateResponse(HttpStatusCode.Created);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                string sFeilmelding = "";
                do
                {
                    sFeilmelding += string.Format("- {0}", ex.Message);
                    ex = ex.InnerException;
                } while (ex != null);
                result = Request.CreateResponse(HttpStatusCode.BadRequest, sFeilmelding);
            }
            finally
            {
                if (scope != null)
                {
                    scope.Dispose();
                }
            }

            return result;
        }

        [JwtAuthentication]
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage ImportBikeTeams()
        {
            HttpResponseMessage result = null;

            //TransactionScope scope = null;
            var httpRequest = HttpContext.Current.Request;

            try
            {
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {

                        var postedFile = httpRequest.Files[file];
                        var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);

                        using (var excel = new OfficeOpenXml.ExcelPackage(postedFile.InputStream))
                        {
                            var tbl = new DataTable();
                            //var ws = excel.Workbook.Worksheets["Year Ranking"];
                            var ws = excel.Workbook.Worksheets.First();
                            var hasHeader = true;  // adjust accordingly

                            // add DataColumns to DataTable
                            foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                            {
                                tbl.Columns.Add(hasHeader ? firstRowCell.Text
                                    : String.Format("Column {0}", firstRowCell.Start.Column));
                            }

                            DateTime dtTimeStamp = DateTime.Now;
                            // add DataRows to DataTable
                            int startRow = hasHeader ? 2 : 1;
                            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                            {
                                var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                                DataRow row = tbl.NewRow();
                                foreach (var cell in wsRow)
                                {
                                    row[cell.Start.Column - 1] = cell.Text;
                                }
                                tbl.Rows.Add(row);
                            }
                            var msg = String.Format("DataTable successfully created from excel-file. Colum-count:{0} Row-count:{1}",
                                                    tbl.Columns.Count, tbl.Rows.Count);

                            string sSqlConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Context"].ConnectionString;

                            //using (scope = PP.Common.TransactionScopeHelper.GetTransactionScope())
                            //{
                            //    this.logic.DeleteAllTransportRoutes();//truncate to delete and reseed
                            //    this.logic.AddTransportRouteInBulk(tbl, sSqlConnStr);
                            //    scope.Complete();
                            //}
                        }
                    }

                    result = Request.CreateResponse(HttpStatusCode.Created);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                string sFeilmelding = "";
                do
                {
                    sFeilmelding += string.Format("- {0}", ex.Message);
                    ex = ex.InnerException;
                } while (ex != null);
                result = Request.CreateResponse(HttpStatusCode.BadRequest, sFeilmelding);
            }
            finally
            {
                //if (scope != null)
                //{
                //    scope.Dispose();
                //}
            }

            return result;
        }
    }
}
