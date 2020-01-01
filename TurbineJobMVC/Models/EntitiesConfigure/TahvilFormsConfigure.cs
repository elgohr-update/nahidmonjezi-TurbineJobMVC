using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurbineJobMVC.Models.Entites;

namespace TurbineJobMVC.Models.EntitiesConfigure
{
    public class TahvilFormsConfigure : BaseViewConfigure<TahvilForms>
    {
        public override void Configure(EntityTypeBuilder<TahvilForms> builder)
        {
            base.Configure(builder);
            builder.HasQueryFilter(q => q.TypeTahvil != 4 & q.AmvalNo != 0);
        }
    }
}