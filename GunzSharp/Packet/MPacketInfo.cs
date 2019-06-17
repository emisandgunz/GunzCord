namespace GunzSharp.Packet
{
	public class MPacketInfo
	{
		public MCommObject CommObj { get; set; }

		public byte[] Packet { get; set; }

		public MPacketInfo(MCommObject commObj, byte[] packet)
		{
			CommObj = commObj;
			Packet = packet;
		}
	}
}
