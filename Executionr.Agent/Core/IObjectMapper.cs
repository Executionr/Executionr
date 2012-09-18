using System;
using System.Collections.Generic;

namespace Executionr.Agent.Core
{
    public interface IObjectMapper
    {
        TTo Map<TFrom, TTo>(TFrom frm);
        IEnumerable<TTo> Map<TFrom, TTo>(IEnumerable<TFrom> frm);
    }
}

