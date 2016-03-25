using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace _10_SqlDatAdapter
{
    /*SQLDATAADAPTER
     SQLDATAREADER CONNECTED ORIENTED'dır. Yani çalışmaya başladığında veritabanına gidebileceği açık bir bağlantı bekler.
     SqlDataAdapter CLASS'ının SqlComman gibi 4 overload'ı var.  
     SqlDataAdapter overload'ları -> 1. Parametresiz 2. SqlComman 3. string, string 4. string, SqlConnection parmetreleri bekler.
     SqlCommand overload'ları -> 1. Parametresiz 2. string, 3. string,SqlConnection 4. string, SqlConnection,SqlTransaction parmetreleri bekler.
     
     Bir SqlDataAdapter'in Fill() Instance methodu veri tabanındaki tabloyu programa çekmek için kullanılır. Method parametre olarak bir DataSet örneği bekler. Bu method veritabanına bağlantı açar, sql komutlarını çalıştırır, veri ile dataseti doldurur, ve bağlantıyı kapatır. Bağlantı ihtiyacı olduğu kadar açık tutulur. Fill() methodu işini birtirdiğinde DataSet'i kontrolün veri kaynağı olarak ayarlayabiliriz.
     
     Not: SQLDATAADAPTER ve DATASET bağlantısız veri bağlama modeli sağlar     
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageLoad1();

            PageLoad();
            
        }
        #region sorgu ile DATAADAPTER
        private void PageLoad()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter("select * from tblProductInventory", connection);
                DataSet dataset = new DataSet();
                dataAdapter.Fill(dataset);
                GridView1.DataSource = dataset;
                GridView1.DataBind();
            }
        }
        #endregion
        #region STORED PROCEDURE ile DATAADAPTER
        private void PageLoad1()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter("spGetProductInventoryById", connection);
                dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                dataAdapter.SelectCommand.Parameters.AddWithValue("@ProductId", 1);
                DataSet dataset = new DataSet();
                dataAdapter.Fill(dataset);
                GridView1.DataSource = dataset;
                GridView1.DataBind();
            }
        }
        #endregion
    }
}