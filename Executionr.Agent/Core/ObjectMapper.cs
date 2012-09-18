using System;
using AutoMapper;
using System.Collections.Generic;

namespace Executionr.Agent.Core
{
    public class ObjectMapper : IObjectMapper
    {
        #region IObjectMapper implementation

        TTo IObjectMapper.Map<TFrom, TTo>(TFrom frm)
        {
            return Mapper.DynamicMap<TFrom, TTo>(frm);
        }

        public IEnumerable<TTo> Map<TFrom, TTo>(IEnumerable<TFrom> frm)
        {
            foreach (var f in frm)
            {
                yield return Mapper.DynamicMap<TFrom, TTo>(f);
            }
        }

        #endregion
    }
}

