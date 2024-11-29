using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Service;

namespace YadiYad.Pro.Services.Common
{
    public class ExpertiseService
    {
        #region Fields
        private readonly IRepository<Expertise> _ExpertiseRepository;
        private readonly IRepository<ServiceExpertise> _ServiceExpertiserepository;
        #endregion

        #region Ctor

        public ExpertiseService
            (
            IRepository<Expertise> ExpertiseRepository,
            IRepository<ServiceExpertise> ServiceExpertiserepository)
        {
            _ExpertiseRepository = ExpertiseRepository;
            _ServiceExpertiserepository = ServiceExpertiserepository;

        }

        #endregion


        #region Methods

        public virtual IPagedList<Expertise> GetAllExpertise(
            int pageIndex = 0, 
            int pageSize = int.MaxValue, 
            string keyword = null,
            int categoryId = 0)
        {
            var query = _ExpertiseRepository.Table.Where(x => x.Published == true);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n => n.Name.ToLower().Contains(keyword.ToLower()));

            if(categoryId != 0)
            {
                query = query.Where(x => x.JobServiceCategoryId == categoryId);
            }
            query = query.OrderBy(n => n.Name);

            var data = new PagedList<Expertise>(query, pageIndex, pageSize);

            return data;
        }

        public virtual IPagedList<Expertise> SearchExpertise(string name, int categoryId,
         int pageIndex = 0,
         int pageSize = int.MaxValue)
        {
            var query = _ExpertiseRepository.Table;

            if (!string.IsNullOrEmpty(name))
                query = query.Where(n => n.Name.ToLower().Contains(name.ToLower()));

            if (categoryId > 0)
                query = query.Where(n => n.JobServiceCategoryId == categoryId);

            query = query.OrderBy(n => n.Name);

            var data = new PagedList<Expertise>(query, pageIndex, pageSize);

            return data;
        }

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        public virtual Expertise GetExpertById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            return _ExpertiseRepository.GetById(categoryId);
        }

        public virtual List<Expertise> GetExpertisesByIds(int[] expertiseIds)
        {
            if (expertiseIds == null)
                return new List<Expertise>();

            var query = from p in _ExpertiseRepository.Table
                        where expertiseIds.Contains(p.Id)
                        select p;

            return query.ToList();
        }

        public virtual List<ServiceExpertiseDTO> GetExpertisesByServiceProfileId(int serviceProfileId)
        {
            if (serviceProfileId == 0)
                return new List<ServiceExpertiseDTO>();

            var query = from e in _ExpertiseRepository.Table
                        from se in _ServiceExpertiserepository.Table.DefaultIfEmpty()
                        where se.ExpertiseId == e.Id && se.ServiceProfileId == serviceProfileId && se.Deleted == false
                        select new ServiceExpertiseDTO
                        {
                            Id = e.Id,
                            Name = e.Name
                        };

            return query.ToList();
        }

        //Update
        public virtual void UpdateExpertise(Expertise expertise)
        {
            if (expertise == null)
                throw new ArgumentNullException(nameof(expertise));

            _ExpertiseRepository.Update(expertise);
        }

        public virtual void InsertExpertise(Expertise expertise)
        {
            if (expertise == null)
                throw new ArgumentNullException(nameof(expertise));

            _ExpertiseRepository.Insert(expertise);
        }

        public virtual void DeleteMany(IList<Expertise> expertises)
        {
            if (expertises == null)
                throw new ArgumentNullException(nameof(expertises));

            foreach (var expert in expertises)
            {
                Delete(expert);
            }
        }

        public virtual void Delete(Expertise expertise)
        {
            if (expertise == null)
                throw new ArgumentNullException(nameof(expertise));

            _ExpertiseRepository.Delete(expertise);
        }

        #endregion
    }
}
