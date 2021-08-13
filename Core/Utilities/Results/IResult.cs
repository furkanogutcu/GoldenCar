using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    //This type can be used for void
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
    }
}
