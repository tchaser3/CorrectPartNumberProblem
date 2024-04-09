/* Title:           Correct Part Number Problem
 * Date:            1-14-17
 * Author:          Terry Holmes
 * 
 * Description:     This program is for correcting the Part Number Problem */

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
using NewEventLogDLL;
using NewEmployeeDLL;
using DataValidationDLL;

namespace CorrectPartNumberProblem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting global data variables
        public static VerifyLogonDataSet TheVerifyLogonDataSet = new VerifyLogonDataSet();

        int gintNumberOfMisses;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            gintNumberOfMisses = 0;
        }
        private void LogonFailed()
        {
            gintNumberOfMisses++;

            if(gintNumberOfMisses == 3)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Correct Part Number Problem has Three Attempts to Logon");

                TheMessagesClass.ErrorMessage("There Have Three Attempts to logon, the Application will now Close");

                Application.Current.Shutdown();
            }
            else
            {
                TheMessagesClass.InformationMessage("The Logon Information is In Correct or You are not an Administrator");
                return;
            }
        }

        private void btnLogon_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intRecordsReturned;
            string strValueForValidation;
            int intEmployeeID = 0;
            string strLastName;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";

            //performing data validation
            strValueForValidation = pbxEmployeeID.Password;
            blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
            if(blnFatalError == true)
            {
                blnThereIsAProblem = true;
                strErrorMessage = strErrorMessage + "The Employee ID is not an Integer\n";
            }
            else
            {
                intEmployeeID = Convert.ToInt32(strValueForValidation);
            }
            strLastName = txtLastName.Text;
            blnFatalError = TheDataValidationClass.VerifyTextData(strLastName);
            if (blnFatalError == true)
            {
                blnThereIsAProblem = true;
                strErrorMessage = strErrorMessage + "The Last Name was not Entered\n";
            }
            if(blnThereIsAProblem == true)
            {
                TheMessagesClass.ErrorMessage(strErrorMessage);
                return;
            }

            //getting the data set
            TheVerifyLogonDataSet = TheEmployeeClass.VerifyLogon(intEmployeeID, strLastName);

            intRecordsReturned = TheVerifyLogonDataSet.VerifyLogon.Rows.Count;

            if(intRecordsReturned == 0)
            {
                LogonFailed();
            }
            else
            {
                if(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup != "ADMIN")
                {
                    LogonFailed();
                }
                else
                {
                    MainMenu MainMenu = new MainMenu();
                    MainMenu.Show();
                    this.Hide();
                }
            }
        }
    }
}
