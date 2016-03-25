using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace _8_SqlDataReader
{
    /*
      SqlDataReader SEALED bir CLASS'dır. Comman sınıfının ExecuteReader() Instance methodu dönüş türü olarak kullanılır. ExecuteReader() methodu çalıştığında Sql'den aldığı tüm sonucu reader örneğine atar. SqlReader CLASS'ının READ() methodunu örneğe uyguladığımızda, örnek içindeki tabloyu satır satır okuyabiliriz. Bu method boolen döndüğü için while ile kullanarak her satır ile ayrı ayrı çalışabileceğimiz bir ortam oluşturmuş oluruz. While içinde yaptığımız iş satırların değerlerini değiştirecekse ve yeni değerleri programda kullanacaksak, While içinde yapmamız gereken 2 temel iş var.
      1. Read()'in okuduğu satırın sütun değerlerini ayrı ayrı almak. SqlReader CLASS'ının INDEX'ine parametre olarak sütun adı vererek değerini alabiliriz. INDEX READ-ONLY olduğu için yeni verileri veritabanındaki tabloya uygulayamayız ve programda kullanmayı istediğimiz veriler yeni veriler ise 2. yapmamız gereken iş programda bir tablo oluşturmaktır.
      
      Programda tablo oluşturmak için DataTable CLASS'ı kullanılır. Oluşturduğumuz DataTable'ın sütunlarını oluşturmak/belirlemek için Columns Property'sine Add() methodunu kullanırız. Bu methodun 5 overload'ı var. String isteyen overload'ına sütun adı vererek sütunu oluşturabiliriz. Sütun üzerinde tam kontrol istiyorsak veya aynı sütunu birden fazla kez kullanacaksak, Add() methoduna parametre olarak DataColumn örneğini oluşturup verebiliriz.
     Satır verileri programa While içinde geldiği için while içinde yapmamız gereken tek şey tablonun yeni satırlarını oluşturup değerlerini vermek. Oluşturulacak satırın tablo ile ilikliki olması için DataRow örneğine DataTable CLASS'ının NewRow8) methodunu uyguluyoruz. Bu method döngü her döndüğünde tabloya yeni bir satır ekleyecektir.
      
      1. Not: Read() methodu DataReader örneği içindeki veriyi okumasına rağmen sanki veriyi veritabanındaki tablodan alıyormuş gibi çalışırken açık bir bağlantı gerektiriyor.
      2. Not: DataRow'un Contructer'ını protectedinternal olduğu için kullanamaıyoruz. Bir CLASS oluşturup bu CLASS'ı base CLASS olarak veridiğimizde CONSTRUCTER'ı parametre olarak DataRowBuilder istiyor. Bu CLASS' .net'in alt CLASS'larından olduğu için hiç bir özelliğini kullanmamıza izin vermiyor. Eğer bir Row oluşturma şansımı tablo oluşturmamıza gerek kalamayabilirdi.
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*
             string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
			 using (SqlConnection connection = new SqlConnection(ConnectionString))
			 {
			 	 connection.Open();
			 	 SqlCommand command = new SqlCommand("Select * from tblProductInventory", connection);
			 	 using (SqlDataReader reader = command.ExecuteReader())
			 	 {
			 	 	 ProductsGridView.DataSource = reader;
			 	 	 ProductsGridView.DataBind();
			 	 }
			 }
             */
            VeriAl();
        }

        private void VeriAl()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Select * from tblProductInventory", connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    #region DATATABLE oluşturduk.
                    DataTable sourceTable = new DataTable();
                    sourceTable.Columns.Add("ID");
                    sourceTable.Columns.Add("Name");
                    sourceTable.Columns.Add("Price");
                    sourceTable.Columns.Add("DiscountedPrice");

                    while (reader.Read())
                    {
                        //Calculate the 10% discounted price
                        int OriginalPrice = Convert.ToInt32(reader["UnitPrice"]);
                        double DiscountedPrice = OriginalPrice * 0.9;

                        // Populate datatable column values from the SqlDataReader
                        DataRow datarow = sourceTable.NewRow();
                        datarow["ID"] = reader["ProductId"];
                        datarow["Name"] = reader["ProductName"];
                        datarow["Price"] = OriginalPrice;
                        datarow["DiscountedPrice"] = DiscountedPrice;

                        //Add the DataRow to the DataTable
                        sourceTable.Rows.Add(datarow);
                    }
                    #endregion

                    // Set sourceTable as the DataSource for the GridView
                    ProductsGridView.DataSource = sourceTable;
                    DataGrid1.DataSource = sourceTable;
                    ProductsGridView.DataBind();
                }
            }
        }
    }
}