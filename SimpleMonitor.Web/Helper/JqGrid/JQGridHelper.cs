using SimpleMonitor.Business;
using SimpleMonitor.Web.Helper.Linq;
using SimpleMonitor.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleMonitor.Web.Helper.JqGrid
{
    public class JQGridHelper : Controller
    {
        public JsonResult GetResult<T>(JQGridViewModel jqGridViewModel, IQueryable<T> source)
        {
            //get the default page size from web.config
            int pageSize = 0;
            if (jqGridViewModel.rows == 0)
            {
                int.TryParse(ApplicationSetting.JqGridDefaultPageSize, out pageSize);
            }
            else
            {
                pageSize = jqGridViewModel.rows;
            }

            if (string.IsNullOrEmpty(jqGridViewModel.sord))
            {
                jqGridViewModel.sord = "desc";
            }

            //direction of sort
            bool dir = jqGridViewModel.sord.ToLower().Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? true : false;
            var totalRows = 0;

            var result = new List<T>();

            if (jqGridViewModel._search)
            {
                //get the filter
                string filterCriteria = JQGridFilterHelper.FilterQuery<T>(jqGridViewModel);

                // result = source.Where(filterCriteria).OrderBy(jqGridViewModel.sidx, dir).ToList();

                //total rows in table
                totalRows = source.Where(filterCriteria).OrderBy(jqGridViewModel.sidx, dir).Count();

                result = source.Where(filterCriteria).OrderBy(jqGridViewModel.sidx, dir)
                   .Skip((jqGridViewModel.page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                //result = source.OrderBy(jqGridViewModel.sidx, dir).ToList();
                //total rows in table
                totalRows = source.OrderBy(jqGridViewModel.sidx, dir).Count();

                //get the paged result without filter
                result = source.OrderBy(jqGridViewModel.sidx, dir)
                   .Skip((jqGridViewModel.page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }

            var jsonResult = Json(new
            {
                // colName = result[0].GetType().GetProperties().Select(s => s.Name).ToArray(),
                page = jqGridViewModel.page,
                rows = result.ToArray(),
                records = totalRows,
                total = Convert.ToInt32(Math.Ceiling(Convert.ToSingle(totalRows) / Convert.ToSingle(pageSize)))
            }, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        /// <summary>
        /// get the dataset with the filter from the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jqGridViewModel"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public IQueryable<T> GetFilteredDataSet<T>(JQGridViewModel jqGridViewModel, IQueryable<T> source)
        {

            if (string.IsNullOrEmpty(jqGridViewModel.sord))
            {
                jqGridViewModel.sord = "desc";
            }

            //direction of sort
            bool dir = jqGridViewModel.sord.ToLower().Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? true : false;

            if (jqGridViewModel._search)
            {
                //get the filter
                string filterCriteria = JQGridFilterHelper.FilterQuery<T>(jqGridViewModel);

                return source.Where(filterCriteria).OrderBy(jqGridViewModel.sidx, dir);
            }
            else
            {
                //get the paged result without filter
                return source.OrderBy(jqGridViewModel.sidx, dir);
            }
        }
    }
}