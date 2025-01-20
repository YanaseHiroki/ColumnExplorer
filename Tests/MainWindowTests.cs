//using NUnit.Framework;
//using System.Windows.Controls;
//using System.Windows.Input;
//using System.IO;
//using System;
//using System.Windows.Media;
//using System.Windows;

//namespace ColumnExplorer.Views
//{
//    [TestFixture]
//    public class MainWindowTests
//    {
//        private MainWindow _mainWindow;

//        [SetUp]
//        public void Setup()
//        {
//            _mainWindow = new MainWindow();
//        }

//        [Test]
//        public void Test_LoadHomeDirectory()
//        {
//            _mainWindow.LoadHomeDirectory();
//            Assert.That(_mainWindow.CenterColumnPath, Is.EqualTo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
//        }

//        [Test]
//        public void Test_LoadAllContent_WithValidPath()
//        {
//            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
//            _mainWindow.LoadAllContent(path);
//            Assert.That(_mainWindow.CenterColumnPath, Is.EqualTo(path));
//        }

//        [Test]
//        public void Test_LoadAllContent_WithEmptyPath()
//        {
//            _mainWindow.LoadAllContent(string.Empty);
//            Assert.That(_mainWindow.CenterColumnPath, Is.EqualTo(string.Empty));
//        }

//        [Test]
//        public void Test_FocusSelectedItemInCenterColumn_WithValidItem()
//        {
//            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
//            _mainWindow.LoadAllContent(path);
//            _mainWindow.FocusSelectedItemInCenterColumn(path);
//            Assert.That(_mainWindow.CenterColumnPath, Is.EqualTo(path));
//        }

//        [Test]
//        public void Test_SelectItemInColumn_WithValidPath()
//        {
//            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
//            _mainWindow.LoadAllContent(path);
//            _mainWindow.SelectItemInColumn(_mainWindow.CenterColumn, path);
//            Assert.That((_mainWindow.CenterColumn.SelectedItem as ListBoxItem)?.Tag, Is.EqualTo(path));
//        }

//        [Test]
//        public void Test_OnKeyDown_RightKey()
//        {
//            var e = new KeyEventArgs(Keyboard.PrimaryDevice, new FakePresentationSource(), 0, Key.Right) { RoutedEvent = Keyboard.KeyDownEvent };
//            _mainWindow.OnKeyDown(e);
//            // Add assertions based on expected behavior
//        }

//        [Test]
//        public void Test_OnKeyDown_LeftKey()
//        {
//            var e = new KeyEventArgs(Keyboard.PrimaryDevice, new FakePresentationSource(), 0, Key.Left) { RoutedEvent = Keyboard.KeyDownEvent };
//            _mainWindow.OnKeyDown(e);
//            // Add assertions based on expected behavior
//        }

//        [Test]
//        public void Test_OnKeyDown_UpKey()
//        {
//            var e = new KeyEventArgs(Keyboard.PrimaryDevice, new FakePresentationSource(), 0, Key.Up) { RoutedEvent = Keyboard.KeyDownEvent };
//            _mainWindow.OnKeyDown(e);
//            // Add assertions based on expected behavior
//        }

//        [Test]
//        public void Test_OnKeyDown_DownKey()
//        {
//            var e = new KeyEventArgs(Keyboard.PrimaryDevice, new FakePresentationSource(), 0, Key.Down) { RoutedEvent = Keyboard.KeyDownEvent };
//            _mainWindow.OnKeyDown(e);
//            // Add assertions based on expected behavior
//        }

//        [Test]
//        public void Test_OnKeyDown_EnterKey()
//        {
//            var e = new KeyEventArgs(Keyboard.PrimaryDevice, new FakePresentationSource(), 0, Key.Enter) { RoutedEvent = Keyboard.KeyDownEvent };
//            _mainWindow.OnKeyDown(e);
//            // Add assertions based on expected behavior
//        }

//        [Test]
//        public void Test_OnKeyDown_F5Key()
//        {
//            var e = new KeyEventArgs(Keyboard.PrimaryDevice, new FakePresentationSource(), 0, Key.F5) { RoutedEvent = Keyboard.KeyDownEvent };
//            _mainWindow.OnKeyDown(e);
//            // Add assertions based on expected behavior
//        }

//        [Test]
//        public void Test_MoveToSubDirectory()
//        {
//            _mainWindow.MoveToSubDirectory();
//            // Add assertions based on expected behavior
//        }

//        [Test]
//        public void Test_MoveToParentDirectory()
//        {
//            _mainWindow.MoveToParentDirectory();
//            // Add assertions based on expected behavior
//        }

//        [Test]
//        public void Test_OpenSelectedItems()
//        {
//            _mainWindow.OpenSelectedItems();
//            // Add assertions based on expected behavior
//        }

//        [Test]
//        public void Test_UpdateRightColumnWithSelectedItems()
//        {
//            _mainWindow.UpdateRightColumnWithSelectedItems();
//            // Add assertions based on expected behavior
//        }

//        [Test]
//        public void Test_MoveItems()
//        {
//            var source = new ListBox();
//            var destination = new ListBox();
//            source.Items.Add(new ListBoxItem { Content = "Item1" });
//            _mainWindow.MoveItems(source, destination);
//            Assert.That(source.Items.Count, Is.EqualTo(0));
//            Assert.That(destination.Items.Count, Is.EqualTo(1));
//        }
//    }

//    // Fake PresentationSource for KeyEventArgs
//    public class FakePresentationSource : PresentationSource
//    {
//        protected override CompositionTarget GetCompositionTargetCore() => null;
//        public override Visual RootVisual { get; set; }
//        public override bool IsDisposed { get; }
//    }
//}
