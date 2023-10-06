﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Pages
{
    public class ScoreDashboardController : Controller
    {
        // GET: ScoreDashboardController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ScoreDashboardController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ScoreDashboardController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ScoreDashboardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ScoreDashboardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ScoreDashboardController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ScoreDashboardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ScoreDashboardController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
