namespace DatamaxPlugin
{
    /// <summary>
    /// GoTo: DataMax Mark III: Get Extended Status String
    /// </summary>
    public enum ExtendedStatus
    {
        [Enum("Неопределен")]None = 0,
        [Enum("Занят обработкой")]InterpreterBusy = 1,
        [Enum("Нет бланков")]PaperOutOrFault = 2,
        [Enum("Нет ленты")]RibbonOutOrFaul = 3,
        PrintingBatch = 4,
        BusyPrinting = 5,
        PrinterPaused = 6,
        LabelPresented = 7,
        RewinderOutOrFault = 8,
        DeltaA = 9,
        CutterFaul = 10,
        PaperOut = 11,
        RibbonSaverFaul = 12,
        PrintHeadUp = 13,
        TopOfFormFault = 14,
        RibbonLow = 15,
        Reserved1 = 16,
        Reserved2 = 17,
        DeltaB = 18,
        Ready = 19,
        WaitngForSignal = 20,
        WaitngForData = 21,
        Com1HasDataNotParsed = 22
    }
}