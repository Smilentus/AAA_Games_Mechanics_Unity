
public interface ISaveLoadConverter
{    /// <summary>
     ///     ������������ ��� ���������� ��������� �������� � ����������
     /// </summary>
     /// <returns>
     ///     ���������� ����� ��� ������ ������ ��� ���������� � ����� �����
     /// </returns>
    public object GetConverterData();

    /// <summary>
    ///     ������������ ��� �������� ������� ���������� ��� ����, ����� ������������ ��� ������� ���������
    /// </summary>
    /// <param name="generalSaveData">
    ///     ��� ���������� � ����������
    /// </param>
    public void ExtractGeneralSaveData(GeneralSaveData generalSaveData);

    /// <summary>
    ///     ������������� ��������� ��������� ��������� ��������
    /// </summary>
    public void SetDefaultData();
}