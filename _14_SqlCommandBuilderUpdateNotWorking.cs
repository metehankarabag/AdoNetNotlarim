using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace _14_SqlCommandBuilderUpdateNotWorking
{
    /*
     1. Neden Builderin Update() methodunu çalıştıran SqlDataadpter ile ilişkilendirilmemiş olması.
     2. Veritabanından gelen tablonun PRIMARY KEY veya UNIQUE sütununun olmamaması
     3.
     GetInsertCommand()
     GetUpdateCommand()
     GetDeleteCommand()
     SQLCOMMAND NESNESİ DÖNÜYORLAR.
     Bunların text özelliklerini aldığımızda sorunun nereden kaynaklandığını bulabiliriz.
     
     SQLCOMMANDBUILDER kullandığımızda parametreler sql injection a izin vermez.
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void btnGetStudent_Click(object sender, EventArgs e)
        {
            #region Burda veri tabanında bilgliler alınıyor.
            string connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            string selectQuery = "Select * from tblStudents where ID = " + txtStudentID.Text;
            // buraya sorguyazdığımda çalışıyor fakat ID'ye değer olarak sorgu girdiğim için hata alıyorum
            SqlDataAdapter dataAdapter = new SqlDataAdapter(selectQuery, connection);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "Students");

            //sorgu sonucundan gelen tablodu ve sorguyu POSTBACK'te koruyoruz.
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
            #region Burada veri tabanından alınan bilgiye güncelleme yapılıyor.
            string connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            dataAdapter.SelectCommand = new SqlCommand((string)ViewState["SELECT_QUERY"], con);//burada bir dataAdapter'in bir değeri var
            
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

            lblInsert.Text = builder.GetInsertCommand().CommandText;
            lblUpdate.Text = builder.GetUpdateCommand().CommandText;
            lblDelete.Text = builder.GetDeleteCommand().CommandText; 
        }
    }
}