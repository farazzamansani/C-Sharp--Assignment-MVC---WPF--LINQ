using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HRIS.Entities;    //Data entities
using HRIS.Database;    //Database queries

namespace HRIS.Control
{
    /// <summary>
    /// Keeps a master copy of lists, updates them and returns filtered results.
    /// </summary>
    public class Listcontroler
    {
        public List<Employee> MasterStaffList = new List<Employee>();
        public List<Unit> MasterUnitList = new List<Unit>();
        public List<Classdetails> MasterClassDetailsList = new List<Classdetails>();
        public List<Consultation> MasterConsultationList = new List<Consultation>();


        /// <summary>
        /// The Constructor for a ListController Instance
        /// </summary>
        public Listcontroler()
        {
            //Iniates the masterlists when Listcontroler is instantiated..
            MasterStaffList = database.LoadStaffList();
            MasterUnitList = database.LoadUnitList();
            //MasterClassDetailsList = database.LoadClassList();
        }

        /// <summary>
        /// Gets All details for a Staffmember, updates the MasterStaffList from the SQL database if details dont already exist.
        /// </summary>
        /// <param name="idgiven">The staff id of the Employee's details you need</param>
        /// <returns>Employee class object</returns>
        public Employee GetStaffDetails (int idgiven)
        {
  
            IEnumerable<Employee> ielist = MasterStaffList.Where(Employee => 
                Employee.staffid==idgiven       //find the existing Employee by staffid
                && Employee.photo!=null);       //if photo is not null then we already have the details

            Employee temp = new Employee();

            if (ielist.Count() == 0)            //0 results means no photo and other details are missing
            {
                
                temp = database.LoadOneStaff(idgiven);  //get all the details for staff member

                //find that staff member in the Master list and fill in the missing details
                foreach (var emplo in MasterStaffList.Where(x => x.staffid == idgiven))
                {
                    emplo.given_name = temp.given_name;
                    emplo.family_name = temp.family_name;
                    emplo.title = temp.title;
                    emplo.campus = temp.campus;
                    emplo.phone = temp.phone;
                    emplo.room = temp.room;
                    emplo.email = temp.email;
                    emplo.photo = temp.photo;
                    emplo.category = temp.category;
                }
            }
            else
            {
                //so grab the already filled in staffmember from the master list
                temp = (from Employee emplo in MasterStaffList
                        where emplo.staffid == idgiven
                        select emplo).Single();
                
            }

            //Update availability reguardless if we have that value or not (much time may have passed since last check)
            temp.availability = CheckAvailability(temp.staffid);
            return temp;
        }

        /// <summary>
        /// Filter a List of Employee's based on a partial match to the search term.
        /// </summary>
        /// <param name="unfilteredlist">List of Employee's to search through</param>
        /// <param name="searchTerm">Search term for partial match</param>
        /// <returns>List of Employee class objects</returns>
        public List<Employee> FilterEmployeeListByName(List<Employee> unfilteredlist, string searchTerm)
        {
            //receive the unfiltered list and the searchterm typed

            //create an IEnumerable list and place within it the employees from the unfiltered list whos given name string contains the searchterm
            IEnumerable<Employee> ielist = unfilteredlist.Where(Employee => 
                Employee.given_name.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()) 
                || Employee.family_name.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()));

            //create a list and place the IEnumerable's filtered list contents inside
            List<Employee> filteredlist = new List<Employee>();
            filteredlist = ielist.ToList();


            //return the filtered list
            return filteredlist;
        }

        /// <summary>
        /// Filters a List of Employees by Category.
        /// </summary>
        /// <param name="unfilteredlist">List of Employee's to search through</param>
        /// <param name="searchTerm">The exact Category enum to match</param>
        /// <returns>List of Employee class objects</returns>
        public List<Employee> FilterEmployeeListByCategory(List<Employee> unfilteredlist, Category searchTerm)
        {
            IEnumerable<Employee> ielist = unfilteredlist.Where(Employee => Employee.category == searchTerm);
            List<Employee> filteredlist = new List<Employee>();
            filteredlist = ielist.ToList();
            return filteredlist;
        }

        /// <summary>
        /// Filter a List of Units that by unit code and title that partialy match the search term.
        /// </summary>
        /// <param name="unfilteredlist">List of Units to search through</param>
        /// <param name="searchTerm">Search term for partial match</param>
        /// <returns>List of Unit class objects</returns>
        public List<Unit> FilterUnitListByTitleCode(List<Unit> unfilteredlist, string searchTerm)
        {
            IEnumerable<Unit> ielist = unfilteredlist.Where(Unit => 
                Unit.title.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()) 
                || Unit.unitcode.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()));
            List<Unit> filteredlist = new List<Unit>();
            filteredlist = ielist.ToList();
            return filteredlist;
        }

        /// <summary>
        /// Filters a List of Units by a Staff ID.
        /// </summary>
        /// <param name="unfilteredlist">List of Units to search through</param>
        /// <param name="idgiven">The exact staff id to match</param>
        /// <returns>List of Unit class objects</returns>
        public List<Unit> FilterUnitListByStaffId(List<Unit> unfilteredlist, int idgiven)
        {
            IEnumerable<Unit> ielist = unfilteredlist.Where(Unit => Unit.coordinator==idgiven );
            List<Unit> filteredlist = new List<Unit>();
            filteredlist = ielist.ToList();
            return filteredlist;
        }

        /// <summary>
        /// Filters a List of ClassDetails by Campus
        /// </summary>
        /// <param name="unfilteredlist">List of ClassDetails to search through</param>
        /// <param name="searchTerm">The Exact Campus enum to match</param>
        /// <returns>List of ClassDetails</returns>
        public static List<Classdetails> FilterClassListByCampus(List<Classdetails> unfilteredlist, Campus searchTerm)
        {
            IEnumerable<Classdetails> ielist = unfilteredlist.Where(Classdetails => Classdetails.campus == searchTerm);
            List<Classdetails> filteredlist = new List<Classdetails>();
            filteredlist = ielist.ToList();
            return filteredlist;
        }

        public List<Classdetails> GetTimeTable(string unitCodeGiven)
        {
            List<Classdetails> timTable = database.LoadClassDetailsList(unitCodeGiven);

            foreach (Classdetails CD in timTable)
            {
                Employee temp = (from Employee emplo in MasterStaffList
                                 where emplo.staffid == CD.staffid
                                 select emplo).Single();
                CD.displayName = temp.title.ToString() + " " + temp.family_name.ToString();
            }
            return timTable;
        }


        /// <summary>
        /// Calculates the current Availability of a Staff member based on the current operating systems time, and the Staff members schedule.
        /// </summary>
        /// <param name="staffid">Exact staff id to check availability for</param>
        /// <returns>Availability enum</returns>
        public static Availability CheckAvailability(int staffid)
        {
            Availability current = Availability.Free;

            DateTime now = DateTime.Now;
            List<Classdetails> teaching = new List<Classdetails>();
            teaching = database.LoadClassDetailsList((staffid).ToString());

            if (teaching != null)
            {

                foreach (Classdetails c in teaching)
                {

                    if (now.DayOfWeek == c.day &&
                now.TimeOfDay >= c.starttime &&
                now.TimeOfDay < c.endtime)
                    {
                        current = Availability.Teaching;
                    }
                }
            }
            if (current == Availability.Free)
            {
                List<Consultation> consultating = new List<Consultation>();
                consultating = database.LoadConsultation(staffid);

                if (consultating != null)
                {
                   
                    foreach (Consultation c in consultating)
                    {
                        if (now.DayOfWeek == c.day &&
                      now.TimeOfDay >= c.starttime &&
                      now.TimeOfDay < c.endtime)
                        {
                            current = Availability.Consulting;
                        }
                    }
                }
            }

            return current;
        }


    }
}