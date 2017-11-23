using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components;
using Vegricht.RoguelikeEva.Scenes.Core;
using System.Collections;

namespace Vegricht.RoguelikeEva.Pathfinding
{
    class Path : IEnumerable<MapNode>
    {
        public enum PathAction
        {
            Move,
            Attack,
            Use,
            Retreat
        }

        public PathAction Action { get; private set; }
        MapNode[] _path;

        public Path(IList<MapNode> path, PathAction action)
        {
            _path = path.ToArray();
            Action = action;
        }

        public int Length
        {
            get
            {
                return _path.Length;
            }
        }

        public MapNode this[int index]
        {
            get
            {
                return _path[index];
            }
        }

        public void Retreat()
        {
            Array.Reverse(_path);
            Action = PathAction.Retreat;
        }

        public IEnumerator<MapNode> GetEnumerator()
        {
            for (int i = 0; i < _path.Length; i++)
                yield return _path[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}