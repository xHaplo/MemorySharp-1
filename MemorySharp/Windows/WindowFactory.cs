﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Binarysharp.MemoryManagement.Common.Builders;

namespace Binarysharp.MemoryManagement.Windows
{
    /// <summary>
    ///     Class providing tools for manipulating windows.
    /// </summary>
    public class WindowFactory : IFactory
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The reference of the <see cref="MemoryManagement.MemorySharp" /> object.
        /// </summary>
        private MemoryBase MemorySharp { get; }
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowFactory" /> class.
        /// </summary>
        /// <param name="memorySharp">The reference of the <see cref="MemoryManagement.MemorySharp" /> object.</param>
        internal WindowFactory(MemoryBase memorySharp)
        {
            // Save the parameter
            MemorySharp = memorySharp;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets all the child windows that belong to the application.
        /// </summary>
        public IEnumerable<RemoteWindow> ChildWindows
        {
            get { return ChildWindowHandles.Select(handle => new RemoteWindow(MemorySharp, handle)); }
        }

        /// <summary>
        ///     Gets all the child window handles that belong to the application.
        /// </summary>
        internal IEnumerable<IntPtr> ChildWindowHandles
            => WindowCore.EnumChildWindows(MemorySharp.Process.MainWindowHandle);

        /// <summary>
        ///     Gets the main window of the application.
        /// </summary>
        public RemoteWindow MainWindow => new RemoteWindow(MemorySharp, MainWindowHandle);

        /// <summary>
        ///     Gets the main window handle of the application.
        /// </summary>
        public IntPtr MainWindowHandle => MemorySharp.Process.MainWindowHandle;

        /// <summary>
        ///     Gets all the windows that have the same specified title.
        /// </summary>
        /// <param name="windowTitle">The window title string.</param>
        /// <returns>A collection of <see cref="RemoteWindow" />.</returns>
        public IEnumerable<RemoteWindow> this[string windowTitle] => GetWindowsByTitle(windowTitle);

        /// <summary>
        ///     Gets all the windows that belong to the application.
        /// </summary>
        public IEnumerable<RemoteWindow> RemoteWindows
        {
            get { return WindowHandles.Select(handle => new RemoteWindow(MemorySharp, handle)); }
        }

        /// <summary>
        ///     Gets all the window handles that belong to the application.
        /// </summary>
        internal IEnumerable<IntPtr> WindowHandles => new List<IntPtr>(ChildWindowHandles) {MainWindowHandle};
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Releases all resources used by the <see cref="WindowFactory" /> object.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose... yet
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Gets all the windows that have the specified class name.
        /// </summary>
        /// <param name="className">The class name string.</param>
        /// <returns>A collection of <see cref="RemoteWindow" />.</returns>
        public IEnumerable<RemoteWindow> GetWindowsByClassName(string className)
        {
            return WindowHandles
                .Where(handle => WindowCore.GetClassName(handle) == className)
                .Select(handle => new RemoteWindow(MemorySharp, handle));
        }

        /// <summary>
        ///     Gets all the windows that have the same specified title.
        /// </summary>
        /// <param name="windowTitle">The window title string.</param>
        /// <returns>A collection of <see cref="RemoteWindow" />.</returns>
        public IEnumerable<RemoteWindow> GetWindowsByTitle(string windowTitle)
        {
            return WindowHandles
                .Where(handle => WindowCore.GetWindowText(handle) == windowTitle)
                .Select(handle => new RemoteWindow(MemorySharp, handle));
        }

        /// <summary>
        ///     Gets all the windows that contain the specified title.
        /// </summary>
        /// <param name="windowTitle">A part a window title string.</param>
        /// <returns>A collection of <see cref="RemoteWindow" />.</returns>
        public IEnumerable<RemoteWindow> GetWindowsByTitleContains(string windowTitle)
        {
            return WindowHandles
                .Where(handle => WindowCore.GetWindowText(handle).Contains(windowTitle))
                .Select(handle => new RemoteWindow(MemorySharp, handle));
        }
        #endregion
    }
}