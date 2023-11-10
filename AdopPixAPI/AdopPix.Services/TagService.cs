using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdopPix.Services.IServices;
using AdopPixAPI.DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace AdopPix.Services
{
    public class TagService : ITagService
    {
        private readonly IEntityUnitOfWork entityUnitOfWork;

        public TagService(IEntityUnitOfWork entityUnitOfWork)
        {
            this.entityUnitOfWork = entityUnitOfWork;
        }

        public async Task<string> FindByIdAsync(string tagId)
        {
            return await entityUnitOfWork.TagRepository.Find(x => x.Id == tagId).Select(x => x.Title).FirstOrDefaultAsync();
        }
    }
}