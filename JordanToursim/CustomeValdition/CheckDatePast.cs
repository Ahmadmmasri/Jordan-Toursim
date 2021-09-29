using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JordanToursim.CustomeValdition
{
    public class CheckDatePast: ValidationAttribute
    {
        public CheckDatePast() : base("{0} date can't be in past")
        {

        }



        public override bool IsValid(object value)
        {
            DateTime date = Convert.ToDateTime(value);
            bool _ = (date >= DateTime.Now) ? true : false;
            return _;

           



        }
    }
}