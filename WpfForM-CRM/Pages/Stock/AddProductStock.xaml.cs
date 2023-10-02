﻿using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfForM_CRM.Context;
using System.Data.SqlTypes;

namespace WpfForM_CRM.Pages.Stock;

/// <summary>
/// Interaction logic for AddProductStock.xaml
/// </summary>
public partial class AddProductStock : Window
{

    public AddProductStock(StockPage stockPage)
    {
        this.stockPage = stockPage;
        InitializeComponent();
        ComboBoxCategoryNames();
    }

    StockPage stockPage;
    private Guid categoryId;

    public void ComboBoxCategoryNames()
    {
        var db = new AppDbContext();
        var categories = db.Categories.ToList();
        var comboBoxes = new List<string>();

        foreach (var category in categories)
        {
            comboBoxes.Add(category.Name);
            //categoryId = category.Id;
        }

        categoryNameComboBox.ItemsSource = comboBoxes;
    }

    private void ProductName_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        string pattern = @"^[а-яА-Яa-zA-Z0-9]+$";

        // Create a regular expression object with the pattern
        Regex regex = new Regex(pattern);

        // Check if the entered text matches the pattern
        if (!regex.IsMatch(e.Text))
        {
            e.Handled = true; // Ignore the input
        }
    }

    private void ProductPrice_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {

        string pattern = @"^[0-9]+$";

        // Create a regular expression object with the pattern
        Regex regex = new Regex(pattern);

        // Check if the entered text matches the pattern
        if (!regex.IsMatch(e.Text))
        {
            e.Handled = true; // Ignore the input
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var db = new AppDbContext();

        if (productName.Text.Length == 0 || productOriginalPrice.Text.Length == 0 || productSellingPrice.Text.Length == 0 || productCount.Text.Length == 0 || categoryNameComboBox.SelectedValue == null || childCategoryNameComboBox.SelectedValue == null)
        {
            MessageBox.Show("not variable");
            return;
        }

        var category = db.Categories.First(c => c.Name == categoryNameComboBox.SelectedValue.ToString());

        var childCategory = db.ChildCategories.Where(p => p.CategoryId == category.Id).First(p => p.Name == childCategoryNameComboBox.SelectedValue.ToString());
        var shop = db.Shops.First(p => p.Id == category.ShopId);

        var product = new Entities.Product()
        {
            Name = Helper.Helper.ToUpperNamesOneChar(productName.Text),
            OriginalPrice = long.Parse(productOriginalPrice.Text),
            SellingPrice = long.Parse(productSellingPrice.Text),
            Count = int.Parse(productCount.Text),
            ChildCategoryId = childCategory.Id,
            UserId = stockPage.shopsPage.UserId,
            CategoryId = category.Id,
            ShopId = shop.Id
        };

        db.Products.Add(product);
        product.Barcode = Helper.Helper.GenerateBarcode();
        childCategory.Products!.Add(product);
        db.ChildCategories.Update(childCategory);
        db.SaveChanges();
        stockPage.Load();
        Close();
    }

    private void Category_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var db = new AppDbContext();
        var category = db.Categories.First(p => p.Name == categoryNameComboBox.SelectedValue.ToString());

        var childCategories = db.ChildCategories.Where(ch => ch.CategoryId == category.Id);
        var childCategoryComboBoxes = new List<string>();

        foreach (var childCategory in childCategories)  
        {
            childCategoryComboBoxes.Add(childCategory.Name);
        }

        childCategoryNameComboBox.ItemsSource = childCategoryComboBoxes;
    }

}


