using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using JordanToursim.Models;
using SelectList = System.Web.Mvc.SelectList;

namespace JordanToursim.Controllers
{
    public class TripsController : Controller
    {
        private JordanToursimEntities db = new JordanToursimEntities();

        SerialPort sp = new SerialPort();

        // GET: Trips
        public ActionResult Index()
        {
            //check if admin authorized allow access
            if (Session["adminName"] != null)
            {
                Dictionary<int, string> My_City = new Dictionary<int, string>();
                //show cities name rathen numbers
                string[] citiesName = new string[] { "", "Amman", "Irbid", "Jarash", "Maan", "Ramtha", "Tafela", "Karak", "Aqaba", "Zarqa", "Ajlon" };
                for (int i = 1; i < citiesName.Length; i++)
                {
                    My_City.Add(i, citiesName[i]);
                }
                ViewBag.My_City = My_City;
                //end
                return View(db.Trips.ToList());
            }
            //end of checking access


            return RedirectToAction("Login");
        }

        // GET: Trips/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["adminName"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Trip trip = db.Trips.Find(id);
                if (trip == null)
                {
                    return HttpNotFound();
                }
                return View(trip);
            }
            return RedirectToAction("Login");
           
        }

        // GET: Trips/Create
        public ActionResult Create()
        {
            //check if admin authorized allow access
            if (Session["adminName"] != null)
            {
                return View();
            }
            //end of checking access
            return RedirectToAction("Login");

        }





        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TripId,Trip_Name,Trip_Description,Trip_Date_Time_From,Trip_Date_Time_To,Trip_Capacity,City_Name,Department_Name,Ticket_Price,Trip_Image_web,Trip_Image_phone,IsPublished,BusMovement,Lunch,OvernightStay,TouristGuide")] Trip trip , City city, HttpPostedFileBase ImageFilePhone, HttpPostedFileBase ImageFileWeb)
        {
            if (ModelState.IsValid)
            {
                //check if admin pick at least one checkbox
                if (trip.Lunch==true || trip.TouristGuide==true || trip.OvernightStay==true || trip.BusMovement==true)
                {

                    //upload pic 
                    string webImagePath = "";
                    if (ImageFileWeb.FileName.Length > 0)
                    {
                        webImagePath = "~/Images/TripImages" + Path.GetFileName(ImageFileWeb.FileName);
                        ImageFileWeb.SaveAs(Server.MapPath(webImagePath));
                    }
                    trip.Trip_Image_web = webImagePath;

                    string PhoneImagePath = "";
                    if (ImageFilePhone.FileName.Length > 0)
                    {
                        PhoneImagePath = "~/Images/TripImages" + Path.GetFileName(ImageFilePhone.FileName);
                        ImageFilePhone.SaveAs(Server.MapPath(PhoneImagePath));
                    }
                    trip.Trip_Image_phone = PhoneImagePath;
                    //end of uploading

                    db.Trips.Add(trip);
                    db.SaveChanges();
                    
                    //pass submit pass state 
                    Session["success"]= "true";
                    if (Session["success"] == "true")
                        return RedirectToAction("Create");
                    Session["success"] = "false";
                    // end of submit pass state 



                }
                else
                {
                    ModelState.AddModelError("TouristGuide", "You should choose at least one service");
                    return View(trip);
                }
            }

            return View();
        }

        // GET: Trips/Edit/5
        public ActionResult Edit(int? id)
        {
            //check if admin authorized allow access
            if (Session["adminName"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Trip trip = db.Trips.Find(id);
                if (trip == null)
                {
                    return HttpNotFound();
                }
                return View(trip);
            }
            //end of checking access

            return RedirectToAction("Login");
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TripId,Trip_Name,Trip_Description,Trip_Date_Time_From,Trip_Date_Time_To,Trip_Capacity,City_Name,Department_Name,Ticket_Price,Trip_Image_web,Trip_Image_phone,IsPublished,BusMovement,Lunch,OvernightStay,TouristGuide")] Trip trip, HttpPostedFileBase ImageFileWeb, HttpPostedFileBase ImageFilePhone)
        {
            if (ModelState.IsValid)
            {
                //check if admin pick at least one checkbox
                if (trip.Lunch == true || trip.TouristGuide == true || trip.OvernightStay == true || trip.BusMovement == true)
                {
                    //upload pic 
                    string webImagePath = "";
                    if (ImageFileWeb.FileName.Length > 0)
                    {
                        webImagePath = "~/Images/TripImages" + Path.GetFileName(ImageFileWeb.FileName);
                        ImageFileWeb.SaveAs(Server.MapPath(webImagePath));
                    }
                    trip.Trip_Image_web = webImagePath;

                    string PhoneImagePath = "";
                    if (ImageFilePhone.FileName.Length > 0)
                    {
                        PhoneImagePath = "~/Images/TripImages" + Path.GetFileName(ImageFilePhone.FileName);
                        ImageFilePhone.SaveAs(Server.MapPath(PhoneImagePath));
                    }
                    trip.Trip_Image_phone = PhoneImagePath;
                    //end of uploading

                    db.Entry(trip).State = EntityState.Modified;
                    db.SaveChanges();

                    //pass submitEdit state 
                    Session["successEdit"] = "true";
                    if (Session["successEdit"] == "true")
                        return RedirectToAction("Edit");
                    Session["successEdit"] = "false";
                    // end of submit pass state 
                }
                else
                {
                    ModelState.AddModelError("TouristGuide", "You should choose at least one service");
                    return View(trip);
                }
            }
            return View(trip);
        }

        // GET: Trips/Delete/5
        public ActionResult Delete(int? id)
        {
               //check if admin authorized allow access
                if (Session["adminName"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Trip trip = db.Trips.Find(id);
                if (trip == null)
                {
                    return HttpNotFound();
                }
                return View(trip);
            }
               //end of checking access
            return RedirectToAction("Login");
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trip trip = db.Trips.Find(id);
            db.Trips.Remove(trip);
            db.SaveChanges();

            //pass submit Delete pass state 
                    Session["successDelete"]= "true";
                    if (Session["successDelete"] == "true")
                        return RedirectToAction("Delete", new { id = id+1 });
                    Session["successDelete"] = "false";
            // end of submit pass Delete state 

            return View(trip);

        }

        public ActionResult DeleteOrder(int? id)
        {
            if (Session["adminName"] != null)
            {
                Order Particepant = db.Orders.Find(id);
                int isLetter = Regex.Matches(id.ToString(), @"[a-zA-Z]").Count;
                if (Particepant != null && isLetter == 0)
                {

                    if (Particepant.Order_Status == false && Session["adminName"] != null)
                    {
                        db.Orders.Remove(Particepant);
                        db.SaveChanges();
                        //pass submit Delete pass state 
                        ViewBag.successDelete = "true";
                        if (ViewBag.successDelete == "true")
                            return RedirectToAction("Requests", new { id = id });
                        ViewBag.successDelete = "false";
                        // end of submit pass Delete state 
                    }
                }
                return View("NotFound", new { id = id });
            }
            return HttpNotFound();

        }

        public ActionResult Login() 
        {
            Session["adminName"] = null;

            return View();

        }


        [HttpPost]
        public ActionResult Login([Bind(Include = "Admin,password")] LogIn logIn ) 
        {
            //check Admin name and password
            var name = db.LogIns.Where(Value => Value.Admin == logIn.Admin && Value.password==logIn.password).ToList().FirstOrDefault();
            if (name != null)
            {
                Session["adminName"] = name.Admin;
                return RedirectToAction("Index");
            }

                ViewBag.error = "Invalid user";
                return View(logIn);
        }

        public ActionResult All()
        {   //check if admin authorized allow access
            if (Session["adminName"] != null)
            {
                var All = db.Trips.ToList();
                return View(All);
            }
            //end of checking access
            return RedirectToAction("Login");

        }


        public ActionResult Requests() 
        {
            Dictionary<int, string> My_City = new Dictionary<int, string>();
             //show cities name rathen numbers
            string[] citiesName = new string[] { "", "Amman", "Irbid", "Jarash", "Maan", "Ramtha", "Tafela", "Karak", "Aqaba", "Zarqa", "Ajlon" };
            for (int i = 1; i < citiesName.Length; i++)
            {
                My_City.Add(i, citiesName[i]);
            }
            ViewBag.My_City = My_City;
            //end


            //Join 2 tables Trip and order
            var convertKey = (from t1 in db.Orders
                              join t2 in db.Departments on t1.TripId equals t2.TripId
                              select (new { t2.DepartmentId })).ToList();
            //end of Join

            //check if admin authorized allow access
            if (Session["adminName"] != null)
            {
                var AllRequestList = db.Orders.Distinct()
               .OrderByDescending(d => d.Order_Date).ToList();
                return View(AllRequestList);
            }
            return RedirectToAction("Login");
            //end of checking access

        }

        public ActionResult EscortsDetail(int id)
        {
            if (Session["adminName"] != null)
            {
                var Details = db.Partners.Where(x => x.Participant_ID == id).ToList();
                return View(Details);
            }
            return RedirectToAction("Login");

        }

        public new ActionResult Response(int? id)
        {
            if (Session["adminName"] != null)
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Order order = db.Orders.Find(id);
                if (order == null)
                    return HttpNotFound();
                else
                    return View(order);
                
            }
            return RedirectToAction("Login");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Response ([Bind(Include = "TripId,Participant_ID,ParticipantName,Participant_National_ID,Participant_Birthdate,Participant_Email,NumberList,Participant_Phone_No,Participant_Image_IDcard,Order_Date,Movement_way,City,isPartner,Order_Status,agreement")] Order order)
        {
            
                if (ModelState.IsValid)
                {
                    string useremail = order.Participant_Email.ToString();
                    string subject = "Jordan Toursim";
                    string body = "";
                    switch (order.Order_Status)
                    {
                        case true:
                            body = "Dear " + order.ParticipantName + "<br/> Thanks for your intresting in our Journey <br/> we are happy to inform you u have been accepted, our team will contact you soon <br/> for more details you can review your submit state using this code" + order.Participant_ID + "  <br/> thanks <br/> Joran Toursim Team";
                            break;
                        case false:
                            body = "Dear " + order.ParticipantName + "<br/> Thanks for your intresting in " + "our Journey <br/> we are sorry to inform you that you application was not contain correct information so will not keep going with it, <br/> you can see your submition state and reapply again <br/> please make sure of using referance number " + order.Participant_ID + " <br/> thanks <br/> Joran Toursim Team";
                            break;
                        default:
                            db.Entry(order).State = EntityState.Modified;
                            db.SaveChanges();
                            return View();
                    }
                    string num = order.NumberList + order.Participant_Phone_No;
                    WebMail.Send(useremail, subject, body, null, null, null, true, null, null, null, null, null, null);
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.msg = "Email sent successfully!";

                }

                return View(order);

        }

        public ActionResult CitiesList() {

            return View(db.Cities.ToList());

        }

        public ActionResult City(int? id)
        {
            //check if admin authorized allow access
            if (Session["adminName"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                City city = db.Cities.Find(id);
                if (city == null)
                {
                    return HttpNotFound();
                }
                return View(city);
            }
            //end of checking access

            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult City([Bind(Include = "Id,CityName,cityImage,Image,Sort")] City city, HttpPostedFileBase ImageCity)
        {
            if (ModelState.IsValid)
            {
                    //upload pic 
                    string CityImagePath = "";
                    if (ImageCity.FileName.Length > 0)
                    {
                        CityImagePath = "~/Images/CityPhoto" + Path.GetFileName(ImageCity.FileName);
                    ImageCity.SaveAs(Server.MapPath(CityImagePath));
                    }
                    city.cityImage = CityImagePath;

                    //end of uploading

                     db.Entry(city).State = EntityState.Modified;
                     db.SaveChanges();

                    //pass submit pass state 
                    Session["success"] = "true";
                    if (Session["success"].ToString() == "true")
                        return RedirectToAction("Index");
                    Session["success"] = "false";
                    // end of submit pass state 
                }
                else
                {
                    //ModelState.AddModelError("TouristGuide", "You should choose at least one service");
                    return View();
                }
          return View();
        }


        public ActionResult DepartmentList()
        {
            if (Session["adminName"] != null)
            {
                return View(db.Departments.ToList());
            }
            return RedirectToAction("Login");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Department([Bind(Include = "DepartmentId,id,TripId,departmentName,DepartmentImage")] Department department, HttpPostedFileBase ImageDep)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }


        // GET: Departments/Create
        public ActionResult CreateDep()
        {
            if (Session["adminName"] != null)
            {

                ViewBag.id = new SelectList(db.Cities, "Id", "CityName");
                return View();
            }
                return RedirectToAction("Login");
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult CreateDep([Bind(Include = "DepartmentId,id,TripId,departmentName,DepartmentImage")] Department department, HttpPostedFileBase DepImage)
        {
            if (ModelState.IsValid)
            {
                string DepImagePath = "";
                if (DepImage.FileName.Length > 0)
                {
                    DepImagePath = "~/Images/DepartmentPhoto" + Path.GetFileName(DepImage.FileName);
                    DepImage.SaveAs(Server.MapPath(DepImagePath));
                }
                department.DepartmentImage = DepImagePath;
                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
                //pass submit pass state 
                Session["success"] = "true";
                if (Session["success"] == "true")
                    return RedirectToAction("Create");
                Session["success"] = "false";
                // end of submit pass state 
            }

            ViewBag.id = new SelectList(db.Cities, "Id", "CityName", department.id);
            ViewBag.TripId = new SelectList(db.Trips, "TripId", "Trip_Name", department.TripId);
            return View(department);
        }

        [HttpGet]
        public ActionResult DepaEdit(int? id)
        {
            if (Session["adminName"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Department department = db.Departments.Find(id);
                if (department == null)
                {
                    return HttpNotFound();
                }
                ViewBag.id = new SelectList(db.Cities, "Id", "CityName", department.id);
                //ViewBag.TripId = new SelectList(db.Trips, "TripId", "Trip_Name", department.TripId);
                return View(department);
            }
            return RedirectToAction("Login");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DepaEdit([Bind(Include = "DepartmentId,id,TripId,departmentName,DepartmentImage")] Department department,HttpPostedFileBase ImageDep)
        {
            ViewBag.id = new SelectList(db.Cities, "Id", "CityName", department.id);

            if (ModelState.IsValid)
            {
                string DepImagePath = "";
                if (ImageDep.FileName.Length > 0)
                {
                    DepImagePath = "~/Images/DepartmentPhoto" + Path.GetFileName(ImageDep.FileName);
                    ImageDep.SaveAs(Server.MapPath(DepImagePath));
                }
                department.DepartmentImage = DepImagePath;
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();

                Session["success"] = "true";
                if (Session["success"] == "true")
                    return View();
                Session["success"] = "false";
                // end of submit pass state 
                return RedirectToAction("Index");
            }
            return View(department);
        }


        // POST: Departments/Delete/5
        public ActionResult DeleteDep(int? id)
        {
            if (Session["adminName"] != null)
            {
                Department department = db.Departments.Find(id);
                if (department != null)
                {

                    db.Departments.Remove(department);
                    db.SaveChanges();
                }
                return RedirectToAction("NotFound");
            }
            return RedirectToAction("Login");


        }


        //404 page
        public ActionResult NotFound(int id)
        {
            return View(id);
        }
        //end of 404


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // functions to calculate Count , Max, Min and average 

        public int Count()
        {
            return db.Trips.ToList().Count;
        }
        public double Max()
        {
            return db.Trips.Max(m => m.Ticket_Price); 
        }
        public double Min()
        {
            return db.Trips.Min(m => m.Ticket_Price);

        }
        public double Average()
        {
            return db.Trips.Average(m => m.Ticket_Price);

        }
        public void CitiesDropDown()
        {
            var cityList = db.Cities.ToList();
            Session["citiesList"] = new SelectList(cityList, "Id", "CityName");
            
            var DepList = db.Departments.ToList();
            Session["DepList"] = new SelectList(DepList, "id", "departmentName");
        }

        public int CitiesNumber()
        {
            var cityList = db.Cities.Count();
            return cityList;
        }

        //Send sms (optional)

        //public void SendSMS(string Number, String Message)
        //{
        //    string TelNum = Char.ConvertFromUtf32(34) + Number + Char.ConvertFromUtf32(34);
        //    sp.PortName = "COM4";
        //    sp.Open();
        //    sp.Write("AT+CMGF=1"+Char.ConvertFromUtf32(13));
        //    sp.Write("AT+CMGs="+TelNum+Char.ConvertFromUtf32(13));
        //    sp.Write(Message + Char.ConvertFromUtf32(26) + Char.ConvertFromUtf32(13));
        //    sp.Close();
        //}

        //end sms

        // end of functions

    }
}
