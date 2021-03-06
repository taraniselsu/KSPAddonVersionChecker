﻿// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using System;
using System.Text.RegularExpressions;

#endregion

namespace KSP_AVC
{
    public class VersionInfo : IComparable
    {
        public VersionInfo(long major = 0, long minor = 0, long patch = 0, long build = 0)
        {
            this.SetVersion(major, minor, patch, build);
        }

        public VersionInfo(string version)
        {
            var sections = Regex.Replace(version, @"[^\d\.]", string.Empty).Split('.');

            switch (sections.Length)
            {
                case 1:
                    this.SetVersion(Int64.Parse(sections[0]));
                    return;

                case 2:
                    this.SetVersion(Int64.Parse(sections[0]), Int64.Parse(sections[1]));
                    return;

                case 3:
                    this.SetVersion(Int64.Parse(sections[0]), Int64.Parse(sections[1]), Int64.Parse(sections[2]));
                    return;

                case 4:
                    this.SetVersion(Int64.Parse(sections[0]), Int64.Parse(sections[1]), Int64.Parse(sections[2]), Int64.Parse(sections[3]));
                    return;

                default:
                    this.SetVersion();
                    return;
            }
        }

        public static VersionInfo MaxValue
        {
            get { return new VersionInfo(Int64.MaxValue, Int64.MaxValue, Int64.MaxValue, Int64.MaxValue); }
        }

        public static VersionInfo MinValue
        {
            get { return new VersionInfo(); }
        }

        public long Build { get; set; }

        public long Major { get; set; }

        public long Minor { get; set; }

        public long Patch { get; set; }

        public static bool operator ==(VersionInfo v1, VersionInfo v2)
        {
            return Equals(v1, v2);
        }

        public static bool operator >(VersionInfo v1, VersionInfo v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator >=(VersionInfo v1, VersionInfo v2)
        {
            return v1.CompareTo(v2) >= 0;
        }

        public static implicit operator System.Version(VersionInfo version)
        {
            return new System.Version(Convert.ToInt32(version.Major), Convert.ToInt32(version.Minor), Convert.ToInt32(version.Patch), Convert.ToInt32(version.Build));
        }

        public static implicit operator VersionInfo(System.Version version)
        {
            return new VersionInfo(version.Major, version.Minor, version.Build, version.Revision);
        }

        public static bool operator !=(VersionInfo v1, VersionInfo v2)
        {
            return !Equals(v1, v2);
        }

        public static bool operator <(VersionInfo v1, VersionInfo v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator <=(VersionInfo v1, VersionInfo v2)
        {
            return v1.CompareTo(v2) <= 0;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var other = obj as VersionInfo;
            if (other == null)
            {
                throw new ArgumentException("Not a VersionInfo object.");
            }

            var major = this.Major.CompareTo(other.Major);
            if (major != 0)
            {
                return major;
            }

            var minor = this.Minor.CompareTo(other.Minor);
            if (minor != 0)
            {
                return minor;
            }

            var patch = this.Patch.CompareTo(other.Patch);
            return patch != 0 ? patch : this.Build.CompareTo(other.Build);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == this.GetType() && this.Equals((VersionInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Major.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Minor.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Patch.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Build.GetHashCode();
                return hashCode;
            }
        }

        public void SetVersion(long major = 0, long minor = 0, long patch = 0, long build = 0)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Build = build;
        }

        public override string ToString()
        {
            if (this.Build > 0)
            {
                return string.Format("{0}.{1}.{2}.{3}", this.Major, this.Minor, this.Patch, this.Build);
            }
            return this.Patch > 0 ? string.Format("{0}.{1}.{2}", this.Major, this.Minor, this.Patch) : string.Format("{0}.{1}", this.Major, this.Minor);
        }

        protected bool Equals(VersionInfo other)
        {
            return this.Major.Equals(other.Major) && this.Minor.Equals(other.Minor) && this.Patch.Equals(other.Patch) && this.Build.Equals(other.Build);
        }
    }
}