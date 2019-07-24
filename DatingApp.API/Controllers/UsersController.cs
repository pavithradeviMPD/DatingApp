using System;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using DatingApp.API.Helpers;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo , IMapper mapper)
        {
            _mapper=mapper;
            _repo=repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo=await _repo.GetUser(currentUserId);

            userParams.UserId=currentUserId;
            if(string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender=userFromRepo.Gender=="male"? "female":"male";
            }

           var users=await _repo.GetUsers(userParams);

            var userToReturn=_mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage,users.PageSize,
                        users.TotalCount,users.TotalPage);

           return Ok(userToReturn);
        }
        
        [HttpGet("{id}",Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user =await _repo.GetUser(id);

            var userToReturn=_mapper.Map<UserForDetailedDto>(user);
            
            return Ok(userToReturn);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto){
            if(id !=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            var userFromRepo=await _repo.GetUser(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if(await _repo.SaveAll()){
                return NoContent();
            }

            throw new Exception($"Updating user {id} failed on save");
        }
    }
}