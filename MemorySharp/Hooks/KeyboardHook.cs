using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Binarysharp.MemoryManagement.Common.Builders;
using Binarysharp.MemoryManagement.Native;
using Binarysharp.MemoryManagement.Native.Enums;

namespace Binarysharp.MemoryManagement.Hooks
{
    /// <summary>
    ///     Class to handle keyboard events.
    /// </summary>
    public class KeyboardHook : IHook
    {
        #region Public Delegates/Events
        /// <summary>
        ///     Function that will be called when defined events occur.
        /// </summary>
        /// <param name="key">VKeys</param>
        public delegate void KeyboardHookCallback(Keys key);

        /// <summary>
        ///     Occurs when [key down].
        /// </summary>
        public event KeyboardHookCallback KeyDown;

        /// <summary>
        ///     Occurs when [key up].
        /// </summary>
        public event KeyboardHookCallback KeyUp;
        #endregion

        #region Fields, Private Properties
        private bool _isEnabled;

        /// <summary>
        ///     Gets or sets the hook identifier.
        /// </summary>
        /// <value>The hook identifier.</value>
        private IntPtr HookId { get; set; } = IntPtr.Zero;

        /// <summary>
        ///     Gets or sets the hook handler.
        /// </summary>
        /// <value>The hook handler.</value>
        private KeyboardHookHandler HookHandler { get; set; }
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyboardHook" /> class.
        /// </summary>
        /// <param name="name">The name that represents this keyboard hook instance.</param>
        /// <param name="mustBeDisposed">Whether this instance must be disposed when the Garbage Collector collects the object.</param>
        public KeyboardHook(string name, bool mustBeDisposed = true)
        {
            Name = name;
            MustBeDisposed = mustBeDisposed;
        }

        /// <summary>
        ///     Destructor. Unhook current hook
        /// </summary>
        ~KeyboardHook()
        {
            Dispose();
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     The name that represents this instance.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets a value indicating whether the element must be disposed when the Garbage Collector collects the object.
        /// </summary>
        public bool MustBeDisposed { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the keyboard hook is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the keyboard hook is currently enabled or not.
        /// </summary>
        /// <value>If the keyboard hook is enabled, <code>true</code>, else <code>false</code>.</value>
        public bool IsEnabled { get; set; }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!MustBeDisposed) return;
            Disable();
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }


        /// <summary>
        ///     Disables the low level keyboard hook.
        /// </summary>
        public void Disable()
        {
            UnhookWindowsHookEx(HookId);
            IsEnabled = false;
        }

        /// <summary>
        ///     Enables the low level keyboard hook.
        /// </summary>
        public void Enable()
        {
            HookHandler = HookFunc;
            HookId = SetHook(HookHandler);
            IsEnabled = true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        ///     Registers hook with Windows API.
        /// </summary>
        /// <param name="proc">Callback function</param>
        /// <returns>Hook ID.</returns>
        private static IntPtr SetHook(KeyboardHookHandler proc)
        {
            using (var module = Process.GetCurrentProcess().MainModule)
                return SetWindowsHookEx(0xd, proc, NativeMethods.GetModuleHandle(module.ModuleName), 0);
        }


        /// <summary>
        ///     Default hook call, which analyses pressed keys.
        /// </summary>
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return NativeMethods.CallNextHookEx(HookId, nCode, wParam, lParam);
            }

            if (IsKeyDownOrUpEvent((int) wParam))
            {
                KeyDown?.Invoke((Keys) Marshal.ReadInt32(lParam));
            }

            if (IsSysKeyDownOrUpEvent((int) wParam))
            {
                KeyUp?.Invoke((Keys) Marshal.ReadInt32(lParam));
            }

            return NativeMethods.CallNextHookEx(HookId, nCode, wParam, lParam);
        }


        /// <summary>
        ///     Determines whether the message given contains key up and down event codes.
        /// </summary>
        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        private static bool IsKeyDownOrUpEvent(int messages)
        {
            switch ((WindowsMessages) messages)
            {
                case WindowsMessages.KeyDown:
                    return true;
                case WindowsMessages.KeyUp:
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Determines whether the message given contains system key up and down event codes.
        /// </summary>
        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        private static bool IsSysKeyDownOrUpEvent(int messages)
        {
            switch ((WindowsMessages) messages)
            {
                case WindowsMessages.SysKeyDown:
                    return true;
                case WindowsMessages.SysKeyUp:
                    return true;
            }
            return false;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookHandler lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        #endregion

        /// <summary>
        ///     Internal callback processing function
        /// </summary>
        private delegate IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
    }
}