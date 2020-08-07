using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NMediator.Request
{
    public interface IRequestHandler
    {
        TResult Handle<TRequest, TResult>(TRequest request);
    }
}
