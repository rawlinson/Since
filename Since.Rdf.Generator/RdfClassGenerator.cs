using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj80;

namespace Since.Rdf.Generator
{
    [CodeGeneratorRegistration(typeof(RdfClassGenerator), "C# Rdf Class Generator", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
    [Guid("B0F8D785-CB45-4138-A008-95A9A14514DC")]
    [ProvideObject(typeof(RdfClassGenerator))]
    class RdfClassGenerator : IVsSingleFileGenerator, IObjectWithSite
    {
        private object _site;
        private CodeDomProvider _codeDomProvider;

        #region IVsSingleFileGenerator

        int IVsSingleFileGenerator.DefaultExtension(out string defaultExtension)
        {
            try
            {
                defaultExtension = GetDefaultExtension();
                return VSConstants.S_OK;
            }
            catch (Exception e)
            {
                Trace.WriteLine("GetDefaultExtension Failed");
                Trace.WriteLine(e.ToString());
                defaultExtension = string.Empty;
                return VSConstants.E_FAIL;
            }
        }

        int IVsSingleFileGenerator.Generate(string inputFilePath, string inputFileContents, string defaultNamespace, IntPtr[] outputFileContents, out uint output, IVsGeneratorProgress generateProgress)
        {
            var bytes = this.GenerateCode(inputFileContents);

            if (bytes == null)
            {
                // This signals that GenerateCode() has failed. Tasklist items have been put up in GenerateCode()
                outputFileContents = null;
                output = 0;

                // Return E_FAIL to inform Visual Studio that the generator has failed (so that no file gets generated)
                return VSConstants.E_FAIL;
            }
            else
            {
                // The contract between IVsSingleFileGenerator implementors and consumers is that 
                // any output returned from IVsSingleFileGenerator.Generate() is returned through  
                // memory allocated via CoTaskMemAlloc(). Therefore, we have to convert the 
                // byte[] array returned from GenerateCode() into an unmanaged blob.  

                var outputLength = bytes.Length;
                outputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
                Marshal.Copy(bytes, 0, outputFileContents[0], outputLength);
                output = (uint)outputLength;
                return VSConstants.S_OK;
            }
        }

        #endregion

        private byte[] GenerateCode(string inputFileContents)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
            CodeCompileUnit unit = BuildHelloWorldGraph();

            using (var stringWriter = new StringWriter())
            using (var tw = new IndentedTextWriter(stringWriter))
            {
                provider.GenerateCodeFromCompileUnit(unit, tw, new CodeGeneratorOptions());

                return Encoding.UTF8.GetBytes(stringWriter.ToString());
            }
        }

        public static CodeCompileUnit BuildHelloWorldGraph()
        {
            // Create a new CodeCompileUnit to contain 
            // the program graph.
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            // Declare a new namespace called Samples.
            CodeNamespace samples = new CodeNamespace("Samples");
            // Add the new namespace to the compile unit.
            compileUnit.Namespaces.Add(samples);

            // Add the new namespace import for the System namespace.
            samples.Imports.Add(new CodeNamespaceImport("System"));

            // Declare a new type called Class1.
            CodeTypeDeclaration class1 = new CodeTypeDeclaration("Class1");
            // Add the new type to the namespace type collection.
            samples.Types.Add(class1);

            // Declare a new code entry point method.
            CodeEntryPointMethod start = new CodeEntryPointMethod();

            // Create a type reference for the System.Console class.
            CodeTypeReferenceExpression csSystemConsoleType = new CodeTypeReferenceExpression("System.Console");

            // Build a Console.WriteLine statement.
            CodeMethodInvokeExpression cs1 = new CodeMethodInvokeExpression(
                csSystemConsoleType, "WriteLine",
                new CodePrimitiveExpression("Hello World!"));

            // Add the WriteLine call to the statement collection.
            start.Statements.Add(cs1);

            // Build another Console.WriteLine statement.
            CodeMethodInvokeExpression cs2 = new CodeMethodInvokeExpression(
                csSystemConsoleType, "WriteLine",
                new CodePrimitiveExpression("Press the Enter key to continue."));

            // Add the WriteLine call to the statement collection.
            start.Statements.Add(cs2);

            // Build a call to System.Console.ReadLine.
            CodeMethodInvokeExpression csReadLine = new CodeMethodInvokeExpression(
                csSystemConsoleType, "ReadLine");

            // Add the ReadLine statement.
            start.Statements.Add(csReadLine);

            // Add the code entry point method to
            // the Members collection of the type.
            class1.Members.Add(start);

            return compileUnit;
        }



        protected virtual CodeDomProvider GetCodeProvider()
        {
            if (_codeDomProvider == null)
            {
                    _codeDomProvider = CodeDomProvider.CreateProvider("C#");
            }
            return _codeDomProvider;
        }

        protected string GetDefaultExtension()
        {
            var codeDom = GetCodeProvider();

            var extension = codeDom.FileExtension;
            if (extension != null && extension.Length > 0)
            {
                extension = "." + extension.TrimStart(".".ToCharArray());
            }
            return extension;
        }


        #region IObjectWithSite

        void IObjectWithSite.GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (_site == null)
            {
                throw new COMException("object is not sited", VSConstants.E_FAIL);
            }

            var pUnknownPointer = Marshal.GetIUnknownForObject(_site);
            var intPointer = IntPtr.Zero;
            Marshal.QueryInterface(pUnknownPointer, ref riid, out intPointer);

            if (intPointer == IntPtr.Zero)
            {
                throw new COMException("site does not support requested interface", VSConstants.E_NOINTERFACE);
            }

            ppvSite = intPointer;
        }

        void IObjectWithSite.SetSite(object pUnkSite)
        {
            _site = pUnkSite;
            //_codeDomProvider = null;
            //_serviceProvider = null;
        }

        #endregion
    }
}
