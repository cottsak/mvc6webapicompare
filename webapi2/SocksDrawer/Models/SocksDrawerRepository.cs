using System;

namespace SocksDrawer.Models
{
    public interface ISocksDrawerRepository
    {
        SocksPair AddPair(SocksPair pair);
    }

    class SocksDrawerRepository : ISocksDrawerRepository
    {
        public SocksPair AddPair(SocksPair pair)
        {
            return pair;
        }
    }
}