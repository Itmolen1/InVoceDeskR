using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InvoiceDiskLast.Controllers
{
    public class APIPurchaseController : ApiController
    {
        private DBEntities db = new DBEntities();


        //get list
        public IHttpActionResult GetPurchaseOrder()
        {
            object ob = new object();
            try
            {
                ob = db.PurchaseOrderTables.ToList().Select(p => new MvcPurchaseModel
                {
                    PurchaseOrderID = p.PurchaseOrderID,
                    PurchaseID = p.PurchaseID,
                    PurchaseDate = p.PurchaseDate,
                    PurchaseDueDate = p.PurchaseDueDate,
                    PurchaseRefNumber = p.PurchaseRefNumber,
                    PurchaseSubTotal = p.PurchaseSubTotal,
                    PurchaseDiscountPercenteage = p.PurchaseDiscountPercenteage,
                    PurchaseDiscountAmount = p.PurchaseOrderID,
                    PurchaseVatPercentage = p.PurchaseVatPercentage,
                    PurchaseTotoalAmount = p.PurchaseTotoalAmount,
                    PurchaseVenderNote = p.PurchaseVenderNote,
                    Status = p.Status,
                    CompanyId = p.CompanyId,
                    UserId = p.UserId,
                    AddedDate = p.AddedDate,
                }).ToList();
            }
            catch (Exception ex)
            {
             
            }

            return Ok(ob);

        }

    }
}
