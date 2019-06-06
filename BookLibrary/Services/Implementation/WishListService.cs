using DataAccess.Interfaces;
using DataAccess.Models;
using Services.DTO;
using Services.Exceptions;
using Services.Filters;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Services.Interfaces
{
    public class WishListService: Service<WishList, WishListDTO, IFilter>, IWishListService
    {
        public WishListService(IUnitOfWork unitOfWork) :
            base(unitOfWork)
        {
        }

        public override WishListDTO Get(string id)
        {
            WishList entity = Repository
              .Get(e => e.Id == id)
              .SingleOrDefault();

            if (entity == null)
            {
                return new WishListDTO();
            }

            return MapToDto(entity);
        }

        public override IEnumerable<WishListDTO> Get(IFilter filter)
        {
            Func<WishList, bool> predicate = GetFilter(filter);
            List<WishList> entities = Repository
              .Get(p => predicate(p))
              .ToList();

            if (!entities.Any())
            {
                return new List<WishListDTO>();
            }

            return entities.Select(e => MapToDto(e));
        }



        public override IEnumerable<WishListDTO> GetAll()
        {
            List<WishList> entities = Repository
              .Get(p => p != null)
              .ToList();

            if (!entities.Any())
            {
                return new List<WishListDTO>();
            }

            return entities.Select(e => MapToDto(e));
        }

        public override void Add(WishListDTO dto)
        {
            WishList checkEntity = Repository
                .Get(e => e.Id == dto.Id)
                .SingleOrDefault();

            if (checkEntity != null)
            {
                throw new DuplicateNameException();
            }

            WishList entity = MapToEntity(dto);
            Repository.Add(entity);
            _unitOfWork.SaveChanges();
        }

        public override void Remove(string id)
        {
            WishList entity = Repository
             .Get(e => e.Id == id)
             .SingleOrDefault();

            if (entity == null)
            {
                throw new ObjectNotFoundException();
            }

            Repository.Remove(entity);
            _unitOfWork.SaveChanges();
        }
        public override void Update(WishListDTO dto)
        {
            WishList entity = Repository
             .Get(e => e.Id == dto.Id)
             .SingleOrDefault();

            if (entity == null)
            {
                throw new ObjectNotFoundException();
            }


            entity.UserId = dto.UserId;
            entity.BookId = dto.BookId;
            entity.Name = dto.Name;


            Repository.Update(entity);
            _unitOfWork.SaveChanges();
        }

        protected override WishListDTO MapToDto(WishList entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            WishListDTO dto = new WishListDTO
            {
                Id = entity.Id,
                BookId = entity.BookId,
                UserId = entity.UserId,
                Name = entity.Name
            };

            return dto;
        }

        protected override WishList MapToEntity(WishListDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException();
            }

            WishList entity = new WishList
            {
                Id = dto.Id,
                BookId = dto.BookId,
                UserId = dto.UserId,
                Name = dto.Name
            };

            return entity;
        }

        private Func<WishList, bool> GetFilter(IFilter filter)
        {
            Func<WishList, bool> result = e => true;

            if (filter is WishListFilterByUserId)
            {
                if (!String.IsNullOrEmpty((filter as WishListFilterByUserId)?.UserId))
                {
                    result += e => e.UserId == (filter as WishListFilterByUserId).UserId;
                }
            }
            else if (filter is WishListFilterByName)
            {
                if (!String.IsNullOrEmpty((filter as WishListFilterByName)?.Name))
                {
                    result += e => e.Name == (filter as WishListFilterByName).Name;
                }
            }
            else if (filter is WishListFilterByBookId)
            {
                if (!String.IsNullOrEmpty((filter as WishListFilterByBookId)?.BookId))
                {
                    result += e => e.BookId == (filter as WishListFilterByBookId).BookId;
                }
            }
            else if (filter is WishListFullFilter)
            {
                if (!String.IsNullOrEmpty((filter as WishListFullFilter)?.BookId) 
                    && !String.IsNullOrEmpty((filter as WishListFullFilter)?.UserId) 
                    && !String.IsNullOrEmpty((filter as WishListFullFilter)?.Name))
                {
                    result += e => e.BookId == (filter as WishListFullFilter).BookId;
                    result += e => e.UserId == (filter as WishListFullFilter).UserId;
                    result += e => e.Name == (filter as WishListFullFilter).Name;
                }
            }
            return result;
        }



    }
}
