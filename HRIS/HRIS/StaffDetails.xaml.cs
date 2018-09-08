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
using HRIS.Control;

namespace HRIS
{
    /// <summary>
    /// Interaction logic for StaffDetails.xaml
    /// </summary>
    public partial class StaffDetails : UserControl
    {
                public List<Unit> unitsInvolved
        {
            get { return (List<Unit>)unitsInvolved_Grid.ItemsSource; }
            set { unitsInvolved_Grid.ItemsSource = value; }
        }

        public UnitTimeTable alternative_user_control;

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

        public List<Consultation> consultationTimes
        {
            get { return (List<Consultation>)Consultation_Times_Grid.ItemsSource; }
            set { Consultation_Times_Grid.ItemsSource = value; }
        }

        public StaffDetails()
        {
            InitializeComponent();
        }

        private void unitsInvolved_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (unitsInvolved_Grid.SelectedIndex != -1 && Back_Button.Visibility == Visibility.Collapsed)
            {
                //get employee id from selection
                Unit item = (Unit)unitsInvolved_Grid.SelectedItem;
                //populate timetable with class details..
                alternative_user_control.TimeTable = Lists.GetTimeTable(item.unitcode);
                //change the display...
                alternative_user_control.Visibility = Visibility.Visible;
                this.Visibility = Visibility.Collapsed;
                //reset selection so that after clicking back they can click the same unit again for the same result.
                unitsInvolved_Grid.SelectedIndex = -1;
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            alternative_user_control.Visibility = Visibility.Visible;
        }
    }
}
