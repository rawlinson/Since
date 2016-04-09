using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Text;
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

        private byte[] GenerateCode(string inputFileContents)
        {
            return Encoding.UTF8.GetBytes("// Not Implemented");
        }

        protected virtual CodeDomProvider GetCodeProvider()
        {
            if (_codeDomProvider == null)
                _codeDomProvider = CodeDomProvider.CreateProvider("C#");
            return _codeDomProvider;
        }

        #region IVsSingleFileGenerator

        int IVsSingleFileGenerator.DefaultExtension(out string defaultExtension)
        {
            defaultExtension = null;
            if (this.GetCodeProvider().FileExtension is string s && s.Length > 0)
                defaultExtension = "." + s.TrimStart(new[] { '.' });

            return VSConstants.S_OK;
        }

        int IVsSingleFileGenerator.Generate(string inputFilePath, string inputFileContents, string defaultNamespace, IntPtr[] outputFileContents, out uint output, IVsGeneratorProgress generateProgress)
        {
            var bytes = this.GenerateCode(inputFileContents);

            output = (uint)(bytes?.Length ?? 0);
            if (output > 0)
            {
                outputFileContents[0] = Marshal.AllocCoTaskMem(bytes.Length);
                Marshal.Copy(bytes, 0, outputFileContents[0], bytes.Length);
            }

            return output > 0 ? VSConstants.S_OK : VSConstants.E_FAIL;
        }

        #endregion

        #region IObjectWithSite

        void IObjectWithSite.GetSite(ref Guid riid, out IntPtr site)
        {
            if (_site == null)
                throw new COMException("Object is not sited", VSConstants.E_FAIL);
            
            IntPtr pointer;
            var ret = Marshal.QueryInterface(Marshal.GetIUnknownForObject(_site), ref riid, out pointer);
            if (ret != VSConstants.S_OK)
                throw new COMException("Site does not support requested interface", VSConstants.E_NOINTERFACE);            

            site = pointer;
        }

        void IObjectWithSite.SetSite(object unkSite)
        {
            _site = unkSite;
            _codeDomProvider = null;
        }

        #endregion
    }
}
