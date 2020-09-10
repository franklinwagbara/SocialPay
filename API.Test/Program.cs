using System;
using System.Diagnostics;

namespace API.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // Process.Start("chrome.exe", "http://www.YourUrl.com");
            //Mid^Amt^OrderId
            string mid = "106013";
            decimal amount = 120;
            var refId = "7337627233";

            var algorithm = new EncryptDecrypt();
            var encryptedText = mid +"^" + amount +"^" + refId;
           // var de = algorithm.DecryptAlt("+hgnDAzlw5UjYY5b8shjSOVFQz2r29q8");

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
