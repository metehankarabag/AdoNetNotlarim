using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace _11_DataSet
{
    /*DATASET
      Bir DataSet örneği birden fazla tablo tutabilir ve bir nesnenin alt nesnelerini arayabilmek için INDEX kullanmamız gerekir. DataSet içindeki tablolara ulaşmak için DataSet'in Tables PROPERTY'sini kullanıyoruz. Bu PROPERTY'nin türü DataTableCollection'dır ve bu CLASS'da DataTable dönen 3 INDEX tanımlanmıştır. Bu sayede DataSet'in Tables PROPERTY'sine INDEX değeri verip DataSet içinden aldığımız DataTable'ı kontrolün DataSoruce özelliğine atabiliriz.
      DataSet varsayılan olarak barındırdığı her tablonun adını yüklenme sırasına göre table1,table2,... olarak değiştirdiği için Tableas PROPERTY'sine bu isimleri girmeliyiz. Fakat bu isimleri girip tabloyu DataSource'a attığımızda hangi tablonun kullanıldığı anlaşılmaz. Bu yüzden DataSet içindeki tabloların adını değiştirmeliyiz. 
      INDEX ve PROPERTY READ-ONLY olduğu için sadece tablolara ulaşabiliriz tablo adlarını değiştiremeyiz. Tables PROPERTY'si DataTable döndüğü için ve TableName PROPERTY'si DataTable CLASS'ının bir üyesi olduğu için Tables PROPERTY'sinden TableName PRoperty'sini ulaşabiliriz. Bu Property DataSet içine ulaşılan tablonun adını değiştirebilir. Değiştirdikten sonra Tables'e index değeri olarak yeni tablo adını girdiğimizide daha anlaşılır bir kod yazmış oluruz.
      
     Not: DataSet örneğine Tables Property'sini uygulanmadığında, örnekten varsayılan olarak ilk tabloyu alırız.
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter("spGetProductAndCategoriesData", connection);
                dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataSet dataset = new DataSet();
                dataAdapter.Fill(dataset);//dataAdapter'den gelen veri ile dataset'i dolduruyor. Sonra tablolar oluşuyor.

                dataset.Tables[0].TableName = "Products";
                dataset.Tables[1].TableName = "Categories";

                GridViewProducts.DataSource = dataset.Tables["Products"];
                GridViewProducts.DataBind();

                GridViewCategories.DataSource = dataset.Tables["Categories"];
                GridViewCategories.DataBind(); 
            }
            /*
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter("spGetProductAndCategoriesData", connection);
                dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataSet dataset = new DataSet();
                dataAdapter.Fill(dataset);

                GridViewProducts.DataSource = dataset;
                GridViewProducts.DataBind();

                GridViewCategories.DataSource = dataset;
                GridViewCategories.DataBind();
            } 
            */

        }
    }
}