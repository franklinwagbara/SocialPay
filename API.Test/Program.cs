using bankService;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace API.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string data = "THExxQUICKxxBROWNxxFOX";

            var ho = data.Split(new string[] { "xx" }, StringSplitOptions.None);

            var tranreference = string.Empty;

            //foreach (var item in ho)
            //{
            //    var jn = item;
            //    if(jn.Contains("BR"))
            //    {
            //        tranreference = jn;
            //    }
            //}

            string sDate = DateTime.Now.ToShortDateString();

            var random = new Random();
            string randomNumber = string.Join(string.Empty, Enumerable.Range(0, 10).Select(number => random.Next(0, 9).ToString()));
           
            var timeString = DateTime.Now.ToString("hh:mm:ss");
            string date = DateTime.UtcNow.ToString("MM-dd-yyyy");
            string date2 = DateTime.UtcNow.ToString("MMddyyyyhhmmss");

            string braCode = "000001" + date2 + randomNumber;
            //DateTime nextDay = DateTime.Now.Date;
            //String myDate = "05-12-2020";
            //DateTime sdate = DateTime.Parse(myDate);

            //var lockAccountModel = new TestApps
            //{
            //    eDate = DateTime.Today
            //};

            ////var getFees = GetNIPFee(12000);
            ////var dataList = getFees.Tables["Table"]
            ////   .AsEnumerable()
            ////   .Select(i => new FeesViewModel
            ////   {
            ////       FeeAmount = i["feeAmount"].ToString(),
            ////       Vat = i["vat"].ToString()
            ////   }).FirstOrDefault();

            var sec = new EncryptDecrypt();
            var myUrl = "http://socialpay-web.sterlingapps.p.azurewebsites.net/#/confirm-payments?q=3Xd1AuUoqehJ2fK%20YXm9Yeq5ucFy5Na%205JXgmcDqdJERG78qIDVYKtyaAkmp%2F34tbnLqUDWUX3zM%2FmMhO4uZFw%3D%3D";
            //var decodeString = "PpfjduWjfRUoNMbQnrfIwqJ1piIJVJexGDKKJMt6evqbUkilDLUUwooxhgDnPBE6o%2FsE5lumxNYOWL5DuHvKaQ%3D%3D";
            var decodeWorkingString = "QcKGLrMvsAUJ08snV7PKPNyYBnx6zErBI7T6l7BlDQa1ieYtT3NtjvKCZjjlBP7m2V1oVT7Zac1Jubh2DMld78wzibzRC1DBuRgq4XoUqqCKKM5sIxwSOWhJfhXlB6yGUw%20hu2W0nX6AHR8%2F89wCENwYIJYxi52w3rGHjWFDuxLU1FBjtsb5MayKcwPWSksx";
            var decodeString = "eOnQBWdWVJ9cuPMmcDbEwCKVWtRxZgexLIOvor2YppSzR%20t%20flTqH2cm%208uY8bmi6jZbty28XpF1cL37r3GvycfIJBOgoBs6GM2GXb8TbsQy0LLRcX8LVjw9ake9EPjk";
            var decodeMessage = System.Uri.UnescapeDataString(decodeWorkingString);
            if (decodeMessage.Contains(" "))
            {
                decodeMessage = decodeMessage.Replace(" ", "+");
            }
            var getMessage = sec.DecryptAlt(decodeMessage);

            var newreference = getMessage.Split("^");

            foreach (var item in newreference)
            {
                if (item.Contains("SBP") || item.Contains("sbp"))
                {
                    tranreference = item;
                }

                var jn = item;
                if (jn.Contains("SBP") || jn.Contains("sbp"))
                {
                    tranreference = jn;
                }
            }

            var reference = getMessage.Split("^")[7];

            string newRes = string.Empty;

            if (getMessage.Contains("approve") || getMessage.Contains("success") || getMessage.Contains("Approve"))
            {
                newRes = "Approved";
            }
            newRes = "Failed";
            var bankService = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap,
                  "");

            //var createCorporateAccount = bankService.CreateCorporateCustomer2Async

            var dateMain = DateTime.Now.ToString("MM-yyyy-dd");

            Console.WriteLine("Hello World!");
            // Process.Start("chrome.exe", "http://www.YourUrl.com");
            //Mid^Amt^OrderId
            string newParameter = "3214Xd1AuUoqehJ2fK YXm9Yeq5ucFy5Na 5JXgmcDqdJERG78qIDVYKtyaAkmp/34tbnLqUDWUX3zM/mMhO4uZFw==";
            //string mid = "111024";
            string mid = "112024";
            string toDecrypt = "3Xd1AuUoqehJ2fK%20YXm9Yeq5ucFy5Na%205JXgmcDqdJGK1iPjKRN22N%2FY7GClg7pIUlGky5mFmoquTSt2dVQcOg%3D%3D";
            decimal amount = 2333;
            var refId = Guid.NewGuid().ToString("N");
            //string url1 =  HttpUtility.UrlDecode(newParameter);
             //string url =  HttpUtility.UrlDecode(newParameter);

           

            var encryptedText = mid + "^" + amount + "^" + refId;

            var kk = sec.EncryptAlt(encryptedText);

            var lo = "3Xd1AuUoqehJ2fK%20YXm9Yeq5ucFy5Na%205JXgmcDqdJEhkeumNM2I3SCmCgdaeX7khQkmY09j81I%3D";

            if (newParameter.Contains(" "))
            {
                newParameter = newParameter.Replace(" ", "+");
            }
            var pp = "EgbOBbHXQXvsy7fst1ARZJJLdz9X2CtGvpPwSRFEHFB5vwwCHBPNwxR/1K8+R6GoNyNKe4TXQcrT7ISgKoA2YdFIxJ93j033";
            var decryptedData1 = sec.DecryptAlt(pp);

            string getReference = decryptedData1.Split("^")[5];

            var decryptedData = EncryptandDecrypt.DecryptAlt(newParameter);
           // ViewBag.Status = decryptedData;

            string deConfig = EncryptandDecrypt.DecryptAlt(newParameter);
              //string config = sec.Repad(newParameter);
             // string config1 = sec.DecryptAlt(config);
            //var dn = Convert.FromBase64String(url);
            var algorithm = new EncryptDecrypt();
           
           
            // var de = algorithm.DecryptAlt("+hgnDAzlw5UjYY5b8shjSOVFQz2r29q8");
            var decryptTest = algorithm.DecryptAltOld(newParameter);
           // byte vc = algorithm.(newParameter);
            var en = algorithm.EncryptAlt(encryptedText);

            System.Diagnostics.Process.Start("cmd", "/C start https://pass.sterling.ng/sterlinggateway/?q=" + en);
            System.Diagnostics.Process.Start("http://www.google.com");
            Process.Start("https://pass.sterling.ng/sterlinggateway/?q=+hgnDAzlw5UjYY5b8shjSOVFQz2r29q8/");
            var psi = new ProcessStartInfo("chrome.exe");
            psi.Arguments = "https://pass.sterling.ng/sterlinggateway/?q=+hgnDAzlw5UjYY5b8shjSOVFQz2r29q8/";
            Process.Start(psi);
        }
        
        public class TestApps
        {
            public DateTime? eDate { get; set; }
        }

        public class FeesViewModel
        {
            public string FeeAmount { get; set; }
            public string Vat { get; set; }
        }
        public static DataSet GetNIPFee(decimal amount)
        {
            DataSet ds = new DataSet();

            try
            {
                SqlCommand sqlcomm = new SqlCommand
                {
                    Connection = new SqlConnection("Server=10.0.41.101;Database=nfpdb_test;User Id=sa; Password=tylent;MultipleActiveResultSets=true;")
                };

                using (sqlcomm.Connection)
                {
                    using SqlDataAdapter da = new SqlDataAdapter();
                    sqlcomm.Parameters.AddWithValue("@amt", amount);
                    sqlcomm.CommandText = "spd_getNIPFeeCharge";
                    da.SelectCommand = sqlcomm;
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;

                    da.Fill(ds);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return ds;
        }

    }
}
