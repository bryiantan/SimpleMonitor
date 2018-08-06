using SimpleMonitor.Business.Entity;
using SimpleMonitor.Business.UnitOfWork;
using SimpleMonitor.Web.Helper.JqGrid;
using SimpleMonitor.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleMonitor.Web.Controllers
{
    public class HomeController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            return View();
        }

        //   [Helper.Security.AttributeHelpers.ValidateAjaxAntiForgeryToken]
        public JsonResult GetLogView(JQGridViewModel jqGridViewModel)
        {
            JQGridHelper helper = new JQGridHelper();

            var model = unitOfWork.IISLogRepository.GetMany()
                .Select(r => new IisLogViewModel
                {
                    cIp = r.cIp,
                    csCookie = r.csCookie,
                    csMethod = r.csMethod,
                    csReferer = r.csReferer,
                    csUriQuery = r.csUriQuery,
                    csUriStem = r.csUriStem,
                    csUserAgent = r.csUserAgent,
                    date = r.date,
                    Id = r.Id,
                    scStatus = r.scStatus.ToString(),
                    sPort = r.sPort,
                    IsBlocked = r.IsBlocked,
                    BlockHit = r.BlockHit,
                    ApplicationName = r.ApplicationName
                });

            return helper.GetResult<IisLogViewModel>(jqGridViewModel, model);
        }

        [Helper.Security.AttributeHelpers.ValidateAjaxAntiForgeryToken]
        public JsonResult GetHttpStatus()
        {
            try
            {
                var model = unitOfWork.IISLogRepository.GetMany()
              .Select(r => new HttpStatusViewModel
              {
                  ScStatus = r.scStatus.ToString()
              }).Distinct().ToList();

                //add default
                HttpStatusViewModel defaultStatus = new HttpStatusViewModel();
                defaultStatus.ScStatus = "ALL";

                model.Insert(0, defaultStatus);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [Helper.Security.AttributeHelpers.ValidateAjaxAntiForgeryToken]
        public JsonResult GetApplicationName()
        {
            try
            {
                var model = unitOfWork.IISLogRepository.GetMany()
              .Select(r => new ApplicationNameViewModel
              {
                  Name = r.ApplicationName
              }).Distinct().ToList();

                //add default
                ApplicationNameViewModel defaultName = new ApplicationNameViewModel();
                defaultName.Name = "ALL";

                model.Insert(0, defaultName);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [Helper.Security.AttributeHelpers.ValidateAjaxAntiForgeryToken]
        public JsonResult BlockIpAddress(IpViewModel model)
        {

            if (!string.IsNullOrEmpty(model.Ip))
            {
                if (model.Action == "Block")
                {
                    unitOfWork.BlockedIpRepository.BlockIpAddress(model.Ip);
                }
                else
                {
                    unitOfWork.BlockedIpRepository.UnBlockIpAddress(model.Ip);
                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
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
    }
}