using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.CSharp;

namespace Since.Tasks
{
    public class GenerateAssemblyVersionInfo : Microsoft.Build.Utilities.Task
    {
        public GenerateAssemblyVersionInfo()
        {
            Init();
        }

           [Required]
        public string InputPath { get; set; }

        [Output]
        public string OutputPath { get; private set; }

        string Host(string code)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                sb.AppendLine(s);

            var s2 = sb.ToString();

            string generatatorClassCode;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Since.Tasks.VersionGeneratorTemplate.cs"))
            using (var reader = new StreamReader(stream))
            {
                generatatorClassCode = reader.ReadToEnd();
            }
            
            // {0}: class name e.g. "MyGenerator"
            // {1}: function code
            // {2}: usings
            return String.Format(generatatorClassCode, "MyGenerator", code, "");
        }

        CompilerResults Compile(string code)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var sinceAssemblyName = thisAssembly.GetReferencedAssemblies().Where(an => an.Name.Equals("Since")).FirstOrDefault();
            if (sinceAssemblyName == null)
            {
                Log.LogError("uhoh, can't find since.dll");
                return null;
            }

            var sinceAssembly = Assembly.Load(sinceAssemblyName);
            if (sinceAssembly == null)
            {
                Log.LogError("uhoh, can't find since.dll");
                return null;
            }

            var codeProvider = new CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] {
                    "mscorlib.dll",
                    "System.Core.dll",
                    "System.Runtime.dll",
                    sinceAssembly.Location,
                    thisAssembly.Location
                }
            //,
            //    Path.GetTempFileName() + ".dll",
            //    true         
            );

            parameters.IncludeDebugInformation = true;
            parameters.GenerateInMemory = true;

            string source = Host(code);

            var compile = codeProvider.CompileAssemblyFromSource(parameters, new[] { source });

            return compile;
        }
        static Dictionary<string, Assembly> assemblies;

        public void Init()
        {

            assemblies = new Dictionary<string, Assembly>();

            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {

            Assembly assembly = null;

            assemblies.TryGetValue(args.Name, out assembly);

            return assembly;

        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {

            Assembly assembly = args.LoadedAssembly;
            assemblies[assembly.FullName] = assembly;
        }

        public override bool Execute()
        {
            string content;
            try
            {
                content = File.ReadAllText(this.InputPath);
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }

            var compile = Compile(content);
            if (compile == null)
            {
                Log.LogError("can't compile");
                return false;
            }
            if (compile.Errors.Count > 0)
            {
                foreach (var error in compile.Errors.OfType<CompilerError>())
                    Log.LogError(null, error.ErrorNumber, null, error.FileName, error.Line, error.Column, 0, 0, error.ErrorText);
                return false;
            }
                        
            VersionGeneratorBase generator = compile.CompiledAssembly.CreateInstance("MyGenerator") as VersionGeneratorBase;

            var types = compile.CompiledAssembly.GetExportedTypes();

            if (generator == null)
            {
                Log.LogError("can't load generator");
                return false;
            }

            generator.ActiveFile = Path.GetFullPath(this.InputPath);

            var output = generator.Execute();
            if (string.IsNullOrWhiteSpace(output))
            {
                Log.LogError("generator returned no output");
                return false;
            }

            try
            {
                var outputPath = this.OutputPath;
                if (string.IsNullOrWhiteSpace(outputPath))
                    outputPath = Path.GetDirectoryName(this.InputPath) +"/"+ Path.GetFileNameWithoutExtension(this.InputPath) + ".cs";
                
                File.WriteAllText(outputPath, output);
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }

            return true;
        }
    }
}
