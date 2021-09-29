using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JordanToursim.CustomeValdition
{

    public class CheckDateFuture : ValidationAttribute
    {
        public CheckDateFuture() : base("{0} Datebirth can't be in future")
        {

        }



        public override bool IsValid(object value)
        {
            DateTime date = Convert.ToDateTime(value);
            bool _ = (date <= DateTime.Now) ? true : false;
            return _;
        }
    }
}