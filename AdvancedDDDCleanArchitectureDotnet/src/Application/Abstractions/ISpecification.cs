using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstractions
{

    public interface ISpecification<T>
    {
        Func<IQueryable<T>, IQueryable<T>> Criteria { get; }
    }
}
