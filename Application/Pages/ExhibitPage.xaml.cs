using Application.Models;
using Application.Patterns.Singleton;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Controls.Xaml;
using System.Windows.Input;

namespace Application.Pages;

public partial class ExhibitPage : ContentPage
{
    public int pressedCount = 0; // ���-�� ������� �� ������ ��������� / ������� ������ ���������� �������
    Exhibition thisExhibition; //������� ���������
    List<string> colors = new List<string> { "#c3426b", "#dc724a", "#33986a", "#80c442" }; // ������������ ����� ����������
    List<string> images = new List<string> { "start", "one", "two", "three", "four", "five", "six", "seven", "eight" };
    int currentColourIndex = 100;
    int currentImageIndex = 1;
    MyTimer timer = new MyTimer();
    ObservableCollection<ExhibitRoute> routes = new ObservableCollection<ExhibitRoute> { };
    public ICommand DeleteItemCommand { get; set; }

    public ObservableCollection<Exhibit> Items { get; private set; }
    public ExhibitPage(Exhibition exhibition) //�����������
    {
        BindingContext = this;
        InitializeComponent();

        thisExhibition = exhibition;
        ExhibitName.Text = exhibition.Name;

        ExhibitsList.ItemsSource = thisExhibition.exhibits;
        currentColourIndex = getRandomColorIndex(currentColourIndex);
        AddButton.Background = Microsoft.Maui.Graphics.Color.FromRgba(colors[currentColourIndex]);

        DeleteItemCommand = new Command<Exhibit>(DeleteItem);
    }

    private int getRandomColorIndex(int previousColour) // ���������� ��������� ������ ����� ���������, � ����������� �� �����������
    {
        while (true)
        {
            Random rand = new Random();
            int currentColour = rand.Next(colors.Count);
            if (previousColour != currentColour)
            {
                return currentColour;
            }
        }
    }

    private void ShowHideConroller(object sender, EventArgs e) //����� � ������� ������ ���������� �������
    {
        if (pressedCount == 0)
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

    async private void AddExhibit(object sender, EventArgs e) //���������� ���������
    {
        double exhibitTime = 0;
        var name = await DisplayPromptAsync("�������� ���������", "������� ��������:", "OK", "������");
        if (name != null && name != "" && routes.Count != 0)
        {
            ObservableCollection<ExhibitRoute> newExhibitRoutes = new ObservableCollection<ExhibitRoute> { };
            for(int i = 0; i < routes.Count; i++)
            {
                newExhibitRoutes.Add(routes[i]);
                exhibitTime += routes[i].elapsedSeconds;
            }
            thisExhibition.exhibits.Add(new Exhibit { Name = name, Color = colors[currentColourIndex], exhibitRoutes = newExhibitRoutes});
            routes.Clear();
            thisExhibition.ExhibitCount++;
            thisExhibition.Time += Math.Round(exhibitTime / 60, 2);
        }
        currentColourIndex = getRandomColorIndex(currentColourIndex);
        AddButton.Background = Microsoft.Maui.Graphics.Color.FromRgba(colors[currentColourIndex]);
    }

    private void Forward_buttonPressed(object sender, EventArgs e)
    {
        timer.Start();
        routes.Add(new ExhibitRoute { Route = '1', source = "up", color = colors[currentColourIndex]});
    }
    private void Left_buttonPressed(object sender, EventArgs e)
    {
        timer.Start();
        routes.Add(new ExhibitRoute { Route = '3', source = "left", color = colors[currentColourIndex] });
    }
    private void Right_buttonPressed(object sender, EventArgs e)
    {
        timer.Start();
        routes.Add(new ExhibitRoute { Route = '4', source = "right", color = colors[currentColourIndex] });
    }
    private void Back_buttonPressed(object sender, EventArgs e)
    {
        timer.Start();
        routes.Add(new ExhibitRoute { Route = '2', source = "down", color = colors[currentColourIndex] });
    }

    private void ButtonsReleased(object sender, EventArgs e)
    {
        timer.Stop();
        routes[routes.Count - 1].elapsedSeconds = Math.Round(timer.GetElapsedSeconds(), 3);
    }

    private void DeleteItem(Exhibit exhibit)
    {
        thisExhibition.exhibits.Remove(exhibit);
        thisExhibition.ExhibitCount--;
    }

    public async void ExhibitList_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Exhibit tappedExhibit)
        {
            await Navigation.PushModalAsync(new ExhibitRoutePage(tappedExhibit));
        }
    }
}