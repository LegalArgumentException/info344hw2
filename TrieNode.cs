using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// This class is the single node of a larger Trie structure
/// </summary>
namespace WebRole1
{

    public class TrieNode
    {
        public bool isQuery = false;
        public Dictionary<char, TrieNode> tDict = new Dictionary<char, TrieNode>();
    }
}