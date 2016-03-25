using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace _15_DisconnectedDataAccess
{
     /*GetDataFromDB(), GetDataFromCache(), Edit Button, Edit -> Cancel Button
      GetDataFromDB()
      Bu method içinde veri tabanından veri alma işlemlerini gerçekleştiriyoruz. Ek olarak 2 iş yapılması gerekiyor. 
      DataSet içindeki bir satırı bulmak için DataSet'in Primary Key sütununu kullanmak zorundayız. Bu yüzden yapılası gereken ilk iş DataSet'in Primary Key sütununu belirlemek. Yapılması gereken 2. iş ise DataSet'i CACHE'e almak. DataSet üzerinde gerçekleşen her işlem sonrasında Cache'i yenilemek zorundayız.
      DataSet'in Primary sütununu belirlemek için Tables PROPERY'sinden aldığımız "DataTable'a" DataTable'ın üyesi olan DataColumn[] türündeki PrimaryKey PROPERTY'sini uyguluyoruz. parametre olarak DataColumn[] örneği istiyor. Örneğe DatSet'in Tables Property'sine Columns Property'sini uyguluyoruz ve PrimaryKey'e atıyoruz.(Property Index özelliğin türünden alır.)
      Cache Class'ının Insart() methodunu sonra yazarım.
      
      GetDataFromCache()
      Cache nesnesini oluşturduktan sonra bunu bir methoda alıyoruz. Gridview CLASS'ının RowEditing,RowCancelingEdit, EVENT'larında yapılan tek bir iş Gridview'ın görünümünü belirlemek. Görünüm her değiştiğinde veri yeniden yüklenmelidir. Bu yüzden Cache'den DataSet'i çağarıyoruz.
     */
    /*Edıt -> Update Button, Delete Button
      GRIDVIEW_ROWUPDATING EVENT'ı çalıştığında eski Row verisi yeni Row verisile değişecek. Bu Event'in Eventargs nesne örneği güncellenen satırın tüm sütun verilerini içerir. Yani verileri Cache'deki DataSet'de atabilmek için ilk önce DataSet'deki doğru satırı bulmalıyız. Rows Property'si DataRowCollection türünde bir DataTable'ın Read-Only PROPERTY'sidir. DataSet'in Tables[] Propert'si DataTable getirdiği için bu Property'den sonra Rows Property'sini uygulayabiliriz. Rows Property'si DataRowCollection döner ve bu CLASS'ın DataRow türünde veri dönen Find() methodu var. Bu sayede Rows Property'sinden Find() methoduna ulaşabiliriz. Find() methodu object parametre bekliyor. Fakat methoda uygulandığı satırın hangi sütununu verirsek verelim değer olarak sadece Primary Key ve Unique Key değerlerini ve onların türündeki değerleri kullanabiliyor. Farklı türde bir veri girdiğimizde düzenleme zamanında hata vermesebile çalışma zamanında Input "type" was not in a correct format. hatası veriyor.
      
      Not: Find() methoduna parametre olarak Primary veya Unique sütun yerine başka bir sütun verdiğimizde EvetArgs nesnesinin NewValue Property'sinden yeni değerlere ulaşamıyoruz. hata veriyor.
      
      Silme işlemi için Find() methodunun döndüğü DataRow CLASS'ının void türündeki Delete() methodunu kullanıyoruz.
      
    */
    /*DATABASE UPDATE BUTTON
     Daha önceki dersler ile aynı mantık. Bir SqlDataAdapter örneği oluşturup veri tabanında sorgusunu çalıştıracağız. Veritabanı tablosunda yapmayı istediğimiz 2 iş var. (Update ve Delete). Bu işleri yapan iki SqlCommand nesnesi oluşturuyoruz. Sorguların çalışması için gerekli olan parametreleri SqlCommand nesnelerine verip SqlCommanları SqlDataAdapter'in Update ve Delete Command Property'lerine attıktan sonra SqlDataAdpater örneğinde Update() methodunu çalıştırıyoruz. Bu method DataSet ile veritabanındaki tabloyu karşılaştırıp verdiğimiz SqlComamnd nesnelerini çalıştırır.
     Not: SqlCommandBuilder, SqldataAdapter'in update/delete/InsertCommand Property'leri ile aynı işi yapar.
     önemli Not: DataAdapter adı üstünde sadece yükleyici çalıştığında her zaman programa veri getirmesi gerekir. Contructor'u bu yüzden sadece Select sorgusu istiyor. SqlDataAdapter'in oluşturulma mantığı gereği updateComand gibi PROPERTY'lerini sadece hazırladığımız SqlCommand nesneleri atmamız gerekir. Hazırlanan SqlComman nesnesin parameterleri değerlerini almışssa çalışır. Bu yüzden AddWithValue() methodu yerine add() methodunu kullanıyoruz. Add() methodu sadece parametreleri oluşturuyor. Bu parametreler'i SqlDataAdapter'in Update() methodu doldurup SqlCommand nesnesini çalıştıracak. Update() methodu programdaki bir tablo ile veritabanındaki bir tabloyu karşılarştırır sorguları otomatik olarak çalıştırır. Bu sayede program çalışmaya başladığında veritabanındaki tabloyu bir DataSet'e alır, program çalışması sırasında tüm değişiklikleri DataSet'e uygularsak, programı kapatmadan önce gerçekleşen tüm değişiklikleri tek seferde veritabanına geçirmiş oluruz.
     */

    public partial class WebForm1 : System.Web.UI.Page
    {
        #region GetDataFromDB()

        private void GetDataFromDB()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string selectQuery = "Select * from tblStudents";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(selectQuery, connection);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "Students");
            //Buradaki yazımın mantığını araştır iyi öğren.
            dataSet.Tables["Students"].PrimaryKey = new DataColumn[]
            {
                dataSet.Tables["Students"].Columns["ID"] 
            };

            Cache.Insert("DATASET", dataSet, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);
            
            GridView1.DataSource = dataSet;
            GridView1.DataBind();
            
            lblStatus.Text = "Data loded from Database";
        }
        #endregion
        #region GetDataFromCache()
        private void GetDataFromCache()
        {
            if (Cache["DATASET"] != null)
            {
                GridView1.DataSource = (DataSet)Cache["DATASET"];
                GridView1.DataBind();
            }
        }
        #endregion
        #region EDIT BUTTON
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GetDataFromCache();
        }
        #endregion
        #region EDIT -> CANCEL BUTTON
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GetDataFromCache();
        }
        #endregion
        #region EDIT -> UPDATE BUTTON
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //yeni değerler için tablo
            DataSet dataSet = (DataSet)Cache["DATASET"];
            DataRow dataRow = dataSet.Tables["Students"].Rows.Find(e.Keys["ID"]);
            dataRow["Name"] = e.NewValues["Name"];
            dataRow["Gender"] = e.NewValues["Gender"];
            dataRow["TotalMarks"] = e.NewValues["TotalMarks"];

            Cache.Insert("DATASET", dataSet, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);
            // Remove the row from edit mode
            GridView1.EditIndex = -1;
            // Reload data to gridview from cache
            GetDataFromCache();
        }
        #endregion
        #region DELETE BUTTON
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataSet dataSet = (DataSet)Cache["DATASET"];
            dataSet.Tables["Students"].Rows.Find(e.Keys["ID"]).Delete();
            Cache.Insert("DATASET", dataSet, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);
            GetDataFromCache();
        }
        #endregion
        #region DATABASE UPDATE BUTTON
        protected void btnUpdateDatabaseTable_Click(object sender, EventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string selectQuery = "Select * from tblStudents";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(selectQuery, connection);

                string strUpdateCommand = "Update tblStudents set Name = @Name, Gender = @Gender, TotalMarks = @TotalMarks where Id = @Id";
                SqlCommand updateCommand = new SqlCommand(strUpdateCommand, connection);
                updateCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50, "Name");
                updateCommand.Parameters.Add("@Gender", SqlDbType.NVarChar, 20, "Gender");
                updateCommand.Parameters.Add("@TotalMarks", SqlDbType.Int, 0, "TotalMarks");
                updateCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");

                dataAdapter.UpdateCommand = updateCommand;

                string strDeleteCommand = "Delete from tblStudents where Id = @Id";
                SqlCommand deleteCommand = new SqlCommand(strDeleteCommand, connection);
                deleteCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                dataAdapter.DeleteCommand = deleteCommand;
                //SqlCommandBuilder scb = new SqlCommandBuilder(dataAdapter); -- üstteki 2 komutun yaptığı işin yanısını yapar.

                
                dataAdapter.Update((DataSet)Cache["DATASET"], "Students");
                lblStatus.Text = "Database table updated";
            }
        }
        #endregion

        protected void btnGetDataFromDB_Click(object sender, EventArgs e)
        {
            GetDataFromDB();
        }
    }
}
