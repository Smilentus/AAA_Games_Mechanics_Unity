using System.IO;
using UnityEngine;

/// <summary>
///     ����� ����� ����������� ���� ������ ����������/�������� ������ ��� �������������
///     (���������� � �������� ���������� � ������ �����)
/// </summary>
public static class SaveLoadSystemStarter
{
    private static string SaveFolderName = "Saves";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void SetupSystems()
    {
        SaveLoadSystemController.Instance.SetSaveFolderPath(Path.Combine(Application.persistentDataPath, SaveFolderName));

        SaveLoadSystemController.Instance.AddSaveLoadConverter(new TemplateSaveLoadConverter());
    }
}