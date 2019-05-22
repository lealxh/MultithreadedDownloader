using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescargaCartolas
{
    public class WsDTO
    {
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
        public object[] Args { get; set; }
    }

    public class WsSource : IWsSource
    {

        private string UrlWebService { get; set; }

        public WsSource(string webServicesUrl)
        {
            this.UrlWebService = webServicesUrl;
        }

        public object CallWebService(WsDTO param)
        {
            string ErrWebService = string.Empty;

            System.Net.WebClient client = new System.Net.WebClient();

            // Connect To the web service

            System.IO.Stream stream = client.OpenRead(this.UrlWebService + "?wsdl");

            // Now read the WSDL file describing a service.

            var description = System.Web.Services.Description.ServiceDescription.Read(stream);

            ///// LOAD THE DOM /////////

            // Initialize a service description importer.

            var importer = new System.Web.Services.Description.ServiceDescriptionImporter();

            importer.ProtocolName = "Soap12"; // Use SOAP 1.2.

            importer.AddServiceDescription(description, null, null);

            // Generate a proxy client.

            importer.Style = System.Web.Services.Description.ServiceDescriptionImportStyle.Client;

            // Generate properties to represent primitive values.

            importer.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;

            // Initialize a Code-DOM tree into which we will import the service.

            var nmspace = new System.CodeDom.CodeNamespace();

            var unit1 = new System.CodeDom.CodeCompileUnit();

            unit1.Namespaces.Add(nmspace);

            // Import the service into the Code-DOM tree. This creates proxy code that uses the service.

            var warning = importer.Import(nmspace, unit1);

            if (warning == 0) // If zero then we are good to go
            {
                  // Generate the proxy code

                var provider1 = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp");

                // Compile the assembly proxy with the appropriate references

                string[] assemblyReferences = new string[5] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll" };

                var parms = new System.CodeDom.Compiler.CompilerParameters(assemblyReferences);

                var results = provider1.CompileAssemblyFromDom(parms, unit1);

                // Check For Errors

                if (results.Errors.Count > 0)
                {

                    foreach (System.CodeDom.Compiler.CompilerError oops in results.Errors)
                    {
                        ErrWebService += " " + oops.ErrorText;
                    }

                    throw new System.Exception(string.Format("Error al llamar a WS [{0}]", ErrWebService));

                }

                // Finally, Invoke the web service method

                object wsvcClass = results.CompiledAssembly.CreateInstance(param.ServiceName);

                var mi = wsvcClass.GetType().GetMethod(param.MethodName);

                return mi.Invoke(wsvcClass, param.Args);

            }

            else
            {

                return null;

            }
        }
    }
}
