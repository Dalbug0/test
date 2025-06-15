namespace Application.Pages;

using Application.Models;
using Application.Patterns.Singleton;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

public partial class Settings : ContentPage
{
    private const string PlaceholderText = "������� �������� ��������";

    public Settings()
    {
        InitializeComponent();
    }

    private async void EditSpeed_Clicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("�������� ������", "������� ��������:", "OK", "������");
        if (name != "" && name != null)
        {
            if(Convert.ToInt32(name) > 255) 
            {
                name = "255";
            }
            if (Convert.ToInt32(name) < 70)
            {
                name = "70";
            }
            SendData(name);
            SpeedLabel.Text = "�������� ������: " + name;
        }
    }

    private async void SendData(string message)
    {
        try
        {
            //message += "\n";
            //byte[] buffer = Encoding.UTF8.GetBytes(message);
            await ExhibitionManager.Instance.SendDataAsync(message);

            //await ExhibitionManager.Instance.socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            await DisplayAlert("������", $"������ �������� ������: {ex.Message}", "��");

        }
    }
}
