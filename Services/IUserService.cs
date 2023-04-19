using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;
using Microsoft.AspNetCore.Identity;

public interface IUserService
{

    Task<IdentityResult> AddUser(UserDto user);


    Task<bool> DeleteUser(string id);

    Task<User> GetUserByUserName(string uName);

    Task<User> GetUserById(string id);

    Task<User> GetUserByEmail(string email);


}