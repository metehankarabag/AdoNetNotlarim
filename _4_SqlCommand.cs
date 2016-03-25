using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace _4_SqlCommand
{
    /*
      SqlCommand CLASS veritabanında çalışacak SQL sorgularını ve sorgular ile ilgili bir çok şeyi belirler.
         Connection PROPERTY: Kullanılacak bağlantı nesnesini belirler.
         CommandTimeout: Sorgunun çalışma süresini belirler.(Sorgu belirtilen sürede bitmesse hata verir.)
         CommandText: Çalıştırılacak sorguyu belirler.
       
      Hazırlanan SQLCOMMAN sorgusunun veritabanında çalışması için açık bir bağlantı gerekir. Fakat Command nesnesii hazırlanırken bağlantının açık olması gerekmez ve bağlantı ne kadar kısa süre açık kalırsa o kadar iyidir. Bu yüzden ayarlama işlemlerini hazırladıktan sonra bağlantıyı açmalıyız.
     Not: Açılan her bağlantı kapatılmalıdır. Kapatılmassa açık kalır.
      
      Aşağıdaki methodlar çalışmak için açık bir bağlantı ister.
      EXECUTEREADER(): T-SQL açıklaması birden fazla satır dönderen COMMAND nesnesini çalıştırır. - SQLREADER DÖNER.
      EXECUTESCALAR() T-SQL açıklamasının sonuç setinin sadece ilk hücresindeki değerini döner. -OBJECT DÖNER
      EXECUTENONQUERY(): Çalıştırılan TSQL sorgusunun kaç satırı etkilediğini döner. -INT DÖNER.
     */

    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageLoad();
        }
        #region Parametresiz SQLCOMMAND CONSTRUCTOR'u kullanma
        protected void PageLoad()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection("data source=.; database=Sample_Test_DB; integrated security=SSPI"))
            {

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select Id,ProductName,QuantityAvailable from tblProductInventory";
                cmd.Connection = connection;
                connection.Open();
                Gridview1.DataSource = cmd.ExecuteReader();
                Gridview1.DataBind();
            }
        }
        #endregion
        
        #region ExecuteReader()
        private void PageLoad1()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection("data source=.; database=Sample_Test_DB; integrated security=SSPI"))
            {                                       //T-SQL komutu
                SqlCommand cmd = new SqlCommand("Select Id,ProductName,QuantityAvailable from tblProductInventory", connection);
                connection.Open();
                //Birden fazla satır döneceği için ExecuteReader() kullanıyoruz.
                Gridview1.DataSource = cmd.ExecuteReader();
                Gridview1.DataBind();
            }
        }
        #endregion
        #region ExecuteScalar() tek bir INT değer döner.

        protected void PageLoad2()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection("data source=.; database=Sample; integrated security=SSPI"))
            {
            SqlCommand cmd = new SqlCommand("Select Count(Id) from tblProductInventory", connection);
            connection.Open();
            int TotalRows = (int)cmd.ExecuteScalar();
            Response.Write("Total Rows = " + TotalRows.ToString());
            }
        }
        #endregion
        #region ExecuteNonQuery()
        protected void PageLoad3()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection("data source=.; database=Sample_Test_DB; integrated security=SSPI"))
            {
                SqlCommand cmd = new SqlCommand("insert into tblProductInventory values (103, 'Apple Laptops', 100)", connection);
                connection.Open();
                //Since we are performing an insert operation, use ExecuteNonQuery() 
                //method of the command object. ExecuteNonQuery() method returns an 
                //integer, which specifies the number of rows inserted
                int rowsAffected = cmd.ExecuteNonQuery();
                Response.Write("Inserted Rows = " + rowsAffected.ToString() + "<br/>");
    
                cmd.CommandText = "update tblProductInventory set QuantityAvailable = 101 where Id = 101";
                rowsAffected = cmd.ExecuteNonQuery();
                Response.Write("Updated Rows = " + rowsAffected.ToString() + "<br/>");
    
                cmd.CommandText = "Delete from tblProductInventory where Id = 102";
                rowsAffected = cmd.ExecuteNonQuery();
                Response.Write("Deleted Rows = " + rowsAffected.ToString() + "<br/>");
            }
        }
        #endregion
    }
}