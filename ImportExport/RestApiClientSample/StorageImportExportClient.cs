using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using StorageImportExport.Models;

namespace StorageImportExport
{
    class StorageImportExportClient
    {
        private StorageImportExportLib client;
        private WebRequestHandler webHandler;

        /// <summary>
        /// Initialize a import/export service client
        /// </summary>
        /// <param name="subscriptionId">The subscription ID for the Azure user.</param>
        /// <param name="clientCertificate">The client certificate for the Azure user.</param>
        public StorageImportExportClient(string subscriptionId, X509Certificate2 clientCertificate)
        {
            webHandler = new WebRequestHandler();
            client = new StorageImportExportLib(webHandler);
            client.SubscriptionId = subscriptionId;
            webHandler.ClientCertificates.Add(clientCertificate);
        }

        /// <summary>
        /// Create a new job in the Windows Azure Import/Export service.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="configFilePath"></param>
        public void CreateJob(string jobName, string configFilePath)
        {
            var XConf = XDocument.Load(configFilePath);

            // ReturnAddress: Specifies the return address information for the job.
            var XReturnAddress = XConf.Descendants("ReturnAddress").First();
            var returnAddress = new ReturnAddress(
                XReturnAddress.Element("Name").Value,
                XReturnAddress.Element("Address").Value,
                XReturnAddress.Element("Phone").Value,
                XReturnAddress.Element("Email").Value
            );

            // ReturnShipping: Specifies the return carrier and customer’s account with the carrier
            var XReturnShipping = XConf.Descendants("ReturnShipping").First();
            var returnShipping = new ReturnShipping(
                XReturnShipping.Element("CarrierName").Value,
                XReturnShipping.Element("CarrierAccountNumber").Value
            );            

            // Properties: The list of properties for the job.
            // refer to https://msdn.microsoft.com/en-us/library/azure/dn529110.aspx for more details
            var XJobProperty = XConf.Descendants("JobProperty").First();
            var putJobProperties = new PutJobProperties(
                backupDriveManifest: bool.Parse(XJobProperty.Element("BackupDriveManifest").Value),
                description: XJobProperty.Element("Description").Value,
                enableVerboseLog: bool.Parse(XJobProperty.Element("EnableVerboseLog").Value),
                friendlyName: XJobProperty.Element("FriendlyName").Value,
                type: (XJobProperty.Element("JobType").Value.Equals("Import", StringComparison.InvariantCultureIgnoreCase)? JobType.Import: JobType.Export),
                location: XJobProperty.Element("Location").Value,
                storageAccountKey: XJobProperty.Element("StorageAccountKey").Value,
                storageAccountName: XJobProperty.Element("StorageAccountName").Value,
                importExportStatesPath: XJobProperty.Element("ImportExportStatesPath").Value,               
                returnAddress: returnAddress,
                returnShipping: returnShipping
            );

            // must include either StorageAccountKey or ContainerSas in the request
            if (string.IsNullOrEmpty(XJobProperty.Element("StorageAccountKey").Value))
            {
                putJobProperties.StorageAccountKey = null;
                putJobProperties.ContainerSas = XJobProperty.Element("ContainerSas").Value;
            }

            var putJobParameters = new PutJobParameters(jobName, putJobProperties);
            if (putJobProperties.Type == JobType.Export)
            {
                // BlobList: contain information about the blobs to be exported for an export job.  
                var XBlobList = XConf.Descendants("BlobList").First().Elements();
                var blobList = new BlobList(new List<string>(), new List<string>());
                foreach (var XBlob in XBlobList)
                {
                    if (!String.IsNullOrWhiteSpace(XBlob.Attribute("BlobPaths").Value))
                    {
                        blobList.BlobPath.Add(XBlob.Attribute("BlobPaths").Value);
                    }
                    if (!String.IsNullOrWhiteSpace(XBlob.Attribute("BlobPathPrefixes").Value))
                    {
                        blobList.BlobPathPrefix.Add(XBlob.Attribute("BlobPathPrefixes").Value);
                    }
                }
                putJobParameters.Export = new Export(blobList: blobList);
            }
            else
            {
                // DriveList: List of up to ten drives that comprise the job.
                var XDriveList = XConf.Descendants("DriveList").First().Elements();
                var driveList = new List<Drive>();
                foreach (var XDrive in XDriveList)
                {
                    driveList.Add(new Drive(
                        driveId: XDrive.Element("DriveId").Value,
                        bitLockerKey: XDrive.Element("BitLockerKey").Value,
                        manifestFile: XDrive.Element("ManifestFile").Value,
                        manifestHash: XDrive.Element("ManifestHash").Value
                    ));
                }
                putJobParameters.DriveList = driveList;
            }

            client.PutJob(XJobProperty.Element("StorageAccountName").Value, jobName, putJobParameters);
        }

        /// <summary>
        /// Notify the Import/Export service that the hard drives comprising the import or export job have been shipped to the Microsoft data center.
        /// </summary>
        /// <param name="storageAccountName">Name of the storage account you want to operate on.</param>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="deliveryPackage">Contains information about the package being shipped by the customer to the Microsoft data center.</param>
        public void ShipJob(string storageAccountName, string jobName, PackageInfomation deliveryPackage)
        {
            var patchJobParameters = new PatchJobParameters(deliveryPackage: deliveryPackage, state: "Shipping");
            client.PatchJob(storageAccountName, jobName, patchJobParameters);
        }

        /// <summary>
        /// Get information about an existing job from the Windows Azure Import/Export service. 
        /// </summary>
        /// <param name="storageAccountName">Name of the storage account you want to operate on.</param>
        /// <param name="jobName">Name of the job.</param>
        /// <returns></returns>
        public GetJobResponse GetJob(string storageAccountName, string jobName)
        {
            return client.GetJob(storageAccountName, jobName);
        }

        /// <summary>
        /// List active and completed jobs for a storage account in a subscription.
        /// </summary>
        /// <param name="storageAccountName">Name of the storage account you want to operate on.</param>
        /// <param name="jobType">Optional. List jobs with the job type.</param>
        /// <returns></returns>
        public IList<ListJobsResponseValueItem> ListJobs(string storageAccountName, JobType? jobType = null)
        {
            var jobList = client.ListJobs(storageAccountName, jobType);
            return jobList.Value;
        }


    }
}
