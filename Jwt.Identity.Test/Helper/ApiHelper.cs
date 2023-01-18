using System.IO;
using Jwt.Identity.BoursYarServer.Services.TokenServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS1591 // همه توابع راهنما ندارند

namespace Jwt.Identity.Test.Helper
{
    /// <summary>
    ///     API برای پروژه Helper
    /// </summary>
    public static class ApiHelper
    {
        public static TokenGenrators CreaTokenGenrators()
        {
           // return new TokenGenrators(LoadJson());
           return null;
        }

        public static TokenValidators CreaTokenValidators()
        {
            return new TokenValidators(LoadJson());
        }

        private static JwtSettingModel LoadJson()
        {
            using (var r = new StreamReader("JwtIdentitySharedSettings.json"))
            {
                var json = r.ReadToEnd();
                var data = (JObject)JsonConvert.DeserializeObject(json);
                var jwt = data["JWT"].ToString();
                var items = JsonConvert.DeserializeObject<JwtSettingModel>(jwt);
                return items;
            }
        }
    }
}