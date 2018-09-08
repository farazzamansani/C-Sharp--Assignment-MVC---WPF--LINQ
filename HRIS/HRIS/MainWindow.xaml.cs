using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using HRIS.Control;
using HRIS.Entities;
using HRIS.Database;

namespace HRIS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Listcontroler Lists = new Listcontroler();

        public MainWindow()
        {
            InitializeComponent();
            
            //Setup the Staff TAB...
            Staff_List.ItemsSource = Lists.MasterStaffList;             //populate staff list display
            Staff_Details.back_button_visibility = Visibility.Collapsed;//default user control for this tab needs no back button
            Staff_Details.alternative_user_control = Staff_TimeTable;   //hand over its replacement user controll
            Staff_TimeTable.back_button_visibility = Visibility.Visible;//Replacement user controll needs a back button
            Staff_TimeTable.alternative_user_control = Staff_Details;   //hand over its replacement user controll
            foreach (var cat in Enum.GetValues(typeof(Category)))       //setup category filter
            {
                //the XAML for the combo box contains selection 0 "ALL" and the enum does not
                CatFilter.Items.Add(cat);           //so add each item manually on the end
            }

            //Setup the Units Tab...
            Units_List.ItemsSource = Lists.MasterUnitList;              //populate unit list display
            Units_TimeTable.back_button_visibility = Visibility.Collapsed;//default user control for this tab needs no back button..
            Units_TimeTable.alternative_user_control = Units_StaffDetails;//hand over its replacement user controll
            Units_StaffDetails.back_button_visibility = Visibility.Visible;//Replacement user controll needs a back button
            Units_StaffDetails.alternative_user_control = Units_TimeTable;//hand over its replacement user controll
        }

        /*  User Clicks on Staff List
         * */
        private void Staff_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Staff_List.SelectedIndex != -1)
            {
                //get employee id from selection and send to list controller to get full details
                Employee item = (Employee)Staff_List.SelectedItem;
                Staff_Details.DataContext = Lists.GetStaffDetails(item.staffid);

                //populate list of units staff member is involved with
                Staff_Details.unitsInvolved = Lists.FilterUnitListByStaffId(Lists.MasterUnitList, item.staffid);

                //populate list of consultation times
                Staff_Details.consultationTimes = database.LoadConsultation(item.staffid);

                //hand the accoiated timetable user control to use with a unit click on the Staff_Details user controll
                Staff_Details.alternative_user_control = Staff_TimeTable;

                //hand it the list controller so it can populate that Unit TimeTable with information.
                Staff_Details.Lists = Lists;

                //Reste the visibiliy of controlls
                Staff_Details.Visibility = Visibility.Visible;
                Staff_TimeTable.Visibility = Visibility.Collapsed;
            }
            else
            {
                //no selected staff in list so hide staff details
                Staff_Details.Visibility = Visibility.Collapsed;
            }
        }

        private void Seach_Staff_KeyUp(object sender, KeyEventArgs e)
        {
            Staff_List.ItemsSource = Lists.FilterEmployeeListByName(Lists.MasterStaffList, Seach_Staff.Text);
            //also refine by category if selected
            if (CatFilter.SelectedIndex > 0)
            {
                Staff_List.ItemsSource = Lists.FilterEmployeeListByCategory((List<Employee>)Staff_List.ItemsSource, (Category)CatFilter.SelectedIndex);
            }
        }

        private void CatFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Staff_List != null)//prevent this code from running during construction when the Staff_List doesnt exist yet
            {
                //Get selected index
                int selInt = CatFilter.SelectedIndex;
                //get selection as Category enum
                Category selCat = (Category)selInt;

                //only filter list if All is not selected
                if (selInt != 0)
                {
                    //update Staff List with filtered result
                    Staff_List.ItemsSource = Lists.FilterEmployeeListByCategory(Lists.MasterStaffList, selCat);
                    //and also refine search by name
                    Staff_List.ItemsSource = Lists.FilterEmployeeListByName((List<Employee>)Staff_List.ItemsSource, Seach_Staff.Text);
                }
                else
                {
                    //When ALL is selected display the complete list of staff
                    Staff_List.ItemsSource = Lists.FilterEmployeeListByName(Lists.MasterStaffList,Seach_Staff.Text);
                }

            }
        }

        private void Search_Units_KeyUp(object sender, KeyEventArgs e)
        {
            Units_List.ItemsSource = Lists.FilterUnitListByTitleCode(Lists.MasterUnitList, Search_Units.Text);
        }

        /// <summary>
        ///  User clicks on unit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Units_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Units_List.SelectedIndex != -1)
            {
                //get unit code from selection and send to list controller to get full details
                Unit item = (Unit)Units_List.SelectedItem;

                //populate timetable user control
                Units_TimeTable.TimeTable = Lists.GetTimeTable(item.unitcode);
                
                //hand over the StaffDetails user control for it to use
                Units_TimeTable.alternative_user_control = Units_StaffDetails;

                //Hand over current list controller instance
                Units_TimeTable.Lists = Lists;

                //Reste the visibiliy of controlls
                Units_TimeTable.Visibility = Visibility.Visible;
                Units_StaffDetails.Visibility = Visibility.Collapsed;
            }
            else
            {
                //no selected staff in list so hide staff details
                //*DISABLED THIS FEATURE FOR NOW (could be bad for the cra)* Staff_Details.Visibility = Visibility.Collapsed;
            }
        }

    }
}
