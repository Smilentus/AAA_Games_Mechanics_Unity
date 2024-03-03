
public interface ISaveLoadConverter
{    /// <summary>
     ///     Используется для добавления кастомных объектов к сохранению
     /// </summary>
     /// <returns>
     ///     Возвращает любой тип данных класса для сохранения в общее место
     /// </returns>
    public object GetConverterData();

    /// <summary>
    ///     Используется для парсинга объекта сохранения для того, чтобы обрабатывать его разными способами
    /// </summary>
    /// <param name="generalSaveData">
    ///     Вся информация о сохранении
    /// </param>
    public void ExtractGeneralSaveData(GeneralSaveData generalSaveData);

    /// <summary>
    ///     Устанавливает дефолтное состояние зависимых объектов
    /// </summary>
    public void SetDefaultData();
}