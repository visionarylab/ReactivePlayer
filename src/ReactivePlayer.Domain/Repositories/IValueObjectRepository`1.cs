﻿using ReactivePlayer.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Repositories
{
    public interface IValueObjectRepository<TValueObject, TEntityParent>
        where TValueObject : ValueObject<TValueObject>
        where TEntityParent : Entity
    {
        Task<TValueObject> GetByParentAsync(TEntityParent entityParent);
        Task<TValueObject> AddForParentAsync(TEntityParent entityParent, TValueObject valueObject);
    }
}