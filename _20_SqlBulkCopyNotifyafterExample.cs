using System;
using System.Data.SqlClient;
using System.Configuration;

namespace _20_SqlBulkCopyNotifyafterExample
{
    /*
     Bu derste SqlBulkCopy CLASS'ının 2 Property'sini ve SqlRowsCopied Event'ını göreceğiz. -> BatchSize(yığın genişliği),NotifyAfter(Sonra bildir)
     BatchSize: Tek seferde destination veritabanına kaç tane satırın kopyalanacağını belirler. Veri transferinin hızı bu PORPERTY'e bağlı olduğu için çok önemlidir. Varsayılan değeri 1'dir. Yani bir DataReader bir satır okuduğunda SqlBulkCopy'e verir kopyalama işlemi yapılır. 10000 yaparsak SqlBulkCopy bu kadar satırı aldıktan sonra tek bir Batch içinde kopyalama işlemi yapar.
     NotifyAfter: SqlRowsCopied EVENT'ının çalışması için kaçtane satırın işleme girmesi gerektiğini belirler. Yani 5000 olarak ayarlandığında ve 5000 satır destination veritabanına kopyalandığında SqlRowsCopied EVENT'ı çalışır.(Böyle demiş.)
     SqlRowsCopied: NotifyAfter PROPERTY'sinde belirtilen kopyalanmış satır değerine göre yazıldığı çalıştığı için işlem durumunu reporlamak için kullanışlıdır.
      NotifyAfter veriler destination veritabanına kopyalandıktan sonra Event'i çalıştırıyorsa, BatchSize'ı 10000 verdiğimizde veritabanına 10000 satırda bir gideceği için 5000'i kopyaladıktan sonra programa geri dönüp event'i çalıştırması lazım. başka çalışma mantığı yok gibi.(Nedense sinirimi bozdu.)
      
      Not: ConfigurationManager CLASS'ının Assembly'si Consoel uygulamalarında yüklü değil ve Application Configuration file eklemenden de References'a eklenmiyor.
      tabloları oluşturmadım.
    */
    class Program
    {
        static void Main(string[] args)
        {
            string sourceCS = ConfigurationManager .ConnectionStrings["SourceDBCS"].ConnectionString;
            string destinationCS = ConfigurationManager.ConnectionStrings["DestinationDBCS"].ConnectionString;

            using (SqlConnection sourceCon = new SqlConnection(sourceCS))
            {
                SqlCommand cmd = new SqlCommand("select * from Products_Source", sourceCon);
                sourceCon.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    using (SqlConnection destinationCon = new SqlConnection(destinationCS))
                    {
                        using (SqlBulkCopy bc = new SqlBulkCopy(destinationCon))
                        {
                            bc.DestinationTableName = "Products_Destination";
                            bc.BatchSize = 10000;
                            bc.NotifyAfter = 5000;
                            bc.SqlRowsCopied += (sendeR, eventArgs) => { Console.WriteLine(eventArgs.RowsCopied + "loaded..."); };
                            destinationCon.Open();
                            bc.WriteToServer(rdr);
                        }
                    }
                }
            }
        }
        //void bc_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        //{
        //    Response.Write(e.RowsCopied + "loaded...");
        //}
    }
}
