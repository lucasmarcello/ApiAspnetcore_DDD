using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.DTO.User;
using Api.Domain.Entities;
using Api.Domain.Interfaces;
using Api.Domain.Interfaces.Services.User;
using Api.Domain.Models.User;
using AutoMapper;

namespace Api.Service.Services
{
    public class UserService : IUserService
    {
        private IRepository<UserEntity> _repository;

        private readonly IMapper _mapper;

        public UserService(IRepository<UserEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Delete(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<UserDTO> Get(Guid id)
        {
            UserEntity userEntity = await _repository.SelectAsync(id);
            return _mapper.Map<UserDTO>(userEntity);
        }

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            IEnumerable<UserEntity> listUserEntity =  await _repository.SelectAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(listUserEntity);
        }

        public async Task<UserDTOCreateResult> Post(UserDTOCreate user)
        {
            UserModel userModel = _mapper.Map<UserModel>(user);
            UserEntity userEntity = _mapper.Map<UserEntity>(userModel);
            UserEntity userEntityCreate = await _repository.InsertAsync(userEntity);

            return _mapper.Map<UserDTOCreateResult>(userEntityCreate);
        }

        public async Task<UserDTOUpdateResult> Put(UserDTOUpdate user)
        {
            UserModel userModel = _mapper.Map<UserModel>(user);
            UserEntity userEntity = _mapper.Map<UserEntity>(userModel);
            UserEntity userEntityCreate = await _repository.UpdateAsync(userEntity);
            
            return _mapper.Map<UserDTOUpdateResult>(userEntityCreate);
        }
    }
}
