namespace Application.Pages;

using Application.Models;
using Application.Patterns.Singleton;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Text.Json;

public partial class Settings : ContentPage
{
    private string exhibitionsFilePath;
    private string currentExhibitionFilePath;

    public Settings()
    {
        InitializeComponent();
        exhibitionsFilePath = Path.Combine(FileSystem.AppDataDirectory, "exhibition.json");
        currentExhibitionFilePath = Path.Combine(FileSystem.AppDataDirectory, "currentExhibition.json");
    }

    private void SaveData(object sender, EventArgs e)
    {
        var jsonexhibitionsString = JsonSerializer.Serialize(ExhibitionManager.Instance.Items);
        File.WriteAllText(exhibitionsFilePath, jsonexhibitionsString);
        var jsonCurrentExhibitionString = JsonSerializer.Serialize(ExhibitionManager.Instance.CurrentItem);
        File.WriteAllText(currentExhibitionFilePath, jsonCurrentExhibitionString);
        DisplayAlert("���������", "������ ���������", "OK");
    }

    private void LoadData(object sender, EventArgs e) 
    {
        if (File.Exists(exhibitionsFilePath)) //�������� ���������� ���������
        {
            ExhibitionManager.Instance.Items.Clear();
            var jsonString = File.ReadAllText(exhibitionsFilePath);
            var exhibitions = JsonSerializer.Deserialize<ObservableCollection<Exhibition>>(jsonString);

            string message = "";
            
            foreach (var exhibition in exhibitions)
            {
                ExhibitionManager.Instance.Items.Add(exhibition);
                DisplayAlert("���������", $"�������� ����������� ���������: {exhibition.Name}", "OK");
            }
        }
        else
        {
            DisplayAlert("������", "���� � ������� ��������� �� ������", "OK");
        }

        if (File.Exists(currentExhibitionFilePath)) //�������� ������� ���������
        {
            ExhibitionManager.Instance.CurrentItem.Clear();
            var jsonString = File.ReadAllText(currentExhibitionFilePath);
            var exhibitions = JsonSerializer.Deserialize<ObservableCollection<Exhibition>>(jsonString);

            foreach (var exhibition in exhibitions)
            {
                ExhibitionManager.Instance.Items.Add(exhibition);
                DisplayAlert("���������", $"�������� ����������� ���������: {exhibition.Name}", "OK");
            }
        }
        else
        {
            DisplayAlert("������", "���� � ������� �������� �� ������", "OK");
        }
    }
}
