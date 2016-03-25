using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace _5_SQLInjectionAttack
{
	 /*
      Kullanıcı sorguya sadece değer girebilmeli. Kullanıcıya sorgunun bir parçasını yazdıracak kodu yazarsan hapı yutarsın.
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void GetProductsButton_Click(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection("DatabaseConnectionString"))
            {
                SqlCommand cmd = new SqlCommand("Select * from tblProductInventory where ProductName like '" + ProductNameTextBox.Text + "%'", connection);
                connection.Open();
                ProductsGridView.DataSource = cmd.ExecuteReader();
                ProductsGridView.DataBind();
            }
        } 

    }
}