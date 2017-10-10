using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace QuadTree
{
    /// <summary>
    ///     A node for a QuadTree using a List implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QTreeNode<T>
    {
        [JsonRequired] [JsonProperty("Children")] private List<QTreeNode<T>> _children = new List<QTreeNode<T>>(4);

        /// <summary>
        ///     Creates a new QuadTree node
        /// </summary>
        /// <param name="parent">The parent node</param>
        public QTreeNode(QTreeNode<T> parent)
        {
            Parent = parent;
        }

        /// <inheritdoc />
        /// <param name="parent">The parent node</param>
        /// <param name="content">The content at this node</param>
        public QTreeNode(QTreeNode<T> parent, T content) : this(parent)
        {
            Content = content;
        }

        /// <summary>
        ///     Creates a new QuadTree node with the specfied content and no parent.
        /// </summary>
        /// <param name="content">The content at this node</param>
        public QTreeNode(T content)
        {
            Content = content;
        }

        [JsonConstructor]
        private QTreeNode()
        {
        }

        [JsonIgnore]
        public QTreeNode<T> Parent { get; set; }

        /// <summary>
        ///     The conent stored at this node
        /// </summary>
        [JsonProperty("Value")]
        public T Content { get; set; }

        /// <summary>
        ///     Indicates if this node has children
        /// </summary>
        [JsonIgnore]
        public bool HasChildren => _children.Count > 0;

        /// <summary>
        ///     Returns the number of descendant nodes and this node.
        /// </summary>
        [JsonIgnore]
        public int Count
        {
            get { return 1 + _children.Sum(node => node.Count); }
        }

        /// <summary>
        ///     Indicates that this node has no content and there are no child nodes
        /// </summary>
        [JsonIgnore]
        public bool IsEmpty => _children.Count == 0 && Content == null;


        /// <summary>
        ///     Returns the child node located at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public QTreeNode<T> Child(int index)
        {
            try
            {
                return _children[index];
            }
            catch (IndexOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException("index parameter is out of range.", e);
            }
        }

        /// <summary>
        ///     Saves this node to a JSON file. The contents stored in the node must be serializable.
        /// </summary>
        /// <param name="filename"></param>
        public void SaveToJson(string filename)
        {
            using (var file = File.CreateText(filename))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, this);
            }
        }

        /// <summary>
        ///     Fixes parent links after deserializing a json file into a tree.
        /// </summary>
        /// <param name="root"></param>
        private static void FixParentLinks(QTreeNode<T> root)
        {
            foreach (var child in root._children)
            {
                child.Parent = root;
                FixParentLinks(child);
            }
        }


        /// <summary>
        ///     Deserialize a json file into a Quad Tree of type T
        /// </summary>
        /// <param name="fileName">The name of the json file</param>
        /// <returns></returns>
        public static QTreeNode<T> LoadFromJson(string fileName)
        {
            QTreeNode<T> serializedTree;

            // deserialize JSON directly from a file
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                serializedTree = (QTreeNode<T>) serializer.Deserialize(file, typeof(QTreeNode<T>));
            }
            FixParentLinks(serializedTree);
            return serializedTree;
        }


        /// <summary>
        ///     Inserts a QTreeNode into this node. The inserted node becomes a child of this node.
        /// </summary>
        /// <param name="child">The child node to insert</param>
        public void Insert(QTreeNode<T> child)
        {
            if (_children.Count < 4)
            {
                _children.Add(child);
                child.Parent = this;
            }
            else
            {
                throw new IndexOutOfRangeException("There are already four children in this node.");
            }
        }

        /// <summary>
        ///     Removes the child at the specified index and returns it
        /// </summary>
        /// <param name="index">The index of the child to remove</param>
        /// <returns></returns>
        public QTreeNode<T> RemoveAt(int index)
        {
            if (_children.Count - 1 < index)
                throw new IndexOutOfRangeException();
            var item = _children[index];
            _children.RemoveAt(index);
            return item;
        }

        /// <summary>
        ///     Returns a collection of items that match the predicate
        /// </summary>
        /// <param name="predicate">The predicate used for searching</param>
        /// <returns></returns>
        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            if (predicate(Content))
                yield return Content;
            foreach (var item in _children)
            foreach (var x in item.Where(predicate))
                yield return x;
        }
        /// <summary>
        /// Returns a collection of lists containaing nodes at each level in the tree.
        /// </summary>
        /// <returns></returns>
        public List<List<QTreeNode<T>>> GetNodeLevels()
        {
            
            var levels = new List<List<QTreeNode<T>>>();
            var level = new List<QTreeNode<T>>();

            var queue = new Queue<QTreeNode<T>>();
            
            
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var levelNodes = queue.Count;
                while (levelNodes > 0)
                {
                    var node = queue.Dequeue();
                    foreach (var child in node._children)
                    {
                        queue.Enqueue(child);
                    }
                    levelNodes--;
                    level.Add(node);
                    if (levelNodes == 0)
                    {
                        levels.Add(level);
                        level = new List<QTreeNode<T>>();

                    }

                }
            }
            return levels;

            
        }

        /// <summary>
        /// Returns a collection of items using in order traversal.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> BreathFirstEnumerable()
        {
            var queue = new Queue<QTreeNode<T>>();
            if (IsEmpty)
            {
                yield break;
            }
            
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var levelNodes = queue.Count;
                while (levelNodes > 0)
                {
                    var node = queue.Dequeue();
                    var content = node.Content;
                    foreach (var child in node._children)
                    {
                        queue.Enqueue(child);
                    }
                    levelNodes--;
                    yield return content;

                }
            }
        }

        /// <summary>
        ///     Returns the first value that matches the predicate.
        /// </summary>
        /// <param name="predicate">The predicate used for searching.</param>
        /// <returns></returns>
        public T SelectFirstOrDefault(Func<T, bool> predicate)
        {
            if (predicate(Content))
                return Content;
            foreach (var item in _children)
            {
                var x = item.SelectFirstOrDefault(predicate);
                if (predicate(x))
                    return x;
            }
            return default(T);
        }

        

        
    }
}