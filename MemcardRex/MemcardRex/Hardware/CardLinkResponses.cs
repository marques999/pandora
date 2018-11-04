namespace MemcardRex.Hardware
{
    /// <summary>
    /// </summary>
    internal enum CardLinkResponses
    {
        Good = 0x47,
        Error = 0xE0,
        BadSector = 0xFF,
        BadChecksum = 0x4E
    }
}