using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using JordanToursim.Models;
using Rotativa.Options;

namespace JordanToursim.Controllers
{
    public class HomeController : Controller
    {
        private JordanToursimEntities db = new JordanToursimEntities();
        public ActionResult Index()
        {
            //Join 2 tables Trip and order
            //var popular = (from t1 in db.Trips
            //                  join t2 in db.Orders on t1.TripId equals t2.TripId
            //                  select (new { t1 })).Count();
            ////end of Join
            //ViewBag.popular = popular;
            return View(db.Departments.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Gallery()
        {
            ViewBag.Message = "Gallery page.";

            return View();
        }

        public ActionResult Cities()
        {
            //bring all citie
            return View(db.Cities.ToList());
            //end
        }

        public ActionResult Departments(string CityN, int? id)
        {

            //bring departments belong to the city by foriegnKey referance
            var CityDepartmnt = from key in db.Departments where key.id == id select key;
            return View(CityDepartmnt);
            //end
        }

        public ActionResult Trips(int id) {
            //bring all visible trips by fk 
            var trips = db.Trips.Where(x => x.TripId == id && x.IsPublished == true).ToList();
            return View(trips);
            //end
        }

        public static int total;

        public ActionResult Detail(int? id)
        {
            var trips = db.Trips.Where(x => x.TripId == id).FirstOrDefault();

            ViewBag.TotalCapacity = CapacityDetails(id);
            return View(trips);

        }
        //global variables of dates from - to
        public static DateTime from;
        public static DateTime To;

        public ActionResult Form(int id)
        {
            //get trip date from - to
            from = db.Trips.Where(x => x.TripId == id).Select(x => x.Trip_Date_Time_From).FirstOrDefault();
            To = db.Trips.Where(x => x.TripId == id).Select(x => x.Trip_Date_Time_To).FirstOrDefault();
            ViewBag.From = from.ToString("dd/MM/yyyy");
            ViewBag.To = To.ToString("dd/MM/yyyy");

            //end of dates
            var trips = db.Trips.Where(x => x.TripId == id).FirstOrDefault();

            int totalPriceDetail = CapacityDetails(id);
            if (trips!=null)
            {
                if (trips.TripId != 0)
                {

                    if (totalPriceDetail < trips.Trip_Capacity)
                    {
                        Order order = new Order { TripId = id };
                        return View(order);
                    }
                    return Content("this Trip has full sit");
                }
            }
            return View("NotFound", new { @id = id });

        }


        [HttpPost]
        public ActionResult Form([Bind(Include = "TripId,Participant_ID,ParticipantName,Participant_National_ID,Participant_Birthdate,Participant_Email,NumberList,Participant_Phone_No,Participant_Image_IDcard,Order_Date,Movement_way,City,isPartner,Order_Status,agreement")] Order order, HttpPostedFileBase IdentityImage)
        {
            if (ModelState.IsValid)
            {
                
                var partnerId = Session["PartnerId"];


                //upload pic 
                string ImagePath = "";
                if (IdentityImage.FileName.Length > 0)
                {
                    ImagePath = "~/Images/IdentityPhoto" + Path.GetFileName(IdentityImage.FileName);
                    IdentityImage.SaveAs(Server.MapPath(ImagePath));
                }
                order.Participant_Image_IDcard = ImagePath;
                //compare dates with order date
                if(order.Order_Date>from && order.Order_Date < To)
                {
                    db.Orders.Add(order);
                    db.SaveChanges();
                    if (order.isPartner == false)
                    {
                        TempData["message"] = "Your data has been inserted succesfully! waiting admin respons..";
                        return RedirectToAction("Cities", "Home");
                    }
                    else if (order.isPartner == true)
                    {
                        Session["TripPartnerId"] = order.TripId;
                        return RedirectToAction("Partner", new { id = order.Participant_ID, TripId = order.TripId });
                    }
                }
                else
                {
                    ModelState.AddModelError("Order_Date", "Date should be berween "+from.ToString("dd/MM/yyyy") + " - "+To.ToString("dd/MM/yyyy"));
                    return View();
                }
                //end of comparing
               
            }
            return View();

        }


        public ActionResult Partner(int id)
        {
            Order isWithGroup = db.Orders.Find(id);
            Partner withPartner = new Partner { Participant_ID = id };
            //show number of Escotrs
            int isExsit = db.Orders.Where(x => x.Participant_ID == id).Count();
            ViewBag.PartnerNumbers = db.Partners.Where(x => x.Participant_ID == id).Count();
            
            if (isExsit > 0 && isWithGroup.isPartner==true)
                return View(withPartner);
            else
                return View("NotFound");

            // end show number of Escotrs

        }

        [HttpPost]
        public ActionResult Partner([Bind(Include = "Participant_ID,Id,PartnerName,PartnerNationalNumber,PartnerBirthDay,PartnerIdentityImg")] Partner partner, HttpPostedFileBase IdentityPartner)
        {
            if (ModelState.IsValid)
            {
                string ImagePath = "";
                if (IdentityPartner.FileName.Length > 0)
                {
                    ImagePath = "~/Images/IdentityPartner" + Path.GetFileName(IdentityPartner.FileName);
                    IdentityPartner.SaveAs(Server.MapPath(ImagePath));
                }
                partner.PartnerIdentityImg = ImagePath;

                db.Partners.Add(partner);
                db.SaveChanges();


                //pass submit pass state 
                Session["successPartner"] = "true";
                if (Session["successPartner"] == "true")
                    return RedirectToAction("Partner");
                Session["successPartner"] = "false";
                // end of submit pass state 

            }
            return View();
        }


        public ActionResult Print(int id,string TripId) {
            
            //Join 2 tables Trip and order
            List<Trip> TripList = db.Trips.ToList();
            var convertKey = (from t1 in db.Orders 
                              where t1.Participant_ID==id 
                              join t2 in db.Trips on t1.TripId equals t2.TripId
                              select( new { t1.ParticipantName, t1.Order_Status ,t1.Participant_ID,t1.Order_Date, t2.Ticket_Price ,t2.TripId, t2.Trip_Name})).FirstOrDefault();
            //end of Join

            //add join tables result
            ArrayList TripDetails = new ArrayList();
            if (convertKey != null)
            {
                TripDetails.Add(convertKey.TripId);
                TripDetails.Add(convertKey.Participant_ID);
                TripDetails.Add(convertKey.Trip_Name);
                TripDetails.Add(convertKey.ParticipantName);
                TripDetails.Add(convertKey.Ticket_Price);
                TripDetails.Add(convertKey.Order_Date);
                TripDetails.Add(convertKey.Order_Status);
                ViewBag.TripDetails = TripDetails;

                var AllPartners= db.Partners.Where(x => x.Participant_ID == id).ToList();
                Session["cool"] = convertKey.Participant_ID;
                return View(AllPartners);
            }
            //end of add

            // 404 page for unexcist id
            else
                return RedirectToAction("NotFound", new {@id=id });
            //end of 404 page
        }

        //rotativa tool to export page as pdf
        public ActionResult PrintTicket(int? id)
        {
            Order order = db.Orders.Find(id);

            if (order.Order_Status == true)
            {
                return new Rotativa.ActionAsPdf("exportPdf", new { @id = id })
                {
                    FileName = "JT-Ticket.pdf",
                    PageSize = Size.A4,
                    PageWidth = 210,
                    PageHeight = 130
                };
            }
            else
                RedirectToAction("NotFound", new { @id = id });

            return null;
        }
        //end of tool


        //convert page to pdf 
        public ActionResult exportPdf(int? id)
        {
            //Join 2 tables Trip and order
            List<Trip> TripList = db.Trips.ToList();
            var convertKey = (from t1 in db.Orders where (t1.Participant_ID==id && t1.Order_Status==true)
                              join t2 in db.Trips on t1.TripId equals t2.TripId
                              select( new { t1.ParticipantName, t1.Order_Status ,t1.Participant_ID,t1.Order_Date, t2.Ticket_Price ,t2.TripId, t2.Trip_Name})).FirstOrDefault();
            //end of Join

            //add join tables result
            ArrayList TripDetails = new ArrayList();
            if (convertKey != null)
            {
                TripDetails.Add(convertKey.TripId);
                TripDetails.Add(convertKey.Participant_ID);
                TripDetails.Add(convertKey.Trip_Name);
                TripDetails.Add(convertKey.ParticipantName);
                TripDetails.Add(convertKey.Ticket_Price);
                TripDetails.Add(convertKey.Order_Date);
                TripDetails.Add(convertKey.Order_Status);
                ViewBag.TripDetails = TripDetails;

                var AllPartners= db.Partners.Where(x => x.Participant_ID == id).ToList();
                Session["cool"] = convertKey.Participant_ID;
                return View(AllPartners);
            }
            //end of add

            // 404 page for unexcist id
            else
                return RedirectToAction("NotFound", new {@id=id });
            //end of 404 page
        }
        //end of convert page


        //show order state page 
        public ActionResult Response(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            return View(order);
        }
        //end of order state page

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Response([Bind(Include = "TripId,Participant_ID,ParticipantName,Participant_National_ID,Participant_Birthdate,Participant_Email,NumberList,Participant_Phone_No,Participant_Image_IDcard,Order_Date,Movement_way,City,isPartner,Order_Status,agreement")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(order);

        }

        [HttpGet]
        //search trips page 
        public ActionResult Search(string SearchBy,string search)
        {
            if (search!="")
            {
                if (SearchBy == "location" )
                {
                    int count = db.Departments.Where(x => x.departmentName.StartsWith(search)).ToList().Count;
                    ViewBag.locationSearch = true;
                    ViewBag.countlocation = count;

                    return View(db.Departments.Where(x => x.departmentName.StartsWith(search) && count>0).ToList());
                }
                else if (SearchBy == "city")
                {
                    int count = db.Cities.Where(x => x.CityName == search).ToList().Count;
                    ViewBag.locationCity = true;
                    ViewBag.countCity = count;

                    return View(db.Cities.Where(x => x.CityName == search && count > 0).ToList());
                }
                else if (SearchBy == "price")
                {
                    int errorCounter = Regex.Matches(search, @"[a-zA-Z]").Count;
                    if (errorCounter== 0)
                    {
                        ViewBag.locationPrice = true;
                        int price = Int32.Parse(search);
                        int count = db.Trips.Where(x => x.Ticket_Price == price).ToList().Count;
                        ViewBag.countPrice = count;
                        return View(db.Trips.Where(x => x.Ticket_Price <= price && count > 0).ToList());
                    }
                    return View();
                }
            }
            ViewBag.nullV = "";
            ViewBag.nullCount = 0;


            return View();
        }
        //end of search trips page
        

        //review order state by search
        public ActionResult Review(int? id, string NID) {
            ViewBag.NIdMsg = "";
            if (id != null)
            {
                if (id != null)
                {
                    Order order = db.Orders.Find(id);
                    if (order != null)
                    {
                        return RedirectToAction("print", new { id = order.Participant_ID });
                    }
                    else
                    {
                        ViewBag.idMsg = "not ticket for this person";
                        return View();
                    }

                }
                return View();
            }

            if (NID != null)
            {
               

                var nId = db.Orders.Where(x => x.Participant_National_ID == NID).Select(x => x.Participant_ID).FirstOrDefault();
                Order order2 = db.Orders.Find(nId);
                if (nId != null && id!=0 && order2!=null)
                {
                    return RedirectToAction("print", new { id = order2.Participant_ID });
                }
                else
                ViewBag.NIdMsg = "not ticket for this person";

                return View();
            }
            return View();
        }
        //end of order state search


        //404 page
        public ActionResult NotFound(int id){
            return View(id);
        }
        //end of 404

        //cities list
        public void CitiesDropDown()
        {
            var cityList = db.Cities.ToList();
            Session["cityList"] = new SelectList(cityList, "Id", "CityName");
        }
        //end of cities list  
        
        //Show Trip Capacity
        public int CapacityDetails(int? TripNum)
        {
            int aloneParticepnt = db.Orders.Where(x => x.Participant_ID == TripNum).Count();
            int years = DateTime.Now.Year;

            int TotalCapacity = (from t1 in db.Orders
                                 where t1.Trip.TripId == TripNum
                                 join t2 in db.Partners on t1.Participant_ID equals t2.Participant_ID 
                                 select (new { t1.Participant_ID, t2.PartnerName })).ToList().Count();
            int total = TotalCapacity + aloneParticepnt;
            ViewBag.TotalCapacity = total;
            return total;
        }
        //End of trip Capacity

    }
}
