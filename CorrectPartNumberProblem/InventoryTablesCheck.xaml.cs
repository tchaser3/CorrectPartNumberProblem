/* Title:           Inventory Tables Check
 * Date:            1-27-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to check the tables for duplicate part numbers*/

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
using InventoryDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using PartNumberDLL;

namespace CorrectPartNumberProblem
{
    /// <summary>
    /// Interaction logic for InventoryTablesCheck.xaml
    /// </summary>
    public partial class InventoryTablesCheck : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        

        //Setting up the data sets
        InventoryAdjustmentDataSet TheInventoryAdjustmentDataSet = new InventoryAdjustmentDataSet();
        FindPartsWarehousesDataSet TheFindPartsWarehouseDataSet = new FindPartsWarehousesDataSet();
        InventoryDataSet TheInventoryDataSet;
        InventoryDataSet TheSearchedInventoryDataSet = new InventoryDataSet();
        WarehouseInventoryDataSet TheWarehouseInventoryDataSet;
        WarehouseInventoryDataSet TheSearchedWarehouseInventoryDataSet = new WarehouseInventoryDataSet();
        PartNumbersDataSet TheSortedPartNumbersDataSet = new PartNumbersDataSet();

        public InventoryTablesCheck()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindPartsWarehouseDataSet = TheEmployeeClass.FindPartsWarehouses();

                //setting up the number of records
                intNumberOfRecords = TheFindPartsWarehouseDataSet.FindPartsWarehouses.Rows.Count - 1;

                cboWarehouse.Items.Add("Select Warehouse");

                //loop
                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboWarehouse.Items.Add(TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboWarehouse.SelectedIndex = 0;

                dgrResults.ItemsSource = TheInventoryAdjustmentDataSet.adjustments;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Correct Part Number Problem Inventory Table Check Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this will load up the combo box
            //setting local variables
            string strWarehouseName;
            int intCounter;
            int intPartID = 0;
            int intNumberOfRecords;
            int intWarehouseID = 0;
            int intReturnedRecords = 0;

            //setting the text
            strWarehouseName = cboWarehouse.SelectedItem.ToString();
            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();
            //System.Threading.Thread.Sleep(1300);

            if (strWarehouseName != "Select Warehouse")
            {
                try
                {
                    TheSearchedWarehouseInventoryDataSet.WarehouseInventory.Rows.Clear();
                    TheSearchedInventoryDataSet.Inventory.Rows.Clear();

                    intNumberOfRecords = TheFindPartsWarehouseDataSet.FindPartsWarehouses.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if (strWarehouseName == TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].FirstName)
                        {
                            intWarehouseID = TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].EmployeeID;
                            break;
                        }
                    }

                    TheSearchedWarehouseInventoryDataSet = TheInventoryClass.GetWarehouseInventoryByWarehouseID(intWarehouseID);

                    intNumberOfRecords = TheSearchedWarehouseInventoryDataSet.WarehouseInventory.Rows.Count - 1; ;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if(TheSearchedWarehouseInventoryDataSet.WarehouseInventory[intCounter].IsTablePartIDNull() == false)
                        {
                            intPartID = TheSearchedWarehouseInventoryDataSet.WarehouseInventory[intCounter].TablePartID;

                            TheSortedPartNumbersDataSet = ThePartNumberClass.GetPartNumberByPartID(intPartID);

                            intReturnedRecords = TheSortedPartNumbersDataSet.partnumbers.Rows.Count;

                            if (intReturnedRecords > 0)
                            {
                                TheSearchedWarehouseInventoryDataSet.WarehouseInventory[intCounter].PartDescription = TheSortedPartNumbersDataSet.partnumbers[0].Description;
                            }
                        }
                    }

                    dgrResults.ItemsSource = TheSearchedWarehouseInventoryDataSet.WarehouseInventory;
                }
                catch (Exception Ex)
                {
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Correct Part Number Problem Inventory Tables Check Combo Box changed index " + Ex.Message);

                    TheMessagesClass.ErrorMessage(Ex.Message);
                }

            }

            PleaseWait.Hide();
        }
    }
}
