using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace QuadTree.Tests
{
    public class QuadTreeTest
    {
        [Fact]
        public void CountChildrenWithChildren()
        {
            var root = new QTreeNode<int>(1);
            root.Insert(new QTreeNode<int>(2));
            root.Child(0).Insert(new QTreeNode<int>(3));
            root.Insert(new QTreeNode<int>(4));
            root.Insert(new QTreeNode<int>(5));
            root.Child(2).Insert(new QTreeNode<int>(6));
            root.Child(2).Child(0).Insert(new QTreeNode<int>(7));
            Assert.True(root.Count == 7, "root.Count != 7");
        }

        [Fact]
        public void CountEmptyContent()
        {
            var root = new QTreeNode<int>(null);
            Assert.True(root.Count == 1, "root.Count == 1");
        }

        [Fact]
        public void SelectFirstOrDefault0()
        {
            var root = new QTreeNode<int>(1);
            root.Insert(new QTreeNode<int>(2));
            root.Child(0).Insert(new QTreeNode<int>(3));
            root.Insert(new QTreeNode<int>(4));
            root.Insert(new QTreeNode<int>(5));
            root.Child(2).Insert(new QTreeNode<int>(6));
            root.Child(2).Child(0).Insert(new QTreeNode<int>(7));
            Assert.True(root.SelectFirstOrDefault(x => x == -4) == 0, "root.SelectFirstOrDefault(x => x == -4) == 0");
        }


        [Fact]
        public void SelectFirstOrDefault6()
        {
            var root = new QTreeNode<int>(1);
            root.Insert(new QTreeNode<int>(2));
            root.Child(0).Insert(new QTreeNode<int>(3));
            root.Insert(new QTreeNode<int>(4));
            root.Insert(new QTreeNode<int>(5));
            root.Child(2).Insert(new QTreeNode<int>(6));
            root.Child(2).Child(0).Insert(new QTreeNode<int>(7));
            Assert.True(root.SelectFirstOrDefault(x => x == 6) == 6, "root.SelectFirstOrDefault(x => x == 6) == 6");
        }

        [Fact]
        public void SelectFirstOrDefault7()
        {
            var root = new QTreeNode<int>(1);
            root.Insert(new QTreeNode<int>(2));
            root.Child(0).Insert(new QTreeNode<int>(3));
            root.Insert(new QTreeNode<int>(4));
            root.Insert(new QTreeNode<int>(5));
            root.Child(2).Insert(new QTreeNode<int>(6));
            root.Child(2).Child(0).Insert(new QTreeNode<int>(7));
            Assert.True(root.SelectFirstOrDefault(x => x == 7) == 7, "root.SelectFirstOrDefault(x => x == 7) == 7");
        }

        [Fact]
        public void SelectFirstOrDefault90()
        {
            var root = new QTreeNode<int>(1);
            root.Insert(new QTreeNode<int>(2));
            root.Child(0).Insert(new QTreeNode<int>(3));
            root.Insert(new QTreeNode<int>(4));
            root.Insert(new QTreeNode<int>(5));
            root.Child(2).Insert(new QTreeNode<int>(6));
            root.Child(2).Child(0).Insert(new QTreeNode<int>(7));
            Assert.True(root.SelectFirstOrDefault(x => x == 90) == 0, "root.SelectFirstOrDefault(x => x == 90) == 0");
        }

        [Fact]
        public void SelectFirstOrDefaultNotFound()
        {
            var root = new QTreeNode<int>(1);
            root.Insert(new QTreeNode<int>(2));
            root.Child(0).Insert(new QTreeNode<int>(3));
            root.Insert(new QTreeNode<int>(4));
            root.Insert(new QTreeNode<int>(5));
            root.Child(2).Insert(new QTreeNode<int>(6));
            root.Child(2).Child(0).Insert(new QTreeNode<int>(7));
            Assert.True(root.SelectFirstOrDefault(x => x == 123) == default(int),
                "root.SelectFirstOrDefault(x => x == 123) == 123");
        }

        [Fact]
        public void TestHasChildren()
        {
            var root = new QTreeNode<string>("root");
            Assert.False(root.HasChildren);
            root.Insert(new QTreeNode<string>("child1"));
            Assert.True(root.HasChildren);
            Assert.False(root.Child(0).HasChildren);
            Assert.True(root.Child(0).Parent.HasChildren);
        }

        [Fact]
        public void TestInsertMoreThanFourChild()
        {
            var root = new QTreeNode<string>("Root");
            var child = new QTreeNode<string>(root, "Child");
            root.Insert(child);

            child.Insert(new QTreeNode<string>("child1"));
            child.Insert(new QTreeNode<string>("child2"));
            child.Insert(new QTreeNode<string>("child3"));
            child.Insert(new QTreeNode<string>("child4"));

            Assert.Throws(typeof(IndexOutOfRangeException), () => { child.Insert(new QTreeNode<string>("child5")); });

            Assert.True(root.Child(0).Child(3).Content == "child4");
        }


        [Fact]
        public void TestInsertMoreThanFourRoot()
        {
            Assert.Throws(typeof(IndexOutOfRangeException), () =>
            {
                var root = new QTreeNode<string>("Root");
                root.Insert(new QTreeNode<string>("child1"));
                root.Insert(new QTreeNode<string>("child2"));
                root.Insert(new QTreeNode<string>("child3"));
                root.Insert(new QTreeNode<string>("child4"));
                root.Insert(new QTreeNode<string>("child5"));
            });
        }

        [Fact]
        public void TestRmoveAt()
        {
            var root = new QTreeNode<int>(0);
            root.Insert(new QTreeNode<int>(1));
            root.Insert(new QTreeNode<int>(2));
            root.Insert(new QTreeNode<int>(3));
            root.Insert(new QTreeNode<int>(4));
            Assert.True(root.RemoveAt(3).Content == 4);
            Assert.True(root.Count - 1 == 3);
            Assert.True(root.RemoveAt(2).Content == 3);
            Assert.True(root.Count - 1 == 2);
            Assert.True(root.RemoveAt(1).Content == 2);
            Assert.True(root.Count - 1 == 1);
            Assert.True(root.RemoveAt(0).Content == 1);
            Assert.True(root.Count - 1 == 0);

            Assert.Throws(typeof(IndexOutOfRangeException), () => { root.RemoveAt(0); });
        }

        [Fact]
        public void TestSerializeDeserializeJson()
        {
            var root = new QTreeNode<int>(1);
            root.Insert(new QTreeNode<int>(2));
            root.Insert(new QTreeNode<int>(3));
            root.Child(1).Insert(new QTreeNode<int>(5));
            root.Child(1).Insert(new QTreeNode<int>(6));
            root.Child(1).Insert(new QTreeNode<int>(7));
            root.Child(1).Insert(new QTreeNode<int>(8));
            root.Child(1).Child(3).Insert(new QTreeNode<int>(24));
            root.Child(0).Insert(new QTreeNode<int>(48));

            root.SaveToJson("test.json");

            var serializedTree = QTreeNode<int>.LoadFromJson("test.json");


            Assert.True(serializedTree.Content == root.Content);
            Assert.True(serializedTree.Child(0).Content == root.Child(0).Content);
            Assert.True(serializedTree.Child(1).Content == root.Child(1).Content);
            Assert.True(serializedTree.Child(1).Child(0).Content == root.Child(1).Child(0).Content);
            Assert.True(serializedTree.Child(1).Child(1).Content == root.Child(1).Child(1).Content);
            Assert.True(serializedTree.Child(1).Child(2).Content == root.Child(1).Child(2).Content);
            Assert.True(serializedTree.Child(1).Child(3).Content == root.Child(1).Child(3).Content);
            Assert.True(serializedTree.Child(1).Child(3).Child(0).Content == root.Child(1).Child(3).Child(0).Content);
            Assert.True(serializedTree.Child(0).Child(0).Content == root.Child(0).Child(0).Content);

            Assert.True(root.Child(0).Parent.Content == serializedTree.Child(0).Parent.Content);
            Assert.True(serializedTree.Child(1).Child(3).Child(0).Parent.Parent.Content == root.Child(1).Content);
        }

        [Fact]
        public void WhereEqualToOneRoot()
        {
            var root = new QTreeNode<int>(1);
            var items = root.Where(x => x == 1);
            Console.WriteLine(items);
            var test = new[] {1};
            Assert.True(!test.Except(items).Any());
        }

        [Fact]
        public void WhereGreaterThanSevenRootChildWithChild()
        {
            var root = new QTreeNode<int>(1);
            root.Insert(new QTreeNode<int>(2));
            root.Insert(new QTreeNode<int>(3));
            root.Child(1).Insert(new QTreeNode<int>(5));
            root.Child(1).Insert(new QTreeNode<int>(6));
            root.Child(1).Insert(new QTreeNode<int>(7));
            root.Child(1).Insert(new QTreeNode<int>(8));
            root.Child(1).Child(3).Insert(new QTreeNode<int>(24));
            root.Child(0).Insert(new QTreeNode<int>(48));
            var items = root.Where(x => x > 7);

            var enumerable = items as int[] ?? items.ToArray();
            Assert.True(enumerable.Contains(24));
            Assert.True(enumerable.Contains(48));
            Assert.True(enumerable.Contains(8));
            Assert.False(enumerable.Contains(7));

            Assert.False(enumerable.Contains(1));
            Assert.True(enumerable.Length == 3);
        }

        [Fact]
        public void WhereGreaterThanZeroRoot()
        {
            var root = new QTreeNode<int>(1);
            var items = root.Where(x => x > 0);
            Console.WriteLine(items);
            var test = new[] {1};
            Assert.True(!test.Except(items).Any());
        }

        [Fact]
        public void WhereGreaterThanZeroRootChild()
        {
            var root = new QTreeNode<int>(1);
            root.Insert(new QTreeNode<int>(2));
            root.Insert(new QTreeNode<int>(3));
            var items = root.Where(x => x > 0);

            var enumerable = items as int[] ?? items.ToArray();
            Assert.True(enumerable.Contains(3));
            Assert.True(enumerable.Contains(2));
            Assert.True(enumerable.Contains(1));
        }

    }

    public class Team
    {
        public Team()
        {
            
        }

        public Team(string name, int number)
        {
            Name = name;
            Number = number;
        }

        public string Name { get; set; }

        public int Number { get; set; }
    }
}