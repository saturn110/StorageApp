using StorageAsp.Models;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System.Collections;
using System.Collections.Generic;

namespace StorageAsp.Controllers
{
    public class TablesController : Controller
    {
        // GET: Tables
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("andreydragunov_AzureStorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");
            ViewBag.Success= table.CreateIfNotExists();
            ViewBag.TableName = table.Name;
            return View();
        }

        public ActionResult DeleteTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("andreydragunov_AzureStorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");
            table.Delete();
          
            return View();
        }
        public ActionResult AddEntity()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("andreydragunov_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");
            CustomerEntity customer1 = new CustomerEntity("Lopes", "Janifer");
            customer1.Email = "lopes@mail.ru";
            TableOperation insertOperation = TableOperation.Insert(customer1);
            TableResult result = table.Execute(insertOperation);
            ViewBag.TableName = table.Name;
            ViewBag.Result = result.HttpStatusCode;
            return View();
        }

        public ActionResult AddEntities()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("andreydragunov_AzureStorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");

            CustomerEntity customer1 = new CustomerEntity("Pupkin", "Vasiliy");
            customer1.Email = "pupkin@mail.ru";

            CustomerEntity customer2 = new CustomerEntity("Pupkin", "Kolya");
            customer2.Email = "pupkinkolya@mail.ru";

            CustomerEntity customer3 = new CustomerEntity("Pupkin", "Sveta");
            customer3.Email = "pupkinksveta@mail.ru";

            TableBatchOperation batchOperation = new TableBatchOperation();
            batchOperation.Insert(customer1);
            batchOperation.Insert(customer2);
            batchOperation.Insert(customer3);
            IList<TableResult> results = table.ExecuteBatch(batchOperation);
            ViewBag.Success = table.CreateIfNotExists();
            
            return View(results);
        }

        public ActionResult GetSingle()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
               CloudConfigurationManager.GetSetting("andreydragunov_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Pupkin", "Sveta");
            TableResult result = table.Execute(retrieveOperation);
            return View(result);
        }

        public ActionResult GetPartition()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
               CloudConfigurationManager.GetSetting("andreydragunov_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");
            TableQuery<CustomerEntity> query =
                            new TableQuery<CustomerEntity>()
                            .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Pupkin"));

            List<CustomerEntity> customers = new List<CustomerEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<CustomerEntity> resultSegment = table.ExecuteQuerySegmented(query, token);
                token = resultSegment.ContinuationToken;

                foreach (CustomerEntity customer in resultSegment.Results)
                {
                    customers.Add(customer);
                }
            } while (token != null);

            return View(customers);
        }

        public ActionResult DeleteEntity()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        CloudConfigurationManager.GetSetting("andreydragunov_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");

            TableOperation deleteOperation =
                TableOperation.Delete(new CustomerEntity("Pupkin", "Kolya") { ETag = "*" });
            TableResult result = table.Execute(deleteOperation);

            return View(result);
        }
    }
}