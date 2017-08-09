﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core
{
    public class ServiceResponse<T>
    {
        public ServiceResponse(T result, IEnumerable<ServiceResponseError<T>> errors)
        {
            this.Result = result;
            this.Errors = errors;
        }

        public ServiceResponse(T result) : this(result, null)
        {
        }

        public T Result { get; }

        public IEnumerable<ServiceResponseError<T>> Errors { get; }

        public bool HasErrors => this.Errors != null || this.Errors.Any();
    }
}