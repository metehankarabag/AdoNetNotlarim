using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

namespace _7_CallingStoredProcedureWithOutputParameters
{
	/*
      Bu derste OUTPUT parametresi de bekleyen PROCEDURE çalıştıracağız. PROCEDURE'ın veya SQL'de çalışabilecek herhangi bir şeyin tüm gerekliliği COMMAND nesnesine eklenmelidir. Geçen derste PROCEDURE'ın INPUT parmetrelerini command nesnesine AddWithValue() kullanarak ekledik. Bu methodun tek bir OVERLOAD'ı var ve bu OVERLOAD ile parametre türünü belirleyemiyoruz. Bu yüzden SQLPARAMETER nesnesi oluşturmalıyız. Bu CLASS ile parametre üzerinde tam kontrol sağlayabiliriz.
      
     SqlParmeter CLASS SEALD bir CLASS'dır. 7 OVERLOAD'ı vardır ve sadece CONTRUCTER'ını kullanarak da parametreyi belirleyebiliriz. Constucter'ları toplam 13 PROPERTY kullanılıyor. Consturctor'da kullanılmayıp SQLParametre CLASS'ında olan 7 PROEPRTY daha var. (CompareInfo, DbType, LocaleId, Offset, SqlValue, TypeName, UdtTypeName)
      
      Direction PROPERTY'si PARAMETER'in INPUT mu OUTPUT'u olduğunu belirlemek için kullanılır. Değer olarak ParameterDirection ENUMU bekler.
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                #region 1. kısım SQL'de çalıştıracağımız komutu belirlemek. STORE PROCEDURE kullandığında beklediği INPUT değerleri gönderme
                //SQL komut nesnesini oluşturuldu.
                SqlCommand cmd = new SqlCommand("spAddEmployee", con);
                //Komutub STORE PROCEDURE olduğu belirlendi.
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //Komutdaki STORE PROCEDURE'ın beklediği giriş değerleri eklendi.
                cmd.Parameters.AddWithValue("@Name", txtEmployeeName.Text);
                cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);
                cmd.Parameters.AddWithValue("@Salary", txtSalary.Text);
                #endregion

                #region 2. SQL'de çalışacak komuta parametre oluşturma
                //SQL parametresi oluşturuldu.
                SqlParameter outPutParameter = new SqlParameter();
                //SQL SERVER'den alınacak parametre belirlendi.
                outPutParameter.ParameterName = "@EmployeeId";
                //Parametre türü belirlendi.
                outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                //Çıkış paremetresi olduğu belirlendi.
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                #endregion
                //Oluşturulan parametre çalıştırılacak komuta eklendi.
                cmd.Parameters.Add(outPutParameter);
                con.Open();
                cmd.ExecuteNonQuery();

                string EmployeeId = outPutParameter.Value.ToString();
                lblMessage.Text = "Employee Id = " + EmployeeId;
            }
        }
    }
}