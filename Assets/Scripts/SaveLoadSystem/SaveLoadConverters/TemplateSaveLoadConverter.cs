public class TemplateSaveLoadConverter : SaveLoadConverter<TemplateSaveLoadData>
{
    public override TemplateSaveLoadData SetConverterData()
    {
        TemplateSaveLoadData playerSaveData = new TemplateSaveLoadData();

        // ��� �������� ������ �� ������ ������������ ��� ����������

        return playerSaveData;
    }

    public override void SetDefaultData() { } // ��� ������������� ��������� ������ ��� ������ ������������
    protected override void ParseExtractedData() { } // ��� ��������� ������ ��� ��������
}

[System.Serializable]
public class TemplateSaveLoadData { }