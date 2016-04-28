using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// This class is a structure that allows for information on the creation of a trie to be 
/// serialized and returned as JSON later
/// </summary>
namespace WebRole1
{
    public class jsonTest
    {
        public float counter { get; set; }
        public float startMemory { get; set; }
        public float memory { get; set; }
        public string lastLine { get; set; }
    }
}