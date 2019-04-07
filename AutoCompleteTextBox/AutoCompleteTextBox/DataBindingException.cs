using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCustomControls
{
    public class DataBindingException : Exception
    { 
        public DataBindingException() : base()
        {

        }

        public DataBindingException(string message) : base(message)
        {

        }

        public DataBindingException(string message, Exception exception)
            : base(message, exception)
        {

        }
    }
}
