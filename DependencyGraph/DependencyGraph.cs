// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        //private fields of DependencyGraph
        private Dictionary<string, LinkedList<string>> indegreeMap;
        private Dictionary<string, LinkedList<string>> outdegreeMap;
        private int amountOfPairs;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            //assign values to private variables
            indegreeMap = new Dictionary<string, LinkedList<string>>();
            outdegreeMap= new Dictionary<string, LinkedList<string>>();
            amountOfPairs = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            //uses private var 'amountOfPairs' to internally count and increment the amount of pairs in DependencyGraph
            get { return amountOfPairs; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                //the outdegree map tracks variables dependees, find the list of dependees and return the length
                if (outdegreeMap.ContainsKey(s))
                {
                    return outdegreeMap[s].Count;
                }
                else
                    return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            //method checks if the given map has a list with length greater than zero for string s
            return HasDependentsBothWays(s, indegreeMap);
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            //method checks if the given map has a list with length greater than zero for string s
            return HasDependentsBothWays(s, outdegreeMap);
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            //enumrerates the given map with key 's'
            return getDependentsBothWays(s, indegreeMap);
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            //enumerates the given map with key 's'
            return getDependentsBothWays(s, outdegreeMap);
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            //add (s, t) to indegree map, then add (t, s) to outdegree map, increments size
            addDependencyBothWays(s, t, indegreeMap, true);
            addDependencyBothWays(t, s, outdegreeMap, false);
            
            
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            //remove dependency (s, t) to indegree map, then remove (t, s) from outdegree map, incremement size
            removeDependencyBothWays(s, t, indegreeMap, true);
            removeDependencyBothWays(t, s, outdegreeMap, false);
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            //does method call for given inputs
            replaceDependencyBothWays(s, indegreeMap, outdegreeMap, newDependents);
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            //does method call for given inputs
            replaceDependencyBothWays(s, outdegreeMap, indegreeMap, newDependees);
        }


        /// <summary>
        /// adds a dependency with the given input pair onto the specified map, choice to increment the size of the map if user wishes
        /// </summary>
        /// <param name="indegree"></param>
        /// <param name="outdegree"></param>
        /// <param name="inputMap"></param>
        /// <param name="wishToIncrement"></param>
        private void addDependencyBothWays(string indegree, string outdegree, Dictionary<string, LinkedList<string>> inputMap, bool wishToIncrement)
        {
            //check if indegreeMap has element 'indegree' yet
            if (inputMap.ContainsKey(indegree))
            {
                //get the linkedList of the key value
                LinkedList<string> outdegreeSubList = inputMap[indegree];

                //check if outdegreeSubList has 'outdegree', if doesn't add, else do nothing
                if (!outdegreeSubList.Contains(outdegree))
                {
                    outdegreeSubList.AddFirst(outdegree);
                    if (wishToIncrement)
                        amountOfPairs++;
                }
                else
                    return;
            }
            else
            {
                //indegreeMap doesn't have indegree value yet, create linked list
                LinkedList<string> newOutDegreeSubList = new LinkedList<string>();

                //add 't' to the indegreeSubList
                newOutDegreeSubList.AddFirst(outdegree);

                //map 's' to the indegreeSubList
                inputMap.Add(indegree, newOutDegreeSubList);

                //increment if 'wishToIncrement' is true
                if (wishToIncrement)
                    amountOfPairs++;
            }
        }

        /// <summary>
        /// removes dependency using the given indegree and outdegrees, to the given map, with the choice to increment
        /// </summary>
        /// <param name="indegree"></param>
        /// <param name="outdegree"></param>
        /// <param name="inputMap"></param>
        /// <param name="wishToIncrement"></param>
        private void removeDependencyBothWays(string indegree, string outdegree, Dictionary<string, LinkedList<String>> inputMap, bool wishToIncrement)
        {
            //check if indegree exists, else return
            if (inputMap.ContainsKey(indegree))
            {
                //get LinkedList from indegree
                LinkedList<string> outdegreeSubList = inputMap[indegree];

                //check if outdegree is linked to indegree
                if (outdegreeSubList.Contains(outdegree))
                {
                    //if outdegreeList contains outdegree, remove it then incrememnt if wished
                    outdegreeSubList.Remove(outdegree);
                    if (wishToIncrement)
                        amountOfPairs--;
                    //if outdegreeSublist is down to zero, remove the key from inputMap
                    if (outdegreeSubList.Count == 0)
                        inputMap.Remove(indegree);
                }
                else
                    return;
            }
            else
                return;
        }

        /// <summary>
        /// with the given inputs, method replaces all dependency's from a varaible with the given list
        /// </summary>
        /// <param name="indegree"></param>
        /// <param name="inputMap"></param>
        /// <param name="outputMap"></param>
        /// <param name="newDependents"></param>
        private void replaceDependencyBothWays(string indegree, Dictionary<string, LinkedList<String>> inputMap, Dictionary<string, LinkedList<String>> outputMap, IEnumerable<string> newDependents)
        {
            //make sure s is in the map
            if (inputMap.ContainsKey(indegree))
            {
                //find the linkedList associated with 's'
                LinkedList<string> outdegreeSubList = inputMap[indegree];

                //change the outdegree list to match new indegree list
                foreach (string outdegreee in outdegreeSubList)
                {
                    //removes outdegree association and increments the count
                    removeDependencyBothWays(outdegreee, indegree, outputMap, true);
                }
                //clear outdegreeSubList
                outdegreeSubList.Clear();

                //sets outdegreeSubList to desired outputs, and set outdegreeMap
                foreach (string newOutdegree in newDependents)
                {
                    outdegreeSubList.AddFirst(newOutdegree);
                    addDependencyBothWays(newOutdegree, indegree, outputMap, false);
                }
                //increment size
                amountOfPairs += newDependents.Count();

            }
            else
            {
               foreach(string s in newDependents)
                {
                    addDependencyBothWays(indegree, s, inputMap, true);
                    addDependencyBothWays(s, indegree, outputMap, false);
                }
            }
        }

        /// <summary>
        /// Takes the a variable as the key, and checks the given map and returns the list associeated with the key given
        /// </summary>
        /// <param name="s"></param>
        /// <param name="inputMap"></param>
        /// <returns>The list of the key from the map, or nothing if no relation is found</returns>
        private IEnumerable<string> getDependentsBothWays(string s,  Dictionary<string, LinkedList<string>> inputMap)
        {
            //create list to return in case no return from inputMap
            List<string> dependents = new List<string>();

            if (inputMap.ContainsKey(s))
            {
                return inputMap[s];
            }
            else
                return dependents;
        }

        /// <summary>
        /// Takes the variable as a key, and checks to see if it has values associated with the key form the given map
        /// </summary>
        /// <param name="s"></param>
        /// <param name="inputMap"></param>
        /// <returns>True if relation found between map and value, false otherwise</returns>
        private bool HasDependentsBothWays(string s, Dictionary<string, LinkedList<string>> inputMap)
        {
            if (inputMap.ContainsKey(s))
            {
                return true;
            }
            else
                return false;
        }
    }

}