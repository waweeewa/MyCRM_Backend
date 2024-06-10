using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Common
{
    public class ErrorMessage
    {
        public ErrorMessage()
        {

        }
        public ErrorMessage(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
