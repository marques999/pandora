namespace MemcardRex.Hardware
{
    /// <summary>
    /// </summary>
    internal enum MemCarduinoCommands
    {
        GetIdentifier = 0xA0,
        GetVersion = 0xA1,
        Mcr = 0xA2,
        Mcw = 0xA3
    }
}