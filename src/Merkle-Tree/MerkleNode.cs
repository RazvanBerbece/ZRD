#nullable enable
using System;

namespace MerkleTreeNS.MerkleNodeNS
{
    public class MerkleNode
    {

        public string Value { get; set; }

        public MerkleNode? Left { get; set; }
        public MerkleNode? Right { get; set; }

        public MerkleNode(string value, MerkleNode left, MerkleNode right)
        {
            if (value == "")
            {
                throw new ArgumentException("Value in a MerkleNode cannot be the empty string");
            }

            this.Value = value;
            this.Left = left;
            this.Right = right;
        }
    }
}
#nullable disable
