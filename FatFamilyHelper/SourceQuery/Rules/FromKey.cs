﻿using System;

namespace FatFamilyHelper.SourceQuery.Rules
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FromKey : Attribute
    {
        public string KeyName { get; }

        public FromKey(string keyName)
        {
            KeyName = keyName;
        }
    }
}
