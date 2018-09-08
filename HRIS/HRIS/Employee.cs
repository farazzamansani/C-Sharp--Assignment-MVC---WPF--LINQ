using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRIS.Entities
{
    /*  Note: Due to the marking criteria that all classes must be in their own .cs file,
     *  the entities are sharing enums under the same namespace but the enums are defined
     *  once in a single .cs file.(Classdetails.cs)
     * */
    public class Employee
    {
        public int staffid { get; set; } //eg123461
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string title { get; set; } //eg Dr.
        public Campus campus { get; set; }
        public string phone { get; set; }
        public string room { get; set; }
        public string email { get; set; }
        public string photo { get; set; } //url to photo
        public Category category { get; set; }
        public Availability availability { get; set; }

        //Allow a tring to be returned with the title and name as one value
        //public string 
    }
}
