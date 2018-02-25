using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vegricht.LevelGenerator
{
    class GraphNode
    {
        public enum BranchRoomStatus
        {
            None,
            Starting,
            Ending
        }

        public List<GraphNode> Neighbors { get; private set; } = new List<GraphNode>();
        public byte RoomId { get; set; }
        public byte BranchId { get; set; }
        public bool IsStartingRoom { get; set; }
        public BranchRoomStatus Status { get; set; }

        public bool IsFree
        {
            get
            {
                return BranchId == 0 && !IsStartingRoom;
            }
        }

        public GraphNode(byte roomId)
        {
            RoomId = roomId;
        }
    }
}