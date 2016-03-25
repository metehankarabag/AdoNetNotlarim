using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace _18_LoadXmlDataIntoSqlServerTableUsingSqlBulkCopy
{
    /*
      SqlBulkCopy CLASS'ı herhangi bir veri kaynağındaki tablo verilerini sadece Sql veritabanındaki tablolara kopyalamak için kullanılır.
      SqlBulkCopy CLASS'ının DestinationTableName Property'si ile veritabanında kullanılacak tablonun belirlenir.
      SqlBulkCopy CLASS'ının üyesi olan ColumnMappings READ-ONLY PROPERTY'si Source tablonun sütunları ile veri tabanındaki tablonun sütunlarını işaretlemek için kullanılır. Property SqlBulkCopyColumnMappingCollection türündedir.  Bu CLASS'ın SqlBulkCopyColumnMapping dönen ve 5 overload'ı olan Add() methodu var. Bir de READ-ONLY INDEX'ı var. Bu yüzden ColumnMappings Property'sine INDEX veya Add() methodu uygulayabiliriz.
      
      SqlBulkCopy CLASS'ının WriteToServer() INSTANCE methodu parametre olarak Source Table'ı alır ve veritabanında kopyalama işini yapar.
      Methodun 4 overload'ı var. Metohd parametrelerinde DataRow[], DataTable ve IDataReader Class'ları kullanıldığı için Source Table'dan gelen veri IDataReader ile okunmalı veya DataTable nesnesine doldurulmalıdır. Bu method açık bir bağlantı gerektirir.
      SqlBulkCopy hem kullanımı kolay hemde performans çok etkilidir. 
      
      Veri kaynağı olarak derste XML kullanıldı. Bir Xml dosyasının DataSet'e atmak için ReadXml() methodu kullanılır.
      Not: Debug modda çalışırken, dataset üzerine geldiğimizde açılan penceredeki arama simgesine tıkladığımızda DataSetVisualizer açılır ve örneğini içindeki tabloları ve verilerini görebiliriz.
      
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string conn = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString.ToString();
            DataSet ds = new DataSet();
            ds.ReadXml(Server.MapPath("~/Data.xml"));
            DataTable DepartmentsDataTable = ds.Tables["Department"];
            DataTable EmployeesDataTable = ds.Tables["Employee"];
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                using (SqlBulkCopy bc = new SqlBulkCopy(con))
                {
                    bc.DestinationTableName = "Departments";
                    bc.ColumnMappings.Add("ID", "Id");
                    bc.ColumnMappings.Add("NAME", "Name");
                    bc.ColumnMappings.Add("LOCATION", "Location");
                    bc.WriteToServer(DepartmentsDataTable);

                }
                using (SqlBulkCopy bc = new SqlBulkCopy(con))
                {
                    bc.DestinationTableName = "Employees";
                    bc.ColumnMappings.Add("ID", "Id");
                    bc.ColumnMappings.Add("NAME", "Name");
                    bc.ColumnMappings.Add("GENDER", "Gender");
                    bc.ColumnMappings.Add("DEPARTMENTID", "DepartmentId");
                    bc.WriteToServer(EmployeesDataTable);
                }
            }
        }
    }
}