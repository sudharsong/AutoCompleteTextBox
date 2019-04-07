
namespace AutoCompleteTextBoxUIApplication
{
    internal class Person : NotifyChage
    {
        private int id;
        private string name;
        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
                this.NotifyPropertyChanged();
            }
        }

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
            return string.Format("{0} - {1} ", id, name);
        }
    }
}
