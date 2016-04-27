using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyTinyCollege.ViewModels
{
    public class EnrollmentDateGroup
    {   
        //this will be used to show student body stats report
        //counting how many students enrolled on a particular
        //enrollment date

        //without this annotation we should get a date time
        // 9/1/2016 12:00:00 AM
        [DataType(DataType.Date)]        
        public DateTime? EnrollmentDate { get; set; }
        public int StudentCount { get; set; }

    }
}