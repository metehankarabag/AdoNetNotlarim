using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace _16_AcceptChangesAndRejectChangesMethods
{

    /*
      DataRow Class'ında object dönen 6 INDEX tanımlanmış. Hepsi aldığı parametreleri sütunlarına ulaşmak için kullanır. İlk 3 INDEX tek parametre alır ve bu INDEX'lerin Set() methodu olduğu için bulunan sütuna değer atayabiliriz. 
      Son 3 INDEX iki parmetrelidir ve 2. parametre olarak DataRowVersion Enum'u alır. Bu parametre satırın farklı durumlarındaki sütunlarına/bilgisine ulaşmamızı sağlar.
      DataRowVersion Enum'unda 4 şeçenek var Default, Original, Curent,Proposed. Bir satırın ilk durumundaki veriye ulaşmak için Original kullanılır. Son durumundaki veriye ulaşmak için ise Current kullanılır. Varsayılan durum Current'dir ve bir satıra silme işlemi uygulandığında Current hali yoktur. Bu yüzden olmayan bir satırın bir verisine/kendisine ulaşmaya çalıştığımızda hata alırız. Satırların Original verisi saklandığı için Orginal verilerine ulaşabiliriz. DataRowVersion'nun kullanım mantığından dolayı DataRowVersion parametresi alan INDEX'ler READ-ONLY'dir.
      DataRow'un 5 farklı durum vardır(added,Deleted,Modified,Unchanged,Detached) ve durumları DataRow üzerinde gerçekleşen işlemler belirler. Satır durumu satırların bulunduğu tablo güncellenirken değişir. DataRow CLASS'ının RowState PROPERTY'si satırın geçerli satır durumunu verir. DataSet'in güncelleme sırasında değişmeyen satırlarının Unchaned değişenler ise değişme türüne göre değer alır. 
      Not: Detached satırın bir tablodan bağımsız olarak DataRow oluşturulduğu anlamına gelir.
      
      Current Verison -> Geçerli RowState değeri Deleted olan satırlarda yokdur.
      Default Versiyon -> Geçerli RowState değeri Added,Modified ve Unchanged olan satırlarda Current'dir, Deleted RowState'lerde Original, Detached'de Proposed'dir.
      Original Versiyon -> Geçerli RowState değeri Added olan satırlarda yoktur.
      Proposed veriyon -> Geçerli RowState değeri sadece Modified ve Detached olan satırlarda vardır.
       
     AcceptChanges() ve RejectChanges()(virtual) DataSet CLASS'ının INSTANCE METHOD'larıdır. AcceptChanges() uygulandığı DataSet'de yapılan değişiklikleri onaylar, RejectChanges() methodu ise geri alır. Bu methodlar satırların RowState Property'lerinide etkiler.
     RejectChanges() methodu çalıştırılmadan önce eklenen satırlar tamamen silinir, Modified ve Deleted satırlar ise değerleri geri alındıktan sonra DataSet güncelleneceği için RowState değerleri Unchanged olur.
     AcceptChanges() methodu çalıştırımadan önce Silinen satırları tamamen silinir, Modified ve  Added satırların yeni değerleri onaylandıkdan sonra RowState değerleri yenilenir yani Unchanged olur.
       
      UnChanged -> AcceptChanges() methodu son çalıştırıldıktan sonra değişmeyen veya Fill() ile oluşturulan satırların RowState değeridir.
      Added/Deleted -> Eklenen/Silinen satırdan sonra AcceptChanges()'in çalışmadığı satırların RowState değeridir.
      Modified -> Değişiklik olan satırların RowState değeridir.
      Detached -> herhangi bir DataRowCollection üyesi olmayan DataRow'un DataRowCollection'a eklendikten sonraki RowState değeridir. Aynı zamanda AcceptChanges() methodundan sonra DataRowCollection'dan silinen satırların da değeridir.
      
      Not: Bu methodlar DataTable,DataSet veya DataRow seviyesinde çalıştırılabilir. 
      Not: AcceptChanges() RowState değerini resetlediği için bu method çalıştıktan sonra update() methodu ile veritabanındaki bir tabloyu güncelleyemiyoruz. Büyük ihtimalle karşılaştırma işlemlerinde ilk önce RowState değerlerine bakılıyor Unchanged olanlar karşılaştırılmıyor. Silinen satırlar onaylandıktan sonra tamamen silindiğinden dolayı karşılaştırılamaz.
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        #region Part 15
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
            // Set row in editing mode
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

        protected void btnGetDataFromDB_Click(object sender, EventArgs e)
        {
            GetDataFromDB();
        }

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
                //SqlCommandBuilder scb = new SqlCommandBuilder(dataAdapter); -- üstteki 2 komutun yaptığı için yanısını yapar.

                
                dataAdapter.Update((DataSet)Cache["DATASET"], "Students");
                lblStatus.Text = "Database table updated";
            }
        }
        #endregion
        #endregion

        protected void Button1_Click(object sender, EventArgs e)
        {
            DataSet dataSet = (DataSet)Cache["DATASET"];

            #region AcceptChanges()
            DataRow newDataRow = dataSet.Tables["Students"].NewRow();
            newDataRow["ID"] = 101;
            dataSet.Tables["Students"].Rows.Add(newDataRow);
            dataSet.AcceptChanges();
            Cache.Insert("DATASET", dataSet, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);
            GetDataFromCache();
            #endregion


            //if (dataSet.HasChanges())
            //{
            //    DataRow newDataRow = dataSet.Tables["Students"].NewRow();
            //    newDataRow["ID"] = 101;
            //    dataSet.Tables["Students"].Rows.Add(newDataRow);
            //    dataSet.RejectChanges();
            //    Cache.Insert("DATASET", dataSet, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);
            //    GetDataFromDB();
            //    lblStatus.Text = "Changes Undo";
            //    lblStatus.ForeColor = System.Drawing.Color.Green;
            //}
            //else { lblStatus.Text = "No changes to Undo"; lblStatus.ForeColor = System.Drawing.Color.Red; }

            //DataRow newDataRow = dataSet.Tables["Students"].NewRow();
            //newDataRow["ID"] = 101;
            ////dataSet.Tables["Students"].Rows.Add(newDataRow);

            //foreach (DataRow dr in dataSet.Tables["Students"].Rows)
            //	 if (dr.RowState == DataRowState.Deleted)
            //      Response.Write(dr["ID", DataRowVersion.Original].ToString() + " - " + dr.RowState.ToString() + "<br />");    
            //   else
            //      Response.Write(dr["ID"].ToString() + " - " + dr.RowState.ToString() + "<br />");
			
            //Response.Write(newDataRow.RowState.ToString());
        }

    }
}
