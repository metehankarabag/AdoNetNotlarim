using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

namespace _9_NextResultMethodOfSqlDataReader
{
     /*
     SqlReader CLASS örneğine birden fazla tablo atılmışsa, SqlReader CLASS'ının NextResult() methodu sıradaki set'i alır.
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Read() sadece verileni okur. NextResult() birden fazla RESULT-SET'i dönmek için kullanılır.
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select * from tblProductInventory; select * from tblProductCategories", connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ProductsGridView.DataSource = reader;
                    ProductsGridView.DataBind();

                    while (reader.NextResult())
                    {
                        CategoriesGridView.DataSource = reader;
                        CategoriesGridView.DataBind();
                    }
                }
            }
            #endregion

            /*bu kod ile çalıştığında sadece TBLPRODUCTINVENTORY'den RESULT-SET gösterilir. TBLPRODUCTCATEGORIES'den RESULT-SET  gösterilmez
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;   
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select * from tblProductInventory; select * from tblProductCategories", connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ProductsGridView.DataSource = reader;
                    ProductsGridView.DataBind();

                    CategoriesGridView.DataSource = reader;
                    CategoriesGridView.DataBind();
                }
            }
            */
            
        }
    }
}