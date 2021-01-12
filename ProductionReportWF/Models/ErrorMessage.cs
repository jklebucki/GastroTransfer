using System;

namespace ProductionReportWF.Models
{
    public class ErrorMessage
    {
        public int MessageId { get; set; }
        public DateTime MessageDate { get; set; }
        public double ErrorValue { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}
