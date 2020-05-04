using System;

namespace MRRC.Cursive
{
    /// <summary>
    /// Allows overriding the name of a property on a class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NameAttribute : Attribute
    {
        public NameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}