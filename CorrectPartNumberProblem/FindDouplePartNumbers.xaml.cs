/* Title:       Find Duplicate Part Numbers
 * Date:        1-19-17
 * Author:      Terry Holmes
 * 
 * Description: This is the find duplicate part number from */

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
using NewEventLogDLL;
using PartNumberDLL;
using KeyWordDLL;

namespace CorrectPartNumberProblem
{
    /// <summary>
    /// Interaction logic for FindDouplePartNumbers.xaml
    /// </summary>
    public partial class FindDouplePartNumbers : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();
        PleaseWait PleaseWait = new PleaseWait();

        //setting up the dataset
        PartNumbersDataSet ThePartNumbersDataSet;
        DuplicatePartsDataSet TheDuplicateDataSet = new DuplicatePartsDataSet();
        DuplicatePartsDataSet TheSortedDuplicateDataSet = new DuplicatePartsDataSet();

        //setting up the global variables
        int gintDuplicateCounter;
        int gintDuplicateUpperLimit;
        int gintPartNumberOfRecords;

        string gstrSearchSelection;

        public FindDouplePartNumbers()
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
                PleaseWait.Show();

                ThePartNumbersDataSet = ThePartNumberClass.GetPartNumbersInfo();

                gintPartNumberOfRecords = ThePartNumbersDataSet.partnumbers.Rows.Count - 1;
                gintDuplicateCounter = 0;
                gintDuplicateUpperLimit = 0;

                FindDuplicatePartNumbers();

                gstrSearchSelection = "Part Number";

                dgrResults.ItemsSource = TheDuplicateDataSet.parts;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Correct Part Number Problem Find Double Part Number Grid Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Hide();
        }
        private void FindDuplicateDescriptions()
        {

        }
        private void FindDuplicateJDEPartNumbers()
        {
            int intFirstCounter;
            int intSecondCounter;
            int intDuplicateCounter;
            int intPartID;
            string strJDEPartNumber;
            bool blnAddItem = false;
            bool blnItemFound = false;

            gintDuplicateCounter = 0;
            gintDuplicateUpperLimit = 0;
            cboPartNumbers.Items.Add("Select JDE Part");

            try
            {
                for(intFirstCounter = 0; intFirstCounter <= gintPartNumberOfRecords; intFirstCounter++)
                {
                    strJDEPartNumber = ThePartNumbersDataSet.partnumbers[intFirstCounter].JDEPartNumber;
                    intPartID = ThePartNumbersDataSet.partnumbers[intFirstCounter].PartID;
                    blnAddItem = false;

                    if(strJDEPartNumber != "NOT PROVIDED") 
                    {
                        if(strJDEPartNumber != "NOT REQUIRED")
                        {
                            //second counter;
                            for (intSecondCounter = 0; intSecondCounter <= gintPartNumberOfRecords; intSecondCounter++)
                            {
                                blnItemFound = false;

                                if (strJDEPartNumber == ThePartNumbersDataSet.partnumbers[intSecondCounter].JDEPartNumber)
                                {
                                    if (intPartID != ThePartNumbersDataSet.partnumbers[intSecondCounter].PartID)
                                    {
                                        //checking the duplicates
                                        if (gintDuplicateCounter > 0)
                                        {
                                            for (intDuplicateCounter = 0; intDuplicateCounter <= gintDuplicateUpperLimit; intDuplicateCounter++)
                                            {
                                                if (ThePartNumbersDataSet.partnumbers[intSecondCounter].PartID == TheDuplicateDataSet.parts[intDuplicateCounter].PartID)
                                                {
                                                    blnItemFound = true;
                                                }
                                            }
                                        }

                                        if (blnItemFound == false)
                                        {
                                            DuplicatePartsDataSet.partsRow NewPartsRow = TheDuplicateDataSet.parts.NewpartsRow();

                                            NewPartsRow.Description = ThePartNumbersDataSet.partnumbers[intSecondCounter].Description;
                                            NewPartsRow.JDEPartNumber = ThePartNumbersDataSet.partnumbers[intSecondCounter].JDEPartNumber;
                                            NewPartsRow.PartID = ThePartNumbersDataSet.partnumbers[intSecondCounter].PartID;
                                            NewPartsRow.PartNumber = ThePartNumbersDataSet.partnumbers[intSecondCounter].PartNumber;
                                            NewPartsRow.Keep = false;
                                            NewPartsRow.Discard = false;

                                            TheDuplicateDataSet.parts.Rows.Add(NewPartsRow);
                                            gintDuplicateUpperLimit = gintDuplicateCounter;
                                            gintDuplicateCounter++;
                                            blnAddItem = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if(blnAddItem == true)
                    {
                        blnItemFound = false;

                        for (intDuplicateCounter = 0; intDuplicateCounter <= gintDuplicateUpperLimit; intDuplicateCounter++)
                        {
                            if (ThePartNumbersDataSet.partnumbers[intFirstCounter].PartID == TheDuplicateDataSet.parts[intDuplicateCounter].PartID)
                            {
                                blnItemFound = true;
                            }
                        }

                        if (blnItemFound == false)
                        {
                            DuplicatePartsDataSet.partsRow NewPartsRow = TheDuplicateDataSet.parts.NewpartsRow();

                            NewPartsRow.Description = ThePartNumbersDataSet.partnumbers[intFirstCounter].Description;
                            NewPartsRow.JDEPartNumber = ThePartNumbersDataSet.partnumbers[intFirstCounter].JDEPartNumber;
                            NewPartsRow.PartID = ThePartNumbersDataSet.partnumbers[intFirstCounter].PartID;
                            NewPartsRow.PartNumber = ThePartNumbersDataSet.partnumbers[intFirstCounter].PartNumber;
                            NewPartsRow.Keep = false;
                            NewPartsRow.Discard = false;
                            cboPartNumbers.Items.Add(ThePartNumbersDataSet.partnumbers[intFirstCounter].JDEPartNumber);

                            TheDuplicateDataSet.parts.Rows.Add(NewPartsRow);
                            gintDuplicateUpperLimit = gintDuplicateCounter;
                            gintDuplicateCounter++;
                            blnAddItem = true;
                        }
                    }
                }

                tblSelect.Text = "Description";
                cboPartNumbers.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Correct Part Numbers Problem Find Duplicate JDE Part Numbers " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void FindDuplicatePartNumbers()
        {
            //setting local variables
            int intFirstCounter;
            int intSecondCounter;
            int intSecondNumberOfRecords;
            int intDuplicateCounter;
            string strPartNumber;
            int intPartID;
            bool blnKeyWordNotFound;
            bool blnItemFound;
            bool blnAddItem;

            try
            {
                intSecondNumberOfRecords = gintPartNumberOfRecords;

                cboPartNumbers.Items.Add("Select Part Number");

                gstrSearchSelection = "Part";

                //first loop
                for(intFirstCounter = 0; intFirstCounter <= gintPartNumberOfRecords; intFirstCounter++)
                {
                    intPartID = ThePartNumbersDataSet.partnumbers[intFirstCounter].PartID;
                    strPartNumber = ThePartNumbersDataSet.partnumbers[intFirstCounter].PartNumber;
                    blnAddItem = false;

                    //second loop
                    for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                    {
                        blnItemFound = false;

                        blnKeyWordNotFound = TheKeyWordClass.FindKeyWord(strPartNumber, ThePartNumbersDataSet.partnumbers[intSecondCounter].PartNumber);

                        if(blnKeyWordNotFound == false)
                        {
                            if(intPartID != ThePartNumbersDataSet.partnumbers[intSecondCounter].PartID)
                            {
                                blnAddItem = true;

                                if (gintDuplicateCounter > 0)
                                {
                                    for (intDuplicateCounter = 0; intDuplicateCounter <= gintDuplicateUpperLimit; intDuplicateCounter++)
                                    {
                                        if (ThePartNumbersDataSet.partnumbers[intSecondCounter].PartID == TheDuplicateDataSet.parts[intDuplicateCounter].PartID)
                                        {
                                            blnItemFound = true;
                                        }
                                    }
                                }

                                if (blnItemFound == false)
                                {
                                    DuplicatePartsDataSet.partsRow SecondPartsRow = TheDuplicateDataSet.parts.NewpartsRow();

                                    SecondPartsRow.Description = ThePartNumbersDataSet.partnumbers[intSecondCounter].Description;
                                    SecondPartsRow.JDEPartNumber = ThePartNumbersDataSet.partnumbers[intSecondCounter].JDEPartNumber;
                                    SecondPartsRow.PartID = ThePartNumbersDataSet.partnumbers[intSecondCounter].PartID;
                                    SecondPartsRow.PartNumber = ThePartNumbersDataSet.partnumbers[intSecondCounter].PartNumber;
                                    SecondPartsRow.Keep = false;
                                    SecondPartsRow.Discard = false;
                                   
                                    TheDuplicateDataSet.parts.Rows.Add(SecondPartsRow);
                                    gintDuplicateUpperLimit = gintDuplicateCounter;
                                    gintDuplicateCounter++;
                                    
                                }
                            }
                        }
                    }

                    if(blnAddItem == true)
                    {
                        blnItemFound = false;

                        if(gintDuplicateCounter >0)
                        {
                            for (intDuplicateCounter = 0; intDuplicateCounter <= gintDuplicateUpperLimit; intDuplicateCounter++)
                            {
                                if (ThePartNumbersDataSet.partnumbers[intFirstCounter].PartID == TheDuplicateDataSet.parts[intDuplicateCounter].PartID)
                                {
                                    blnItemFound = true;
                                }
                            }
                        }

                        if(blnItemFound == false)
                        {
                            DuplicatePartsDataSet.partsRow NewPartsRow = TheDuplicateDataSet.parts.NewpartsRow();

                            NewPartsRow.Description = ThePartNumbersDataSet.partnumbers[intFirstCounter].Description;
                            NewPartsRow.JDEPartNumber = ThePartNumbersDataSet.partnumbers[intFirstCounter].JDEPartNumber;
                            NewPartsRow.PartID = ThePartNumbersDataSet.partnumbers[intFirstCounter].PartID;
                            NewPartsRow.PartNumber = ThePartNumbersDataSet.partnumbers[intFirstCounter].PartNumber;
                            NewPartsRow.Keep = false;
                            NewPartsRow.Discard = false;
                            cboPartNumbers.Items.Add(ThePartNumbersDataSet.partnumbers[intFirstCounter].PartNumber);

                            TheDuplicateDataSet.parts.Rows.Add(NewPartsRow);
                            gintDuplicateUpperLimit = gintDuplicateCounter;
                            gintDuplicateCounter++;
                        }
                    }
                }

                cboPartNumbers.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Correct Part Number Problem Find Duplicate Part Numbers " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboPartNumbers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strPartNumber;
            int intCounter;
            int intNumberOfRecords;
            bool blnKeyWordNotFound;

            try
            {
                if (cboPartNumbers.SelectedItem != null)
                {
                    strPartNumber = cboPartNumbers.SelectedItem.ToString();

                    if (strPartNumber != "Select Part Number")
                    {
                        //getting the number of records
                        intNumberOfRecords = TheDuplicateDataSet.parts.Rows.Count - 1;

                        TheSortedDuplicateDataSet.parts.Rows.Clear();

                        if (gstrSearchSelection == "Part Number")
                        {
                            //loop
                            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                blnKeyWordNotFound = TheKeyWordClass.FindKeyWord(strPartNumber, TheDuplicateDataSet.parts[intCounter].PartNumber);

                                if (blnKeyWordNotFound == false)
                                {
                                    DuplicatePartsDataSet.partsRow NewTableRow = TheSortedDuplicateDataSet.parts.NewpartsRow();

                                    NewTableRow.JDEPartNumber = TheDuplicateDataSet.parts[intCounter].JDEPartNumber;
                                    NewTableRow.Keep = TheDuplicateDataSet.parts[intCounter].Keep;
                                    NewTableRow.PartID = TheDuplicateDataSet.parts[intCounter].PartID;
                                    NewTableRow.Description = TheDuplicateDataSet.parts[intCounter].Description;
                                    NewTableRow.Discard = TheDuplicateDataSet.parts[intCounter].Discard;
                                    NewTableRow.PartNumber = TheDuplicateDataSet.parts[intCounter].PartNumber;

                                    TheSortedDuplicateDataSet.parts.Rows.Add(NewTableRow);
                                }
                            }
                        }
                        else if(gstrSearchSelection == "JDE Parts")
                        {
                            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                blnKeyWordNotFound = TheKeyWordClass.FindKeyWord(strPartNumber, TheDuplicateDataSet.parts[intCounter].JDEPartNumber);

                                if (blnKeyWordNotFound == false)
                                {
                                    DuplicatePartsDataSet.partsRow NewTableRow = TheSortedDuplicateDataSet.parts.NewpartsRow();

                                    NewTableRow.JDEPartNumber = TheDuplicateDataSet.parts[intCounter].JDEPartNumber;
                                    NewTableRow.Keep = TheDuplicateDataSet.parts[intCounter].Keep;
                                    NewTableRow.PartID = TheDuplicateDataSet.parts[intCounter].PartID;
                                    NewTableRow.Description = TheDuplicateDataSet.parts[intCounter].Description;
                                    NewTableRow.Discard = TheDuplicateDataSet.parts[intCounter].Discard;
                                    NewTableRow.PartNumber = TheDuplicateDataSet.parts[intCounter].PartNumber;

                                    TheSortedDuplicateDataSet.parts.Rows.Add(NewTableRow);
                                }
                            }
                        }
                        

                        dgrResults.ItemsSource = TheSortedDuplicateDataSet.parts;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Correct Part Number Problem Find Duplicate Part Numbers Combo Selection Changes " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        

                
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if(gstrSearchSelection == "Part Number")
            {
                TheDuplicateDataSet.parts.Rows.Clear();

                cboPartNumbers.Items.Clear();

                FindDuplicateJDEPartNumbers();

                dgrResults.ItemsSource = TheDuplicateDataSet.parts;

                gstrSearchSelection = "JDE Parts";

            }
        }
    }
}
