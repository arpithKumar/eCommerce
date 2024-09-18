using AuthenticationApi.Application.DTOs;
using eCommerce.SharedLibrary.Responses;

namespace AuthenticationApi.Application.Interfaces;

public interface IUser
{
    Task<Response> RegisterAsync(AppUserDto dto);
    Task<Response> LoginAsync(LoginDto dto);
    Task<GetUserDto> GetUser(int userId);
    
}