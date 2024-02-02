namespace Abstraction
{
    public class CreationRoomData
    {
        public string RoomName { get; set; }
        public byte MaxPlayers { get; set; }
        public bool IClosed { get; set; }
        public string[] ReserveSlots { get; set; }
    }
}
