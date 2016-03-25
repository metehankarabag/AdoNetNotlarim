using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace _12_CachingDataSet
{
	/*
      Cache PROPERTY'si Page CLASS'ının READ-ONLY üyesidir ve türü Cache'dir. Cache CLASS'nın object türünde ve değer alabilen bir INDEX vardır. Bu sayede Page CLASS'ının Cache PROPERTY'sine index değeri girebiliriz ve değer atayabiliriz. 
      Cache CLASS'ında 5 tane overload'ı olan bir INSERT() methodu, tek overload'ı olan add(),Remove(),Get() methodu var. Bir kaç tanede PROPERTY'si var. Bu method'lar cache'deki nesnenin nasıl saklanacağını belirler.
     Not: Cache[] object dönüdüğü için bunların hiç birine bu PROPERTY'den ulaşmayız.
      
     Bu derste yapılan işin hiçbiri ado.net ile ilgili değil. Cache kullanım mantığı tüm işlerde aynı. Index değeri olan Cache varmı yokmu diye kontrol et yoksa oluştur doldur varsa kullan.
     */
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void btnLoadData_Click(object sender, EventArgs e)
        {
            if (Cache["Data"] == null)
            {
                string CS = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(CS))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter("Select * from tblProductInventory", connection);
                    DataSet dataset = new DataSet();
                    dataAdapter.Fill(dataset);

                    gvProducts.DataSource = dataset;
                    gvProducts.DataBind();

                    Cache["Data"] = dataset;
                    lblMessage.Text = "Data loaded from the Database";
                }
            }
            else { gvProducts.DataSource = (DataSet)Cache["Data"]; gvProducts.DataBind(); lblMessage.Text = "Data loaded from the Cache"; }
        }

        protected void btnClearnCache_Click(object sender, EventArgs e)
        {
            if (Cache["Data"] != null) { Cache.Remove("Data"); lblMessage.Text = "DataSet removed from the cache"; }
            else						lblMessage.Text = "There is nothing in the cache to remove";
            
        }
    }
}