﻿// Copyright (c) 2012 Blue Onion Software, All rights reserved
using System;
using Microsoft.Win32;

namespace BlueOnionSoftware
{
    public interface IRegistryKey : IDisposable
    {
        object GetValue(string name);
        void SetValue(string name, object value);
    }

    public class RegistryKeyImpl : IRegistryKey
    {
        RegistryKey _registryKey;

        public RegistryKeyImpl(RegistryKey registryKey)
        {
            _registryKey = registryKey;
        }

        public object GetValue(string name)
        {
            return _registryKey.GetValue(name);
        }

        public void SetValue(string name, object value)
        {
            _registryKey.SetValue(name, value);
        }

        public void Dispose()
        {
            if (_registryKey != null)
            {
                _registryKey.Close();
                _registryKey = null;
            }
        }
    }
}