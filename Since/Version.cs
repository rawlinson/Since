using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Since
{
    public sealed class Version
    {
        public Version()
        { }

        public Version(int major, int minor, int patch, int revision)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Revision = revision;
        }

        /// <summary>
        /// Gets the value of the major component of the version number for the current Version object.
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Gets the value of the minor component of the version number for the current Version object.
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Gets the value of the patch component of the version number for the current Version object.
        /// </summary>
        public int Patch { get; set; }

        /// <summary>
        /// Gets the value of the patch component of the version number for the current Version object.
        /// </summary>
        public int Revision { get; set; }

        public string TagString { get; set; }
        public string PreString { get; set; }
        public string PostString { get; set; }
        public string BuildString { get; set; }

        public string GetVersionString(int parts = 4)
        {
            string result = null;
            if (parts > 0) result += this.Major;
            if (parts > 1) result += "." + this.Minor;
            if (parts > 2) result += "." + this.Patch;
            if (parts > 3) result += "." + this.Revision;
            return result;
        }

        public string ToString(bool published)
            => this.GetVersionString(3)
                + Concat("_", this.TagString)
                + Concat("-", this.PreString)
                + (!published ? (Concat("+", this.PostString) + Concat(" (", this.BuildString, ")")) : null);
        
        static string Concat(params string[] values)
        {
            string result = String.Empty;
            foreach (var value in values)
            {
                if (value == null)
                    return null;
                result += value;
            }
            return result;
        }

        public static Version Parse(string versionString)
        {
            Version version;
            if (!Version.TryParse(versionString, out version))
                throw new FormatException();
            return version;
        }

        public static bool TryParse(string versionString, out Version version)
        {
            version = null;
            return false;
        }
    }
}
