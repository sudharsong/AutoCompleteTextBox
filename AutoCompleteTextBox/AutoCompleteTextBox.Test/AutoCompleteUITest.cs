extern alias UiClient;
using Microsoft.Test;
using Microsoft.Test.ApplicationControl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;


namespace AutoCompleteTextBoxTest
{
    [TestClass]
    public class UnitTest1
    {
        private string sampleAppPath;
        
        [TestInitialize]
        public void Setup()
        {
            sampleAppPath = string.Concat(AppDomain.CurrentDomain.BaseDirectory
                .Replace("AutoCompleteTextBox.Test", "AutoCompleteTextBoxUIApplication"),
                @"\AutoCompleteTextBoxUIApplication.exe");
        }

        [TestCleanup]
        private void TearDown()
        {
            //Todo
        }

        [TestMethod]
        public void ItemSourceLengthTest()
        {
            InProcessApplication application = GetApplicationConfiguration();
            application.Start();
            try
            {
                application.WaitForMainWindow(TimeSpan.FromMilliseconds(10));
                var mainWindow = application.MainWindow as AutomationElement;
                MyCustomControls.AutoCompleteTextBox autoCompleteBox = TestHelper.GetVisualChild<MyCustomControls.AutoCompleteTextBox>(application.MainWindow as DependencyObject);
                Assert.AreEqual(15, (autoCompleteBox.ItemSource as ICollection).Count);
            }
            finally
            {
                application.Close();
            }
        }

        [TestMethod]
        public void TargetMemberNameTest()
        {
            InProcessApplication application = GetApplicationConfiguration();
            application.Start();
            try
            {
                application.WaitForMainWindow(TimeSpan.FromMilliseconds(10));
                var mainWindow = application.MainWindow as AutomationElement;
                MyCustomControls.AutoCompleteTextBox autoCompleteBox = TestHelper.GetVisualChild<MyCustomControls.AutoCompleteTextBox>(application.MainWindow as DependencyObject);
                Assert.AreEqual("Name", autoCompleteBox.TargetMember);
            }
            finally
            {
                application.Close();
            }
        }

        [TestMethod]
        public void SetTextValueAndCheckFilterTest()
        {
            InProcessApplication application = GetApplicationConfiguration();
            application.Start();
            try
            {
                application.WaitForMainWindow(TimeSpan.FromMilliseconds(10));
                var mainWindow = application.MainWindow as AutomationElement;
                MyCustomControls.AutoCompleteTextBox autoCompleteBox = TestHelper.GetVisualChild<MyCustomControls.AutoCompleteTextBox>(application.MainWindow as DependencyObject);
                autoCompleteBox.Text = "mic";
                Assert.AreEqual(5, autoCompleteBox.ListBoxControl.Items.Count);
            }
            finally
            {
                application.Close();
            }
        }

        [TestMethod]
        public void ZeroItemsFilteredTextCharactersLessThanThreeTest()
        {
            InProcessApplication application = GetApplicationConfiguration();
            application.Start();
            try
            {
                application.WaitForMainWindow(TimeSpan.FromMilliseconds(10));
                var mainWindow = application.MainWindow as AutomationElement;
                MyCustomControls.AutoCompleteTextBox autoCompleteBox = TestHelper.GetVisualChild<MyCustomControls.AutoCompleteTextBox>(application.MainWindow as DependencyObject);
                autoCompleteBox.Text = "mi";
                Assert.AreEqual(0, autoCompleteBox.ListBoxControl.Items.Count);
            }
            finally
            {
                application.Close();
            }
        }

        [TestMethod]
        public void TextSetToNullTest()
        {
            InProcessApplication application = GetApplicationConfiguration();
            application.Start();
            try
            {
                application.WaitForMainWindow(TimeSpan.FromMilliseconds(10));
                var mainWindow = application.MainWindow as AutomationElement;
                MyCustomControls.AutoCompleteTextBox autoCompleteBox = TestHelper.GetVisualChild<MyCustomControls.AutoCompleteTextBox>(application.MainWindow as DependencyObject);
                autoCompleteBox.Text = "m";
                autoCompleteBox.Text = null;
                Assert.AreEqual(0, autoCompleteBox.ListBoxControl.Items.Count);
            }
            finally
            {
                application.Close();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MyCustomControls.DataBindingException))]
        public void ItemSourceSetNullExceptionExpectedTest()
        {
            InProcessApplication application = GetApplicationConfiguration();
            application.Start();
            try
            {
                application.WaitForMainWindow(TimeSpan.FromMilliseconds(10));
                var mainWindow = application.MainWindow as AutomationElement;
                MyCustomControls.AutoCompleteTextBox autoCompleteBox = TestHelper.GetVisualChild<MyCustomControls.AutoCompleteTextBox>(application.MainWindow as DependencyObject);
                autoCompleteBox.ItemSource = null;                
            }
            finally
            {
                application.Close();
            }
        }

        private InProcessApplication GetApplicationConfiguration()
        {
            InProcessApplication application = new InProcessApplication(new WpfInProcessApplicationSettings
            {
                Path = sampleAppPath,
                InProcessApplicationType = InProcessApplicationType.InProcessSameThread,
                WindowClassName = "AutoCompleteTextBoxUIApplication.MainWindow",
                ApplicationImplementationFactory = new WpfInProcessApplicationFactory()
            });
            return application;
        }
    }
}
