using System;
using System.Collections.Generic;
using System.Text;


    public class MatrixException : Exception
    {
        string error;

        public MatrixException(string e)
        {
            error = e;
        }
    }

