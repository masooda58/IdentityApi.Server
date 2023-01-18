using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Jwt.Identity.BoursYarServer.Helpers.Extensions
{
    public static class TempDataExtensions
    {
        /// <summary>
        ///     set class to TempData
        /// </summary>
        /// <typeparam name="T"> class name</typeparam>
        /// <param name="tempData">temptData Name</param>
        /// <param name="key">temp data key name</param>
        /// <param name="value"> class value</param>
        /// <remarks>
        ///     <example>
        ///         <code>
        /// var cont = new Contact { FirstName = "Masoud", Domain = "BoursYar.com" };
        /// TempData.Set("TempDataName", cont);
        /// Contact c = TempData.Get&lt;Contact&gt;("TempDataName");
        /// </code>
        ///     </example>
        /// </remarks>
        public static void Set<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">نام کلاس </typeparam>
        /// <param name="tempData"></param>
        /// <param name="key"> نام کلید</param>
        /// <returns></returns>
        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            tempData.TryGetValue(key, out var o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}