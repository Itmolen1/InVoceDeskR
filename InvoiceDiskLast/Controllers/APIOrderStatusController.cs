﻿using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace InvoiceDiskLast.Controllers
{
    public class APIOrderStatusController : ApiController
    {
        private DBEntities db = new DBEntities();

        [ResponseType(typeof(QutationOrderStatusTable))]
        [Route("api/InsertOrderStatus")]
        public IHttpActionResult PostProductTable(QutationOrderStatusTable QutationOrderStausTable)
        {
            if (!ModelState.IsValid)
            {
               return BadRequest(ModelState);
            }
            try
            {
                db.QutationOrderStatusTables.Add(QutationOrderStausTable);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }        
        }


       
        [Route("api/Addpending")]
        public IHttpActionResult PostPendingtable(PendingTable pendintTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                db.PendingTables.Add(pendintTable);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }



        [Route("api/GetPendint/{purchaseId:int}")]
        public IHttpActionResult GetPendingtable(int purchaseId)
        {           
            try
            {
                var query = db.PendingTables.Where(Q=>Q.Purchase_QuataionId==purchaseId).ToList();              
                return Ok(query);
            }
            catch (Exception ex)
            {
               return NotFound();
               
            }
        }






    }
}
