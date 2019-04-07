using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoCompleteTextBoxUIApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Initialize the main window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ///this.DataContext = GeneratePersonList();
            this.DataContext = GenerateCompanyList();
        }

        /// <summary>
        /// Generate mock company list.
        /// </summary>
        /// <returns>GeneratedList.</returns>
        private List<Company> GenerateCompanyList()
        {
            List<Company> companies = new List<Company>
            {
                new Company { Name = "Microsoft"},
                new Company { Name = "MindTree"},
                new Company { Name = "MindPlayers"},
                new Company { Name = "Reliance"},
                new Company { Name = "Siemens"},
                new Company { Name = "Micron"},
                new Company { Name = "MicroFlex"},
                new Company { Name = "Nokia"},
                new Company { Name = "Apple"},
                new Company { Name = "Micetek"},
                new Company { Name = "Microtel"},
                new Company { Name = "Reliance communications"},
                new Company { Name = "Tata CS"},
                new Company { Name = "Tata Aviation"},
                new Company { Name = "Tata Energy"}
            };

            return companies;
        }

        /// <summary>
        /// Generates Mock persons list.
        /// </summary>
        /// <returns>GeneratedList.</returns>
        private List<Person> GeneratePersonList()
        {
            List<Person> persons = new List<Person>
            {
                new Person { Id = 1, Name = "John"},
                new Person { Id = 2, Name = "Johnson"},
                new Person { Id = 3, Name = "Mathews"},
                new Person { Id = 4, Name = "Matt"},
                new Person { Id = 5, Name = "George"},
                new Person { Id = 6, Name = "Kelly"},
                new Person { Id = 7, Name = "Karthik"},
                new Person { Id = 8, Name = "Stephen"},
                new Person { Id = 9, Name = "Williams"},
                new Person { Id = 10, Name = "Wilson"},
                new Person { Id = 11, Name = "BillGie"},
                new Person { Id = 12, Name = "Tony Fu"}
            };

            return persons;
        }
    }
}
