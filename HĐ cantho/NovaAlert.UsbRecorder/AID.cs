using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NovaAlert.UsbRecorder
{
    public static class AID
    {
        [DllImport("AID.dll")]
        public static extern uint FT_ListDevices();
        [DllImport("AID.dll")]
        public static extern uint FT_Open();
        [DllImport("AID.dll")]
        public static extern uint FT_Close();
        [DllImport("AID.dll")]
        public static extern uint FT_Write([MarshalAs(UnmanagedType.LPArray)] byte[] p_data, ulong size);
        [DllImport("AID.dll")]
        public static extern uint FT_GetStatus(ref ulong rxsize, ref ulong txsize);
        [DllImport("AID.dll")]
        public static extern uint FT_SetBitMode(byte mask, byte enable);
        [DllImport("AID.dll")]
        public static extern uint FT_Read([MarshalAs(UnmanagedType.LPArray)] byte[] p_data, ulong size);
        [DllImport("AID.dll")]
        public static extern uint FT_EE_Read(ref ushort vid, ref ushort pid, ref ushort power);
        [DllImport("AID.dll")]
        public static extern uint FT_EE_Program(ushort power);
        [DllImport("AID.dll")]
        public static extern uint FT_EE_ProgramToDefault();
        [DllImport("AID.dll")]
        public static extern uint KCAN_Send(uint channel, uint id, uint dlc, [MarshalAs(UnmanagedType.LPArray)] byte[] p_data);
        [DllImport("AID.dll")]
        public static extern uint KCAN_Receive(ref uint channel, ref uint id, ref uint dlc, [MarshalAs(UnmanagedType.LPArray)] byte[] p_data);
        [DllImport("AID.dll")]
        public static extern uint KCAN_Init(uint baudraute);
    }
}
