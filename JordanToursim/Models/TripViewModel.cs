using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JordanToursim.Models
{
    public class TripViewModel
    {
        public int TripId { get; set; }

        public string Trip_Name { get; set; }

        public string Trip_Description { get; set; }



        public System.DateTime Trip_Date_Time_From { get; set; }

        public System.DateTime Trip_Date_Time_To { get; set; }


        public int Trip_Capacity { get; set; }



        public string City_Name { get; set; }


        public string Department_Name { get; set; }




        public double Ticket_Price { get; set; }

        public string Trip_Image_web { get; set; }


        public string Trip_Image_phone { get; set; }


        public bool IsPublished { get; set; }
        public bool BusMovement { get; set; }
        public bool Lunch { get; set; }
        public bool OvernightStay { get; set; }
        public bool TouristGuide { get; set; }
    }


}