namespace MyCustomControls
{
    using System.Linq;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Diagnostics;
    using System;
    using System.Threading;

    public class SearchTextBoldBehavior
    {   
        /// <summary>
        /// Member to hold the textblock instance.
        /// </summary>
        private static TextBlock textBlock;

        /// <summary>
        /// DP for the Boldtext.
        /// </summary>
        public static readonly DependencyProperty BoldTextProperty =
            DependencyProperty.RegisterAttached("BoldText", typeof(string), typeof(SearchTextBoldBehavior), new UIPropertyMetadata(string.Empty, BoldedTextPropertyChanged));

        /// <summary>
        /// Get the bold text.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string GetBoldText(DependencyObject d)
        {
            return (string)d.GetValue(SearchTextBoldBehavior.BoldTextProperty);
        }

        /// <summary>
        /// Sets the bold text.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="o"></param>
        public static void SetBoldText(DependencyObject d, object o)
        {
            d.SetValue(SearchTextBoldBehavior.BoldTextProperty, o);
        }

        /// <summary>
        /// Value change handler for the bold text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void BoldedTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            textBlock = (sender as TextBlock);
            if (textBlock == null)
            {
                return;
            }

            ChangeTextBlockInlines();
        }

        /// <summary>
        /// Change the inlines of the text block.
        /// </summary>
        private static void ChangeTextBlockInlines()
        {
            var originalText = GetExistingContent(textBlock);
            textBlock.Inlines.Clear();
            var textToBold = GetBoldText(textBlock);            
            var indexOfStringToBold = originalText.IndexOf(textToBold, StringComparison.InvariantCultureIgnoreCase);

            if (indexOfStringToBold < 0)
            {
                textBlock.Inlines.Add(originalText);
            }
            else
            {
                textBlock.Inlines.Add(originalText.Substring(0, indexOfStringToBold));
                textBlock.Inlines.Add(new Run()
                {
                    Text = originalText.Substring(indexOfStringToBold, textToBold.Length),
                    FontWeight = FontWeights.Bold
                });

                textBlock.Inlines.Add(originalText.Substring(indexOfStringToBold + textToBold.Length));
            }
        }

        /// <summary>
        /// Get the existing content of the textblock.
        /// </summary>
        /// <param name="textBlock"></param>
        /// <returns></returns>
        private static string GetExistingContent(TextBlock textBlock)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var inline in textBlock.Inlines.OfType<Run>())
            {
                sb.Append(inline.Text);
            }

            return sb.ToString();
        }
    }
}
