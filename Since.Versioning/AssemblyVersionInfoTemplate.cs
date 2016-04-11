using System;
using System.Linq;
using LibGit2Sharp;
using Microsoft.VisualStudio.TextTemplating;

namespace Since.Versioning
{
    public abstract class AssemblyVersionInfoTemplate : TextTransformation
    {
        public ITextTemplatingEngineHost Host { get; set; }

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

        public string Version { get; set; } = String.Empty;
        public string Branch { get; set; } = String.Empty;
        public string Pre { get; set; } = String.Empty;
        public string Post { get; set; } = String.Empty;
        public string Build { get; set; } = String.Empty;
        public bool IsPublished { get; set; } = false;

        EnvDTE.DTE _dte = null;
        public EnvDTE.DTE Dte
            => _dte ?? (
               _dte = (this.Host as IServiceProvider)?.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE
            );

        Git _git = null;
        public Git Git
            => _git ?? (
               _git = new Git(this.ActiveFile)
            );

        public string ActiveFile
            => this.Dte.ActiveDocument.FullName;

        string _revision = null;
        public string Revision
            => _revision ?? (
               _revision = this.GetRevision()
            );

        // Get the number of minutes since the last commit of the version file
        //  and the most recent change to the repository. Ish.
        public string GetRevision()
        {
            DateTime start = this.Git.LastCommit.Author.When.DateTime.ToUniversalTime();

            DateTime stop = this.Git.IsModified
                ? DateTime.UtcNow
                : this.Git.Commit.Author.When.DateTime.ToUniversalTime();

            return ((int)stop.Subtract(start).TotalMinutes).ToString();
        }

        public string AssemblyVersion
            => this.IsPublished ? this.SubParts(this.Version, 2) : this.Version;

        public string AssemblyFileVersion
            => this.Version;

        public string AssemblyInformationalVersion
        {
            get
            {
                return this.SubParts(this.Version, 3)
                    + (this.Branch == null ? null : "_" + this.Branch)
                    + (this.Pre == null ? null : "-" + this.Pre)
                    + (IsPublished ? null : (
                          (this.Post == null ? null : "+" + this.Post)
                        + (this.Build == null ? null : " (" + this.Build + ")")
                    ));
            }
        }

        public static string Combine(params string[] values)
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

        public string SubParts(string version, int count)
        {
            int index = 0 - 1;
            while (count-- > 0)
                index = version.IndexOf('.', index + 1);
            return version.Substring(0, index);
        }
    }
}
