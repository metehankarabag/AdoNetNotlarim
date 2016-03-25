using System;
using System.Configuration;
using System.Data.SqlClient;

namespace _3_ConnectionStringFormConfigFile
{
	/*
      ConfigurationManager CLASS'ı config dosyasından bilgi çeklemek için kullandığımız STATIC bir CLASS'dır.
      
      Static CLASS ve Abstract CLASS'ların bir örneği kurulamaz. Fakat STATIC olan sadece STATIC üye barındırabilirken, ABSTRACT olan abstract, static ve instance üye barındırabilir. Temel fark ise ABSTACT CLASS'lar base CLASS olarak kullanılamazken, ABSTRACT'lar kullanılabilir.
      Yanlış anlamadıysam .net çalışma anında STATIC CLASS'lar ile ilgili fikri olmadığı için derleyiciler STATIC CLASS'ları hem ABSTRACT hemde SEALED CLASS olarak derliyormuş. Yani STATIC CLASS'lar hem ABSTRACT hemde SEALED CLASS'lardır.(Böyle düşünmemek lazım fakat kapıya çıkıyor.)
      
      STATIC CLASS'ların kullanım amaçı herhangi bir nesneye ait olmayan method'lar oluşturmakmış. Bu nedenle STATIC CLASS'lar SINGLETON/STATELESS kullanıma uygunken ABSTRACT'lar uygun değil.
     
      +-------------------------+---+--------+--------+--------+----------+
      |       Class Type        |   | normal | static | sealed | abstract |
      +-------------------------+---+--------+--------+--------+----------+
      | Can be instantiated     | : | yes    | no     | yes    | no       |
      | Can be inherited        | : | yes    | no     | no     | yes      |
      | Can inherit from others | : | yes    | no     | yes    | yes      |
      +-------------------------+---+--------+--------+--------+----------+
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Select * from tblProductInventory", connection);
                connection.Open();

                Gridview1.DataSource = cmd.ExecuteReader();
                Gridview1.DataBind();
                //in Form Applications
                //BindingSource source = new BindingSource();
                //source.DataSource = cmd.ExecuteReader();
                //dataGridView1.DataSource = source;
            }
        }
    }
}