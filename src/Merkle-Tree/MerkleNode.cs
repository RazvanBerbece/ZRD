#nullable enable
using System;

namespace MerkleTreeNS.MerkleNodeNS
{
    public class MerkleNode
    {

        public string Value;

        public MerkleNode? Left;
        public MerkleNode? Right;

        public MerkleNode(string hashValue, MerkleNode left, MerkleNode right)
        {
            if (hashValue == "")
            {
                throw new ArgumentException("Value in a MerkleNode cannot be the empty string");
            }

            this.Value = hashValue;
            this.Left = left;
            this.Right = right;
        }
    }
}
#nullable disable
