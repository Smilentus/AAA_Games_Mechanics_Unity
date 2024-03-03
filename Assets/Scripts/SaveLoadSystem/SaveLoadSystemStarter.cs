using System.IO;
using UnityEngine;

/// <summary>
///     Общая точка добавляения всех систем сохранения/загрузки ТОЛЬКО ДЛЯ ИНИЦИАЛИЗАЦИИ
///     (сохранение и загрузка желательно в другом месте)
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