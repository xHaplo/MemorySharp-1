﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.Collections.Generic;
using System.Linq;

namespace Binarysharp.MemoryManagement.Common.Builders
{
    /// <summary>
    ///     Class managing objects implementing <see cref="INamedElement" /> interface.
    /// </summary>
    public abstract class Manager<T> where T : INamedElement
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The collection of the elements (writable).
        /// </summary>
        protected Dictionary<string, T> InternalItems = new Dictionary<string, T>();
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     The collection of the elements.
        /// </summary>
        public IReadOnlyDictionary<string, T> ItemsAsDictionary => InternalItems;

        /// <summary>
        ///     Gets the managersitems as a <see cref="IReadOnlyList{T}" /> instance.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        public IReadOnlyList<T> Items => InternalItems.Values.ToList();
        #endregion

        #region Public Methods
        /// <summary>
        ///     Disables all items in the manager.
        /// </summary>
        public void DisableAll()
        {
            foreach (var item in InternalItems)
            {
                item.Value.Disable();
            }
        }

        /// <summary>
        ///     Enables all items in the manager.
        /// </summary>
        public void EnableAll()
        {
            foreach (var item in InternalItems)
            {
                item.Value.Enable();
            }
        }

        /// <summary>
        ///     Removes an element by its name in the manager.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public void Remove(string name)
        {
            // Check if the element exists in the dictionary
            if (InternalItems.ContainsKey(name))
            {
                try
                {
                    // Dispose the element
                    InternalItems[name].Dispose();
                }
                finally
                {
                    // Remove the element from the dictionary
                    InternalItems.Remove(name);
                }
            }
        }

        /// <summary>
        ///     Remove a given element.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        public void Remove(T item)
        {
            Remove(item.Name);
        }

        /// <summary>
        ///     Removes all the elements in the manager.
        /// </summary>
        public void RemoveAll()
        {
            // For each element
            foreach (var item in InternalItems)
            {
                // Dispose it
                item.Value.Dispose();
            }
            // Clear the dictionary
            InternalItems.Clear();
        }
        #endregion
    }
}