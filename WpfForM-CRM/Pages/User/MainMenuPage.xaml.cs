﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfForM_CRM.Context;
using WpfForM_CRM.Pages.Shop;

namespace WpfForM_CRM.Pages.User;

/// <summary>
/// Interaction logic for MainMenuPage.xaml
/// </summary>
public partial class MainMenuPage : Page
{
    private int eye_counter = 0;
    private string password_change = string.Empty;
    private MainWindow mainWindow;
    public MainMenuPage(MainWindow mainWindow, bool isRegistered = false, string currentRegisterUser = "" , string currentPasswordUser = "")
    {
        this.mainWindow = mainWindow;
        InitializeComponent();
        EnterTextBox(isRegistered, currentRegisterUser, currentPasswordUser);
       
    }

    //public void AddCategory()
    //{
    //    var db = new AppDbContext();

    //    var password = "2003azizC";
    //    var hasher = new SHA256Managed();
    //    var unhashed = System.Text.Encoding.Unicode.GetBytes(password);
    //    var hashed = hasher.ComputeHash(unhashed);
    //    var hashedPassword = Convert.ToBase64String(hashed);

    //    var user = new User()
    //    {
    //        UserName = "azizaskarow",
    //        RememberMe = false,
    //        Password = hashedPassword
    //    };

    //    db.Users.Add(user);

    //    var shop = new Shop()
    //    {
    //        Name = "MyShop",
    //        UserId = user.Id,
    //    };
    //    db.Shops.Add(shop);

    //    user.Shops.Add(shop);
    //    db.Users.Update(user);

    //    var category = new Category()
    //    {
    //        Name = "Kiyim",
    //        ShopId = shop.Id,
    //    };

    //    db.Categories.Add(category);
    //    shop.Categories.Add(category);
    //    db.Shops.Update(shop);

    //    db.SaveChanges();
    //}

    private void EnterTextBox(bool isRegistered = false, string currentRegisterUser = "", string currentPasswordUser = "")
    {
        if (isRegistered)
        {
            parol_txt.Password = currentPasswordUser;
            register_txt.Text = currentRegisterUser;
        }
        else
        {
            if (Properties.Settings.Default.RememberMe)
            {
                register_txt.Text = Properties.Settings.Default.Name;
                parol_txt.Password = Properties.Settings.Default.Password;
            }
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        MainPage.NavigationService.Navigate(new RegistrationPage(mainWindow));
    }

    private void enter_btn_Click(object sender, RoutedEventArgs e)
    {
        var username = register_txt.Text;
        var password = parol_txt.Password;
        var invisiblePassword = parol_tx.Text;

        if (username.Length == 0 || password.Length == 0)
        {
            MessageBox.Show("Введите логин и пароль");
            return;
        }

        var dbContext = new AppDbContext();

        if (dbContext.Users.Any(u => u.UserName == username))
        {
            try
            {
                var user = dbContext.Users.FirstOrDefault(u => u.UserName == username);
                var page = new RegistrationPage(mainWindow);
                var hash = page.GenerateHash(password);
                var invisibleHash = page.GenerateHash(invisiblePassword);

                if (hash == user.Password || invisibleHash == user.Password)
                {
                    if ((bool)checkMe.IsChecked!)
                    {
                        Properties.Settings.Default.Password = password;
                        Properties.Settings.Default.RememberMe = true;
                        Properties.Settings.Default.Name = username;
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        Properties.Settings.Default.Password = "";
                        Properties.Settings.Default.RememberMe = false;
                        Properties.Settings.Default.Name = "";
                        Properties.Settings.Default.Save();
                    }

                    MainPage.NavigationService.Navigate(new ShopsPage(mainWindow, user.Id));
                }
                else
                {
                    MessageBox.Show("Неправильный пароль", "Ошибка" , MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        else
        {
            MessageBox.Show("Пользователь не найден","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (eye_counter == 2)
        {
            eye_counter = 0;
        }
        if (eye_counter == 0)
        {
            parol_tx.Visibility = Visibility.Visible;
            parol_txt.Visibility = Visibility.Hidden;
            eye.Source =
                new BitmapImage(new Uri("/Pages/eye close.png", UriKind.Relative));
            eye_counter = 1;
        }
        else if (eye_counter == 1)
        {
            parol_tx.Visibility = Visibility.Hidden;
            parol_txt.Visibility = Visibility.Visible;
            eye.Source =
                new BitmapImage(new Uri("/Pages/eye open.png", UriKind.Relative));
            eye_counter = 2;
        }
    }

    private void parol_tx_TextChanged(object sender, TextChangedEventArgs e)
    {
        parol_txt.Password = parol_tx.Text;
    }

    private void parol_txt_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!parol_txt.IsVisible)
        {
            parol_tx.Text = parol_txt.Password;
        }
    }

    private void MainMenuPage_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            enter_btn_Click(sender,e);
        }
        else if (e.Key == Key.Escape)
        {
            Application.Current.Shutdown();
        }
    }

    
}