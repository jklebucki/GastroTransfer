
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GastroTransfer.Models
{
    /// <summary>
    /// Positions to create buttons
    /// </summary>
    public class ProducedItemView : INotifyPropertyChanged
    {
        public int ProducedItemId { get; set; }
        public string Name { get; set; }
        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return this._isActive;
            }

            set
            {
                if (value != this._isActive)
                {
                    this._isActive = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string UnitOfMesure { get; set; }
        public decimal ConversionRate { get; set; }
        public string ExternalId { get; set; }
        public string ExternalIndex { get; set; }
        public string ExternalName { get; set; }
        public string ExternalUnitOfMesure { get; set; }
        private int _productGroupId;
        public int ProductGroupId
        {
            get
            {
                return this._productGroupId;
            }

            set
            {
                if (value != this._productGroupId)
                {
                    this._productGroupId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
