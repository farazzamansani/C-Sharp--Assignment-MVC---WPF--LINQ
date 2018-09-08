using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HRIS.Entities
{
    public class Consultation
    {
        /*  Note: Due to the marking criteria that all classes must be in their own .cs file,
     *  the entities are sharing enums under the same namespace but the enums are defined
     *  once in a single .cs file.(Classdetails.cs)
     * */
        public int staffid { get; set; } //eg 123461
        public DayOfWeek day { get; set; }
        public TimeSpan starttime { get; set; }
        public TimeSpan endtime { get; set; }
    }
}
