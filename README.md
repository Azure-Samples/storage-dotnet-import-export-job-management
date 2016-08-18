---
services: azure-storage -import-export
platforms: dotnet
author: renash
---

# Getting Started with Azure Import Export in .NET

This sample demonstrates how to create and manage your [Import Export](https://azure.microsoft.com/en-us/documentation/articles/storage-import-export-service/) job using REST API. This sample only works against classic storage accounts. One can create an Import Export job in their target classic storage account using the Classic portal or the [Import/Export REST API](https://msdn.microsoft.com/en-us/library/dn529096.aspx). We will use the latter to create our job in this sample 

This sample contains Import Export REST-API Swagger definitions which will enable you to generate Import Export models for different languages like Ruby, CSharp, NodeJS, Java and Python using AutoREST. The Swagger definition is located at importexport.json. If you want to use CSharp, this sample already contains the generated files and you do not need to generate your own.

This sample only provides the C# version of Swagger-Based generated code. In this sample `StorageImportExportClient` is a wrapper over the existing REST API in Open specification format.

## Pre Requisite
1. Access to subscription and classic storage account.
2. Access to Management certificate of the subscription in which this storage account lives.
3. Management certificate installed on local machine. If you don't know about management certificates, [here](https://azure.microsoft.com/en-us/documentation/articles/cloud-services-certs-create/) is the guide.
3. Journal file generated when running [Import Export client tool](http://go.microsoft.com/fwlink/?LinkID=301900&clcid=0x409)

## How to get started with Import Job
1.	[Create a storage account](https://azure.microsoft.com/en-us/documentation/articles/storage-create-storage-account-classic-portal/#create-a-storage-account).
2.	[Procure the disk and adapter](https://azure.microsoft.com/en-us/documentation/articles/storage-import-export-service/#pre-requisites).
3.	[Download the client tool](http://go.microsoft.com/fwlink/?LinkID=301900&clcid=0x409).
4.	[Prepare the disk](https://msdn.microsoft.com/library/dn529089.aspx).
5.	Create a job using REST API using this sample.
6.	Ship disks to Azure datacenter using address obtained using `GetLocation` API.
7.	Track your job using REST API using this sample.

## How to get started with Export Job
1.	[Procure a disk](https://azure.microsoft.com/en-us/documentation/articles/storage-import-export-service/#pre-requisites).
2.	Create a job using REST API to specify the data you intend to export using this sample.
3.	Ship disks to Azure datacenterusing address obtained using `GetLocation` API.
4.	Track your job using REST API using this sample.
5.	Retrieve the Bitlocker key using `GetJob` REST API using this sample.
6.	Ulock your drive and retrieve your data using this key when the disk arrives back to you.

## Running this sample

1. Download the solution and open the RestAPISample.sln in Visual Studio.
2. Open TemplateExportConfig.xml or TemplateImportConfig.xml and set the following values


	    <Location>{your azure storage location name}</Location>
	      <StorageAccountName>{your storage account name}</StorageAccountName>
	    <StorageAccountKey>{your storage account key}</StorageAccountKey>
  	      <ReturnAddress>
    	        <Name>{contact person name}</Name>
      	      <Email>{contact email}</Email>
        	    <Address>{contact postal address}</Address>
          	  <Phone>{contact number}</Phone>
          </ReturnAddress>
          <ReturnShipping>
              <CarrierName>{carrier name like Fedex, DHL}</CarrierName>
    	        <CarrierAccountNumber>{account number for return shipping}</CarrierAccountNumber>
          </ReturnShipping>
          <BlobList>
              <Blob BlobPaths="" BlobPathPrefixes="/export/"/>
          </BlobList>

In addition following are fields for an Import job from your journal file.

	    <DriveList>
	      <Drive>
	        <DriveId>{your drive serial number}</DriveId>
	        <BitLockerKey>{drive bitlocker key}</BitLockerKey>
	        <ManifestFile>{path to manifest file}</ManifestFile>
	        <ManifestHash>{your manifest hash}</ManifestHash>
	      </Drive>
	    </DriveList>

3. Update managemnt certificate thumbprint the value in Program.cs

	     var clientCertificateThumbprint = "<client certificate thumbprint>";

4. Update the subscription id

  	     var client = new StorageImportExportClient("<azure subscription id>", clientCert);

5. Update name of the import or export job. This could be any string value to identify your job.

  	     client.CreateJob("<myjoname>"...

6. Update  storage account name and job name you wish to vies

  	     var jobItem = client.GetJob("<storage accountname>", "<jobname import>");

4. Set breakpoints and run the project using F10.

## More information
- [What is Azure Import Export?](https://azure.microsoft.com/en-us/documentation/articles/storage-import-export-service/)
- [Azure Import/Export REST API Reference](https://msdn.microsoft.com/en-us/library/dn529087.aspx)
- [How to use Import/Export Tool?](https://msdn.microsoft.com/en-us/library/dn529093.aspx)
- [Management Certificate](https://azure.microsoft.com/en-us/documentation/articles/cloud-services-certs-create/)
- [AutoREST](https://github.com/Azure/autorest/blob/master/README.md)
- [Open API Specification](https://github.com/OAI/OpenAPI-Specification/blob/master/README.md)
