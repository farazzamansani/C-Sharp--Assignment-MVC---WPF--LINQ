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

using HRIS.Entities;
using HRIS.Database;
using HRIS.Control;

namespace HRIS
{
    /// <summary>
    /// Interaction logic for UnitTimeTable.xaml
    /// </summary>
    public partial class UnitTimeTable : UserControl
    {

        private List<Classdetails> UnfilteredTimeTable;
        public List<Classdetails> TimeTable
        {
            get { return (List<Classdetails>)this.TimeTable_Grid.ItemsSource; }

            //when this is changed from outside the control its when a different unit has been selected
            set { 
                this.TimeTable_Grid.ItemsSource = value;
                UnfilteredTimeTable = value; //so keep a reference for filtering within this User Control
            }
        }

        public StaffDetails alternative_user_control;

        public Listcontroler Lists;

        public Visibility back_button_visibility
        {
            get
            {
                return Back_Button.Visibility;
            }
            set
            {
                Back_Button.Visibility = value;
            }
        }

        public UnitTimeTable()
        {
            InitializeComponent();
            //Get Enum values for Campus Filter
            foreach (var camp in Enum.GetValues(typeof(Campus)))
            {
                //the XAML for the combo box contains selection 0 "ALL" and the enum does not
                this.CampFilter.Items.Add(camp);    //so add each item manually on the end
            }
        }

        private void CampFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TimeTable_Grid != null)
            {
                //Get selected index
                int selInt = CampFilter.SelectedIndex;
                //get selection as Category enum
                Campus selCamp = (Campus)selInt;
                if (selInt != 0)
                {
                    this.TimeTable_Grid.ItemsSource = Listcontroler.FilterClassListByCampus(UnfilteredTimeTable, selCamp);
                }
                else
                {
                    this.TimeTable_Grid.ItemsSource = UnfilteredTimeTable;
                }
            }
        }

        private void TimeTable_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //only execute if the staff members name was clicked is way too hard to impliment...
            // see http://blog.scottlogic.com/2008/12/02/wpf-datagrid-detecting-clicked-cell-and-row.html as to why.
            //so anywhere in the row will triger the event....

            if (TimeTable_Grid.SelectedIndex != -1 && Back_Button.Visibility == Visibility.Collapsed)
            {
                //get selected item
                Classdetails item = (Classdetails)TimeTable_Grid.SelectedItem;
                //populate staff details controll in the units tab
                alternative_user_control.DataContext = Lists.GetStaffDetails(item.staffid);
                //populate list of units staff member is involved with
                alternative_user_control.unitsInvolved = Lists.FilterUnitListByStaffId(Lists.MasterUnitList, item.staffid);
                //populate list of consultation times
                alternative_user_control.consultationTimes = database.LoadConsultation(item.staffid);
                //change the display...
                alternative_user_control.Visibility = Visibility.Visible;
                this.Visibility = Visibility.Collapsed;
                //reset selection so that after clicking back they can click the same unit again for the same result.
                TimeTable_Grid.SelectedIndex = -1;
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            alternative_user_control.Visibility = Visibility.Visible;
        }

    }
}
