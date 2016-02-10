using FluentNHibernate.Mapping;

namespace SocksDrawer.Mvc6.Models
{
    public class SocksPair
    {
        public SocksPair(SocksColour colour)
        {
            Colour = colour;
        }
        protected SocksPair() { }

        public virtual int Id { get; protected set; }
        public virtual SocksColour Colour { get; protected set; }
    }

    public enum SocksColour
    {
        Black,
        White,
    }

    public class SocksPairMap : ClassMap<SocksPair>
    {
        public SocksPairMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Colour);
        }
    }
}
