using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// This Trie class 
/// </summary>
namespace WebRole1
{
    public static class Trie
    {
        // The root node of the Trie
        static public TrieNode root = new TrieNode();

        /// <summary>
        /// This class allows for a string passed in to be input into the Trie structure.
        /// </summary>
        /// <param name="title">Name of the title that needs to be input in the Trie</param>
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

        /// <summary>
        /// This method, when passed in a string, traverses the Trie structure and returns possible
        /// autocomplete query results. If passed in an empty string, it returns an empty List of 
        /// strings.
        /// </summary>
        /// <param name="prefix">The query used to traverse the trie</param>
        /// <returns>Returns a List full of strings of possible autocomplete query results</returns>
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

        /// <summary>
        /// A helper method that recursively traverses the Trie until it finds a full query. It
        /// will continue doing so until it finds 10 queries.
        /// </summary>
        /// <param name="queries">
        /// List full of current queries found - starts as an empty List
        /// </param>
        /// <param name="currentNode">Current node in the Trie</param>
        /// <param name="currentString">Currently completed string</param>
        /// <param name="originalPrefix">The original prefix passed in by the user</param>
        /// <returns></returns>
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