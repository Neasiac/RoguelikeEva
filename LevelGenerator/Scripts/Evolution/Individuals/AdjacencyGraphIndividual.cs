using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution.Individuals
{
    [Serializable]
    class AdjacencyGraphIndividual : Individual
    {
        public Dictionary<byte, GraphNode> Gene { get; set; }
        public GraphNode StartingRoom { get; set; }
        public Dictionary<byte, GraphNode> BranchStarters { get; private set; } = new Dictionary<byte, GraphNode>();

        IList<byte> RoomIds;
        byte BranchCount;
        int BranchMaxLength;
        Random Rnd;
        
        public AdjacencyGraphIndividual(IList<byte> roomIds, byte branchCount, int branchMaxLength)
        {
            RoomIds = roomIds;
            BranchCount = branchCount;
            BranchMaxLength = branchMaxLength;
            Rnd = new Random();
        }

        public override void RandomInitialization()
        {
            StartingRoom = RndRoomNode();
            StartingRoom.IsStartingRoom = true;

            for (byte id = 0; id < BranchCount; id++)
                PlaceBranch(id);
        }

        public void PlaceBranch(byte id)
        {
            // FIXME: co kdyz tu neni dost nodes?
            GraphNode n;

            do n = RndRoomNode();
            while (!n.IsFree);

            int it = 1;
            int length = Rnd.Next(2, BranchMaxLength + 1);

            // if branch with this id already exists, assume it was already replaced and just update dictionary with new information
            if (BranchStarters.ContainsKey(id))
                BranchStarters[id] = n;
            else
                BranchStarters.Add(id, n);

            do
            {
                n.BranchId = id;
                int successorCandidateId = Rnd.Next(n.Neighbors.Count);

                if (it == 0)
                    n.Status = GraphNode.BranchRoomStatus.Starting;
                else if (it == length)
                    n.Status = GraphNode.BranchRoomStatus.Ending;

                while (!n.Neighbors[successorCandidateId].IsFree)
                {
                    successorCandidateId++;

                    if (successorCandidateId == n.Neighbors.Count)
                        successorCandidateId = 0;

                    // FIXME: co kdyz neni zadny naslednik volny??
                }

                n = n.Neighbors[successorCandidateId];
            }
            while (it++ < length);
        }

        GraphNode RndRoomNode()
        {
            return Gene[RoomIds[Rnd.Next(RoomIds.Count)]];
        }
    }
}