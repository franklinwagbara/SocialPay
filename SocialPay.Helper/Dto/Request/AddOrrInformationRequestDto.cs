using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class AddOrrInformationRequestDto
    {

        public int maritalStatus { get; set; }
        public int natureOfIncome { get; set; }
        public string incomeSource { get; set; }
        public int monthlyIncome { get; set; }
        public int incomeSourceBusinessSegmentId { get; set; }
        public int accommodationType { get; set; }
        public int jobChanges { get; set; }
        public int numberOfDependants { get; set; }
        public int yearsInCurrentResidence { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }

}
