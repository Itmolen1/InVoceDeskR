using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InvoiceDiskLast.Controllers
{
    public class DirectoryAPIController : ApiController
    {
        private DBEntities db = new DBEntities();


        [Route("api/CheckForDirectoryExist/{RefrenceId:int}")]
        public IHttpActionResult GetDirectory(int RefrenceId)
        {
            try
            {
                var q = db.DirectoryTables.Where(c => c.Id == RefrenceId).FirstOrDefault();

                if (q != null)
                {
                    return Ok(true);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        [Route("api/CreateDirecoty")]
        public IHttpActionResult POSTtDirEctory(DirectoryTable _directory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.DirectoryTables.Add(_directory);
                    db.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
                throw ex;
            }
        }


        [Route("api/GetDirectory/{RefrenceId:int}")]
        public IHttpActionResult GetDirectorPath(int RefrenceId)
        {
            DirectoryViewModel _cviewModel = new DirectoryViewModel();
            try
            {
                _cviewModel = db.DirectoryTables.Where(d => d.RefrenceId == RefrenceId).Select(D => new DirectoryViewModel
                {
                    DirectoryPath = D.DirectoryPath
                }).FirstOrDefault();

                return Ok(_cviewModel);

            }
            catch (Exception ex)
            {
                return NotFound();
                throw ex;
            }
        }



    }
}
