using System;
using System.Collections.Generic;

namespace StrubT {

	public interface IReadOnlyTree<out T> {

		string Name { get; }

		IReadOnlyCollection<IReadOnlyTreeNode<T>> RootNodes { get; }
	}

	public interface IReadOnlyTreeNode<out T> {

		string Label { get; }

		T Value { get; }

		IReadOnlyCollection<IReadOnlyTreeNode<T>> Children { get; }
	}

	class Tree<T> : IReadOnlyTree<T>, IReadOnlyTreeNode<T> {

		readonly string _name;
		readonly List<Node> _rootNodes = new List<Node>();

		string IReadOnlyTreeNode<T>.Label { get { return _name; } }

		public string Name { get { return _name; } }

		T IReadOnlyTreeNode<T>.Value { get { throw new NotImplementedException(); } }

		IReadOnlyCollection<IReadOnlyTreeNode<T>> IReadOnlyTreeNode<T>.Children { get { return _rootNodes; } }

		public IReadOnlyCollection<IReadOnlyTreeNode<T>> RootNodes { get { return _rootNodes; } }

		public Tree(string name) { _name = name; }

		public INodeReference AddNode(string label, T value) {

			var node = new Node(this, label, value);
			_rootNodes.Add(node);
			return new NodeReference(node);
		}

		INodeReference AddNode(IReadOnlyTreeNode<T> parentNode, string label, T value) {

			var node = new Node(this, label, value);
			((Node)parentNode).AddChild(node);
			return new NodeReference(node);
		}

		public override string ToString() { return string.Format("[Tree<{0}>] '{1}'", typeof(T).Name, _name); }

		class Node : IReadOnlyTreeNode<T> {

			internal readonly Tree<T> tree;
			readonly string _label;
			readonly T _value;
			readonly List<Node> _children;

			public string Label { get { return _label; } }

			public T Value { get { return _value; } }

			public IReadOnlyCollection<IReadOnlyTreeNode<T>> Children { get { return _children; } }

			public Node(Tree<T> tree, string label, T value, params Node[] children) : this(tree, label, value, (IReadOnlyCollection<Node>)children) { }

			public Node(Tree<T> tree, string label, T value, IReadOnlyCollection<Node> children) {

				this.tree = tree;
				_label = label;
				_value = value;
				_children = children as List<Node> ?? new List<Node>(children);
			}

			public void AddChild(Node node) { _children.Add(node); }

			public override string ToString() { return string.Format("{0} ({1}), {2} child(ren)", _label, _value, _children.Count); }
		}

		public interface INodeReference {

			INodeReference AddNode(string label, T value);
		}

		class NodeReference : INodeReference {

			internal readonly Node node;

			public NodeReference(Node node) { this.node = node; }

			public INodeReference AddNode(string label, T value) { return node.tree.AddNode(node, label, value); }
		}
	}
}
