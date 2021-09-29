using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using JordanToursim.CustomeValdition;

namespace JordanToursim.Models
{
    [MetadataType(typeof(TripMetaData))]
    public partial class Trip
    {
          //Add Methode or Add properties
    }

    public class TripMetaData
    {
        [Required]
        [Display(Name = "Trip Name")]
        [StringLength(160, MinimumLength = 6)]

        public string Trip_Name { get; set; }

        [Required]
        [MinLength(15, ErrorMessage = "Description must be at least 15 characters")]
        [MaxLength(70, ErrorMessage = "The Address cannot be more than 70 characters")]
        [Display(Name ="Description")]
        [DataType(DataType.MultilineText)]

        public string Trip_Description { get; set; }

        [Required]
        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:mm-DD-yyyy}")]
        [CheckDatePast]


        public System.DateTime Trip_Date_Time_From { get; set; } 

        [Display(Name = "To Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:mm-DD-yyyy}")]
        [CheckDatePast]

        public System.DateTime Trip_Date_Time_To { get; set; }  
        
        [Display(Name ="Capacity")]
        [Required]
        public int Trip_Capacity { get; set; }

        [Required]
        [Display(Name = "City")]

        public string City_Name { get; set; }

        [Required]
        [Display(Name = "Region")]

        public string Department_Name { get; set; }

        [Required]
        [Display(Name = "Price")]
        [Range(10,1000,ErrorMessage ="price should be positive and between [10,1000]")]

        
        public double Ticket_Price { get; set; }

        [Display(Name = "Image for Web")]
        public string Trip_Image_web { get; set; }

        [Display(Name = "Image for Mobile")]

        public string Trip_Image_phone { get; set; }
        [Required]
        [Display(Name = "Published")]

        public bool IsPublished { get; set; }
        public bool BusMovement { get; set; }
        public bool Lunch { get; set; }
        public bool OvernightStay { get; set; }
        public bool TouristGuide { get; set; }
    }


    [MetadataType(typeof(OrderMetaData))]
    public partial class Order
    {
        //Add Methode or Add properties
    }

    public class OrderMetaData
    {
        public int TripId { get; set; }
        public int Participant_ID { get; set; }
        [Display(Name="Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Required]
        [StringLength(160, MinimumLength = 4)]
        public string ParticipantName { get; set; }

        [Display(Name = "National Number")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid input should have 10 numbers only")]
        [Required]
        public int Participant_National_ID { get; set; }

        [Display(Name = "Birthday Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        [CheckDateFuture]
        [Required]


        public System.DateTime Participant_Birthdate { get; set; }

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [Required]
        [Display(Name = "Email")]
        public string Participant_Email { get; set; }

        [Required]
        [Display(Name ="provider")]
        public string NumberList { get; set; }

        [Display(Name = "Mobile Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{7})\)?", ErrorMessage = "Not a valid phone number")]
        [Required]
        public int Participant_Phone_No { get; set; }

        [Display(Name = "Identity pic")]

        public string Participant_Image_IDcard { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        [CheckDatePast]
        [Required]

        public System.DateTime Order_Date { get; set; }

        [Display(Name = "Move way")]
        [Required]

        public bool Movement_way { get; set; }


        public string City { get; set; }


        [Display(Name = "partner option")]
        public bool isPartner { get; set; }
        public Nullable<bool> Order_Status { get; set; }

        [Required]
        public bool agreement { get; set; }
    }



    [MetadataType(typeof(PartnerMetaData))]
    public partial class Partner
    {
        //Add Methode or Add properties
    }

    public partial class PartnerMetaData
    {

        public int Participant_ID { get; set; }
        public int Id { get; set; }

        [Display(Name ="Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Required]
        [StringLength(160, MinimumLength =4)]

        public string PartnerName { get; set; }

        [Display(Name = "National Number")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid input should have 10 numbers only")]
        [Required]

        public int PartnerNationalNumber { get; set; }

        [Display(Name = "Date of birth")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        [CheckDateFuture]

        public System.DateTime PartnerBirthDay { get; set; }

        [Display(Name = "Identit Image")]

        public string PartnerIdentityImg { get; set; }

        public virtual Order Order { get; set; }
    }



    [MetadataType(typeof(DepartmentMetaData))]
    public partial class Department
    {
        //Add Methode or Add properties
    }

    public partial class DepartmentMetaData
    {

        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "City")]
        public int id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Required]
        public int TripId { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Display(Name = "Name")]
        public string departmentName { get; set; }

        [Display(Name = "Image")]
        public string DepartmentImage { get; set; }

    }


}