using Microsoft.EntityFrameworkCore;
using ticktrax_backend.Models;
using System.Collections;
using ticktrax_backend.Data;
using ticktrax_backend.dtomodels;
using Geolocation;
using Microsoft.AspNetCore.Identity;

public class UserService : IUserService
{


    private TickTraxContext context;
    private UserManager<User> manager;
    
    public UserService(TickTraxContext _ctx, UserManager<User> _mng)
    {
        context = _ctx;
        manager = _mng;
    }

    public async Task<IdentityResult> AddUser(UserDto user)
    {

        User newUser = new User
        {
            UserName = user.UserName,
            Email = user.Email
        };

        var result = await manager.CreateAsync(newUser, user.Password);

        return result;
    }

    public async Task<bool> DeleteUser(string id)
    {
        User? userToDelete = await manager.FindByIdAsync(id);

        if(userToDelete!=null)
        {
            var result = manager.DeleteAsync(userToDelete);

            if(result.IsCompletedSuccessfully)
            {
                return true;
            }else{
                return false;
            }

        }
        
        return false;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        User? user = await manager.FindByEmailAsync(email);

        if(user!=null)
        {
            return user;
        }else{
            throw new NullReferenceException();
        }

    }

    public async Task<User> GetUserById(string id)
    {
        User? user = await manager.FindByIdAsync(id);

        if(user!=null)
        {
            return user;
        }else{
            throw new NullReferenceException();
        }
    }

    public async Task<User> GetUserByUserName(string uName)
    {
        var result = await context.Users.Where(user => user.UserName == uName).FirstOrDefaultAsync();

        if(result!=null)
        {
            return result;
        }else{
            throw new NullReferenceException();
        }
    }

   
}