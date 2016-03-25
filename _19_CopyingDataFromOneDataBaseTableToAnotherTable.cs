using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

namespace _19_CopyingDataFromOneDataBaseTableToAnotherTable
{
    /*20. ders Program.cs dosyasında
      bir veri tabanını başka bir veri tabanına kopyalamanın geçen dersten çok fazla bir farklı yok. Değişen teş veriyi programa getiren nesne.
      Geçen derste veriyi DataSet örneğine Readxml() methodu ile almıştık. Bu derste veriyi veritabanında çekeceğimiz için SqlCommand kullanıyoruz.(sqlDataAdapter'de olur.)
            
      Not: Source tablodan gelen sütun isimleri ile destination tablodaki sütun isimleri aynı iste ColumnMappings Property'sini kullanmaya gerek yok.  
      Not: SqlBulkCopy destination veritabanında çalışır. Bu yüzden her zaman destination veritabanını veren connecitonu kullanmak lazım.
      Not: Ders için 2 veri tabanı kullanmak gerektiğinden uğraşmadım.
      Not: SqlConnection ile oluşturduğumuz Using bloğu bağlantıyı otomatik olarak kapatıyor. İçerideki nesnelerde neden bu bloğu kullanıyoruz anlamadım. Ama taminim içeride kullandığımız nesnenin Connection ile bağlantısını otomatik olarak kapatmak.
      
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            string sourceCS = ConfigurationManager.ConnectionStrings["SourceDBCS"].ConnectionString;
            string destinationCS = ConfigurationManager.ConnectionStrings["SourceDBCS"].ConnectionString;

            using (SqlConnection sourceCon = new SqlConnection(sourceCS))
            {
                sourceCon.Open();
                SqlCommand cmd = new SqlCommand("select * from Departmens", sourceCon);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    using (SqlConnection destinationCon = new SqlConnection(destinationCS))
                    {
                        using (SqlBulkCopy bc = new SqlBulkCopy(destinationCon))
                        {
                            bc.DestinationTableName = "Departments";
                            destinationCon.Open();
                            bc.WriteToServer(rdr);

                        }
                    }
                }

                cmd = new SqlCommand("select * from Employees", sourceCon);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    using (SqlConnection destinationCon = new SqlConnection(destinationCS))
                    {
                        using (SqlBulkCopy bc = new SqlBulkCopy(destinationCon))
                        {
                            bc.DestinationTableName = "Employees";
                            destinationCon.Open();
                            bc.WriteToServer(rdr);

                        }
                    }
                }
            }

        }
    }
}