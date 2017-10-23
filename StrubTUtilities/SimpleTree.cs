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

		string IReadOnlyTreeNode<T>.Label => Name;

		public string Name { get; }

		T IReadOnlyTreeNode<T>.Value => throw new NotImplementedException();

		List<Node> _rootNodes { get; } = new List<Node>();

		IReadOnlyCollection<IReadOnlyTreeNode<T>> IReadOnlyTreeNode<T>.Children => _rootNodes;

		public IReadOnlyCollection<IReadOnlyTreeNode<T>> RootNodes => _rootNodes;

		public Tree(string name) => Name = name;

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

		public override string ToString() => $"[Tree<{typeof(T).Name}>] '{Name}'";

		class Node : IReadOnlyTreeNode<T> {

			internal Tree<T> Tree { get; }

			public string Label { get; }

			public T Value { get; }

			List<Node> _children { get; }

			public IReadOnlyCollection<IReadOnlyTreeNode<T>> Children => _children;

			public Node(Tree<T> tree, string label, T value, params Node[] children) : this(tree, label, value, (IReadOnlyCollection<Node>)children) { }

			public Node(Tree<T> tree, string label, T value, IReadOnlyCollection<Node> children) {

				Tree = tree;
				Label = label;
				Value = value;
				_children = children as List<Node> ?? new List<Node>(children);
			}

			public void AddChild(Node node) => _children.Add(node);

			public override string ToString() => $"{Label} ({Value}), {Children.Count} child(ren)";
		}

		public interface INodeReference {

			INodeReference AddNode(string label, T value);
		}

		class NodeReference : INodeReference {

			internal Node Node { get; }

			public NodeReference(Node node) => Node = node;

			public INodeReference AddNode(string label, T value) => Node.Tree.AddNode(Node, label, value);
		}
	}
}
