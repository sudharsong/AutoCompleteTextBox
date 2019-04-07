using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoCompleteTextBoxUIApplication
{
    internal class NotifyChage : INotifyPropertyChanged
    {
        /// <summary>
        /// Event to raise when a property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify the property sent has been changed.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                Debug.WriteLine("propertyName is Empty. CallStack {0}");
                return;
            }

            var handler = PropertyChanged;
            if (handler != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
