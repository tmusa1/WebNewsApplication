using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebProgramiranje.Data;
using WebProgramiranje.Models;
using WebProgramiranje.ViewModels;

namespace WebProgramiranje.Areas.Guest.Controllers
{

    [Area("Guest")]
    public class NewsController : Controller
    {
        HttpClientHandler _clientHandler = new HttpClientHandler();

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
       
        public NewsController(ApplicationDbContext db,
                              IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<News> news = await _db.News.ToListAsync();
            return View(news);
        }

        //public ActionResult Index()
        //{
        //    //List<News> _news = new List<News>();
        //    //using (var httpClient = new HttpClient(_clientHandler))
        //    //{
        //    //    using (var response = await httpClient.GetAsync("https://localhost:44360/Guest/News"))
        //    //    {
        //    //        string apiResponse = await response.Content.ReadAsStringAsync();
        //    //        _news = JsonConvert.DeserializeObject<List<News>>(apiResponse);
        //    //    }
        //    //}
        //    HttpClient client2 = new HttpClient();
        //    List<News> modelList = new List<News>();
        //    var response = client2.GetAsync("https://localhost:5001/api/Guest/News").Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string data = response.Content.ReadAsStringAsync().Result;
        //        modelList = JsonConvert.DeserializeObject<List<News>>(data);
        //    }
        //    return View(modelList);

        //}



        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) {
                return NotFound();
            }

            //var newsFromDb = await _db.News.Include(c => c.Comments)
            //                       .FirstOrDefaultAsync(n => n.Id == id);

            ViewBag.loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            NewsCommentsAndUsersViewModel model = new NewsCommentsAndUsersViewModel
            {
                News = await _db.News.FindAsync(id),
                Comments = await _db.Comment.Include(u => u.ApplicationUser)
                                            .Where(c => c.NewsId == id).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(NewsCommentsAndUsersViewModel model)
        {
            //if(model.Comment == null)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                _db.Comment.Add(model.Comment);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model.Comment);

            //_db.News.Add(model);
            //await _db.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteComment(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var getCommentFromDb = await _db.Comment.FindAsync(id);
            if (getCommentFromDb == null)
            {
                return NotFound();
            }
            else
            {
                _db.Comment.Remove(getCommentFromDb);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
