using System;
using System.Diagnostics;
using System.Web;

namespace API.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var myUrl = "http://socialpay-web.sterlingapps.p.azurewebsites.net/#/confirm-payments?q=3Xd1AuUoqehJ2fK%20YXm9Yeq5ucFy5Na%205JXgmcDqdJERG78qIDVYKtyaAkmp%2F34tbnLqUDWUX3zM%2FmMhO4uZFw%3D%3D";
            var decodeUrl = System.Uri.UnescapeDataString(myUrl);

            Console.WriteLine("Hello World!");
            // Process.Start("chrome.exe", "http://www.YourUrl.com");
            //Mid^Amt^OrderId
            string newParameter = "3Xd1AuUoqehJ2fK%20YXm9Yeq5ucFy5Na%205JXgmcDqdJERG78qIDVYKtyaAkmp%2F34tbnLqUDWUX3zM%2FmMhO4uZFw%3D%3D";
            string mid = "111024";
            string toDecrypt = "3Xd1AuUoqehJ2fK%20YXm9Yeq5ucFy5Na%205JXgmcDqdJGK1iPjKRN22N%2FY7GClg7pIUlGky5mFmoquTSt2dVQcOg%3D%3D";
            decimal amount = 200;
            var refId = "2525263";
            //string url1 =  HttpUtility.UrlDecode(newParameter);
             //string url =  HttpUtility.UrlDecode(newParameter);

            var sec = new EncryptandDecrypt();

            var encryptedText = mid + "^" + amount + "^" + refId;
            string enConfig = sec.EncryptAlt(encryptedText);
            var lo = "3Xd1AuUoqehJ2fK%20YXm9Yeq5ucFy5Na%205JXgmcDqdJEhkeumNM2I3SCmCgdaeX7khQkmY09j81I%3D";
              string deConfig = sec.DecryptAlt(lo);
              string config = sec.Repad(newParameter);
              string config1 = sec.DecryptAlt(config);
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
        
    }
}
