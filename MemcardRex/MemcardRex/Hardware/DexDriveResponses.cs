namespace MemcardRex.Hardware
{
    /// <summary>
    /// </summary>
    internal enum DexDriveResponses
    {
        Pout = 0x20,
        Error = 0x21,
        NoCard = 0x22,
        Card = 0x23,
        WriteOk = 0x28,
        WriteSame = 0x29,
        Wait = 0x2A,
        Identifier = 0x40,
        Data = 0x41
    }
}