using Application.Models;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace Application.Pages;

public partial class ExhibitPage : ContentPage
{
    public int pressedCount = 0;
    public ObservableCollection<Exhibit> Items { get; private set; }
    public ExhibitPage(Exhibition exhibition)
    {
        InitializeComponent();
        ExhibitName.Text = exhibition.Name;

        Items = new ObservableCollection<Exhibit>
            {
                new Exhibit {Name = "������ ��������", Color = "#C3426B", Source = "start.png"},
                new Exhibit { Name = "������� c �������", Color = "#DC724A", Source = "two.png"},
                new Exhibit {Name = "������� �����������", Color = "#33986A", Source = "three.png"},
                new Exhibit { Name = "����� ��� ������", Color = "#80C442", Source = "four.png"}
            };

        ExhibitsList.ItemsSource = Items;
    }

    private void ShowHideConroller(object sender, EventArgs e)
    {
        if(pressedCount == 0)
        {
            Forward_button.IsVisible = true;
            Left_button.IsVisible = true;
            Right_button.IsVisible = true;
            Back_button.IsVisible = true;
            pressedCount = 1;
        }
        else
        {
            Forward_button.IsVisible = false;
            Left_button.IsVisible = false;
            Right_button.IsVisible = false;
            Back_button.IsVisible = false;
            pressedCount = 0;
        }

    }
}