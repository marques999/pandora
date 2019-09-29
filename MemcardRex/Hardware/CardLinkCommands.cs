namespace MemcardRex.Hardware
{
    /// <summary>
    /// </summary>
    internal enum CardLinkCommands
    {
        Mcr = 0xA2,
        Mcw = 0xA3,
        GetVersion = 0xA1,
        GetIdentifier = 0xA0
    }
}