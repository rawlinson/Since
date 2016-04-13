using System;
using Microsoft.VisualStudio.TextTemplating;
using Since.Extensions;

namespace Since.Versioning
{
    public abstract class AssemblyVersionInfoTemplate : TextTransformation
    {
        private EnvDTE.DTE _dte = null;
        private Git _git = null;
        private string _revision = null;

        public string Version { get; set; } = null;
        public string Branch { get; set; } = null;
        public string Pre { get; set; } = null;
        public string Post { get; set; } = null;
        public string Build { get; set; } = null;

        public bool IsPublished { get; set; } = false;

        public string AssemblyVersion
            => this.IsPublished ? SubParts(this.Version, 2) : this.Version;

        public string AssemblyFileVersion
            => this.Version;

        public string AssemblyInformationalVersion
            => SubParts(this.Version, 3)
                + Concat("_", this.Branch)
                + Concat("-", this.Pre)
                + (!IsPublished).Then(Concat("+", this.Post) + Concat(" (", this.Build, ")"));

        static string ConcatIf(bool check, params string[] values)
            => check ? string.Concat(values) : null;

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

        public ITextTemplatingEngineHost Host { get; set; }

        public EnvDTE.DTE Dte
            => _dte ?? (
               _dte = (this.Host as IServiceProvider)?.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE
            );

        public string ActiveFile
            => this.Dte?.ActiveDocument.FullName;

        public Git Git
            => _git ?? (
               _git = new Git(this.ActiveFile)
            );

        public string Revision
            => _revision ?? (
               _revision = this.GetRevision()
            );

        // Get the number of minutes since the last commit of the version file
        //  and the most recent change to the repository. Ish.
        public string GetRevision()
        {
            DateTime start = this.Git.LastVersionCommit.Author.When.DateTime.ToUniversalTime();

            DateTime stop = this.Git.IsModified
                ? DateTime.UtcNow
                : this.Git.Commit.Author.When.DateTime.ToUniversalTime();

            return ((int)stop.Subtract(start).TotalMinutes).ToString();
        }

        public static string Join(params string[] values)
        {
            string result = String.Empty;
            foreach (var value in values)
            {
                if (value == null)
                    return null;
                string sep = String.IsNullOrEmpty(result) || String.IsNullOrEmpty(value) ? "" : ".";
                result += sep + value;
            }
            return result;
        }

        public static string SubParts(string version, int count)
        {
            int index = 0 - 1;
            while (count-- > 0)
                index = version.IndexOf('.', index + 1);
            return version.Substring(0, index);
        }

        public override string TransformText()
        {
            var output =
                $"/*\n" +
                $"    Generated from {this.ActiveFile}\n" +
                $"*/\n" +
                $"using System.Reflection;\n\n" +

                $"[assembly: AssemblyVersion(\"{this.AssemblyVersion}\")]\n" +
                $"[assembly: AssemblyFileVersion(\"{this.AssemblyFileVersion}\")]\n" +
                $"[assembly: AssemblyInformationalVersion(\"{this.AssemblyInformationalVersion}\")]\n";

            this.Write(output);

            return this.GenerationEnvironment.ToString();
        }
    }
}
