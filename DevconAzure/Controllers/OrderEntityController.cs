using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevconAzure.Operations;
using DevconAzure.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace DevconAzure.Controllers
{
    public class OrderEntityController : Controller
    {
        // GET: OrderEntity
        TableOperations tableOperations;
        BlobOperations blobOperations;

        public OrderEntityController()
        {
            tableOperations = new TableOperations();
            blobOperations = new BlobOperations();
        }
        public ActionResult Index()
        {
            var orders = tableOperations.GetEntities("test@test.com");
            return View(orders);
        }

        public ActionResult Create(int id = 0)
        {
            var Order = new OrderEntity();
            if (id != 0)
            {
                Order = tableOperations.GetEntity(id, string.Empty);
                return View(Order);
            }
            Order.OrderId = new Random().Next();
            //Order.Email = User.Identity.Name;
            Order.Email = "test@test.com";
            return View(Order);
        }

        public ActionResult Delete(int id = 0)
        {
            var Order = new OrderEntity();
            if (id != 0)
            {
                Order = tableOperations.GetEntity(id, string.Empty);
                tableOperations.DeleteEntity(Order);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OrderEntity obj, HttpPostedFileBase orderFile)
        {

            //Check if it exist:
            OrderEntity _order = new OrderEntity();
            _order = tableOperations.GetEntity(obj.OrderId, string.Empty);

            if (_order != null)
            {
                _order.ETag = "*";
                _order.PartitionKey = "test@test.com";
                _order.RowKey = obj.OrderId.ToString();
                _order.OrderName = obj.OrderName;
                _order.Quantity = obj.Quantity;
                _order.Description = obj.Description;
                tableOperations.UpdateEntity(_order);
                return RedirectToAction("Index");
            }

            CloudBlockBlob orderBlob = null;
            //Step 1: Uploaded File in Blob Storage
            if (orderFile != null && orderFile.ContentLength != 0)
            {
                orderBlob = await blobOperations.UploadBlob(orderFile);
                obj.OrderPath = orderBlob.Uri.ToString();
            }

            //Step 2: Save the information in the Table Storage
            //Get the original file size
            //obj.Email = User.Identity.Name; //Login email
            obj.Email = "test@test.com";
            obj.RowKey = obj.OrderId.ToString();
            //obj.PartitionKey = obj.Email;
            obj.PartitionKey = "test@test.com";
            
            //Save the file in the Table
            tableOperations.CreateEntity(obj);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        public ActionResult Details(int id = 0)
        {
            var Order = new OrderEntity();
            if (id != 0)
            {
                Order = tableOperations.GetEntity(id, string.Empty);
                //tableOperations.GetEntity(id, string.Empty);
            }
            return View(Order);
        }
    }
}