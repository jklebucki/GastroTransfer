using System;

namespace ProductionReportWF.DbSettings.Models
{
    public class ProductionView
    {
        public string Nazwa { get; set; }
        public string JM { get; set; }
        public decimal Ilosc { get; set; }
        public bool Wyslany { get; set; }
        public DateTime DataProdukcji { get; set; }
        public DateTime? DataWyslaniaDoLSI { get; set; }
    }
}
