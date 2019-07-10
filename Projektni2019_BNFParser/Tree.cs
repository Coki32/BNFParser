using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Projektni2019_BNFParser
{
    class Tree
    {
        public class Node
        {
            public string Name { get; set; }

            public string Value { get; set; }
            public List<Node> Children { get; set; } = new List<Node>();

            public Node AddChild(Node node)
            {
                Children.Add(node);
                return node;
            }

            public void AddChildren(IEnumerable<Node> nodes) => Children.AddRange(nodes);

            public Node(string name, string value) => (Name, Value) = (name, value);

            public XmlElement ToXml(XmlDocument doc)
            {
                XmlElement root = doc.CreateElement(Name==null?"literal":Name);
                if (Value != null)
                    root.InnerText = Value;
                foreach (var child in Children)
                        root.AppendChild(child.ToXml(doc));
                return root;
            }
        }

        public Node Root { get; set; }

        public Tree(string name, string value) => Root = new Node(name, value);

        public XmlElement ToXml(XmlDocument doc) => Root.ToXml(doc);

        public Node AddScannedChild(string value) => Root.AddChild(new Node(null, value));

    }
}
