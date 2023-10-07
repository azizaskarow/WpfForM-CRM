﻿using System.Windows;
using System.Windows.Controls;
using WpfForM_CRM.Pages.Shop;

namespace WpfForM_CRM.Pages.CashRegister
{
    /// <summary>
    /// Interaction logic for KassaPage.xaml
    /// </summary>
    public partial class KassaPage : Page
    {
        public KassaPage(MainWindow mainWindow, ShopsPage shopsPage)
        {
            this.mainWindow = mainWindow;
            this.shopsPage = shopsPage;
            InitializeComponent();
        }

        ShopsPage shopsPage;
        MainWindow mainWindow;
        private int a;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            a++;
            MessageBox.Show($"{a}");
        }
    }
}
