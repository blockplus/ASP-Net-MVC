using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//Include mysql client namespace.
using MySql.Data.MySqlClient;
using System.Configuration;

namespace CSharpMySqlSample
{
    public partial class frmMySqlSample : Form
    {
        //Read connection string from application settings file
        string   ConnectionString = ConfigurationSettings.AppSettings["ConnectionString"];
        MySqlConnection connection;
        MySqlDataAdapter adapter;
        DataTable DTItems;
        public frmMySqlSample()
        {
            InitializeComponent();
        }

        private void frmMySqlSample_Load(object sender, EventArgs e)
        {
            //Initialize mysql connection
            connection = new MySqlConnection(ConnectionString);

            //Get all items in datatable
            DTItems = GetAllItems();

            //Fill grid with items
            dataGridView1.DataSource = DTItems;
        }

        //Get all items from database into datatable
        DataTable GetAllItems()
        {
            try
            {
                //prepare query to get all records from items table
                string query = "select * from items";
                //prepare adapter to run query
                adapter = new MySqlDataAdapter(query, connection);
                DataSet DS = new DataSet();
                //get query results in dataset
                adapter.Fill(DS);

                // Set the UPDATE command and parameters.
                adapter.UpdateCommand = new MySqlCommand(
                    "UPDATE items SET ItemName=@ItemName, Price=@Price, AvailableQuantity=@AvailableQuantity, Updated_Dt=NOW() WHERE ItemNumber=@ItemNumber;",
                    connection);
                adapter.UpdateCommand.Parameters.Add("@ItemNumber", MySqlDbType.Int16, 4, "ItemNumber");
                adapter.UpdateCommand.Parameters.Add("@ItemName", MySqlDbType.VarChar, 100, "ItemName");
                adapter.UpdateCommand.Parameters.Add("@Price", MySqlDbType.Decimal, 10, "Price");
                adapter.UpdateCommand.Parameters.Add("@AvailableQuantity", MySqlDbType.Int16, 11, "AvailableQuantity");
                adapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;

                // Set the INSERT command and parameter.
                adapter.InsertCommand = new MySqlCommand(
                    "INSERT INTO items VALUES (@ItemNumber,@ItemName,@Price,@AvailableQuantity,NOW());",
                    connection);
                adapter.InsertCommand.Parameters.Add("@ItemNumber", MySqlDbType.Int16, 4, "ItemNumber");
                adapter.InsertCommand.Parameters.Add("@ItemName", MySqlDbType.VarChar, 100, "ItemName");
                adapter.InsertCommand.Parameters.Add("@Price", MySqlDbType.Decimal, 10, "Price");
                adapter.InsertCommand.Parameters.Add("@AvailableQuantity", MySqlDbType.Int16, 11, "AvailableQuantity");
                adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;

                // Set the DELETE command and parameter.
                adapter.DeleteCommand = new MySqlCommand(
                    "DELETE FROM items "
                    + "WHERE ItemNumber=@ItemNumber;", connection);
                adapter.DeleteCommand.Parameters.Add("@ItemNumber",
                  MySqlDbType.Int16, 4, "ItemNumber");
                adapter.DeleteCommand.UpdatedRowSource = UpdateRowSource.None;

                //return datatable with all records
                return DS.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Save records in database using DTItems which is datasource for Grid
                adapter.Update(DTItems);
                //Refresh grid
                DTItems = GetAllItems();
                dataGridView1.DataSource = DTItems;
                MessageBox.Show("Items saved successfully...");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                //Delete a row from grid first.
                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);

                //Save records again. This will delete record from database.
                adapter.Update(DTItems);

                //Refresh grid. Get items again from database and show it in grid.
                DTItems = GetAllItems();
                dataGridView1.DataSource = DTItems;
                MessageBox.Show("Selected item deleted successfully...");
            }
            else
            {
                MessageBox.Show("You must select entire row in order to delete it.");
            }
        }
    }
}