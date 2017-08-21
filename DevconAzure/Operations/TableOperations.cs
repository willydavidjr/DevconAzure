using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevconAzure.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;

namespace DevconAzure.Operations
{
    public interface ITableOperation
    {
        void CreateEntity(OrderEntity entity);

        OrderEntity GetEntity(int partitionkey, string rowKey);

        List<OrderEntity> GetEntities(string filter);

        void UpdateEntity(OrderEntity entity);

        void DeleteEntity(OrderEntity entity);
    }
    public class TableOperations : ITableOperation
    {
        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;

        public TableOperations()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["webjobstorage"]);
            tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("OrderEntityTable");
            table.CreateIfNotExists();

        }
        public void CreateEntity(OrderEntity entity)
        {
            CloudTable table = tableClient.GetTableReference("OrderEntityTable");
            //Create a TableOperation object used to insert Entity into Table
            TableOperation insertOperation = TableOperation.Insert(entity);

            //Executes the operation
            table.Execute(insertOperation);
        }

        public void UpdateEntity(OrderEntity entity)
        {
            CloudTable table = tableClient.GetTableReference("OrderEntityTable");

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<OrderEntity>(entity.Email.ToString(), entity.OrderId.ToString());

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity object.
            OrderEntity updateEntity = (OrderEntity)retrievedResult.Result;

            if (updateEntity != null)
            {
                // Change the phone number.
                updateEntity.OrderName = entity.OrderName;
                updateEntity.Quantity = entity.Quantity;
                updateEntity.Description = entity.Description;

                // Create the Replace TableOperation.
                TableOperation updateOperation = TableOperation.Replace(updateEntity);

                // Execute the operation.
                table.Execute(updateOperation);
            }
        }

        public void DeleteEntity(OrderEntity entity)
        {
            CloudTable table = tableClient.GetTableReference("OrderEntityTable");

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<OrderEntity>(entity.Email.ToString(), entity.OrderId.ToString());

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity object.
            OrderEntity updateEntity = (OrderEntity)retrievedResult.Result;

            if (updateEntity != null)
            {
                // Create the Replace TableOperation.
                TableOperation updateOperation = TableOperation.Delete(updateEntity);

                // Execute the operation.
                table.Execute(updateOperation);
            }
        }

        public List<OrderEntity> GetEntities(string filter)
        {
            List<OrderEntity> Orders = new List<OrderEntity>();
            CloudTable table = tableClient.GetTableReference("OrderEntityTable");

            TableQuery<OrderEntity> query = new TableQuery<OrderEntity>()
                .Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, filter));

            foreach (var item in table.ExecuteQuery(query))
            {
                Orders.Add(new OrderEntity()
                {
                    OrderId = item.OrderId,
                    OrderName = item.OrderName,
                    Description = item.Description,
                    Email = item.Email,
                    Quantity = item.Quantity,
                    OrderPath = item.OrderPath
                });
            }

            return Orders;
        }
        
        public OrderEntity GetEntity(int partitionkey, string rowKey)
        {
            List<OrderEntity> Orders = new List<OrderEntity>();
            CloudTable table = tableClient.GetTableReference("OrderEntityTable");

            TableQuery<OrderEntity> query = new TableQuery<OrderEntity>()
                .Where(TableQuery.GenerateFilterConditionForInt("OrderId", QueryComparisons.Equal, partitionkey));

            foreach (var item in table.ExecuteQuery(query))
            {
                Orders.Add(new OrderEntity()
                {
                    OrderId = item.OrderId,
                    OrderName = item.OrderName,
                    Description = item.Description,
                    Email = item.Email,
                    Quantity = item.Quantity,
                    OrderPath = item.OrderPath
                });
            }

            return Orders.Take(1).SingleOrDefault();
            //throw new NotImplementedException();
        }

    }
}