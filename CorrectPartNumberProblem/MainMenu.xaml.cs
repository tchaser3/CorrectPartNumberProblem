/* title:           Main Menu
 * Date:            1-18-17
 * Author:          Terry Holmes
 * 
 * Description:     This is the main menu */

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


namespace CorrectPartNumberProblem
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public MainMenu()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutBox AboutBox = new AboutBox();
            AboutBox.ShowDialog();
        }

        private void btnMasterList_Click(object sender, RoutedEventArgs e)
        {
            MasterPartNumbers MasterPartNumbers = new MasterPartNumbers();
            MasterPartNumbers.Show();
            this.Close();
        }

        private void btnPartProblem_Click(object sender, RoutedEventArgs e)
        {
            FindDouplePartNumbers FindDouplePartNumbers = new FindDouplePartNumbers();
            FindDouplePartNumbers.Show();
            this.Close();
        }

        private void btnAdjustInventory_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.UnderDevelopment();
        }

        private void btnCheckInventoryTables_Click(object sender, RoutedEventArgs e)
        {
            InventoryTablesCheck InventoryTablesCheck = new InventoryTablesCheck();
            InventoryTablesCheck.Show();
            this.Close();
        }
    }
}
