using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using static BlockChainAssignment.BlockList;

namespace BlockChainAssignment
{
    public partial class MainWindow : Window
    {
        public int CurrentTab;

        BlockList theBlockList = GetInitializedBlockChain();

        public MainWindow()
        {
            InitializeComponent();
            
            this.DataContext = theBlockList;
            BlockTabs.SelectedIndex = 0; // Sets the current selected block to tab 0, upon initialization.
        }

        #region Buttons
        private void MineTabBTN(object sender, RoutedEventArgs e)
        {
            if (BlockTabs.SelectedContent is Block B)
            {
                Task.Factory.StartNew(() => B.Mine());
            }
        }
        private void MineAllBTN(object sender, RoutedEventArgs e)
        {
            CurrentTab = BlockTabs.SelectedIndex;
            foreach (int i in Enumerable.Range(0,5))
            {
                BlockTabs.SelectedIndex = i;
                if (BlockTabs.SelectedContent is Block B)
                {
                    if (!B.IsSigned)
                    {
                        Task.Factory.StartNew(() => B.Mine()).Wait();
                    }
                }
            }
            BlockTabs.SelectedIndex = CurrentTab;
        }
        private void HelpBTN(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://anders.com/blockchain/");
        }
        #endregion
    }

    /// <summary>
    /// This code sets the IsSigned booleon value to a color that represents whether a block is signed or not.
    /// </summary>
    /// <remarks> Green is signed, red is unsigned. </remarks>
    /// <see cref="IsSigned"/>
    public class IsSignedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush)) throw new ArgumentException();

            bool IsSigned = (bool)value;

            return IsSigned == true ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
