using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Common
{
    public class HttpRequestResponse<T>
    {
        public T Result { get; set; }
        public List<ErrorMessage> ErrorMessages { get; set; }
        public HttpRequestResponse()
        {
            ErrorMessages = new List<ErrorMessage>();
        }
        public HttpRequestResponse(T result) : this()
        {
            Result = result;
        }

        public HttpRequestResponse<T> AddErrorMessage(string error)
        {
            ErrorMessages.Add(new ErrorMessage(error));
            return this;

        }
        //public HttpRequestResponse<T> AddErrorMessages(ErrorMessage error)
        //{
        //	ErrorMessages.Add(error);
        //	return this;

        //}
    }
}
