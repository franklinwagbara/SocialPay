using bankService;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;

namespace API.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            var newRef = $"{"So-Pay-"}{Guid.NewGuid().ToString().Substring(0, 15)}";

            //string iString = "2005-05-05 22:12 PM";
            //yyyy-MM-dd

            DateTime firstDate = new DateTime(2017, 03, 03);

            //Second Date
            DateTime secondDate = new DateTime(2018, 06, 06); //DateTime.Now;


            DateTime oDT = DateTime.ParseExact("05-Oct-2019", "dd-MMM-yyyy",
             CultureInfo.InvariantCulture);

            DateTime oDT1 = DateTime.ParseExact("05-Oct-2020", "dd-MMM-yyyy",
                CultureInfo.InvariantCulture);


            string s1 = oDT.ToString("yyyy-MM-dd");

            //string startDate = "2019-10-01";
            //string endDate = "2020-9-01";

            //DateTime dateTime11 = DateTime.Parse(startDate);
            //DateTime dateTime12 = DateTime.Parse(endDate);

            //int months = MonthDiff(dateTime11, dateTime12);

            //Console.WriteLine("First Date  :" + firstDate);
            //Console.WriteLine("Second Date :" + secondDate);
            //Console.WriteLine("Months      :" + months);
            //Console.ReadLine();

            //CultureInfo culture = new CultureInfo("en-US");
          
           
            //DateTime stempDate = Convert.ToDateTime(startDate, culture);
            //DateTime etempDate = Convert.ToDateTime(endDate, culture);
            //DateTime date1 = new DateTime(2020, 8, 28);
            //DateTime date3 = new DateTime(2021, 11, 2);

            //int month2 = dateTime12.Month - dateTime11.Month;

            //int month1 = date3.Month - date1.Month;

            //int month = etempDate.Month - stempDate.Month;

         

            //////int month3 = oDT1.Month - oDT.Month;
            ////////string szDT = oDT.ToString("MM-dd-yyyy");
            //////string szDT = oDT.ToString("yyyy-MM-dd");
            //////string data = "THExxQUICKxxBROWNxxFOX";

            //////var ho = data.Split(new string[] { "xx" }, StringSplitOptions.None);

            var tranreference = string.Empty;

            //foreach (var item in ho)
            //{
            //    var jn = item;
            //    if(jn.Contains("BR"))
            //    {
            //        tranreference = jn;
            //    }
            //}

            //////string sDate = DateTime.Now.ToShortDateString();

            //////var random = new Random();
            //////string randomNumber = string.Join(string.Empty, Enumerable.Range(0, 10).Select(number => random.Next(0, 9).ToString()));
           
            //////var timeString = DateTime.Now.ToString("hh:mm:ss");
            //////string date = DateTime.UtcNow.ToString("MM-dd-yyyy");
            //////string date2 = DateTime.UtcNow.ToString("MMddyyyyhhmmss");

            //////string braCode = "000001" + date2 + randomNumber;
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
            //var decodeString = "eOnQBWdWVJ9cuPMmcDbEwCKVWtRxZgexLIOvor2YppSzR%20t%20flTqH2cm%208uY8bmi6jZbty28XpF1cL37r3GvycfIJBOgoBs6GM2GXb8TbsQy0LLRcX8LVjw9ake9EPjk";
            var decodeString = "PpfjduWjfRUoNMbQnrfIwqJ1piIJVJexGDKKJMt6evqbUkilDLUUwooxhgDnPBE6jtaoY8Rz7uHUNnHn10CclMU39USagI5FR%2FEKXma9RnT53tvjMiQw5sw1Xjjnzbtpsa%2FGCxzAm0k%3D";
            var decodeMessage = System.Uri.UnescapeDataString(decodeString);           
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
        public class PayWithSpectaVerificationRequestDto
        {
            public string verificationToken { get; set; }
        }
        public static int MonthDiff(DateTime d1, DateTime d2)
        {
            int m1;
            int m2;
            if(d1<d2)
            {
                m1 = (d2.Month - d1.Month);//for years
                m2 = (d2.Year - d1.Year) * 12; //for months
            }
            else
            {
                m1 = (d1.Month - d2.Month);//for years
                m2 = (d1.Year - d2.Year) * 12; //for months
            }
            
            return  m1 + m2;
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
