using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Http; // Add this to use Session
using Newtonsoft.Json; // Add this to serialize/deserialize stuff into JSON 
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult MyLogin(Login myLogin) // This is the method where the form data from the LoginPartial goes to
        {
            if(ModelState.IsValid)
            {
                // If initial ModelState is valid, query for user with provided email
                var userInDb = dbContext.users.FirstOrDefault(u => u.Email == myLogin.LoginEmail);
                if(userInDb == null) // if User object returned is not defined (i.e. The User with myLogin.Password doesn't exist)
                {
                    // Add a ModelState Error and return to the WeddingPlanner page
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("index");
                }
                // Initialize the hasher object
                var hasher = new PasswordHasher<Login>();
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(myLogin, userInDb.Password, myLogin.LoginPassword);
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Invalid Email/Password");
                    return View("index");
                }
                // Add UserId to the session. Done!
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("success"); // This is the page that the results will go to if the form is valid
            } else {
                return View("index"); // This is the action (method) that will be performed is the form is invalid
            }
        }
        [HttpPost] // This is the Post where we Register a new User
        public IActionResult AddUser(User myUser) // This is the method where the form data from the UserPartial goes to
        {
            if(ModelState.IsValid)
            {
                // Check if the Email is already in use
                if(dbContext.users.Any(u => u.Email == myUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("index");
                }
                // If the ModelState is valid
                PasswordHasher<User> Hasher = new PasswordHasher<User>(); // instantiate a PasswordHasher
                myUser.Password = Hasher.HashPassword(myUser, myUser.Password); // Create Passwword Hash
                dbContext.Add(myUser); // Add the new User to the database
                dbContext.SaveChanges(); // Don't forget to save
                // Level II: (Optional): Log user in directly, obtaining UserId from newly created User. Done!
                // Then, redirect to Success page. Done!
                User userInDb = dbContext.users.FirstOrDefault(u => u.Email == myUser.Email);
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("success");
            } else {
                return View("index"); // This is the action (method) that will be performed is the form is invalid
            }
        }

        [HttpGet("success")]
        public IActionResult success()
        {
            // Only logged-in users should be able to see this page. If no user is in session, redirect to Login page. Done!
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return RedirectToAction("index");
            }
            return RedirectToAction("dashboard");
        }
        [HttpGet("logout")]
        public IActionResult logout()
        {
            // On logout clear Session
            HttpContext.Session.Clear();
            return View("index");
        }
        [HttpGet("dashboard")]
        public IActionResult dashboard()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return RedirectToAction("index");
            }
            var WeddingsWithAttendees = dbContext.weddings
                .Include(w => w.Attendees)
                .ThenInclude(a=>a.Attendee)
                .ToList();
            var AllAssociations = dbContext.associations.ToList();
            Wrapper MyWrapper = new Wrapper();
            MyWrapper.AllWeddings = WeddingsWithAttendees;
            MyWrapper.AllEvents = AllAssociations;
            ViewBag.UserId = (int)UserId;
            return View("dashboard", MyWrapper);
        }
        [HttpGet("new")]
        public IActionResult DisplayNewWedding()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return RedirectToAction("index");
            }
            return View("DisplayWedding");
        }
        [HttpPost]
        public IActionResult NewWedding(Wedding newWedding)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            newWedding.PlannerId = (int)UserId;
            if(ModelState.IsValid)
            {
                dbContext.Add(newWedding);
                dbContext.SaveChanges();
                int x = newWedding.WeddingId;
                return RedirectToAction("ViewWedding", new {WeddingId=$"{newWedding.WeddingId}"});
            } else {
                return RedirectToAction("DisplayNewWedding");
            }
        }
        [HttpGet("display/{Weddingid}")]
        public IActionResult ViewWedding(int WeddingId)
        {
            ApiKeys apiKey = new ApiKeys();
            Wedding ThisWedding = dbContext.weddings.FirstOrDefault(u => u.WeddingId == WeddingId);
            ViewBag.ThisWedding = ThisWedding;
            var WeddingWithGuests = dbContext.weddings.Include(w=>w.Attendees)
                .ThenInclude(a=>a.Attendee)
                .FirstOrDefault(w=>w.WeddingId == WeddingId);
            ViewBag.AllGuests = WeddingWithGuests;
            ViewBag.apiKey = apiKey.Google;
            return View("DisplayWedding", WeddingWithGuests);
        }
        [HttpGet("rsvp/{WeddingId}")]
        public IActionResult Rsvp(int WeddingId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return RedirectToAction("index");
            }
            var IsAttending = dbContext.associations
                .Where(a=>a.WeddingId == WeddingId && a.AttendeeId == UserId).ToList();
            if(IsAttending.Count() == 0)
            {
                Association Attending = new Association();
                Attending.AttendeeId = (int)UserId;
                Attending.WeddingId = WeddingId;
                dbContext.Add(Attending);
                dbContext.SaveChanges();
            }
            return RedirectToAction("dashboard");
        }
        [HttpGet("unrsvp/{WeddingId}")]
        public IActionResult UnRsvp(int WeddingId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return RedirectToAction("index");
            }
            var IsAttending = dbContext.associations
                .Where(a=>a.WeddingId == WeddingId)
                .FirstOrDefault(a=>a.AttendeeId == UserId);
            dbContext.Remove(IsAttending);
            dbContext.SaveChanges();
            return RedirectToAction("dashboard");
        }
        [HttpGet("delete/{WeddingId}")]
        public IActionResult Delete(int WeddingId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return RedirectToAction("index");
            }
            var thisWedding = dbContext.weddings.FirstOrDefault(w=>w.WeddingId == WeddingId);
            dbContext.Remove(thisWedding);
            dbContext.SaveChanges();
            return RedirectToAction("dashboard");
        }
    }
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        // generic type T is a stand-in indicating that we need to specify the type on retrieval
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            // Upon retrieval the object is deserialized based on the type specified
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
