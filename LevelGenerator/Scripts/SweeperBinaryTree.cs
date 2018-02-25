using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vegricht.LevelGenerator
{
    class SweeperBinaryTree<T> : Tree<T> where T : IComparable<T>
    {
        public T Successor(T item)
        {
            TreeNode node = RootNode;

            while (true)
            {
                if (node.Value.CompareTo(item) == 1)
                    return node.Value;

                node = node.RightHand;
            }
        }

        public T Predecessor(T item)
        {
            TreeNode node = RootNode;

            while (true)
            {
                if (node.Value.CompareTo(item) == -1)
                    return node.Value;

                node = node.LeftHand;
            }
        }
    }
}