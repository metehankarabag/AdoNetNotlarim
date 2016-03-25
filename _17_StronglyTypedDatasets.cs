using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace _17_StronglyTypedDatasets
{
    /*2 WebForm var. 1. bu StronglyType olan
      Linq'da kullandığımız DataTable'ın alt nesneleri DataRow'dur.
      Add new item -> Data -> DataSet projeye eklediğimizde veritabanından alınan tabloya göre aylarınmış bir DataAdapter,DataTable ve DataRow,DataColumn oluşturuyor. Bu DataRow DataTable'in Row türü olarak belirliyor.(nasıl yaptığını hiç anlamadım.) DataRow Class'ı içindeki PROPERTY'ler sanırsam Row'un sütunlarından değer alıyor. Row türü olarak StudentsRow belirlendiği için linqu da bu türe ulaşıyoruz ve PROPERTY'lerini kullanabiliyoruz.
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string selectQuery = "Select * from tblStudents";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(selectQuery, connection);

                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Students");

                Session["DATASET"] = dataSet;
                #region LINK CODE
                GridView1.DataSource = from dataRow in dataSet.Tables["Students"].AsEnumerable()
                                       select new Students
                                       {
                                           ID = Convert.ToInt32(dataRow["Id"]),
                                           Name = dataRow["Name"].ToString(),
                                           Gender = dataRow["Gender"].ToString(),
                                           TotalMarks = Convert.ToInt32(dataRow["TotalMarks"])
                                       };
                GridView1.DataBind();
                #endregion
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            DataSet dataSet = (DataSet)Session["DATASET"];

            if (string.IsNullOrEmpty(TextBox1.Text))
            {
                GridView1.DataSource = from dataRow in dataSet.Tables["Students"].AsEnumerable()
                                       select new Students
                                       {
                                           ID = Convert.ToInt32(dataRow["Id"]),
                                           Name = dataRow["Name"].ToString(),
                                           Gender = dataRow["Gender"].ToString(),
                                           TotalMarks = Convert.ToInt32(dataRow["TotalMarks"])
                                       };
                GridView1.DataBind();
            }
            else
            {
                #region Listeden birini bulma
                GridView1.DataSource = from dataRow in dataSet.Tables["Students"].AsEnumerable()
                                       where dataRow["Name"].ToString().ToUpper().StartsWith(TextBox1.Text.ToUpper())
                                       select new Students
                                       {
                                           ID = Convert.ToInt32(dataRow["Id"]),
                                           Name = dataRow["Name"].ToString(),
                                           Gender = dataRow["Gender"].ToString(),
                                           TotalMarks = Convert.ToInt32(dataRow["TotalMarks"])
                                       };
                GridView1.DataBind();
                #endregion
            }
        }
    }
}
