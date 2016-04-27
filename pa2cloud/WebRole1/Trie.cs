using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public static class Trie
    {
        static public TrieNode root = new TrieNode();

        public static void addTitle(string title)
        {
            var charsArray = title.ToCharArray();
            TrieNode currentNode = root;
            for (int i = 0; i < charsArray.Length; i++)
            {
                if (!currentNode.tDict.ContainsKey(charsArray[i]))
                {
                    currentNode.tDict.Add(charsArray[i], new TrieNode());
                }
                currentNode = currentNode.tDict[charsArray[i]];
                if (i == charsArray.Length - 1)
                {
                    currentNode.isQuery = true;
                }
            }
        }

        public static List<string> getPrefix(string prefix)
        {
            List<string> queries = new List<string>();
            var charsArray = prefix.ToCharArray();
            TrieNode currentNode = root;
            /// Loop through trie using the given prefix to find spot in trie where the search for
            /// possible query autocompletions should start, returns an empty List if the given
            /// prefix cannot be reached in the string
            foreach (char currentChar in charsArray)
            {
                if (currentNode.tDict.ContainsKey(currentChar))
                {
                    currentNode = currentNode.tDict[currentChar];
                }
                else
                {
                    return queries;
                }
            }

            return prefixHelper(queries, currentNode, prefix, prefix);
        }

        public static List<string> prefixHelper(List<string> queries, TrieNode currentNode,
                string currentString, string originalPrefix)
        {
            /// Tests to see if the current node is marked as a full query and not the original prefix
            if (currentNode.isQuery && currentString != originalPrefix)
            {
                queries.Add(currentString);
                // Returns the queries if the target amount of queries has been found
                if (queries.Count == 10)
                {
                    return queries;
                }
            }
            /// Loops through each node branching off the current node to find more queries
            foreach (char charKey in currentNode.tDict.Keys)
            {
                if (queries.Count < 10)
                {
                    queries = prefixHelper(queries, currentNode.tDict[charKey],
                            currentString + charKey, originalPrefix);
                }
            }
            return queries;
        }
    }
}