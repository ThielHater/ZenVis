using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ZenVis.Shared
{
    [TypeConverter(typeof(LanguageConverter))]
    public class Language : IEquatable<Language>, IEqualityComparer<Language>
    {
        public string Name { get; set; }
        public int CodePage { get; set; }

        public Language(string name, int codePage)
        {
            Name = name;
            CodePage = codePage;
        }

        public static bool operator ==(Language a, Language b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (((object)a == null) || ((object)b == null))
                return false;

            return ((a.Name == b.Name) && (a.CodePage == b.CodePage));
        }

        public static bool operator !=(Language a, Language b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            return Equals((Language)obj);
        }

        public bool Equals(Language other)
        {
            if (other == null)
                return false;

            return ((Name == other.Name) && (CodePage == other.CodePage));
        }

        public bool Equals(Language x, Language y)
        {
            if ((x == null) || (y == null))
                return false;

            return x.Equals(y);
        }

        public override int GetHashCode()
        {
            return ((Name != null ? Name.GetHashCode() : 701) * 397) ^ CodePage * 127;
        }

        public int GetHashCode(Language obj)
        {
            return obj.GetHashCode();
        }

        public static implicit operator string(Language obj)
        {
            return obj.Name;
        }
    }
}