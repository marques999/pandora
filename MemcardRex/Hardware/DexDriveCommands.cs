namespace MemcardRex.Hardware
{
    /// <summary>
    /// </summary>
    internal enum DexDriveCommands
    {
        Init = 0x00,
        Read = 0x02,
        Write = 0x04,
        Light = 0x07,
        MagicHandshake = 0x27
    }
}