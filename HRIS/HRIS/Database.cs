using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRIS.Entities;

using MySql.Data.MySqlClient;
using MySql.Data.Types;


namespace HRIS.Database
{

    abstract class database
    {

        //According to the class diagram of Bradley's group, the sort method for the list of staff is in 

        private const string host = "kit206";
        private const string user = "kit206";
        private const string pass = "kit206";
        private const string server = "alacritas.cis.utas.edu.au";

        private static MySqlConnection conn = null;

        /// <summary>
        /// turn a string into an enum
        /// </summary>
        /// <typeparam name="T">type of the enum</typeparam>
        /// <param name="value">string value</param>
        /// <returns>an enum</returns>
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// create a MySql connection
        /// </summary>
        /// <returns>connection</returns>
        private static MySqlConnection GetConnection()
        {
            if (conn == null)
            {
                string connectionString = String.Format("Database={0};Data Source={1};User Id={2};Password={3}", host, server, user, pass);
                conn = new MySqlConnection(connectionString);
            }
            return conn;
        }

        /// <summary>
        /// load id, title, given name, family name, title, caterory of all staffs
        /// </summary>
        /// <returns>a list of all staffs</returns>
        public static List<Employee> LoadStaffList()
        {

            List<Employee> staff = new List<Employee>();

            MySqlConnection conn = GetConnection();
            MySqlDataReader rdr = null;

            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("select id, given_name, family_name, title, category from staff order by family_name, given_name", conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    staff.Add(new Employee
                    {
                        staffid = rdr.GetInt32(0),
                        given_name = rdr.GetString(1),
                        family_name = rdr.GetString(2),
                        title = rdr.GetString(3),
                        category = ParseEnum<Category>(rdr.GetString(4))
                    });
                }
            }
            catch (MySqlException e)
            {
                System.Windows.MessageBox.Show("Error connecting to database: " + e);
            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return staff;

        }

        /// <summary>
        /// load all details of one staff
        /// </summary>
        /// <param name="id">staff id</param>
        /// <returns>one Employee</returns>
        public static Employee LoadOneStaff(int id)
        {
            Employee staff = new Employee();

            MySqlConnection conn = GetConnection();
            MySqlDataReader rdr = null;

            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("select * from staff where id=?id", conn);
                cmd.Parameters.AddWithValue("id", id);
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    staff.staffid = rdr.GetInt32(0);
                    staff.given_name = rdr.GetString(1);
                    staff.family_name = rdr.GetString(2);
                    staff.title = rdr.GetString(3);
                    staff.campus = ParseEnum<Campus>(rdr.GetString(4));
                    staff.phone = rdr.GetString(5);
                    staff.room = rdr.GetString(6);
                    staff.email = rdr.GetString(7);
                    staff.photo = rdr.GetString(8);
                    staff.category = ParseEnum<Category>(rdr.GetString(9));
                }
            }
            catch (MySqlException e)
            {
                System.Windows.MessageBox.Show("Error connecting to database: " + e);
            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return staff;

        }

        /// <summary>
        /// load all consultation time of one staff
        /// </summary>
        /// <param name="id">staff id</param>
        /// <returns>a list of consultation</returns>
        public static List<Consultation> LoadConsultation(int id)
        {
            List<Consultation> time = new List<Consultation>();

            MySqlConnection conn = GetConnection();
            MySqlDataReader rdr = null;

            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("select * from consultation where staff_id=?id", conn);
                cmd.Parameters.AddWithValue("id", id);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    time.Add(new Consultation
                    {
                        staffid = rdr.GetInt32(0),
                        day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), (rdr.GetString(1))),
                        starttime = TimeSpan.Parse((rdr.GetString(2))),
                        endtime = TimeSpan.Parse((rdr.GetString(3)))
                    });
                }
            }
            catch (MySqlException e)
            {
                System.Windows.MessageBox.Show("Error connecting to database: " + e);
            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return time;

        }


        /// <summary>
        /// load all information for all units
        /// </summary>
        /// <returns>a list units</returns>
        public static List<Unit> LoadUnitList()
        {
            List<Unit> unit = new List<Unit>();

            MySqlConnection conn = GetConnection();
            MySqlDataReader rdr = null;

            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("select * from unit order by code", conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    unit.Add(new Unit
                    {
                        unitcode = rdr.GetString(0),
                        title = rdr.GetString(1),
                        coordinator = rdr.GetInt32(2)

                    });
                }
            }
            catch (MySqlException e)
            {
                System.Windows.MessageBox.Show("Error connecting to database: " + e);
                
            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return unit;

        }

        /// <summary>
        /// load all details for all classes for one staff or one unit 
        /// </summary>
        /// <param name="id">can be unit code or staff id</param>
        /// <returns></returns>
        public static List<Classdetails> LoadClassDetailsList(String id)
        {
            List<Classdetails> classDetailsList = new List<Classdetails>();

            MySqlConnection conn = GetConnection();
            MySqlDataReader rdr = null;

            try
            {
                conn.Open();

                if (id.All(char.IsDigit) == false)
                {
                    MySqlCommand cmd = new MySqlCommand("select * from class where unit_code=?id order by day, start", conn);
                    cmd.Parameters.AddWithValue("id", id);
                    rdr = cmd.ExecuteReader();
                }
                else
                {
                    MySqlCommand cmd = new MySqlCommand("select * from class where staff=?id order by day, start", conn);
                    cmd.Parameters.AddWithValue("id", Convert.ToInt32(id));
                    rdr = cmd.ExecuteReader();
                }


                while (rdr.Read())
                {
                    classDetailsList.Add(new Classdetails
                    {
                        unitcode = rdr.GetString(0),
                        campus = ParseEnum<Campus>(rdr.GetString(1)),
                        day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), (rdr.GetString(2))),
                        starttime = TimeSpan.Parse((rdr.GetString(3))),
                        endtime = TimeSpan.Parse((rdr.GetString(4))),
                        classtype = ParseEnum<Classtype>(rdr.GetString(5)),
                        room = rdr.GetString(6),
                        staffid = rdr.GetInt32(7)
                    });
                }

            }
            catch (MySqlException e)
            {
                System.Windows.MessageBox.Show("Error connecting to database: " + e);
            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return classDetailsList;
        }

       
    }
}