using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTinyCollege.Models
{
    public abstract class Person
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public string Email { get; set; }

        //FullName is a CALCULATED PROPERTY that returns a value created by concatenating
        //two other property. Therefore it only has a get accessor, and because of this, 
        //no FullName column will be generated in the database.
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }
    }
}