using System;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace MyCustomControls
{
    [TemplatePart(Name="Part_Popup",Type= typeof(Popup))]
    [TemplatePart(Name = "Part_ListBox", Type = typeof(ListBox))]
    public class AutoCompleteTextBox : TextBox
    {        
        CollectionView collectionView = null;
        Popup popup = null;
        ListBox listBox = null;
        private const string Popup_Part_Name = "Part_Popup";
        private const string ListBox_Part_Name = "Part_ListBox";
        ////private const string Part_Inline_TextBlock_Name = "Part_Inline_TextBlock";
        private bool raiseTextChanged = true;
        private volatile string textToFilter;

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(AutoCompleteTextBox), new PropertyMetadata(null));
        
        /// <summary>
        /// Gets or sets SelectedItem
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ItemSource.
        /// </summary>
        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        /// <summary>
        /// Dependency property for the ItemSource.
        /// </summary>
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(IEnumerable), typeof(AutoCompleteTextBox), new PropertyMetadata(ItemSourceChanged));

        /// <summary>
        /// Method to handle the ItemSource changed.
        /// </summary>
        /// <param name="d">dependecncy object.</param>
        /// <param name="e">Arguments passed.</param>
        private static void ItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var autoCompleteBox = d as AutoCompleteTextBox;
            if (e.NewValue == null)
            {
                throw new DataBindingException("Dependency property ItemSource Should not be null");
                return;
            }
            
            autoCompleteBox.ProcessAndSetItemSource(e.NewValue);
        }

        /// <summary>
        /// Gets or Sets the Target Member.
        /// </summary>
        public string TargetMember
        {
            get { return (string)GetValue(TargetMemberProperty); }
            set { SetValue(TargetMemberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetMember.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetMemberProperty =
            DependencyProperty.Register("TargetMember", typeof(string), typeof(AutoCompleteTextBox), new PropertyMetadata(string.Empty, TargetMemberValueChange));

        /// <summary>
        /// Method to handle the Target member dp change.
        /// </summary>
        /// <param name="d">dependecncy object.</param>
        /// <param name="e">Arguments passed.</param>
        private static void TargetMemberValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue == null || string.IsNullOrWhiteSpace(e.NewValue.ToString()))
            {
                throw new DataBindingException("TargetMember Dependency property value should not be empty or null");
            }
        }

        /// <summary>
        /// Initialize Autocomplete box type.
        /// </summary>
        static AutoCompleteTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(typeof(AutoCompleteTextBox)));            
        }
        
        /// <summary>
        /// Gets or sets the PopupControl.
        /// </summary>
        public Popup PopupControl
        {
            get
            {
                return this.popup;
            }

            set
            {
                this.popup = value;
            }
        }

        /// <summary>
        /// Gets or sets the List Box control.
        /// </summary>
        public ListBox ListBoxControl
        {
            get
            {
                return this.listBox;
            }

            set
            {
                if (this.listBox != null)
                {
                    this.listBox.KeyDown -= new KeyEventHandler(listBox_KeyDown);
                }

                this.listBox = value;
                if(this.listBox != null)
                {
                    this.listBox.KeyDown += new KeyEventHandler(listBox_KeyDown);
                    ProcessAndSetItemSource(ItemSource);
                }
            }
        }

        /// <summary>
        /// Event handler for the List box Key down.
        /// </summary>
        /// <param name="sender">List Box instance.</param>
        /// <param name="e">Key down arguments.</param>
        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                this.SelectListBoxItem();
        }

        /// <summary>
        /// Delegate to Filter the collection view items.
        /// </summary>
        /// <param name="o">Object Passed.</param>
        /// <returns>Whether it met the criteria.</returns>
        private bool FilterItems(object o)
        {
            var targetProperty = this.TargetMember;
            var task = Task.Factory.StartNew<bool>(() =>
                {
                    bool result = false;
                    var propertyValue = GetPropertyValue(o, targetProperty);
                    if (propertyValue == null || string.IsNullOrWhiteSpace(propertyValue.ToString()))
                    {
                        return result;
                    }

                    if (!string.IsNullOrWhiteSpace(textToFilter) && textToFilter.Length > 2)
                    {
                        result = propertyValue.ToString().StartsWith(textToFilter, StringComparison.InvariantCultureIgnoreCase);
                    }

                    return result;
                });

            return task.Result;
        }

        /// <summary>
        /// Select the List box item.
        /// </summary>
        private void SelectListBoxItem()
        {
            var selectedItem = this.listBox.SelectedItem;
            if(selectedItem != null)
            {
              var propertyValue = GetPropertyValue(selectedItem, this.TargetMember);
              if (propertyValue != null)
              {
                  PopupControl.IsOpen = false;
                  this.raiseTextChanged = false;
                  Text = propertyValue.ToString();
                  SetValue(AutoCompleteTextBox.SelectedItemProperty, selectedItem);
                  SelectAll();
              }
            }            
        }

        /// <summary>
        /// Method to handle the preview key down.
        /// </summary>
        /// <param name="e">KeyEvent arguments.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            var fs = FocusManager.GetFocusScope(this);
            var o = FocusManager.GetFocusedElement(fs);
            if (e.Key == Key.Escape)
            {
                PopupControl.IsOpen =false;
                Focus();
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                if (listBox != null && o == this)
                {
                    raiseTextChanged = true;
                    listBox.Focus();
                    raiseTextChanged = false;
                }
            }
        }

        /// <summary>
        /// Get the value of the property.
        /// </summary>
        /// <param name="selectedItem"></param>
        /// <returns></returns>
        private object GetPropertyValue(object selectedItem, string targetProperty)
        {
            if (!string.IsNullOrWhiteSpace(targetProperty) && selectedItem != null)
            {
                return selectedItem.GetType().GetProperty(targetProperty).GetValue(selectedItem, null);
            }

            return null;
        }

        /// <summary>
        /// Method to handle the text changed event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (this.collectionView == null)
            {
                throw new DataBindingException("ItemSource should not be null or empty.");
            }

            if(this.raiseTextChanged)
            {
                if (!string.IsNullOrWhiteSpace(this.Text) && this.Text.Length >= 3)
                {
                    this.textToFilter = this.Text;
                    this.collectionView.Refresh();
                    if (this.listBox.Items.Count > 0)
                    {
                        PopupControl.IsOpen = true;
                    }
                }
                else
                {
                    PopupControl.IsOpen = false;
                }
            }
            else
            {
                this.raiseTextChanged = true;
            }

        }

        /// <summary>
        ///  Override method of OnApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PopupControl = GetTemplateChild(Popup_Part_Name) as Popup;
            ListBoxControl = GetTemplateChild(ListBox_Part_Name) as ListBox;
            listBox.PreviewMouseDown += listBox_PreviewMouseDown;
        }

        /// <summary>
        /// Method to handle the preview mouse down.
        /// </summary>
        /// <param name="sender">Listbox instance.</param>
        /// <param name="e">MouseButton Arguments</param>
        void listBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.SelectListBoxItem();
        }

        /// <summary>
        /// Method to process and set items source as collection view.
        /// </summary>
        /// <param name="sourceItems">Source list.</param>
        private void ProcessAndSetItemSource(object sourceItems)
        {
            if(sourceItems == null)
            {
                return;
            }

            if (sourceItems is IList || sourceItems is IEnumerable)
            {
                collectionView = (CollectionView)CollectionViewSource.GetDefaultView(sourceItems);                                
            }
            else if (sourceItems is CollectionBase)
            {
                collectionView = (CollectionView)CollectionViewSource.GetDefaultView(sourceItems);
            }
            else if (sourceItems is ICollectionView)
            {
                
                collectionView = (CollectionView)sourceItems;
            }

            listBox.ItemsSource = collectionView;
            collectionView.Filter = FilterItems;            
        }       
    }
}

