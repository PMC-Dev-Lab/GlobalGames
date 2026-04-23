using GlobalGames.Data;
using GlobalGames.Data.Entities;
using GlobalGames.Helpers;
using GlobalGames.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalGames.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;


        public HomeController(
            ILogger<HomeController> logger,
            ISubscriberRepository subscriberRepository,
            IUserHelper userHelper,
            IConverterHelper converterHelper,
            ILeadRepository leadRepository)
        {
            _logger = logger;
            _subscriberRepository = subscriberRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _leadRepository = leadRepository;
        }

        public IActionResult Home()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        // Email Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailSend(NewsletterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string subscriberEmail = model.Email ?? string.Empty;

                var subscriber = _converterHelper.ToSubscriber(model, subscriberEmail, true);
                try
                {
                    await _subscriberRepository.CreateAsync(subscriber);
                    TempData["SuccessMessage"] = "Thank you for subscribing to our newsletter!";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error while creating subscriber.");
                    TempData["ErrorMessage"] = "We couldn't process your subscription right now. Please try again later.";
                    return RedirectToAction(nameof(Home));
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Failed to create subscriber.");
                    TempData["ErrorMessage"] = "We couldn't process your subscription right now. Please try again later.";
                    return RedirectToAction(nameof(Home));
                }
            }

            return RedirectToAction(nameof(Home));
        }

        // Lead Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeadSend(LeadViewModel model)
        {
            if (ModelState.IsValid)
            {
                string message = model.Message ?? string.Empty;
                var lead = _converterHelper.ToLead(model, message, true);

                try
                {
                    await _leadRepository.CreateAsync(lead);
                    return RedirectToAction(nameof(Home));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error while creating lead.");
                    ModelState.AddModelError(string.Empty, "We couldn't submit your request right now. Please try again.");
                }
            }

            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
