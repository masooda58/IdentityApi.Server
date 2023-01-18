using System.Collections.Generic;

namespace Jwt.Identity.Framework.Response
{
    public class ResultResponse
    {
        public ResultResponse(bool successed, IEnumerable<string> message, object values = null)
        {
            Successed = successed;
            Message = message;
            ResponseValues = values ?? new { };
        }

        public ResultResponse(bool successed, string message) :
            this(successed, new List<string> { message })
        {
        }

        public ResultResponse(bool successed, string message, object values) :
            this(successed, new List<string> { message }, values)
        {
        }

        public bool Successed { get; set; }
        public IEnumerable<string> Message { get; set; }
        public object ResponseValues { get; set; }
    }
}