﻿#nullable enable
using System;

namespace MerkleTreeNS.MerkleNodeNS
{
    public class MerkleNode
    {

        public string value { get; set; }

        public MerkleNode? left { get; set; }
        public MerkleNode? right { get; set; }

        public MerkleNode(string hashValue, MerkleNode left, MerkleNode right)
        {
            if (hashValue == "")
            {
                throw new ArgumentException("Value in a MerkleNode cannot be the empty string");
            }

            this.value = hashValue;
            this.left = left;
            this.right = right;
        }
    }
}
#nullable disable
