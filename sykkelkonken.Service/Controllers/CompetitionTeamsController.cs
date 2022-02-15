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
            List<VMViewCompetitionTeam> lstCompTeams = _unitOfWork.CompetitionTeams.GetCompetitionTeamsFromView(year).ToList();

            List<VMCompetitionTeam> lstCompetitionTeamsToReturn = new List<VMCompetitionTeam>();
            foreach (var compTeam in lstCompTeams.GroupBy(ct => ct.CompetitionTeamId))
            {
                VMCompetitionTeam vmCompetitionTeam = new VMCompetitionTeam()
                {
                    CompetitionTeamId = compTeam.Key,
                    TeamName = compTeam.Select(ct => ct.Name).FirstOrDefault(),
                    TotalCQPoints = compTeam.Sum(ct => ct.CQPoints),
                };
                foreach (var bikeRider in compTeam)
                {
                    vmCompetitionTeam.BikeRiders.Add(new VMBikeRider()
                    {
                        BikeRiderId = bikeRider.BikeRiderId,
                        BikeRiderDetailId = bikeRider.BikeRiderDetailId,
                        BikeRiderName = bikeRider.BikeRiderName,
                        BikeTeamCode = bikeRider.BikeTeamCode,
                        Nationality = bikeRider.Nationality,
                        CQPoints = bikeRider.CQPoints,
                        Year = bikeRider.Year,
                    });
                }
                lstCompetitionTeamsToReturn.Add(vmCompetitionTeam);
            }

            return lstCompetitionTeamsToReturn;
        }

        [JwtAuthentication]
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage ImportCompetitionTeams(int year)//forventer rytterne avkrysset for hvert lag i excel
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
                                //int competitionTeamId = this._unitOfWork.CompetitionTeams.AddCompetitionTeam(competitionTeam);
                                //if (competitionTeamId > 0)
                                //{
                                //    competitionTeam.CompetitionTeamId = competitionTeamId;

                                //}
                            }
                            foreach (var competitionTeam in competitionTeams)
                            {
                                foreach (var bikeRider in competitionTeam.CompetitionTeamBikeRiders)
                                {
                                    //this._unitOfWork.CompetitionTeams.AddBikeRiderToCompetitionTeam(competitionTeam.CompetitionTeamId, bikeRider.BikeRiderId);
                                }
                            }
                            //_unitOfWork.Complete();
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
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage ImportCompetitionTeamsList(int year)//forventer lagene nedover i excel-liste
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

                            int startRow = 1;
                            int iTeamIdx = 0;
                            int iRowIdx = 1;
                            CompetitionTeam compTeam = new CompetitionTeam();
                            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                            {
                                if (ws.Cells[rowNum, 1].Text.Trim().Length == 0)
                                {
                                    continue;
                                }
                                var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                                bool isTeamName = false;
                                string col2 = ws.Cells[rowNum, 2].Text;
                                if (col2.Trim().Length == 0)
                                {
                                    isTeamName = true;
                                }
                                else
                                {
                                    int sum = 0;
                                    Int32.TryParse(col2.Trim(), out sum);
                                    if (sum < 30)
                                    {
                                        isTeamName = true;
                                    }
                                }
                                if (isTeamName)
                                {
                                    iTeamIdx++;
                                    string teamName = ws.Cells[rowNum, 1].Text.Substring(ws.Cells[rowNum, 1].Text.IndexOf(":") + 1).Trim();
                                    compTeam = new CompetitionTeam()
                                    {
                                        CompetitionTeamId = iTeamIdx,
                                        Name = teamName,
                                        Year = year,
                                        CompetitionTeamBikeRiders = new List<CompetitionTeamBikeRider>(),
                                    };
                                    competitionTeams.Add(compTeam);
                                }
                                else
                                {
                                    string bikeRiderName = ws.Cells[rowNum, 1].Text;
                                    if (bikeRiderName.Trim().Length == 0)
                                    {
                                        continue;
                                    }
                                    Data.BikeRiderDetail bikeRider = _unitOfWork.BikeRiders.GetBikeRiderDetailByName(bikeRiderName, year);
                                    if (bikeRider != null)
                                    {
                                        compTeam.CompetitionTeamBikeRiders.Add(new CompetitionTeamBikeRider()
                                        {
                                            CompetitionTeamId = compTeam.CompetitionTeamId,
                                            BikeRider = bikeRider.BikeRider,
                                            BikeRiderId = bikeRider.BikeRiderId,
                                        });
                                    }
                                    else
                                    {
                                        string mainLastName = bikeRiderName.Substring(0, bikeRiderName.IndexOf(" "));//finner hovedetternavn. antar at det er første navn før mellomrom. hva skal gjøres med nederlandske navn?
                                        Data.BikeRiderDetail bikeRiderLike = _unitOfWork.BikeRiders.GetBikeRiderDetailByMainLastName(mainLastName, year);
                                        if (bikeRiderLike != null)
                                        {
                                            compTeam.CompetitionTeamBikeRiders.Add(new CompetitionTeamBikeRider()
                                            {
                                                CompetitionTeamId = compTeam.CompetitionTeamId,
                                                BikeRider = bikeRiderLike.BikeRider,
                                                BikeRiderId = bikeRiderLike.BikeRiderId,
                                            });
                                        }
                                    }
                                }

                                iRowIdx++;
                            }

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

                            var youthLimitYear = year - 25;
                            DateTime dtYouthLimit = new DateTime(youthLimitYear, 1, 1);
                            foreach (var competitionTeam in competitionTeams)
                            {
                                int youthTeamId = -1;
                                bool bIsYouthTeam = true;
                                foreach (var bikeRider in competitionTeam.CompetitionTeamBikeRiders)
                                {
                                    if (bIsYouthTeam)
                                    {
                                        var br = _unitOfWork.BikeRiders.Get(bikeRider.BikeRiderId);
                                        if (br != null)
                                        {
                                            if (br.BirthDate < dtYouthLimit)
                                            {
                                                bIsYouthTeam = false;
                                            }
                                        }
                                    }
                                }
                                if (bIsYouthTeam)
                                {
                                    youthTeamId = _unitOfWork.YouthTeams.AddYouthTeam(new YouthTeam()
                                    {
                                        Name = competitionTeam.Name,
                                        Year = competitionTeam.Year
                                    });
                                }
                                foreach (var bikeRider in competitionTeam.CompetitionTeamBikeRiders)
                                {
                                    this._unitOfWork.CompetitionTeams.AddBikeRiderToCompetitionTeam(competitionTeam.CompetitionTeamId, bikeRider.BikeRiderId);
                                    if (bIsYouthTeam)
                                    {
                                        BikeRiderDetail bikeRiderDetail = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(bikeRider.BikeRiderId, year);
                                        if (bikeRiderDetail != null)
                                        {
                                            _unitOfWork.YouthTeams.AddBikeRiderToYouthTeam(youthTeamId, bikeRiderDetail.BikeRiderDetailId);
                                            _unitOfWork.Complete();
                                            UpdateYouthTeamTotalCQ(youthTeamId);
                                        }
                                    }
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


        /* Youth Teams */

        [JwtAuthentication]
        [HttpGet]
        public IList<VMYouthTeam> GetYouthTeams(int year)
        {
            IList<VMYouthTeam> lstYouthTeams = new List<VMYouthTeam>();
            var youthTeams = _unitOfWork.YouthTeams.Get(year);
            foreach (var youthTeam in youthTeams)
            {
                if (youthTeam.TotalCQPoints == 0)
                {
                    youthTeam.TotalCQPoints = youthTeam.YouthTeamBikeRiders.Sum(cl => cl.BikeRiderDetail.CQPoints);
                    _unitOfWork.Complete();
                }
                lstYouthTeams.Add(new VMYouthTeam(youthTeam));
            }
            return lstYouthTeams;
        }

        [JwtAuthentication]
        [HttpPost]
        public int InsertYouthTeam(VMYouthTeam youthTeam)
        {
            int competitionTeamId = -1;
            try
            {
                competitionTeamId = _unitOfWork.YouthTeams.AddYouthTeam(new YouthTeam()
                {
                    Name = youthTeam.Name,
                    Year = youthTeam.Year
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
        public bool UpdateYouthTeam(VMYouthTeam youthTeam)
        {
            bool bUpdateOk = true;
            try
            {
                _unitOfWork.YouthTeams.UpdateYouthTeam(youthTeam.YouthTeamId, youthTeam.Name);
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
        public bool RemoveYouthTeam(int youthTeamId)
        {
            bool bRemoveOk = true;
            try
            {
                _unitOfWork.YouthTeams.RemoveYouthTeam(youthTeamId);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                bRemoveOk = false;
            }
            return bRemoveOk;
        }

        [HttpGet]
        public IList<VMBikeRider> GetRidersYouthTeam(int youthTeamId)
        {
            return _unitOfWork.YouthTeams.GetBikeRidersYouthTeam(youthTeamId).Select(ct => new VMBikeRider()
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
        public int InsertBikeRiderYouthTeam(VMYouthTeamBikeRider youthTeamRider)
        {
            int competitionTeamId = -1;
            try
            {
                BikeRiderDetail bikeRiderDetail = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(youthTeamRider.BikeRiderId, youthTeamRider.Year);
                if (bikeRiderDetail != null)
                {
                    _unitOfWork.YouthTeams.AddBikeRiderToYouthTeam(youthTeamRider.YouthTeamId, bikeRiderDetail.BikeRiderDetailId);
                    _unitOfWork.Complete();
                    UpdateYouthTeamTotalCQ(youthTeamRider.YouthTeamId);
                }
            }
            catch (Exception ex)
            {
            }
            return competitionTeamId;
        }

        [JwtAuthentication]
        [HttpPut]
        public bool UpdateBikeRiderYouthTeam(VMUpdateYouthTeamBikeRider youthTeamRider)
        {
            bool bUpdateOk = true;
            try
            {
                BikeRiderDetail bikeRiderDetailOrig = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(youthTeamRider.OrigBikeRiderId, youthTeamRider.Year);
                BikeRiderDetail bikeRiderDetailNew = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(youthTeamRider.NewBikeRiderId, youthTeamRider.Year);
                _unitOfWork.YouthTeams.UpdateRiderYouthTeam(youthTeamRider.YouthTeamId, bikeRiderDetailOrig.BikeRiderDetailId, bikeRiderDetailNew.BikeRiderDetailId);
                _unitOfWork.Complete();
                UpdateYouthTeamTotalCQ(youthTeamRider.YouthTeamId);
            }
            catch (Exception ex)
            {
                bUpdateOk = false;
            }
            return bUpdateOk;
        }

        [JwtAuthentication]
        [HttpPut]
        public bool RemoveBikeRiderYouthTeam(VMYouthTeamBikeRider youthTeamRider)
        {
            bool bRemoveOk = true;
            try
            {
                BikeRiderDetail bikeRiderDetail = _unitOfWork.BikeRiders.GetBikeRiderDetailByYear(youthTeamRider.BikeRiderId, youthTeamRider.Year);
                _unitOfWork.YouthTeams.RemoveRiderYouthTeam(youthTeamRider.YouthTeamId, bikeRiderDetail.BikeRiderDetailId);
                _unitOfWork.Complete();
                UpdateYouthTeamTotalCQ(youthTeamRider.YouthTeamId);
            }
            catch (Exception ex)
            {
                bRemoveOk = false;
            }
            return bRemoveOk;
        }

        private int UpdateYouthTeamTotalCQ(int youthTeamId)
        {
            var youthTeam = _unitOfWork.YouthTeams.GetById(youthTeamId);
            int cqPoints = youthTeam.YouthTeamBikeRiders.Sum(br => br.BikeRiderDetail != null ? br.BikeRiderDetail.CQPoints : 0);
            youthTeam.TotalCQPoints = cqPoints;
            _unitOfWork.Complete();
            return cqPoints;
        }
    }
}
