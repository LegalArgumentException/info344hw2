using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{

    public class TrieNode
    {
        public bool isQuery = false;
        public Dictionary<char, TrieNode> tDict = new Dictionary<char, TrieNode>();
    }
}