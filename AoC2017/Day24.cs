using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AoC2017
{
    class Day24
    {
        public static void Part1()
        {
            //The CPU itself is a large, black building surrounded by a bottomless pit. Enormous metal tubes extend 
            //outward from the side of the building at regular intervals and descend down into the void. There's no 
            //way to cross, but you need to get inside.
            //No way, of course, other than building a bridge out of the magnetic components strewn about nearby.
            //Each component has two ports, one on each end. The ports come in all different types, and only matching 
            //types can be connected.You take an inventory of the components by their port types(your puzzle input).
            //Each port is identified by the number of pins it uses; more pins mean a stronger connection for your 
            //bridge. A 3/7 component, for example, has a type-3 port on one side, and a type-7 port on the other.
            //Your side of the pit is metallic; a perfect surface to connect a magnetic, zero-pin port. Because of this, 
            //the first port you use must be of type 0. It doesn't matter what type of port you end with; your goal is 
            //just to make the bridge as strong as possible.
            //The strength of a bridge is the sum of the port types in each component. For example, if your bridge is 
            //made of components 0/3, 3/7, and 7/4, your bridge has a strength of 0 + 3 + 3 + 7 + 7 + 4 = 24.
            //For example, suppose you had the following components:
            // 0 / 2
            // 2 / 2
            // 2 / 3
            // 3 / 4
            // 3 / 5
            // 0 / 1
            // 10 / 1
            // 9 / 10
            //With them, you could make the following valid bridges:
            // 0 / 1
            // 0 / 1--10 / 1
            // 0 / 1--10 / 1--9 / 10
            // 0 / 2
            // 0 / 2--2 / 3
            // 0 / 2--2 / 3--3 / 4
            // 0 / 2--2 / 3--3 / 5
            // 0 / 2--2 / 2
            // 0 / 2--2 / 2--2 / 3
            // 0 / 2--2 / 2--2 / 3--3 / 4
            // 0 / 2--2 / 2--2 / 3--3 / 5
            //(Note how, as shown by 10 / 1, order of ports within a component doesn't matter. However, you may only use each 
            //port on a component once.)
            //Of these bridges, the strongest one is 0 / 1--10 / 1--9 / 10; it has a strength of 0 + 1 + 1 + 10 + 10 + 9 = 31.
            //What is the strength of the strongest bridge you can make with the components you have available?
            IList<Node> components = new List<Node>();
            int maxStrength = 0;

            string line;
            using (StringReader reader = new StringReader(Input))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    var component = line.Split('/').Select(p => int.Parse(p));
                    components.Add(new Node { A = component.First(), B = component.Last() });
                }
            }

            var roots = components.Where(c => c.A == 0 || c.B == 0);
            foreach(Node rootNode in roots)
            {
                Node root = new Node
                {
                    A = rootNode.A == 0 ? rootNode.A : rootNode.B,
                    B = rootNode.A == 0 ? rootNode.B : rootNode.A,
                };
                root.AddChildren(ref components, ref maxStrength);
            }

            Debug.WriteLine("Max strength: " + maxStrength);
        }

        public static void Part2()
        {
            //The bridge you've built isn't long enough; you can't jump the rest of the way.
            //In the example above, there are two longest bridges:
            // 0 / 2--2 / 2--2 / 3--3 / 4
            // 0 / 2--2 / 2--2 / 3--3 / 5
            //Of them, the one which uses the 3 / 5 component is stronger; its strength is 0 + 2 + 2 + 2 + 2 + 3 
            // + 3 + 5 = 19.
            //What is the strength of the longest bridge you can make? If you can make multiple bridges of the 
            //longest length, pick the strongest one.

            IList<Node2> components = new List<Node2>();
            int maxStrengthLongest = 0;
            int longestBridge = 0;

            string line;
            using (StringReader reader = new StringReader(Input))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    var component = line.Split('/').Select(p => int.Parse(p));
                    components.Add(new Node2 { A = component.First(), B = component.Last() });
                }
            }

            var roots = components.Where(c => c.A == 0 || c.B == 0);
            foreach (Node2 rootNode in roots)
            {
                Node2 root = new Node2
                {
                    A = rootNode.A == 0 ? rootNode.A : rootNode.B,
                    B = rootNode.A == 0 ? rootNode.B : rootNode.A,
                };
                root.AddChildren(ref components, ref maxStrengthLongest, ref longestBridge);
            }

            Debug.WriteLine("Max length: " + longestBridge);
            Debug.WriteLine("Max strength: " + maxStrengthLongest);
        }

        private class Node
        {
            public int A { get; set; }
            public int B { get; set; }
            public Node Parent { get; set; }
            public IList<Node> Children { get; set; } = new List<Node>();
            public int Strength => A + B + (Parent?.Strength ?? 0);

            public void AddChildren(ref IList<Node> components, ref int maxStrength)
            {
                foreach(Node component in components.Where(c => !InChain(c)))
                {
                    if (component.A == B)
                    {
                        var node = new Node { A = component.A, B = component.B, Parent = this };
                        Children.Add(node);
                        if (node.Strength > maxStrength) maxStrength = node.Strength;
                        node.AddChildren(ref components, ref maxStrength);
                    }
                    if (component.B == B)
                    {
                        var node = new Node { A = component.B, B = component.A, Parent = this };
                        Children.Add(node);
                        if (node.Strength > maxStrength) maxStrength = node.Strength;
                        node.AddChildren(ref components, ref maxStrength);
                    }
                }
            }

            private bool InChain(Node node)
            {
                //return ((A == node.A || A == node.B) && (B == node.B || B == node.A)) || (Parent?.InChain(node) ?? false);
                //gaat fout voor 39/39 && 1/1
                if ((A == node.A && B == node.B) || (A == node.B && B == node.A))
                {
                    return true;
                }
                if (Parent != null)
                {
                    return Parent.InChain(node);
                }
                return false;
            }

            public override string ToString()
            {
                return A + "/" + B + " -- " + Parent?.ToString();
            }
        }

        private class Node2
        {
            public int A { get; set; }
            public int B { get; set; }
            public Node2 Parent { get; set; }
            public IList<Node2> Children { get; set; } = new List<Node2>();
            public int Strength => A + B + (Parent?.Strength ?? 0);
            public int Length => 1 + (Parent?.Length ?? 0);

            public void AddChildren(ref IList<Node2> components, ref int maxStrength, ref int maxLength)
            {
                foreach (Node2 component in components.Where(c => !InChain(c)))
                {
                    if (component.A == B)
                    {
                        var node = new Node2 { A = component.A, B = component.B, Parent = this };
                        Children.Add(node);
                        if (node.Length > maxLength)
                        {
                            maxLength = node.Length;
                            maxStrength = node.Strength;
                        }
                        if (node.Length == maxStrength && node.Strength > maxStrength)
                        {
                            maxStrength = node.Length;
                        }
                        node.AddChildren(ref components, ref maxStrength, ref maxLength);
                    }
                    if (component.B == B)
                    {
                        var node = new Node2 { A = component.B, B = component.A, Parent = this };
                        Children.Add(node);
                        if(node.Length > maxLength)
                        {
                            maxLength = node.Length;
                            maxStrength = node.Strength;
                        }
                        if (node.Length == maxStrength && node.Strength > maxStrength)
                        {
                            maxStrength = node.Length;
                        }
                        node.AddChildren(ref components, ref maxStrength, ref maxLength);
                    }
                }
            }

            private bool InChain(Node2 node)
            {
                //return ((A == node.A || A == node.B) && (B == node.B || B == node.A)) || (Parent?.InChain(node) ?? false);
                //gaat fout voor 39/39 && 1/1
                if ((A == node.A && B == node.B) || (A == node.B && B == node.A))
                {
                    return true;
                }
                if (Parent != null)
                {
                    return Parent.InChain(node);
                }
                return false;
            }

            public override string ToString()
            {
                return A + "/" + B + " -- " + Parent?.ToString();
            }
        }

        private static string TestInput = @"0/2
2/2
2/3
3/4
3/5
0/1
10/1
9/10";

        private static string Input = @"25/13
4/43
42/42
39/40
17/18
30/7
12/12
32/28
9/28
1/1
16/7
47/43
34/16
39/36
6/4
3/2
10/49
46/50
18/25
2/23
3/21
5/24
46/26
50/19
26/41
1/50
47/41
39/50
12/14
11/19
28/2
38/47
5/5
38/34
39/39
17/34
42/16
32/23
13/21
28/6
6/20
1/30
44/21
11/28
14/17
33/33
17/43
31/13
11/21
31/39
0/9
13/50
10/14
16/10
3/24
7/0
50/50";
    }
}
