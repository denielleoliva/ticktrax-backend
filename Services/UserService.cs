using Microsoft.EntityFrameworkCore;
using ticktrax_backend.Models;
using System.Collections;
using ticktrax_backend.Data;
using ticktrax_backend.dtomodels;
using Geolocation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.IO;

public class UserService : IUserService
{


    private TickTraxContext context;
    private UserManager<User> manager;

    private RoleManager<IdentityRole> roleManager;
    
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

        if(user.ProfilePhoto.Length>0)
        {
            if(user.ProfilePhoto.Length>2097152)
            {
                newUser.ProfilePhoto = user.ProfilePhoto;
                
            }else{
                throw new Exception("file is too large");
            }
        }

        var result = await manager.CreateAsync(newUser, user.Password);

        if (result.Succeeded)  
        {  
            var defaultrole = roleManager.FindByNameAsync("Default").Result;  

            if (defaultrole != null)  
            {  
              IdentityResult roleresult = await  manager.AddToRoleAsync(newUser, defaultrole.Name);  
            }  

    
        }

        return result;

    }

    public async Task<bool> UpdateUser(User user)
    {
        await manager.UpdateAsync(user);

        await context.SaveChangesAsync();

        return true;
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