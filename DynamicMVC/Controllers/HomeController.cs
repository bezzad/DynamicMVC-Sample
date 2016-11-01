using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicMVC.Core;

namespace DynamicMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult DynamicView()
        {
            var model = new TableOption() { Id = "entryInWay" };
            #region Table Data

            var tableData = Connections.Localhost.SqlConn.ExecuteReader("Select * From TestTable").ToDataTable();

            model.Data = tableData;
            model.DisplayRowsLength = 5;

            #endregion
            

            return View(model);
        }
    }
}