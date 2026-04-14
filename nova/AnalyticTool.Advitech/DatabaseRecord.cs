using System;

namespace AnalyticTool.Advitech
{
    public class DatabaseRecord
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Identification { get; set; }
        public string CostCenter { get; set; }
        public string Department { get; set; }
        public DateTime? EmploymentBeginningDate { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public string CardNumber { get; set; }
        public int Pump1 { get; set; }
        public int Pump2 { get; set; }
        public int Pump3 { get; set; }
        public int Pump4 { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public int Invoiced { get; set; }
    }
}
