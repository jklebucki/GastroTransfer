using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GastroTransfer.Models
{
    public class ProductGroupView : INotifyPropertyChanged
    {
        public int ProductGroupId { get; set; }
        public int ExternalGroupId { get; set; }
        private string _groupName = string.Empty;
        public string GroupName
        {
            get
            {
                return this._groupName;
            }

            set
            {
                if (value != this._groupName)
                {
                    this._groupName = value;
                    NotifyPropertyChanged();
                }
            }
        }
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
