namespace SocksDrawer.Models
{
    public class SocksPair
    {
        public SocksPair(SocksColour colour)
        {
            Colour = colour;
        }

        public int Id { get; private set; }
        public SocksColour Colour { get; private set; }
    }

    public enum SocksColour
    {
        Black,
        White,
    }
}