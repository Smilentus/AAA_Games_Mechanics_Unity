public class TemplateSaveLoadConverter : SaveLoadConverter<TemplateSaveLoadData>
{
    public override TemplateSaveLoadData SetConverterData()
    {
        TemplateSaveLoadData playerSaveData = new TemplateSaveLoadData();

        // Тут собираем данные из других контроллеров для сохранения

        return playerSaveData;
    }

    public override void SetDefaultData() { } // Тут устанавливаем дефолтные данные для других контроллеров
    protected override void ParseExtractedData() { } // Тут разбираем данные для загрузки
}

[System.Serializable]
public class TemplateSaveLoadData { }