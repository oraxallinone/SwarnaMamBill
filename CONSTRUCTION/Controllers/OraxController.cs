using CONSTRUCTION.Models;
using HotelBill.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;


namespace CONSTRUCTION.Controllers
{
    public class OraxController : Controller
    {
        Bill_DBEntities db = new Bill_DBEntities();
        #region ----------------------    -------------
        #endregion ------------------------------------

        #region ----------------------  entry  --------
        public ActionResult Entry()
        {
            return View();
        }

        public ActionResult Entry2()
        {
            return View();
        }

        public ActionResult Entry3()
        {
            return View();
        }

        public ActionResult FilterWithDetails()
        {
            return View();
        }


        #region ----------------- entry 3 ---------------------------------
        [HttpPost]
        public ActionResult GetAllExpensive2(GetExpensiveViewModel data)
        {
            IEnumerable<RenderMonthMasterViewModel2> LMonthMasters = Enumerable.Empty<RenderMonthMasterViewModel2>();
            IEnumerable<Entry3BudgetRender> LBudgetList = Enumerable.Empty<Entry3BudgetRender>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionstring"].ToString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_getExpensiveByAllCondition", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@month", data.forMonth);
                    cmd.Parameters.AddWithValue("@year", data.forYear);
                    cmd.Parameters.AddWithValue("@group1", data.group1);
                    cmd.Parameters.AddWithValue("@group2", data.group2);
                    cmd.Parameters.AddWithValue("@group3", data.group3);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    da.Fill(ds);
                    con.Close();

                    if (ds.Tables.Count > 0 && ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        LMonthMasters = ConvertDataTableToListMmaster(ds.Tables[0]);
                    }
                    if (ds.Tables.Count > 1 && ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        LBudgetList = ConvertDataTableToListBudget(ds.Tables[1]);
                    }
                }
            }
            var result = new { monthTransList = LBudgetList, monthMaster = LMonthMasters };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        private IEnumerable<Entry3BudgetRender> ConvertDataTableToListBudget(DataTable dataTable)
        {
            return dataTable.AsEnumerable().Select(row => new Entry3BudgetRender
            {
                id = row["id"]?.ToString(),
                group1 = row["group1"]?.ToString(),
                group2 = row["group2"]?.ToString(),
                group3 = row["group3"]?.ToString(),
                details = row["details"]?.ToString(),
                price = row["price"]?.ToString(),
                createdDate = row["createdDate"]?.ToString()
            }).ToList();
        }

        private IEnumerable<RenderMonthMasterViewModel2> ConvertDataTableToListMmaster(DataTable dataTable)
        {
            return dataTable.AsEnumerable().Select(row => new RenderMonthMasterViewModel2
            {
                id = row["id"]?.ToString(),
                month = row["month"]?.ToString(),
                monthName = row["monthName"]?.ToString(),
                montOrder = row["montOrder"]?.ToString(),
                year = row["year"]?.ToString(),
                salary = row["salary"]?.ToString(),
                fromDate = row["fromDate"]?.ToString(),
                todate = row["todate"]?.ToString(),
                salaryDate = row["salaryDate"]?.ToString(),
                need = row["need"]?.ToString(),
                want = row["want"]?.ToString(),
                saving = row["saving"]?.ToString()
            }).ToList();
        }

        #endregion --------------------------------------------------------

        #region --------------------- calender ---------------------------
        [HttpPost]
        public ActionResult GetDataForCalender(GetExpensiveViewModel data)
        {
            IEnumerable<CalenderDateViewModel> LCalenderDate = Enumerable.Empty<CalenderDateViewModel>();
            IEnumerable<CalenderDataViewModel> LCalenderData = Enumerable.Empty<CalenderDataViewModel>();


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionstring"].ToString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_getExpensiveByCalender", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@month", data.forMonth);
                    cmd.Parameters.AddWithValue("@year", data.forYear);
                    cmd.Parameters.AddWithValue("@group1", data.group1);
                    cmd.Parameters.AddWithValue("@group2", data.group2);
                    cmd.Parameters.AddWithValue("@group3", data.group3);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    da.Fill(ds);
                    con.Close();

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        LCalenderDate = ConvertDataTableToCalenderDate(ds.Tables[0]);
                    }

                    if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        LCalenderData = ConvertDataTableToListCalender(ds.Tables[1]);
                    }
                }
            }
            var result = new { calenderData = LCalenderData, calenderDate = LCalenderDate };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private IEnumerable<CalenderDateViewModel> ConvertDataTableToCalenderDate(DataTable dataTable)
        {
            return dataTable.AsEnumerable().Select(row => new CalenderDateViewModel
            {
                fromDate = row["fromDate"].ToString(),
                toDate = row["toDate"].ToString()
            }).ToList();
        }

        private IEnumerable<CalenderDataViewModel> ConvertDataTableToListCalender(DataTable dataTable)
        {
            return dataTable.AsEnumerable().Select(row => new CalenderDataViewModel
            {
                spending = Convert.ToDecimal(row["spending"]),
                date = Convert.ToDateTime(row["date"]).ToString("yyyy-MM-dd")
            }).ToList();
        }
        #endregion -------------------------------------------------------

        [HttpPost]
        public ActionResult GetAllExpensive(GetExpensiveViewModel data)
        {
            tblBudgetMaster budgetList = null;
            IEnumerable<tblBudget> allList = null;
            List<RenderExpensiveViewModel2> expensiveList = new List<RenderExpensiveViewModel2>();
            try
            {
                budgetList = db.tblBudgetMasters.Where(x => x.month == data.forMonth && x.year == data.forYear).FirstOrDefault();
                DateTime? fromDate = SatyaDBClass.ISTZoneNull(budgetList.fromDate);
                DateTime? toDate = SatyaDBClass.ISTZoneNull(budgetList.todate);
                if (fromDate.HasValue) { fromDate = fromDate.Value.AddDays(-1); }
                if (toDate.HasValue) { toDate = toDate.Value.AddDays(1); }

                if (data.group1 != null)
                {
                    allList = db.tblBudgets.Where(m => m.createdDate > fromDate && m.createdDate < toDate && m.group1 == data.group1).OrderBy(d => d.createdDate).ToList();
                }
                else if (data.group2 != null)
                {
                    allList = db.tblBudgets.Where(m => m.createdDate > fromDate && m.createdDate < toDate && m.group2 == data.group2).OrderBy(d => d.createdDate).ToList();
                }
                else if (data.group3 != null)
                {
                    allList = db.tblBudgets.Where(m => m.createdDate > fromDate && m.createdDate < toDate && m.group3 == data.group3).OrderBy(d => d.createdDate).ToList();
                }
                else
                {
                    //allList = db.tblBudgets.Where(m => m.createdDate > fromDate && m.createdDate < toDate).OrderBy(d => d.createdDate).ToList();

                    //allList = db.tblBudgets.Where(m => m.createdDate > fromDate && m.createdDate < toDate).OrderBy(d => d.createdDate).ToList();

                    allList = db.tblBudgets.Where(m => DbFunctions.TruncateTime(m.createdDate) >= fromDate && DbFunctions.TruncateTime(m.createdDate) <= toDate).OrderBy(d => d.createdDate).ToList();
                    //m => DbFunctions.TruncateTime(m.createdDate
                }


                foreach (var s in allList)
                {
                    RenderExpensiveViewModel2 item = new RenderExpensiveViewModel2();
                    item.id = s.id.ToString();
                    item.group1 = s.group1;
                    item.group2 = s.group2;
                    item.group3 = s.group3;
                    item.details = s.details;
                    item.price = s.price.ToString();
                    item.createdDate = Convert.ToDateTime(SatyaDBClass.ISTZoneNull(s.createdDate) ?? SatyaDBClass.ISTZoneNull(DateTime.Now)).ToString("yyyy-MM-dd");
                    expensiveList.Add(item);
                }
                var result = new { monthTransList = expensiveList, monthMaster = budgetList, toDate = budgetList.todate.ToString() };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                throw;
            }
            finally
            {
                budgetList = null;
                allList = null;
            }
        }

        [HttpPost]
        public ActionResult FilterWithDetails(SearchInputViewModel data)
        {
            IEnumerable<tblBudget> allList = null;
            List<RenderExpensiveViewModel2> expensiveList = new List<RenderExpensiveViewModel2>();
            try
            {
                allList = db.tblBudgets.Where(u => u.details.Contains(data.searchInput)).OrderBy(d => d.createdDate).ToList();

                foreach (var s in allList)
                {
                    RenderExpensiveViewModel2 item = new RenderExpensiveViewModel2();
                    item.id = s.id.ToString();
                    item.group1 = s.group1;
                    item.group2 = s.group2;
                    item.group3 = s.group3;
                    item.details = s.details;
                    item.price = s.price.ToString();
                    item.createdDate = Convert.ToDateTime(SatyaDBClass.ISTZoneNull(s.createdDate) ?? SatyaDBClass.ISTZoneNull(DateTime.Now)).ToString("yyyy-MM-dd");
                    expensiveList.Add(item);
                }
                var result = new { filterData = expensiveList };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                throw;
            }
            finally
            {
                allList = null;
                expensiveList = null;
            }
        }

        [HttpPost]
        public ActionResult GetAllExpensiveByGroup(GetExpensiveViewModel data)
        {

            IEnumerable<CalenderWiseGroup> calenderGroup = null;


            tblBudgetMaster budgetList = null;
            IEnumerable<tblBudget> allList = null;
            List<RenderExpensiveViewModel2> expensiveList = new List<RenderExpensiveViewModel2>();
            List<CalenderWiseGroupViewModel> calenderGroupList = new List<CalenderWiseGroupViewModel>();

            try
            {
                budgetList = db.tblBudgetMasters.Where(x => x.month == data.forMonth && x.year == data.forYear).FirstOrDefault();
                DateTime? fromDate = SatyaDBClass.ISTZoneNull(budgetList.fromDate);
                DateTime? toDate = SatyaDBClass.ISTZoneNull(budgetList.todate);


                if (toDate.HasValue) { toDate = toDate.Value.AddDays(1); }

                if (data.group1 != null)
                {
                    calenderGroup = db.tblBudgets.Where(m => m.createdDate > fromDate && m.createdDate < toDate && m.group1 == data.group1)
                    .OrderBy(d => d.createdDate)
                    .GroupBy(x => x.createdDate)
                    .Select(g => new CalenderWiseGroup { date = g.Key, ammount = g.Sum(x => x.price), }).ToList();
                }
                else if (data.group2 != null)
                {
                    calenderGroup = db.tblBudgets.Where(m => m.createdDate > fromDate && m.createdDate < toDate && m.group2 == data.group2)
                    .OrderBy(d => d.createdDate)
                    .GroupBy(x => x.createdDate)
                    .Select(g => new CalenderWiseGroup { date = g.Key, ammount = g.Sum(x => x.price), }).ToList();
                }
                else if (data.group3 != null)
                {
                    calenderGroup = db.tblBudgets.Where(m => m.createdDate > fromDate && m.createdDate < toDate && m.group3 == data.group3)
                   .OrderBy(d => d.createdDate)
                   .GroupBy(x => x.createdDate)
                   .Select(g => new CalenderWiseGroup { date = g.Key, ammount = g.Sum(x => x.price), }).ToList();
                }
                else
                {
                    calenderGroup = db.tblBudgets.Where(m => m.createdDate > fromDate && m.createdDate < toDate)
                   .OrderBy(d => d.createdDate)
                   .GroupBy(x => x.createdDate)
                   .Select(g => new CalenderWiseGroup { date = g.Key, ammount = g.Sum(x => x.price), }).ToList();
                }

                foreach (var s in calenderGroup)
                {
                    CalenderWiseGroupViewModel item = new CalenderWiseGroupViewModel();
                    item.ammount = s.ammount.ToString();
                    item.date = Convert.ToDateTime(SatyaDBClass.ISTZoneNull(s.date) ?? SatyaDBClass.ISTZoneNull(DateTime.Now)).ToString("yyyy-MM-dd");
                    calenderGroupList.Add(item);
                }


                var result = new { monthTransList = calenderGroupList, monthMaster = budgetList, toDate = budgetList.todate.ToString() };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                throw;
            }
            finally
            {
                budgetList = null;
                allList = null;
            }
        }

        public ActionResult MobileUpdate()
        {
            return View();
        }

        #endregion ------------------------------------

        #region ----------------------  graph  ---------
        public ActionResult Graph()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAsBudgetRule(GetExpensiveViewModel data)
        {
            tblBudgetMaster budgetMaster = null;
            try
            {
                budgetMaster = db.tblBudgetMasters.Where(x => x.month == data.forMonth && x.year == data.forYear).FirstOrDefault();
                var fromDate = SatyaDBClass.ISTZoneNull(budgetMaster.fromDate);
                var toDate = SatyaDBClass.ISTZoneNull(budgetMaster.todate);
                var monthMaster = db.tblBudgetMasters.Where(m => m.month == data.forMonth).FirstOrDefault();
                var groupByBudgetRule = db.tblBudgets.Where(m => m.createdDate > fromDate && m.createdDate < toDate).GroupBy(b => b.group1)
                                        .Select(g => new
                                        {
                                            label = g.Key,
                                            y = g.Sum(b => b.price)
                                        })
                                        .ToList();
                var result = new { monthMaster = monthMaster, groupByBudgetRule = groupByBudgetRule };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                budgetMaster = null;
            }
        }

        [HttpPost]
        public ActionResult GetAllDataBudget(GetExpensiveViewModel data)
        {
            tblBudgetMaster budgetMaster = null;
            try
            {
                budgetMaster = db.tblBudgetMasters.Where(x => x.month == data.forMonth && x.year == data.forYear).FirstOrDefault();
                var fromDate = SatyaDBClass.ISTZoneNull(budgetMaster.fromDate);
                var toDate = SatyaDBClass.ISTZoneNull(budgetMaster.todate);
                var groupByAmt = db.tblBudgets.Where(m => m.createdDate >= fromDate && m.createdDate <= toDate).GroupBy(b => b.group2)
                                        .Select(g => new
                                        {
                                            label = g.Key,
                                            y = g.Sum(b => b.price)
                                        })
                                        .ToList();
                return Json(groupByAmt, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                budgetMaster = null;
            }
        }

        public ActionResult GraphColumn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetColumnChart(GetExpensiveViewModel data)
        {
            IEnumerable<tblBudgetMaster> budgetMasters = null;
            List<BarGraph> barGraphData = new List<BarGraph>();
            IEnumerable<BarGraphdbArrange> datas = null;
            try
            {
                budgetMasters = db.tblBudgetMasters.Where(v => v.month != "all").ToList().OrderBy(m => m.todate);
                var index = 0;

                foreach (var ss in budgetMasters)
                {
                    index = index + 1;

                    if (data.group1 != null)
                    {
                        datas = db.tblBudgets
                        .Where(b => b.createdDate > ss.fromDate && b.createdDate < ss.todate && b.group1 == data.group1)
                        .GroupBy(c => c.group1)
                        .Select(g => new BarGraphdbArrange { y = g.Sum(c => c.price), label = ss.month })
                        .ToList();
                    }
                    if (data.group2 != null)
                    {
                        datas = db.tblBudgets
                        .Where(b => b.createdDate > ss.fromDate && b.createdDate < ss.todate && b.group2 == data.group2)
                        .GroupBy(c => c.group2)
                        .Select(g => new BarGraphdbArrange { y = g.Sum(c => c.price), label = ss.month })
                        .ToList();
                    }
                    if (data.group3 != null)
                    {
                        datas = db.tblBudgets
                        .Where(b => b.createdDate > ss.fromDate && b.createdDate < ss.todate && b.group3 == data.group3)
                        .GroupBy(c => c.group3)
                        .Select(g => new BarGraphdbArrange { y = g.Sum(c => (c.price == null ? 0m : c.price)), label = ss.month })
                        .ToList();
                    }

                    if (datas.Count() == 0)
                    {
                        BarGraph dat = new BarGraph();
                        dat.x = index;
                        dat.y = 0;
                        dat.label = ss.monthName + "-" + "0";
                        barGraphData.Add(dat);
                    }
                    else
                    {
                        int? intValue1 = datas.FirstOrDefault().y.HasValue ? (int?)Convert.ToInt32(datas.FirstOrDefault().y.Value) : null;
                        int intValue2 = 0;
                        if (intValue1 != null)
                        {
                            intValue2 = intValue1 ?? 0;
                        }

                        BarGraph dat = new BarGraph();
                        dat.x = index;
                        dat.y = intValue2;
                        dat.label = MonthName(Convert.ToInt32(datas.FirstOrDefault().label)) + "-" + ((int)datas.FirstOrDefault().y).ToString();
                        barGraphData.Add(dat);
                    }
                }
                var result = new { monthMstr = budgetMasters, barGraphData = barGraphData };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                budgetMasters = null;
                barGraphData = null;
                datas = null;
            }
        }


        [HttpPost]
        public ActionResult GetGraphData(GetExpensiveViewModel data)
        {
            IEnumerable<GraphDataViewModel> LGraphData = Enumerable.Empty<GraphDataViewModel>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionstring"].ToString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_getExpensiveForGraph", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@group1", data.group1);
                    cmd.Parameters.AddWithValue("@group2", data.group2);
                    cmd.Parameters.AddWithValue("@group3", data.group3);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    da.Fill(ds);
                    con.Close();

                    if (ds.Tables.Count > 0 && ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        LGraphData = ConvertDataTableToListGraph(ds.Tables[0]);
                    }
                }
            }
            var result = new { data = LGraphData };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private IEnumerable<GraphDataViewModel> ConvertDataTableToListGraph(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                yield return new GraphDataViewModel
                {
                    price = Convert.ToString(row["price"]),
                    month = Convert.ToString(row["month"]),
                    year = Convert.ToString(row["year"]),
                };
            }
        }




        [HttpPost]
        public ActionResult GetGraph_N_Expensive()
        {
            IEnumerable<GraphSalary_N_ExpensiveViewModel> LGraphSalary_N_Expensive = Enumerable.Empty<GraphSalary_N_ExpensiveViewModel>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionstring"].ToString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_salaryNexpensiveGraphData", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    da.Fill(ds);
                    con.Close();

                    if (ds.Tables.Count > 0 && ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        LGraphSalary_N_Expensive = ConvertDataTableToListSalary_N_Expensive(ds.Tables[0]);
                    }
                }
            }
            var result = new { data = LGraphSalary_N_Expensive };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private IEnumerable<GraphSalary_N_ExpensiveViewModel> ConvertDataTableToListSalary_N_Expensive(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                yield return new GraphSalary_N_ExpensiveViewModel
                {
                    salary = Convert.ToString(row["salary"]),
                    price = Convert.ToString(row["price"]),
                    remaining = Convert.ToString(row["remaining"]),
                    month = Convert.ToString(row["month"]),
                    year = Convert.ToString(row["year"]),
                };
            }
        }



        public ActionResult MobileGraph()
        {
            return View();
        }

        public ActionResult BarGraph()
        {
            return View();
        }

        public ActionResult GraphSummary()
        {
            return View();
        }

        public ActionResult Salary_N_Expensive()
        {
            return View();
        }


        public ActionResult CalenderWise()
        {
            return View();
        }
        #endregion ------------------------------------

        #region ----------------------  common  --------
        public string MonthName(int i)
        {
            string monthName = "";
            switch (i)
            {
                case 1:
                    monthName = "Jan";
                    break;
                case 2:
                    monthName = "Feb";
                    break;
                case 3:
                    monthName = "Mar";
                    break;
                case 4:
                    monthName = "Apr";
                    break;
                case 5:
                    monthName = "May";
                    break;
                case 6:
                    monthName = "Jun";
                    break;
                case 7:
                    monthName = "Jul";
                    break;
                case 8:
                    monthName = "Aug";
                    break;
                case 9:
                    monthName = "Sep";
                    break;
                case 10:
                    monthName = "Oct";
                    break;
                case 11:
                    monthName = "Nov";
                    break;
                case 12:
                    monthName = "Dec";
                    break;
            }
            return monthName;
        }
        #endregion ------------------------------------

        #region ---------------- common 2   -------------
        [HttpPost]
        public ActionResult AddExpensive(CreateExpensiveViewModel data)
        {
            try
            {
                string dateNow = SatyaDBClass.ISTZoneNull(data.createdDate)?.ToString("yyyy-MM-dd hh:mm:ss");
                string proccc = data.price.ToString();
                string param = "@group2,@createdDate,@details,@price,@group1";
                string paramvalue = data.group2 + "," + dateNow + "," + data.details + "," + proccc + "," + "n";
                SatyaDBClass.insertprocedure("sp_mobileInsert", param, paramvalue);
            }
            catch (Exception ex)
            {
                string sssss = ex.Message;
                throw;
            }
            return Json("success");
        }

        [HttpPost]
        public ActionResult GetExpensive(CreateExpensiveViewModel data)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            var selectedData = db.tblBudgets.FirstOrDefault(x => x.id == data.id);
            tblBudget_MV newData = new tblBudget_MV();
            newData.id = selectedData.id;
            newData.createdDate = selectedData.createdDate == null ? "" : Convert.ToDateTime(SatyaDBClass.ISTZoneNull(selectedData.createdDate)).ToString("yyyy-MM-dd");//selectedData.createdDate;
            newData.price = selectedData.price;
            newData.details = selectedData.details;
            return Json(newData);
        }

        [HttpPost]
        public ActionResult GetModelExpensive(CreateExpensiveViewModel data)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            var selectedData = db.tblBudgets.FirstOrDefault(x => x.id == data.id);
            tblBudget_all_MV newData = new tblBudget_all_MV();
            newData.id = selectedData.id;
            newData.createdDate = selectedData.createdDate?.ToString("yyyy-MM-dd");
            newData.price = selectedData.price;
            newData.details = selectedData.details;
            newData.group1 = selectedData.group1;
            newData.group2 = selectedData.group2;
            newData.group3 = selectedData.group3;
            return Json(newData);
        }


        [HttpPost]
        public ActionResult UpdateExpensive(CreateExpensiveViewModel data)
        {
            try
            {
                string dateNow = SatyaDBClass.ISTZoneNull(data.createdDate)?.ToString("yyyy-MM-dd hh:mm:ss");
                string proccc = data.price.ToString();
                string param = "@id,@details,@price";
                string paramvalue = data.id + "," + data.details + "," + data.price;
                SatyaDBClass.insertprocedure("sp_mobileUpdate", param, paramvalue);
            }
            catch (Exception ex)
            {
                string sssss = ex.Message;
                throw;
            }
            return Json("success");
        }

        [HttpPost]
        public ActionResult UpdateBySearch(CreateExpensiveViewModel data)
        {
            try
            {
                string param = "@id,@details";
                string paramvalue = data.id + "," + data.details;
                SatyaDBClass.insertprocedure("sp_updateOnlyDetails", param, paramvalue);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return Json("success");
        }

        [HttpPost]
        public ActionResult UpdateGroupAll(CreateExpensiveViewModel data)
        {
            tblBudget obj = db.tblBudgets.FirstOrDefault(xy => xy.id == data.id);
            if (data.group1 != null) { obj.group1 = data.group1; }
            if (data.group2 != null) { obj.group2 = data.group2Text; obj.group4 = data.group2; }
            if (data.group3 != null) { if (data.group3 == "--") { obj.group3 = null; } else { obj.group3 = data.group3; } }
            db.SaveChanges();
            return Json("success");
        }

        [HttpPost]
        public ActionResult UpdateGroup1(CreateExpensiveViewModel data)
        {
            tblBudget obj = db.tblBudgets.FirstOrDefault(xy => xy.id == data.id);
            obj.group1 = data.group1;
            db.SaveChanges();
            return Json("success");
        }

        [HttpPost]
        public ActionResult UpdateGroup2(CreateExpensiveViewModel data)
        {
            tblBudget obj = db.tblBudgets.FirstOrDefault(xy => xy.id == data.id);
            obj.group2 = data.group2;
            db.SaveChanges();
            return Json("success");
        }

        [HttpPost]
        public ActionResult UpdateGroup3(CreateExpensiveViewModel data)
        {
            tblBudget obj = db.tblBudgets.FirstOrDefault(xy => xy.id == data.id);
            obj.group3 = data.group3;
            db.SaveChanges();
            return Json("success");
        }

        [HttpPost]
        public ActionResult removeRecord(CreateExpensiveViewModel data)
        {
            tblBudget obj = db.tblBudgets.FirstOrDefault(xy => xy.id == data.id);
            db.tblBudgets.Remove(obj);
            db.SaveChanges();
            return Json("success");
        }
        #endregion ------------------------------------

        #region -----------  salary verified ---------
        public ActionResult SalaryEntry()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SalaryEntry(tblBudgetMaster data)
        {
            try
            {
                tblBudgetMaster obj = new tblBudgetMaster();
                obj.month = data.month;
                obj.salary = data.salary;
                obj.fromDate = SatyaDBClass.ISTZoneNull(data.fromDate);
                obj.todate = SatyaDBClass.ISTZoneNull(data.todate);
                obj.need = data.need;
                obj.want = data.want;
                obj.saving = data.saving;
                obj.monthName = data.monthName;
                obj.montOrder = data.montOrder;
                obj.year = data.year;
                obj.salaryDate = SatyaDBClass.ISTZoneNull(data.salaryDate);
                db.tblBudgetMasters.Add(obj);
                db.SaveChanges();
                calculateAllMonth();
            }
            catch (Exception)
            {
                ViewBag.message = "fail";
                throw;
            }
            return RedirectToAction("SalaryList");
        }

        public ActionResult SalaryList()
        {
            return View(db.tblBudgetMasters.ToList().OrderByDescending(x => x.fromDate));
        }

        public ActionResult SalaryEdit(int? id)
        {
            return View(db.tblBudgetMasters.FirstOrDefault(x => x.id == id));
        }

        [HttpPost]
        public ActionResult SalaryEdit(tblBudgetMaster data)
        {
            tblBudgetMaster obj = db.tblBudgetMasters.FirstOrDefault(xy => xy.id == data.id);
            obj.month = data.month;
            obj.salary = data.salary;
            obj.salaryDate = SatyaDBClass.ISTZoneNull(data.salaryDate);
            obj.fromDate = SatyaDBClass.ISTZoneNull(data.fromDate);
            obj.todate = SatyaDBClass.ISTZoneNull(data.todate);
            obj.need = data.need;
            obj.want = data.want;
            obj.saving = data.saving;
            db.SaveChanges();
            calculateAllMonth();
            return RedirectToAction("SalaryList");
        }

        private void calculateAllMonth()
        {
            var monthTotalIncome = db.tblBudgetMasters.Where(x => x.month != "all").Sum(x => x.salary);

            tblBudgetMaster obj = db.tblBudgetMasters.FirstOrDefault(xy => xy.month == "all");
            obj.salary = monthTotalIncome;
            obj.need = monthTotalIncome * 50 / 100;
            obj.want = monthTotalIncome * 30 / 100;
            obj.saving = monthTotalIncome * 20 / 100;
            db.SaveChanges();
        }
        #endregion ----------------------------------------------------------

        #region ----------------------  Mobile-----------
        public ActionResult MobileEntry()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetBymonthMobile(GetExpensiveViewModel data)
        {
            string param = "@forYear,@forMonth";
            string paramvalue = data.forYear.ToString() + "," + data.forMonth.ToString();
            DataTable dTable = SatyaDBClass.SPReturnDataTable("sp_monthlyList", param, paramvalue);
            IEnumerable<RenderExpensiveViewModel2> dList = ConvertDataTableToList(dTable);
            var result = new { monthTransList = dList.ToList() };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<RenderExpensiveViewModel2> ConvertDataTableToList(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                yield return new RenderExpensiveViewModel2
                {
                    id = Convert.ToString(row["id"]),
                    details = Convert.ToString(row["details"]),
                    price = Convert.ToString(row["price"]),
                    createdDate = Convert.ToDateTime(row["createdDate"] ?? DateTime.Now).ToString("yyyy-MM-dd")
                };
            }
        }





        #endregion ------------------------------------

        #region ---------------------- Group Master verified ----
        public ActionResult GroupList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ListGroup()
        {
            return Json(db.tblGroupMasters.ToList().OrderBy(x => x.groupName));
        }

        [HttpPost]
        public ActionResult GroupSave(GroupViewModel data)
        {
            if (Convert.ToInt32(data.hidelId) == 0 && data.btnText == "Save")
            {
                int dbCount = db.CounterMasters.Where(x => x.counterName == "group2id").FirstOrDefault().counterValue;
                int newNo = BillSupport.increamentReferenceNO(dbCount);

                tblGroupMaster obj = new tblGroupMaster();
                obj.groupName = data.groupName;
                obj.haveFixPrice = data.haveFixPrice;
                obj.priceGroup = data.priceGroup;
                obj.IsShow = data.IsShow;
                obj.group2int = newNo;
                db.tblGroupMasters.Add(obj);
                db.SaveChanges();

                CounterMaster cnt = db.CounterMasters.Where(x => x.counterName == "group2id").FirstOrDefault();
                cnt.counterValue = newNo;
                db.SaveChanges();
                return Json("save");
            }
            else if (data.btnText == "Update?")
            {
                tblGroupMaster obj = db.tblGroupMasters.FirstOrDefault(x => x.id == data.hidelId);
                obj.groupName = data.groupName;
                obj.haveFixPrice = data.haveFixPrice;
                obj.priceGroup = data.priceGroup;
                obj.IsShow = data.IsShow;
                db.SaveChanges();
                return Json("update");
            }
            else
            {
                return Json("not");
            }

        }


        [HttpPost]
        public ActionResult GroupGet(GroupViewModel data)
        {
            tblGroupMaster obj = db.tblGroupMasters.FirstOrDefault(x => x.id == data.hidelId);
            return Json(obj);
        }

        [HttpPost]
        public ActionResult GetDDLGroup2()
        {
            IEnumerable<tblGroupMaster> obj = db.tblGroupMasters.Where(x => x.IsShow == true).ToList().OrderBy(c => c.groupName);
            return Json(obj);
        }

        [HttpPost]
        public ActionResult GetDDLGroup2All()
        {
            IEnumerable<tblGroupMaster> obj = db.tblGroupMasters.ToList().OrderBy(c => c.groupName);
            return Json(obj);
        }

        [HttpPost]
        public ActionResult GetDDLGroup2OnlyActive()
        {
            IEnumerable<tblGroupMaster> obj = db.tblGroupMasters.Where(x => x.IsShow == true).ToList().OrderBy(c => c.groupName);
            return Json(obj);
        }
        #endregion ------------------------------------


        #region -----------------------TaskTracker CRUD Operations
        public ActionResult TaskTracker()
        {

            return View();
        }


        [HttpGet]
        public JsonResult GetTasks()
        {
            var res = db.tblTaskTrackers.ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddTask(tblTaskTracker data)
        {
            try
            {
                tblTaskTracker task = new tblTaskTracker
                {
                    TaskDetails = data.TaskDetails,
                    DueDate = DateTime.Now,
                };

                db.tblTaskTrackers.Add(task);
                db.SaveChanges();
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult UpdateTask(tblTaskTracker data)
        {
            try
            {
                var task = db.tblTaskTrackers.FirstOrDefault(x => x.id == data.id);
                if (task != null)
                {
                    task.TaskDetails = data.TaskDetails;
                    task.DueDate = DateTime.Now;
                    db.SaveChanges();
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                return Json("task not found", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult DeleteTask(int id)
        {
            try
            {
                var task = db.tblTaskTrackers.FirstOrDefault(x => x.id == id);
                if (task != null)
                {
                    db.tblTaskTrackers.Remove(task);
                    db.SaveChanges();
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                return Json("task not found", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion


        #region ---------------------- repeat calculation --------------
        public ActionResult RepatEstimate()
        {
            var groupList = db.tblBudgets
               .GroupBy(x => new { x.group3, x.group2 })
               .Select(g => new
               {
                   Group3 = g.Key.group3,
                   Group2 = g.Key.group2
               })
               .ToList();
            ViewBag.GroupList = groupList;
            return View();
        }


        #endregion

        public ActionResult TestTask()
        {

            return View();
        }
        public ActionResult TestTask2()
        {

            return View();
        }



        #region -------------- autocomplete---------------
        private static readonly List<Product> Products = new List<Product>
    {
        new Product { Id = 1, Name = "Laptop", Price = 1200 },
        new Product { Id = 2, Name = "Mobile", Price = 700 },
        new Product { Id = 3, Name = "Tablet", Price = 450 },
        new Product { Id = 4, Name = "Monitor", Price = 300 },
        new Product { Id = 5, Name = "Keyboard", Price = 50 },
        new Product { Id = 7, Name = "Keyboard1", Price = 50 },
        new Product { Id = 8, Name = "Keyboard2", Price = 50 }
    };
        [HttpGet]
        public JsonResult SearchProducts(string term)
        {
            var result = Products
                .Where(p => p.Name.ToLower().Contains(term.ToLower()))
                .Select(p => new
                {
                    id = p.Id,
                    label = p.Name,   // label is required for jQuery UI autocomplete
                    value = p.Name,
                    price = p.Price
                })
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoTest()
        {

            return View();
        }
        #endregion ---------------------------------------

    }


    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
