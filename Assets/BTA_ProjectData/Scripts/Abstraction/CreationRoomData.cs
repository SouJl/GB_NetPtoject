namespace Abstraction
{
    public class CreationRoomData
    {
        public string RoomName { get; set; }
        public byte MaxPlayers { get; set; }
        public bool IsPublic { get; set; }
        public bool PublishUserId { get; set; }
        public string[] Whitelist { get; set; }
    }
}
