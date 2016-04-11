using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace Since.Versioning
{
    public class Git
    {
        public Git(string path)
        {
            this.Repository = this.OpenGitRepo(path);
        }

        public string File { get; set; }

        public Repository Repository { get; }

        public bool IsModified
            => this.Repository.RetrieveStatus((StatusOptions)null).IsDirty;

        public string Modified
            => this.IsModified ? "mod" : "";

        public string Branch
        {
            get
            {
                var name = this.Repository?.Head.FriendlyName;
                return name == "master" ? null : name;
            }
        }

        public Commit Commit
            => this.Repository?.Head.Commits.FirstOrDefault();

        public string CommitId
            => this.Commit?.Id.Sha.Substring(0, 8);

        Commit _lastCommit;
        public Commit LastCommit
            => _lastCommit ?? (
               _lastCommit = this.GetLastCommit()
            );

        public bool BranchMatches(params string[] patterns)
        {
            string name = this.Repository?.Head.CanonicalName;
            return name == null ? false : patterns.Any(pattern => Regex.IsMatch(name, pattern));
        }

        private Repository OpenGitRepo(string path)
        {
            while (!Directory.Exists(Path.Combine(path, ".git")))
            {
                path = Path.GetDirectoryName(path);
                if (path == null)
                    return null;
            }
            return new Repository(path);
        }

        Commit GetLastCommit()
        {
            var file = this.File;
            var commits = this.Repository.Commits
               .Where(c => c.Parents.Count() == 1 && c.Tree[file] != null &&
                  (c.Parents.FirstOrDefault().Tree[file] == null ||
                     c.Tree[file].Target.Id !=
                     c.Parents.FirstOrDefault().Tree[file].Target.Id));

            var lastCommit = commits?.FirstOrDefault();
            if (lastCommit == null)
            {
                Commit c = Commit;
                while (c != null)
                {
                    lastCommit = c;
                    c = c.Parents.FirstOrDefault();
                }
            }

            return lastCommit;
        }
    }
}
