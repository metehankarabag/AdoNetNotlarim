using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace _13_WhatIsSqlCommandBuilder
{
    /*
      SqlCommandBuilder ilişkilendirildiği SqlDataAdapter'in veritabanından getirdiği tabloyu güncellememizi sağlar. Yani SqlCommandBuilder kullandığımızda veritabanındaki tabloyu değiştirmek için kod yazmak sorunda kalmayız. 
      SqlDataAdapter'in bir örneği oluşturup SqlCommanBuilder ile ilişkilendikten sonra SqlDataAdapter'in Update() methodunu çalıştığında SqlCommanBuilder CLASS'ı aktifleşir. Yani SqlCommanBuilder'i kullanmak için sadece oluşturmak ve bir DataAdapter ile ilişiklendirmek yeterlidir.
      Update() methodu parametre olarak programdaki bir tabloyu alır. Alınan tablo, SqlDataAdapter sorgusunda kullanılan tablo ile karşılaştırıldıktan sonra farkları ortadan kardıracak sql sorgusu oluşturulur ve veritabanında çalıştırılır.
     Özetle SqlCommandBuilder SqlDataAdapter'ın kullandığı tablo ile SqlDataAdapter örneğine uygulanan Update() methodu içindeki program tablosunu alır, farkları belirler ve farkları ortadan kaldıracak sorguyu veritabanında çalıştırır. 

      Derste update() metohdunu 4. overload'ı kullanılmış bunun tek bir sebebi DataSet içindeki tabloyu isimlendirmiş olmamızdır. Yani 2. parametredesi isim DataSet içindeki tablonun ismidir. DataSet içindeki dosyayı isimlendirme işi SqlDataAdapter'in Fill() methodunun 3. overload'ını kullanarak DataSet'in yüklenme sırasında yapılıyor.(ilk event'de). Bu sonucu ViewState ile taşıdığımız için Update() methoduna verilen DataSet'de de geçerli oluyor. (Update() methodunun 4. overload yerine 2. overloadı ds.Tables[] şeklindede kullanabilirdik.)
      Derste 2 tane ViewState kullanılmış. Biri SqlDataAdapter'i çalıştıran sorgu diğeri sorgu sonucunda oluşan DataSet. 2. ViewState'i kullanmak yerine sorguyu taşımak aynı DataSet'e ulaşmamızı sağlar. Fakat bunu için Fill() methodu ile tekrar veritabanına bağlanmamız gerekir. Buda iyi olmaz.
     
     1. Not: SqlCommanBuilder 2 ovarload'ı var 1. parametresidir, 2. SqlDataAdapter ister. birinci kullanım için SqlCommandBuilder'in DataAdapter Property'sini kullanarak veriyi ilişkilendirebiliriz.
     2. Not: SqlDataAdapter'da kullanılan sorgu veri tabanında sadece bir tabloyu isaret etmelidir.
     3. Not: Birden fazla DataAdapter aynı DataSet üzerinden fakrlı veritabanı tablolarını güncelleyebiliriz. Yapılamsı gereken şey Update() methodu INSTANCE bir method olduğu için tüm SqlDataAdapter methodları için kullanmak.
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void btnGetStudent_Click(object sender, EventArgs e)
        {
            #region veri tabanında bilgliler alınıyor.
            string connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string selectQuery = "Select * from tblStudents where ID = " + txtStudentID.Text;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(selectQuery, connection);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "Students");

            ViewState["DATASET"] = dataSet;
            ViewState["SELECT_QUERY"] = selectQuery;

            if (dataSet.Tables["Students"].Rows.Count > 0)
            {
                DataRow dataRow = dataSet.Tables["Students"].Rows[0];
                txtStudentName.Text = dataRow["Name"].ToString();
                txtTotalMarks.Text = dataRow["TotalMarks"].ToString();
                ddlGender.SelectedValue = dataRow["Gender"].ToString();
                lblStatus.Text = "";
            }
            else
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "No record with ID = " + txtStudentID.Text;
            }
            #endregion
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            #region veri tabanından alınan bilgiye güncelleme yapılıyor.
            string connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            dataAdapter.SelectCommand= new SqlCommand((string)ViewState["SELECT_QUERY"], con);
            
            SqlCommandBuilder builder = new SqlCommandBuilder(dataAdapter);

            DataSet ds = (DataSet)ViewState["DATASET"];//tabloyu aldım
            DataRow dr = ds.Tables["Students"].Rows[0];// ilk satıra ekledim
            //yeni değerlerini verdim
            dr["Name"] = txtStudentName.Text;
            dr["Gender"] = ddlGender.SelectedValue;
            dr["TotalMarks"] = txtTotalMarks.Text;
            dr["Id"] = txtStudentID.Text;

            int rowsUpdated = dataAdapter.Update(ds, "Students");//burada yeni değeri var.
            if (rowsUpdated == 0)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "No rows updated";
            }
            else
            {
                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = rowsUpdated.ToString() + " row(s) updated";
            }
            #endregion
        }
    }
}