﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public static class TransactionClass
    {
        static bool Result = true;
        public static bool PerformTransaction(AccountTransictionTable _TransactionModel)
        {
            try
            {
              
                 HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", _TransactionModel).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                   return Result = true;
                }
            }
            catch (Exception)
            {
                Result = false;
            }

            return Result;
        }
    }
}