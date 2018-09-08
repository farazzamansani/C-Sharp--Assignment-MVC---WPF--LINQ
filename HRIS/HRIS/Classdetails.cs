using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HRIS.Entities
{
    //enum's defined here are shared by all entities under this namespace.
    //Entities are modeled to match the EER digram of the SQL database.
    public enum Campus { Hobart = 1, Launceston }
    public enum Category { Academic = 1, Technical, Admin, Casual }
    public enum Classtype { Tutorial = 1, Lecture, Practical }
    public enum Availability { Free = 1, Consulting, Teaching }

        public class Classdetails
        {
            public string unitcode { get; set; }
            public Campus campus { get; set; }
            public DayOfWeek day { get; set; }
            public TimeSpan starttime { get; set; }
            public TimeSpan endtime { get; set; }
            public Classtype classtype { get; set; }
            public string room { get; set; }
            public int staffid { get; set; }
            public string displayName { get; set; }
        }
    
       
}