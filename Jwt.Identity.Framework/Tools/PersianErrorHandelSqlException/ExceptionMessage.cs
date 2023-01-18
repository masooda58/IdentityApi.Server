using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Framework.Response;
using Microsoft.Data.SqlClient;

namespace Jwt.Identity.Framework.Tools.PersianErrorHandelSqlException
{
    public static class ExceptionMessage
    {
        public static Exception GetPerisanSqlExceptionMessage(Exception exception)
        {
            var sqlException = exception.GetBaseException() as SqlException;
            var x = sqlException.Data;
            if (sqlException == null)
            {

                return exception;
            }
            try
            {
                var errorCode = sqlException.Number;
                ResourceManager MyResourceClass =
                    new ResourceManager(typeof(PersianSqlExceptionRes));
                var resx = PersianSqlExceptionRes.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
           
                foreach (DictionaryEntry entry in resx)
                {
                    if (entry.Key.ToString() == errorCode.ToString())
                    {
                        return new Exception(entry.Value.ToString()+"-"+sqlException.Number.ToString());
                    }

               
                }

              
                return new Exception(sqlException.Message+"-"+sqlException.Number.ToString());
            }
            catch (Exception e)
            {
                return new Exception(sqlException.Message+"-"+"Message="+e.Message);
            }
        }

    }
}
