/* Title:           Master Part Number
 * Date:            1-18-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is for the master part numbers */

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
using System.Windows.Shapes;
using PartNumberDLL;
using InventoryDLL;
using NewEventLogDLL;

namespace CorrectPartNumberProblem
{
    /// <summary>
    /// Interaction logic for MasterPartNumbers.xaml
    /// </summary>
    public partial class MasterPartNumbers : Window
    {
        //setting the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        PleaseWait PleaseWait = new PleaseWait();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        InventoryDataSet TheInventoryDataSet = new InventoryDataSet();
        WarehouseInventoryDataSet TheWarehouseInventoryDataSet = new WarehouseInventoryDataSet();
        PartNumbersDataSet ThePartNumbersDataSet;

        //setting global variables
        int gintPartUpperLimit;

        public MasterPartNumbers()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            this.Close();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ThePartNumbersDataSet = ThePartNumberClass.GetPartNumbersInfo();

                gintPartUpperLimit = ThePartNumbersDataSet.partnumbers.Rows.Count - 1;

                dgrParts.ItemsSource = ThePartNumbersDataSet.partnumbers;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Correct Part Number Problem Master Part Numbers Grid Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intRecordsReturned;
            int intPartID;
            bool blnRecordFound;
            int intRecordDeletedCount = 0;

            PleaseWait.Show();

            try
            {
                for(intCounter = 0; intCounter <= gintPartUpperLimit; intCounter++)
                {
                    intPartID = ThePartNumbersDataSet.partnumbers[intCounter].PartID;

                    blnRecordFound = false;

                    TheWarehouseInventoryDataSet = TheInventoryClass.GetWarehouseCompleteInventoryByPartID(intPartID);

                    intRecordsReturned = TheWarehouseInventoryDataSet.WarehouseInventory.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        blnRecordFound = true;
                    }
                    else
                    {
                        TheInventoryDataSet = TheInventoryClass.FindInventoryPartByPartID(intPartID);

                        intRecordsReturned = TheInventoryDataSet.Inventory.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            blnRecordFound = true;
                        }
                    }

                    if(blnRecordFound == false)
                    {
                        ThePartNumbersDataSet.partnumbers.Rows[intCounter].Delete();
                        ThePartNumberClass.UpdatePartNumbersDB(ThePartNumbersDataSet);
                        intCounter--;
                        gintPartUpperLimit--;
                        intRecordDeletedCount++;
                    }
                }

                TheMessagesClass.InformationMessage(Convert.ToString(intRecordDeletedCount));
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Correct Part Number Problem Master Part List Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Hide();
        }
    }
}
