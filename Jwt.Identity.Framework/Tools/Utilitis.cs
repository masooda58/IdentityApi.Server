using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Resources;
using Jwt.Identity.Framework.Response;
using Jwt.Identity.Framework.Tools.PersianErrorHandelSqlException;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace Jwt.Identity.Framework.Tools
{
    public static class Utilitis
    {
        public static ResultResponse CheckModelValidation<T>(T tmodel) where T : class
        {
            var vc = new ValidationContext(tmodel);
            ICollection<ValidationResult>
                results = new List<ValidationResult>(); // Will contain the results of the validation
            var isValid = Validator.TryValidateObject(tmodel, vc, results, true);

            if (!isValid)
            {
                var errorList = results.Select(err => err.ErrorMessage);

                CreateException(errorList);
                // return new ResultResponse(false, errorList, results);
            }

            return new ResultResponse(true, true.ToString(), results);
        }

        private static Exception CreateException(IEnumerable<string> str)
        {
            return new Exception(JsonConvert.SerializeObject(str));
        }
      
    }
}