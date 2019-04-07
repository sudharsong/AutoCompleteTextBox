using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCompleteTextBoxUIApplication
{
    class Company : NotifyChage
    {
        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.NotifyPropertyChanged();
            }
        }

        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(this.name) ? this.name : "Not initialized";
        }
    }
}
