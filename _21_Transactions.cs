using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace _21_Transactions
{
    /* 
     Transaction'ın veritabanında gerçekleşecek bir grup işden herhangi birinin hata vermesi durumunda çalışmayı durdurup yapılan işlemleri geri almak için kullanılır. Yani Transaction kullandığımızda ya işlerinin tamamı gerçekleşir yada hiç biri gerçekleşmez.
      SqlTransaction CLASS'ı SEALED'dir ve kullanabileceğimiz herhangi bir CONTRUSTER'ı yoktur. Bir SqlTransaction nesnesi oluşturmak için SqlConnection CLASS'ını BeginTransaction() methodu kullanılır. SqlTransaction işlemleri, örneğinin parametre olarak gidildiği SqlCommand nesneleridir.(SqlCommand nesneleri ile Transaciton'ın kullandığı bağlantı aynı olacak.)
      Transaction'a alınmış veritabanı işlerinin gerçekleşip gereçekleşmeyeceğine SqlTransaction CLASS'ını Commit() ve RollBack() methodları belirler. Programda Commit() çalışırsa işlemler veritabanında gerçekleşir. RoollBack() çalışırsa işlemler geri alınır.
     Yani Propgram kodu hata verdiğinde ROllBack() hata vermediğinde Commit() methodu çalışmalı. Bu yüzden try catch bloğu kullanmak gerekiyor.
     Not: Try catch içindeki bir SqlComman nesnesi varsa ve oluşturulmuş SqlTransacton parametresini kullanmıyorsa program hata veriyor.
     Not: SqlTransaciton parametresi alan bir SqlComman nesnesi aynı sorguyu farklı Connection nesnesinden aldığında bile hata veriyor.
     Not: Bu derste ilk defa veritabanı tablosunda string türünde primary key kullandı.
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetData();
            }
        }
        private void GetData() 
        {
            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("Select * from Account",con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {

                    if (rdr["AccountNumber"].ToString() == "A1")
                    {
                        lblAccountNumber1.Text = "A1";
                        lblName1.Text = rdr["Name"].ToString();
                        lblBalance1.Text = rdr["Balance"].ToString();
                    }
                    else
                    {
                        lblAccountNumber2.Text = "A2";
                        lblName2.Text = rdr["Name"].ToString();
                        lblBalance2.Text = rdr["Balance"].ToString();
                    }
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {

                    SqlCommand cmd = new SqlCommand("Update Account set Balance-=10 where AccountNumber='A1'", con,transaction);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand("Update Account set Balance+=10 where AccountNumber='A2'", con, transaction);
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    lbl.Text = "Transaction Successful";
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    lbl.Text = "Transaction failed";
                }
            }
            GetData();
        }
    }
}