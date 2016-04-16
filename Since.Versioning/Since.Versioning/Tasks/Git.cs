using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace Since.Tasks
{
    public class Git
    {
        private Commit _lastCommit;

        public Git(string path)
        {
            this.Repository = this.OpenRepository(path);
            this.File = path;
        }

        private string File { get; }

        public Repository Repository { get; }

        public Branch Branch
            => this.Repository?.Head;

        private int _revision = -1;
        public int Revision
            => _revision == -1 ? (_revision = this.GetRevision()) : _revision;

        // Get the number of minutes since the last commit of the version file
        //  and the most recent change to the repository. Ish.
        public int GetRevision()
        {
            DateTime start = this.LastVersionCommit.Author.When.DateTime.ToUniversalTime();

            DateTime stop = this.IsModified
                ? DateTime.UtcNow
                : this.Commit.Author.When.DateTime.ToUniversalTime();

            return (int)stop.Subtract(start).TotalMinutes;
        }

        public string BranchName
        {
            get
            {
                var name = this.Branch?.FriendlyName;
                return name == "master" ? null : name;
            }
        }

        public Commit Commit
            => this.Repository?.Head.Commits.FirstOrDefault();

        public string CommitIdShort
            => this.Commit?.Id.Sha.Substring(0, 8);

        public Commit LastVersionCommit
            => _lastCommit ?? (
               _lastCommit = this.GetLastVersionCommit()
            );

        public bool IsModified
            => this.Repository.RetrieveStatus((StatusOptions)null).IsDirty;

        public string ModifiedString
            => this.IsModified ? "mod" : null;

        public bool BranchMatches(params string[] patterns)
        {
            string name = this.Repository?.Head.CanonicalName;
            return name == null ? false : patterns.Any(pattern => Regex.IsMatch(name, pattern));
        }

        private Commit GetRootCommit(Commit commit)
        {
            var filter = new CommitFilter
            {
                IncludeReachableFrom = commit,
                SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Time
            };

            return this.Repository.Commits.QueryBy(filter)
                .Where(c => !c.Parents.Any()).FirstOrDefault();
        }

        private Commit GetLastVersionCommit()
        {
            var file = this.File;
            var commits = this.Repository.Commits
               .Where(c => c.Parents.Count() == 1
                   && c.Tree[file] != null
                   && (c.Parents.First().Tree[file] == null
                    || c.Tree[file].Target.Id != c.Parents.First().Tree[file].Target.Id)
                      );

            return commits?.FirstOrDefault() ?? this.GetRootCommit(this.Commit);
        }

        private Repository OpenRepository(string path)
        {
            Contract.Requires(path != null);

            while (!Directory.Exists(Path.Combine(path, ".git")))
            {
                path = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(path))
                    return null;
            }

            return new Repository(path);
        }
    }
}
