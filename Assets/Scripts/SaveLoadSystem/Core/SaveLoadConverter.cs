public abstract class SaveLoadConverter<T> : ISaveLoadConverter
{
    protected T extractedData;

    object ISaveLoadConverter.GetConverterData() => SetConverterData();

    public abstract T SetConverterData();
    public abstract void SetDefaultData();
    public void ExtractGeneralSaveData(GeneralSaveData generalSaveData)
    {
        extractedData = ExtractDataType(generalSaveData);

        if (extractedData != null)
        {
            ParseExtractedData();    
        }
    }

    protected abstract void ParseExtractedData();

    protected T ExtractDataType(GeneralSaveData generalSaveData)
    {
        foreach (object savedObject in generalSaveData.savedObjects)
        {
            if (savedObject.GetType().Equals(typeof(T)))
            {
                return (T)savedObject;
            }
        }

        return default(T);
    }
}