using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TurbineJobMVC.Models.EntitiesConfigure
{
    public class BaseViewConfigure<T> : IEntityTypeConfiguration<T> where T:class
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasNoKey();
        }
    }
}
