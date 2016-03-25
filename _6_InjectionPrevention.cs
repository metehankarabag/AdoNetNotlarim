using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace _6_InjectionPrevention
{
	/*Kullanıcının sorguya sadece değer gönderebileceği bir kod yazmanın 2 yolu var.
      1. Stored Procedure 
      2. Parametiried Query
      
      1. STORED PROCEDURE
      StoredProcedure kullanırken SqlCommand nesnesinin CommandText özelliğine sorgu yerine StoredProcedure adı verilir.
      CommandText özelliğin bir sorgu vermedik. Bu yüzden CommandText çalışırılırsa hata alırız. girilen değerin bir sorgu değil StoreProcedure adı olduğunu belirlemeliyiz. Böylece veritabanında aranır-bulunur-çalıştırılır.
      Bu belirleme SqlComman örneğinin CommanType PROPERTY'sinde yapılır. PROPERTY parametre olarak CommanType ENUM'u ister. Bu enumu kullanarak belirleyebiliriz.
      
      PROCEDURE'a parametre göndereceksek SqlCommand örneğine parametre eklemek zorundayız. Bunun için SqlCommand nesnesinin Parameters PROPERTY'sine  AddWithValue() methodunu kullanabiliriz.
      Not: Procedure kaç parametresitiyorsa o kadar uygulamak zorundayız.
      
      SqlCommand'ın üyesi olan Parameters PROPERTY'si READ-ONLY'dir ve türü SqlParameterCollection'dır.
      AddWithValue() methodu SqlParametre nesnesi döner ve SqlParameterCollection SEALED CLASS'ının bir üyesidir.
      Yani Parameters PROPERTY'sine SqlCommand örneği üzerinden ulaşılırken, AddWithValue() methoduna SqlParameterCollection örneğinden ulaşılır.
      Property'i programa SqlParameterCollection örneği getirdiği için methoda ulaşabiliyoruz.
      AddWithValue() methodu iki parametre alır. 1. Procedure'un istediği parmetrenin adı(aynı olmak zorunda) 2. gönderilecek değer.
      Method bu değerler ile SqlParameter döndüğü için Parameters PROPERTY'sine bu methodu uygulama yerine add() metohudunu uygulayıp 1. overload'ına daha öncede oluşturduğumuz SqlParametresini girebiliriz. Böylece birden fazla Procedure aynı parametreyi kullanıyorsa bir sefer yazıp birden fazla kez kullanabiliriz.
      
      2. Parameterized Query: Kullanım olarak Procedure ile aynı tek fark STORED PROCEDURE yerine sorgu parametresi olması.
      
	  Not: Stored Procedure parametre olarak 
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageLoad1();


            PageLoad();
          
        }
        #region STORED PROCEDURE
        private void PageLoad1()
        {
            string CS = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spGetProductsByName", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //Cammand nesnesi ile parametreyi ve değerinin geleceği yeri belirle.
                cmd.Parameters.AddWithValue("@ProductName", TextBox1.Text + "%");
                con.Open();
                GridView1.DataSource = cmd.ExecuteReader();
                GridView1.DataBind();
            }
        }
        #endregion
        #region PARAMETERIZED QUERY
        private void PageLoad()
        {
            string CS = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                string Command = "Select * from tblProductInventory where ProductName like @ProductName";
                SqlCommand cmd = new SqlCommand(Command, con);
                cmd.Parameters.AddWithValue("@ProductName", TextBox1.Text + "%");
                con.Open();
                GridView1.DataSource = cmd.ExecuteReader();
                GridView1.DataBind();
            }
        }
        #endregion
    }
}
