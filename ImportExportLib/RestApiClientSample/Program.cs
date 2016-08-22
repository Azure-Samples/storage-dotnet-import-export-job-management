using System;
using StorageImportExport.Models;
using System.Security.Cryptography.X509Certificates;

namespace StorageImportExport
{
    class Program
    {

    static void Main(string[] args)
        {
            try
            {
                // load client certificate from local store
                X509Certificate2 clientCert;
                var clientCertificateThumbprint = "<client certificate thumbprint>";
                if (!GetCertificateFromStore(clientCertificateThumbprint, StoreName.My, StoreLocation.CurrentUser, out clientCert)
                    && !GetCertificateFromStore(clientCertificateThumbprint, StoreName.My, StoreLocation.LocalMachine, out clientCert))
                {
                    var message = string.Format("Could not find client certificate with thumbprint '{0}' in any cert store.", clientCertificateThumbprint);
                    throw new Exception(message);
                }

                // create an import/export client with the azure subscription id and the client certificate
                var client = new StorageImportExportClient("<azure subscription id>", clientCert);

                // create a sample export job
                client.CreateJob("<myexportjoname>", "config\\TemplateExportConfig.xml");

                // create a sample import job
                client.CreateJob("<myimportjobname>", "config\\TemplateImportConfig.xml");
                
                // return a list of locations to which you can ship the disks
                var locations = client.ListLocations();

                // list all jobs in the storage account
                var jobList = client.ListJobs("<mystorageaccountname>");
                jobList = client.ListJobs("<mystorageaccountname>", JobType.Import);

                // ship the sample import job
                var package = new PackageInfomation("<carrier name>", "<tracking number>", <drive id>, "<ship date yyyy/mm/dd");
                client.ShipJob("<storage account name>", "<jobname import>", package);

                // get the sample import job and read its job state
                var jobItem = client.GetJob("<storage accountname>", "<jobname import>");
                Console.WriteLine("{0}: {1}", jobItem.Name, jobItem.Properties.State);
            }
            catch (ErrorResponseException e)
            {
                Console.WriteLine(e.Body.Value);
                Console.WriteLine(e.Body.ExtendedInformation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Get a certificate from local certificate store
        /// </summary>
        /// <param name="thumbprint">Thumbprint of the certificate to look for</param>
        /// <param name="storeName">Store Name to look in to</param>
        /// <param name="storeLocation">Store Location to look in to</param>
        /// <param name="certificate">Certificate</param>
        /// <returns>True if certificate is found in the cert store</returns>
        private static bool GetCertificateFromStore(
            string thumbprint,
            StoreName storeName,
            StoreLocation storeLocation,
            out X509Certificate2 certificate)
        {
            certificate = null;
            // Create a reference to the My certificate store.
            X509Store certStore = new X509Store(storeName, storeLocation);
            certStore.Open(OpenFlags.ReadOnly);

            // Find the certificate that matches the thumbprint.
            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                X509FindType.FindByThumbprint,
                thumbprint,
                false);

            certStore.Close();

            // Check to see if our certificate was added to the collection. If no, throw an error, if yes, create a certificate using it.
            if (0 == certCollection.Count)
            {
                Console.WriteLine(String.Format("No certificate containing thumbprint '{0}' found in '{1}\\{2}'", thumbprint,
                        storeLocation, storeName));
                return false;
            }
            // Create an X509Certificate2 object using our matching certificate.
            certificate = certCollection[0];
            return true;
        }
    }
}
