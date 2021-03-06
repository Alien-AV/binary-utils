﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace binary_utils
{
    public class GlobalHotkeys: IDisposable
    {
        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        private int _currentHotkeyId = 9000;

        public delegate void HotkeyHandler();

        private struct HotkeyKeysAndHandler
        {
            public uint Vk;
            public uint Mod;
            public HotkeyHandler HotkeyHandler;
        }

        private readonly Dictionary<int, HotkeyKeysAndHandler> _hotkeyById = new Dictionary<int, HotkeyKeysAndHandler>();

        public GlobalHotkeys(Window window)
        {
            _source = new HwndSource(new HwndSourceParameters()); //TODO: see if this is good
            _source.AddHook(HwndHook);
        }

        public void Dispose()
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            
            DeleteAllHotKeys();
        }

        public int AddHotKey(Key key, ModifierKeys mod, HotkeyHandler handler)
        {
            var vKey = Convert.ToUInt32(KeyInterop.VirtualKeyFromKey(key));
            var modUint = Convert.ToUInt32(mod);
            _hotkeyById.Add(_currentHotkeyId, new HotkeyKeysAndHandler()
                {
                    Vk = vKey,
                    Mod = modUint,
                    HotkeyHandler = handler
                });

            if(!RegisterHotKey(_source.Handle, _currentHotkeyId, modUint, vKey))
            {
                // handle error
            }

            var justAddedHotkeyId = _currentHotkeyId;
            _currentHotkeyId++;
            return justAddedHotkeyId;
        }

        private void DeleteHotKey(int id)
        {
            UnregisterHotKey(_source.Handle, id);
        }

        private void DeleteAllHotKeys()
        {
            foreach (var key in _hotkeyById.Keys)
            {
                UnregisterHotKey(_source.Handle, key);
            }
        }

        private void RunHotkeyHandlerById(int id)
        {
            _hotkeyById[id].HotkeyHandler();
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if(msg == WM_HOTKEY){
                    RunHotkeyHandlerById(wParam.ToInt32());
                    handled = true;
            }
            return IntPtr.Zero;
        }
    }
}
