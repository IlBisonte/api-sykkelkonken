using HtmlAgilityPack;
using sykkelkonken.Data;
using sykkelkonken.Service.Filters;
using sykkelkonken.Service.Models;
using sykkelkonken.Service.Models.CompetitionTeam;
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
    public class CompetitionTeamsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompetitionTeamsController()
        {
            _unitOfWork = new UnitOfWork();
        }

        [JwtAuthentication]
        public IList<VMCompetitionTeam> Get(int year)
        {
            return _unitOfWork.CompetitionTeams.Get(year).Select(ct => new VMCompetitionTeam(ct)).ToList();
        }

        [JwtAuthentication]
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage ImportCompetitionTeams(int year)
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
                            IList<Data.CompetitionTeam> competitionTeams = new List<Data.CompetitionTeam>();
                            var tbl = new DataTable();
                            //var ws = excel.Workbook.Worksheets["Year Ranking"];
                            var ws = excel.Workbook.Worksheets.First();
                            var hasHeader = true;  // adjust accordingly

                            // add DataColumns to DataTable
                            //foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                            //{
                            //    tbl.Columns.Add(hasHeader ? firstRowCell.Text
                            //        : String.Format("Column {0}", firstRowCell.Start.Column));
                            //}

                            int iColumnIdx = 0;
                            List<int> columnIdxToIgnore = new List<int>();
                            foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                            {
                                if (!firstRowCell.Text.Equals("Name") && !firstRowCell.Text.Equals("CQ") && !firstRowCell.Text.Equals("Name1") && !firstRowCell.Text.Equals("CQ1"))
                                {
                                    competitionTeams.Add(new Data.CompetitionTeam()
                                    {
                                        CompetitionTeamId = iColumnIdx + 1,
                                        Name = firstRowCell.Text,
                                        Year = year,
                                        CompetitionTeamBikeRiders = new List<CompetitionTeamBikeRider>(),
                                    });
                                }
                                else
                                {
                                    columnIdxToIgnore.Add(iColumnIdx);
                                }
                                iColumnIdx++;
                            }

                            int startRow = hasHeader ? 2 : 1;
                            int iRowIdx = 1;
                            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                            {
                                var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                                if (ws.Cells[rowNum, 1].Text.Equals("Sum"))
                                {
                                    break;
                                }
                                string bikeRiderName = ws.Cells[rowNum, 1].Text;
                                Data.BikeRiderDetail bikeRider = _unitOfWork.BikeRiders.GetBikeRiderDetailByName(bikeRiderName, year);
                                if (bikeRider != null)
                                {
                                    int iCellIdx = 0;
                                    foreach (var cell in wsRow)
                                    {
                                        string item = cell.Text;
                                        if (item.Length > 0 && !columnIdxToIgnore.Contains(iCellIdx))
                                        {
                                            Data.CompetitionTeam competitionTeam = competitionTeams.Where(ct => ct.CompetitionTeamId == iCellIdx + 1).First();
                                            if (competitionTeam != null)
                                            {
                                                competitionTeam.CompetitionTeamBikeRiders.Add(new CompetitionTeamBikeRider()
                                                {
                                                    CompetitionTeamId = competitionTeam.CompetitionTeamId,
                                                    BikeRider = bikeRider.BikeRider,
                                                    BikeRiderId = bikeRider.BikeRiderId,
                                                });
                                            }
                                        }
                                        iCellIdx++;
                                    }
                                }

                                iRowIdx++;
                            }
                            // add DataRows to DataTable
                            //for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                            //{
                            //    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                            //    DataRow row = tbl.NewRow();
                            //    foreach (var cell in wsRow)
                            //    {
                            //        row[cell.Start.Column - 1] = cell.Text;
                            //    }
                            //    tbl.Rows.Add(row);
                            //}
                            var msg = String.Format("DataTable successfully created from excel-file. Colum-count:{0} Row-count:{1}",
                                                    tbl.Columns.Count, tbl.Rows.Count);

                            string sSqlConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Context"].ConnectionString;

                            foreach (var competitionTeam in competitionTeams)
                            {
                                int totalCqPoints = 0;
                                foreach (var rider in competitionTeam.CompetitionTeamBikeRiders)
                                {
                                    var bikeRiderDetail = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(rider.BikeRiderId, year);
                                    if (bikeRiderDetail != null)
                                    {
                                        totalCqPoints += bikeRiderDetail.CQPoints;
                                    }
                                }
                                competitionTeam.TotalCQPoints = totalCqPoints;
                                int competitionTeamId = this._unitOfWork.CompetitionTeams.AddCompetitionTeam(competitionTeam);
                                if (competitionTeamId > 0)
                                {
                                    competitionTeam.CompetitionTeamId = competitionTeamId;
                                        
                                }
                            }
                            foreach (var competitionTeam in competitionTeams)
                            {
                                foreach (var bikeRider in competitionTeam.CompetitionTeamBikeRiders)
                                {
                                    this._unitOfWork.CompetitionTeams.AddBikeRiderToCompetitionTeam(competitionTeam.CompetitionTeamId, bikeRider.BikeRiderId);
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
                //if (scope != null)
                //{
                //    scope.Dispose();
                //}
            }

            return result;
        }


        [JwtAuthentication]
        [AcceptVerbs("GET")]
        public string ReadHTML()
        {

            string input = new WebClient().DownloadString(@"https://cqranking.com/men/asp/gen/race.asp?raceid=34664");

            if (input != null)
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(input);
                var tables = doc.DocumentNode.Descendants("table").Where(d => d.GetAttributeValue("class", "").Contains("border"));
                if (tables != null)
                {
                    int i = 0;
                    foreach (var htmltable in tables)
                    {
                        if (i > 0)
                        {
                            var headers = htmltable.SelectNodes("//tr/th");
                            DataTable dt = new DataTable();
                            //foreach (HtmlNode header in headers)
                            //    dt.Columns.Add(header.InnerText); // create columns from th
                            // select rows with td elements 
                            var rows = htmltable.SelectNodes("//tr[td]");
                            if (rows != null)
                            {

                            }
                            foreach (var row in htmltable.SelectNodes("//tr[td]"))
                                dt.Rows.Add(row.SelectNodes("td").Select(td => td.InnerText).ToArray());
                        }
                        i++;
                    }
                }
                HtmlNode firstTable = doc.DocumentNode.SelectSingleNode("//table");
                var orderedCellTexts = firstTable.Descendants("tr")
                    .Select(row => row.SelectNodes("th|td").Take(2).ToArray())
                    .Where(cellArr => cellArr.Length == 2)
                    .Select(cellArr => new { Cell1 = cellArr[0].InnerText, Cell2 = cellArr[1].InnerText })
                    .OrderBy(x => x.Cell1)
                    .ToList();
            }

            return "";
        }

        [JwtAuthentication]
        [AcceptVerbs("GET")]
        public string Remove()
        {
            this._unitOfWork.CompetitionTeams.RemoveCompetitionTeams();
            return "";
        }

        /* Champions League */

        [JwtAuthentication]
        [HttpGet]
        public IList<VMChampionsLeagueTeam> GetChampionsLeagueTeams(int year)
        {
            IList<VMChampionsLeagueTeam> lstClTeams = new List<VMChampionsLeagueTeam>();
            var championsLeagueTeams = _unitOfWork.ChampionsLeagueTeams.Get(year);
            foreach (var clTeam in championsLeagueTeams)
            {
                if (clTeam.TotalCQPoints == 0)
                {
                    clTeam.TotalCQPoints = clTeam.ChampionsLeagueTeamBikeRiders.Sum(cl => cl.BikeRiderDetail.CQPoints);
                    _unitOfWork.Complete();
                }
                lstClTeams.Add(new VMChampionsLeagueTeam(clTeam));
            }
            return lstClTeams;
        }

        [JwtAuthentication]
        [HttpPost]
        public int InsertChampionsLeagueTeam(VMChampionsLeagueTeam clTeam)
        {
            int competitionTeamId = -1;
            try
            {
                competitionTeamId = _unitOfWork.ChampionsLeagueTeams.AddChampionsLeagueTeam(new ChampionsLeagueTeam()
                {
                    Name = clTeam.TeamName,
                    Color = clTeam.Color,
                    Year = clTeam.Year
                });
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
            }
            return competitionTeamId;
        }

        [JwtAuthentication]
        [HttpPut]
        public bool UpdateChampionsLeagueTeam(VMChampionsLeagueTeam clTeam)
        {
            bool bUpdateOk = true;
            try
            {
                _unitOfWork.ChampionsLeagueTeams.UpdateChampionsLeagueTeam(clTeam.ChampionsLeagueTeamId, clTeam.TeamName, clTeam.Color);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                bUpdateOk = false;
            }
            return bUpdateOk;
        }

        [JwtAuthentication]
        [HttpDelete]
        public bool RemoveChampionsLeagueTeam(int championsLeagueTeamId)
        {
            bool bRemoveOk = true;
            try
            {
                _unitOfWork.ChampionsLeagueTeams.RemoveChampionsLeagueTeam(championsLeagueTeamId);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                bRemoveOk = false;
            }
            return bRemoveOk;
        }

        [HttpGet]
        public IList<VMBikeRider> GetRidersChampionsLeagueTeam(int championsLeagueTeamId)
        {
            return _unitOfWork.ChampionsLeagueTeams.GetBikeRidersCLTeam(championsLeagueTeamId).Select(ct => new VMBikeRider()
            {
                BikeRiderDetailId = ct.BikeRiderDetailId,
                BikeRiderId = ct.BikeRiderDetail.BikeRiderId,
                BikeRiderName = ct.BikeRiderDetail.BikeRider.BikeRiderName,
                BikeTeamCode = ct.BikeRiderDetail.BikeTeamCode,
                BikeTeamName = ct.BikeRiderDetail.BikeTeamName,
                CQPoints = ct.BikeRiderDetail.CQPoints,
                Nationality = ct.BikeRiderDetail.BikeRider.Nationality,
                Year = ct.BikeRiderDetail.Year
            }).OrderByDescending(ct => ct.CQPoints).ToList();
        }

        [JwtAuthentication]
        [HttpPost]
        public int InsertBikeRiderChampionsLeagueTeam(VMChampionsLeagueTeamBikeRider clTeamRider)
        {
            int competitionTeamId = -1;
            try
            {
                BikeRiderDetail bikeRiderDetail = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(clTeamRider.BikeRiderId, clTeamRider.Year);
                _unitOfWork.ChampionsLeagueTeams.AddBikeRiderToChampionsLeagueTeam(clTeamRider.ChampionsLeagueTeamId, bikeRiderDetail.BikeRiderDetailId);
                _unitOfWork.Complete();
                UpdateCLTeamTotalCQ(clTeamRider.ChampionsLeagueTeamId);
            }
            catch (Exception ex)
            {
            }
            return competitionTeamId;
        }

        [JwtAuthentication]
        [HttpPut]
        public bool UpdateBikeRiderChampionsLeagueTeam(VMUpdateChampionsLeagueTeamBikeRider clTeamRider)
        {
            bool bUpdateOk = true;
            try
            {
                BikeRiderDetail bikeRiderDetailOrig = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(clTeamRider.OrigBikeRiderId, clTeamRider.Year);
                BikeRiderDetail bikeRiderDetailNew = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(clTeamRider.NewBikeRiderId, clTeamRider.Year);
                _unitOfWork.ChampionsLeagueTeams.UpdateRiderChampionsLeagueTeam(clTeamRider.ChampionsLeagueTeamId, bikeRiderDetailOrig.BikeRiderDetailId, bikeRiderDetailNew.BikeRiderDetailId);
                _unitOfWork.Complete();
                UpdateCLTeamTotalCQ(clTeamRider.ChampionsLeagueTeamId);
            }
            catch (Exception ex)
            {
                bUpdateOk = false;
            }
            return bUpdateOk;
        }

        [JwtAuthentication]
        [HttpPut]
        public bool RemoveBikeRiderChampionsLeagueTeam(VMChampionsLeagueTeamBikeRider clTeamRider)
        {
            bool bRemoveOk = true;
            try
            {
                BikeRiderDetail bikeRiderDetail = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(clTeamRider.BikeRiderId, clTeamRider.Year);
                _unitOfWork.ChampionsLeagueTeams.RemoveRiderChampionsLeagueTeam(clTeamRider.ChampionsLeagueTeamId, bikeRiderDetail.BikeRiderDetailId);
                _unitOfWork.Complete();
                UpdateCLTeamTotalCQ(clTeamRider.ChampionsLeagueTeamId);
            }
            catch (Exception ex)
            {
                bRemoveOk = false;
            }
            return bRemoveOk;
        }

        private int UpdateCLTeamTotalCQ(int clTeamId)
        {
            var clTeam = _unitOfWork.ChampionsLeagueTeams.GetById(clTeamId);
            int cqPoints = clTeam.ChampionsLeagueTeamBikeRiders.Sum(br => br.BikeRiderDetail != null ? br.BikeRiderDetail.CQPoints : 0);
            clTeam.TotalCQPoints = cqPoints;
            _unitOfWork.Complete();
            return cqPoints;
        }

        /* Lottery Teams */

        [JwtAuthentication]
        [HttpGet]
        public IList<VMLotteryTeam> GetLotteryTeams(int year)
        {
            IList<VMLotteryTeam> lstLotteryTeams = new List<VMLotteryTeam>();
            var lotteryTeams = _unitOfWork.LotteryTeams.Get(year);
            foreach (var lotteryTeam in lotteryTeams)
            {
                if (lotteryTeam.TotalCQPoints == 0)
                {
                    lotteryTeam.TotalCQPoints = lotteryTeam.LotteryTeamBikeRiders.Sum(cl => cl.BikeRiderDetail.CQPoints);
                    _unitOfWork.Complete();
                }
                lstLotteryTeams.Add(new VMLotteryTeam(lotteryTeam));
            }
            return lstLotteryTeams;
        }

        [JwtAuthentication]
        [HttpPost]
        public int InsertLotteryTeam(VMLotteryTeam lotteryTeam)
        {
            int competitionTeamId = -1;
            try
            {
                competitionTeamId = _unitOfWork.LotteryTeams.AddLotteryTeam(new LotteryTeam()
                {
                    Name = lotteryTeam.Name,
                    Color = lotteryTeam.Color,
                    Year = lotteryTeam.Year
                });
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
            }
            return competitionTeamId;
        }

        [JwtAuthentication]
        [HttpPut]
        public bool UpdateLotteryTeam(VMLotteryTeam lotteryTeam)
        {
            bool bUpdateOk = true;
            try
            {
                _unitOfWork.LotteryTeams.UpdateLotteryTeam(lotteryTeam.LotteryTeamId, lotteryTeam.Name, lotteryTeam.Color);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                bUpdateOk = false;
            }
            return bUpdateOk;
        }

        [JwtAuthentication]
        [HttpDelete]
        public bool RemoveLotteryTeam(int lotteryTeamId)
        {
            bool bRemoveOk = true;
            try
            {
                _unitOfWork.LotteryTeams.RemoveLotteryTeam(lotteryTeamId);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                bRemoveOk = false;
            }
            return bRemoveOk;
        }

        [HttpGet]
        public IList<VMBikeRider> GetRidersLotteryTeam(int lotteryTeamId)
        {
            return _unitOfWork.LotteryTeams.GetBikeRidersLotteryTeam(lotteryTeamId).Select(ct => new VMBikeRider()
            {
                BikeRiderDetailId = ct.BikeRiderDetailId,
                BikeRiderId = ct.BikeRiderDetail.BikeRiderId,
                BikeRiderName = ct.BikeRiderDetail.BikeRider.BikeRiderName,
                BikeTeamCode = ct.BikeRiderDetail.BikeTeamCode,
                BikeTeamName = ct.BikeRiderDetail.BikeTeamName,
                CQPoints = ct.BikeRiderDetail.CQPoints,
                Nationality = ct.BikeRiderDetail.BikeRider.Nationality,
                Year = ct.BikeRiderDetail.Year
            }).OrderByDescending(ct => ct.CQPoints).ToList();
        }

        [JwtAuthentication]
        [HttpPost]
        public int InsertBikeRiderLotteryTeam(VMLotteryTeamBikeRider lotteryTeamRider)
        {
            int competitionTeamId = -1;
            try
            {
                BikeRiderDetail bikeRiderDetail = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(lotteryTeamRider.BikeRiderId, lotteryTeamRider.Year);
                _unitOfWork.LotteryTeams.AddBikeRiderToLotteryTeam(lotteryTeamRider.LotteryTeamId, bikeRiderDetail.BikeRiderDetailId);
                _unitOfWork.Complete();
                UpdateLotteryTeamTotalCQ(lotteryTeamRider.LotteryTeamId);
            }
            catch (Exception ex)
            {
            }
            return competitionTeamId;
        }

        [JwtAuthentication]
        [HttpPut]
        public bool UpdateBikeRiderLotteryTeam(VMUpdateLotteryTeamBikeRider lotteryTeamRider)
        {
            bool bUpdateOk = true;
            try
            {
                BikeRiderDetail bikeRiderDetailOrig = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(lotteryTeamRider.OrigBikeRiderId, lotteryTeamRider.Year);
                BikeRiderDetail bikeRiderDetailNew = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(lotteryTeamRider.NewBikeRiderId, lotteryTeamRider.Year);
                _unitOfWork.LotteryTeams.UpdateRiderLotteryTeam(lotteryTeamRider.LotteryTeamId, bikeRiderDetailOrig.BikeRiderDetailId, bikeRiderDetailNew.BikeRiderDetailId);
                _unitOfWork.Complete();
                UpdateLotteryTeamTotalCQ(lotteryTeamRider.LotteryTeamId);
            }
            catch (Exception ex)
            {
                bUpdateOk = false;
            }
            return bUpdateOk;
        }

        [JwtAuthentication]
        [HttpPut]
        public bool RemoveBikeRiderLotteryTeam(VMLotteryTeamBikeRider lotteryTeamRider)
        {
            bool bRemoveOk = true;
            try
            {
                BikeRiderDetail bikeRiderDetail = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(lotteryTeamRider.BikeRiderId, lotteryTeamRider.Year);
                _unitOfWork.LotteryTeams.RemoveRiderLotteryTeam(lotteryTeamRider.LotteryTeamId, bikeRiderDetail.BikeRiderDetailId);
                _unitOfWork.Complete();
                UpdateLotteryTeamTotalCQ(lotteryTeamRider.LotteryTeamId);
            }
            catch (Exception ex)
            {
                bRemoveOk = false;
            }
            return bRemoveOk;
        }

        private int UpdateLotteryTeamTotalCQ(int lotteryTeamId)
        {
            var lotteryTeam = _unitOfWork.LotteryTeams.GetById(lotteryTeamId);
            int cqPoints = lotteryTeam.LotteryTeamBikeRiders.Sum(br => br.BikeRiderDetail != null ? br.BikeRiderDetail.CQPoints : 0);
            lotteryTeam.TotalCQPoints = cqPoints;
            _unitOfWork.Complete();
            return cqPoints;
        }
    }
}
